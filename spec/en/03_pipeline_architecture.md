# 03. Pipeline architecture

## I. Incremental pipeline architecture

The Roslyn Incremental Source Generator (ISG) processes compiler events using a LINQ-like pipeline to transform syntax inputs into source code. This architecture leverages the `Kassyi.Generators.Extensions` pipeline helpers to ensure lean, zero-allocation transformations.

### Overall pipeline flow

The following sequence diagram illustrates the pipeline topology, showing the chained Roslyn `IncrementalValuesProvider<T>` APIs. Section III details the internal class interactions.

```mermaid
sequenceDiagram
    autonumber
    participant Compiler as Roslyn Compiler
    participant SP as SyntaxProvider (ISG)
    participant Prepare as PrepareData (Extraction)
    participant Model as DTO (ClassData/DPData)
    participant Source as Sources.* (Generation)

    Compiler->>SP: Syntax / Semantic change notification
    SP->>SP: ForAttributeWithMetadataName...<br/>(Filters target syntax nodes)
    SP->>SP: Combine(Framework, Version)
    SP->>Prepare: Select(PrepareData)
    Note over Prepare: Extracts pure primitive DTOs<br/>(Cached NamedArguments, deduped syntax)
    Prepare-->>Model: Construct (ClassData, DependencyPropertyData)
    SP->>SP: WhereNotNull()
    Note over SP: If Equals == true vs previous compilation,<br/>pipeline stops here (Cache Hit)
    SP->>Source: Select(Generate)
    Source-->>SP: Generated C# source text
    SP->>Compiler: AddSource()
```

### Pipeline phases

The pipeline executes in the following strict order:

1. **Syntax filtering**: The generator uses Roslyn 4.3.0+ APIs (`ForAttributeWithMetadataName`) to filter classes and records decorated with specific attributes.
2. **Data extraction**: The `PrepareData.cs` and `DependencyPropertyDataBuilder` components project raw `AttributeData` and `INamedTypeSymbol` instances into structured DTOs. This phase uses dictionary lookups to cache `NamedArguments` and deduplicates syntax searches to maximize extraction speed.
3. **Equality evaluation and caching**: The Roslyn ISG driver evaluates the `Select` phase output. If the output matches the previous compilation step (`Equals` returns `true`), the pipeline bypasses source generation and uses the incremental cache.
4. **Source code generation**: The generator runs this phase only on cache misses. It transforms DTOs into `.g.cs` source strings, using `SourceWriter` scope management to guarantee zero-allocation formatting.

---

## II. Model equality and caching strategy

The incremental cache hit ratio is the most critical performance metric for an ISG. The data models (`DependencyPropertyData`, `ClassData`, `EventData`, and sub-records) enforce strict value equality semantics (such as adopting `readonly record struct` and wrapping collections with `EquatableArray<T>`) to optimize this ratio.

> [!NOTE]
> For detailed architectural constraints regarding the equality caching strategy, zero-allocation generation, and early detachment of Roslyn syntax trees, see **[05. Code synthesis and performance](./05_synthesis_and_performance.md#iv-performance-optimization-rules)**.

---

## III. Detailed class relationships and data flow

This section defines the responsibilities of internal generator classes and the data flow constraints within the Roslyn pipeline.

### 1. Overall architecture and class relationships

The generator's internal architecture consists of four primary layers:

1. **Generators**: Registered in the Roslyn pipeline to orchestrate execution flow.
2. **Data extraction**: Extracts necessary metadata from the syntax and semantic models.
3. **Models (DTOs)**: Equatable value-type records that store extracted data.
4. **Sources**: Receives DTOs and emits synthesized C# source code strings.

#### 1. Single attribute generator base and implementations (`AttributeGeneratorBase`)

```mermaid
classDiagram
    %% Generators
    class AttributeGeneratorBase~TData~ {
        <<abstract>>
        +Initialize(IncrementalGeneratorInitializationContext)
        #PrepareData(GeneratorAttributeContext) TData?
        #GenerateSource(TData) string
        #GetHintName(TData) string
        #SupportedFrameworks IReadOnlyList~Framework~
    }

    class DependencyPropertyGenerator {
        #PrepareData() Tuple~ClassData, DPData~
        #GenerateSource() string
    }
    class RoutedEventGenerator {
        #PrepareData() Tuple~ClassData, EventData~
    }
    AttributeGeneratorBase <|-- DependencyPropertyGenerator
    AttributeGeneratorBase <|-- RoutedEventGenerator
```

#### 2. Multi attribute generator base and implementations (`MultiAttributeGeneratorBase`)

```mermaid
classDiagram
    class MultiAttributeGeneratorBase~TData~ {
        <<abstract>>
        +Initialize(IncrementalGeneratorInitializationContext)
        #PrepareData(GeneratorMultiAttributeContext) TData?
        #GenerateSource(TData) string
        #GetHintName(TData) string
        #SupportedFrameworks IReadOnlyList~Framework~
        #SelectMany bool
    }

    class AttachedDependencyPropertyGenerator {
        #PrepareData() Tuple~ClassData, DPData~
    }

    class WeakEventGenerator {
        #PrepareData() Tuple~ClassData, EventData~
    }

    MultiAttributeGeneratorBase <|-- AttachedDependencyPropertyGenerator
    MultiAttributeGeneratorBase <|-- WeakEventGenerator
```

#### 3. Data extraction, models (DTOs), and source generation helpers

```mermaid
classDiagram
    %% Data Extraction
    class PrepareData {
        <<static>>
        +GetDependencyPropertyData(GeneratorAttributeContext) DependencyPropertyData
        +GetClassData(INamedTypeSymbol, ...) ClassData
    }

    class DependencyPropertyDataBuilder {
        +WithCoreProperties()
        +WithMetadata()
        +WithDefaultValues()
        +WithCallbacks()
        +Build() DependencyPropertyData
    }

    class DependencyPropertyMetadataExtractor {
        <<static>>
        +GetFrameworkMetadata() FrameworkMetadataData
    }

    %% Models (DTOs)
    class ClassData {
        <<readonly record struct>>
    }
    class DependencyPropertyData {
        <<readonly record struct>>
    }

    %% Source Generation
    class SourceGenerationHelper {
        <<static>>
        +GenerateDependencyPropertySource(ClassData, DPData) string
    }

    %% Relationships
    DependencyPropertyGenerator --> PrepareData : Called by pipeline
    PrepareData --> DependencyPropertyDataBuilder : Delegates data building
    DependencyPropertyDataBuilder --> DependencyPropertyMetadataExtractor : Parses metadata

    DependencyPropertyDataBuilder ..> DependencyPropertyData : Creates
    PrepareData ..> ClassData : Creates

    DependencyPropertyGenerator --> SourceGenerationHelper : Passes DTOs
    SourceGenerationHelper ..> ClassData : Reads
    SourceGenerationHelper ..> DependencyPropertyData : Reads
```

### Roles of major classes

- **`AttributeGeneratorBase<TData>` and `MultiAttributeGeneratorBase<TData>`**: The core foundation of the generator. It encapsulates standard logic for syntax filtering, target framework validation (`SupportedFrameworks`), context encapsulation (`GeneratorAttributeContext`), and source output.
- **`PrepareData`**: The extraction process entry point. It provides extension methods to isolate pure data from complex Roslyn objects like `INamedTypeSymbol`.
- **`DependencyPropertyDataBuilder`**: An internal builder executing step-by-step extraction logic for dependency properties, such as matching callback signatures and extracting XML documentation.
- **`ClassData` and `DependencyPropertyData`**: Data models storing extracted metadata. They are implemented as `readonly record struct` to maximize caching performance.
- **`SourceGenerationHelper`**: Static helpers that consume data models and assemble the final C# source code using `SourceWriter`.

---

### 2. Detailed data flow and internal method calls

The following diagram traces the internal execution flow, illustrating the sequence from detecting a `[DependencyProperty]` attribute to generating the final C# code, and detailing instantiated classes and invoked methods.

#### 1. Data extraction phase (Extraction)

```mermaid
sequenceDiagram
    autonumber
    participant Roslyn as ISG Pipeline
    participant DPG as Generator
    participant PD as PrepareData
    participant Builder as DPDataBuilder
    participant Models as DTOs

    %% Parsing Phase
    Roslyn->>DPG: Syntax change notification<br/>(Detects attributed class)

    %% Extraction Phase
    DPG->>PD: GetClassData(classSymbol)
    Note over PD: Gets modifiers, namespace, etc.
    PD-->>Models: Creates ClassData

    DPG->>PD: GetDependencyPropertyData(attribute)
    PD->>Builder: new Builder()

    Note over Builder: Extracts metadata step-by-step
    Builder->>Builder: WithCoreProperties()
    Builder->>Builder: WithMetadata()
    Builder->>Builder: WithDefaultValues()
    Builder->>Builder: WithCallbacks()

    Builder-->>Models: Creates DPData

    DPG-->>Roslyn: Returns Tuple (ClassData, DPData)
```

#### 2. Caching and source generation phase (Generation)

```mermaid
sequenceDiagram
    autonumber
    participant Roslyn as ISG Pipeline
    participant DPG as Generator
    participant Helper as SourceGenerationHelper

    %% Caching Phase
    Note over Roslyn: [IMPORTANT] Equality check via Models' Equals().<br/>If unchanged from previous compilation,<br/>stops here and uses cache.

    %% Generation Phase
    Roslyn->>DPG: Cache miss, requests generation
    DPG->>Helper: GenerateDependencyPropertySource(Class, DP)
    Note over Helper: Assembles C# string using<br/>SourceWriter (Zero Allocation)
    Helper-->>DPG: Generated source code (string)
    DPG-->>Roslyn: Registers to compiler via AddSource()
```

### 3. Architectural design intent

> [!NOTE]
> **Consolidation of performance optimization principles**
> For detailed prohibitions and best practices regarding early detachment of Roslyn types (Symbol/Syntax) and the zero-allocation generation phase, see **[05. Code synthesis and performance](./05_synthesis_and_performance.md#iv-performance-optimization-rules)**.

**Extensibility and separation of concerns**
By isolating framework-specific mapping logic (in `DependencyPropertyDataBuilder`) from source generation logic (`SourceGenerationHelper`), the architecture ensures that parsing modifications do not affect the zero-allocation generation layer.
