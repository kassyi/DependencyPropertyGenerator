# 08. Diagnostics reference

This document provides a comprehensive list of diagnostic errors emitted by the `DependencyPropertyGenerator` during source code analysis, along with detailed troubleshooting guidelines.
It details the cause of each error and provides concrete code examples to help you quickly resolve build issues.

---

## Diagnostic IDs quick reference

| Diagnostic ID | Severity | Title | Overview |
| :--- | :--- | :--- | :--- |
| [`DPG0000`](#dpg0000-framework-is-not-recognized) | Error | Framework is not recognized | The generator cannot automatically detect the target UI framework. |
| [`DPG0001`](#dpg0001-onchanged-method-not-found-or-unsupported) | Error | OnChanged Method Not Found | The specified callback method is missing or has an invalid signature. |
| [`DPG0002`](#dpg0002-invalid-type-modifier-file-scoped) | Error | Invalid Type Modifier | You cannot apply the generator to classes with the `file` scope modifier. |
| [`DPG0003`](#dpg0003-invalid-property-type-ref-struct) | Error | Invalid Property Type | You cannot use `ref struct` types as DependencyProperties. |
| [`DPG0004`](#dpg0004-reference-type-default-value-sharing) | Error | Reference Type Sharing | Prevents sharing reference type instances across all control instances. |
| [`DPG0005`](#dpg0005-invalid-callback-signature-overridemetadata) | Error | Invalid Callback Signature | The callback requests the old value on a platform that does not support it. |
| [`DPG0007`](#dpg0007-unsupported-callback-signature) | Error | Unsupported Callback Signature | An auto-discovered callback method has an invalid signature. |
| [`DPG0008`](#dpg0008-invalid-default-value-expression) | Error | Invalid Default Expression | The Roslyn parser cannot parse the C# string in `DefaultValueExpression`. |
| `DPG0009` | Info | Duplicate Attribute Helper | Suppresses CS0436 duplicate attribute helper warnings. |
| `DPG9999` | Error | Unhandled Exception | An unexpected internal generator exception occurred. |

---

## Error details and solutions

### DPG0000: Framework is not recognized

The generator cannot automatically detect the target UI framework (WPF, WinUI, Uno, Avalonia, or MAUI) from your project references.

❌ **Cause:**
Your project (`.csproj`) lacks the required UI framework packages (for example, `Avalonia` or `Microsoft.WindowsAppSDK`), or you are using the generator in a pure class library without defining the target platform.

✅ **Solution:**
Install the necessary NuGet packages or explicitly define the target framework compiler constant in your library.

```xml
<!-- Example: Explicitly defining a compiler constant in .csproj -->
<PropertyGroup>
    <DefineConstants>$(DefineConstants);HAS_WPF</DefineConstants>
</PropertyGroup>
```

---

### DPG0001: OnChanged Method Not Found or Unsupported

The method specified in the `OnChanged` attribute argument either does not exist in the class or has an unsupported signature.

❌ **Incorrect code:**

```csharp
[DependencyProperty<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyControl : UserControl
{
    // Error: The first argument must be the containing class type (MyControl).
    private void OnCountChanged(int oldValue, int newValue)
    {
    }
}
```

✅ **Correct code:**

```csharp
[DependencyProperty<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyControl : UserControl
{
    // Solution: Add the containing class type as the first argument.
    private void OnCountChanged(MyControl sender, int oldValue, int newValue)
    {
    }
}
```

---

### DPG0002: Invalid Type Modifier (File scoped)

You applied the generator to a class using the C# 11 `file` scope modifier. Roslyn Source Generators cannot generate code for file-scoped types.

❌ **Incorrect code:**

```csharp
[DependencyProperty<string>("Text")]
file partial class LocalControl : UserControl // Error: file scoped
{
}
```

✅ **Correct code:**

```csharp
[DependencyProperty<string>("Text")]
internal partial class LocalControl : UserControl // Solution: Use internal or public
{
}
```

---

### DPG0003: Invalid Property Type (Ref struct)

`ref struct` types (such as `ReadOnlySpan<T>`) cannot reside on the managed heap. Consequently, you cannot use them as a DependencyProperty type, which relies on boxing or object dictionaries.

❌ **Incorrect code:**

```csharp
// Error: ReadOnlySpan<char> cannot be boxed.
[DependencyProperty<ReadOnlySpan<char>>("Buffer")]
public partial class MyControl : UserControl
{
}
```

✅ **Correct code:**

```csharp
// Solution: Use a normal struct, array, or Memory<T>.
[DependencyProperty<ReadOnlyMemory<char>>("Buffer")]
public partial class MyControl : UserControl
{
}
```

---

### DPG0004: Reference Type Default Value Sharing

You assigned an instance of a reference type (such as a `class` or `List<T>`) directly to `DefaultValue`.
In frameworks like WPF, all control instances share reference type default values, causing memory leaks and shared-state bugs. The generator strictly blocks this to prevent such issues.

❌ **Incorrect code:**

```csharp
// Error: A single List instance will be shared by all MyControl instances.
[DependencyProperty<List<string>>("Items", DefaultValueExpression = "new()")]
public partial class MyControl : UserControl
{
}
```

✅ **Correct code:**

```csharp
// Solution: Use CreateDefaultValueCallback = true.
[DependencyProperty<List<string>>("Items", CreateDefaultValueCallback = true)]
public partial class MyControl : UserControl
{
    // The generator will wire up this partial method to instantiate a new object per instance.
    static partial void GetItemsDefaultValue(ref List<string> defaultValue)
    {
        defaultValue = new List<string>();
    }
}
```

---

### DPG0005: Invalid Callback Signature (OverrideMetadata)

Non-WPF platforms (such as UWP, WinUI, Uno, and MAUI) **do not provide the "old value"** when you override property metadata (`OverrideMetadata`).
This error occurs when your callback signature attempts to receive `oldValue` on a platform that does not support it.

❌ **Incorrect code:**

```csharp
// Error: The underlying framework (WinUI/Uno) cannot provide the old value.
[OverrideMetadata<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyWinUIControl : UserControl
{
    private void OnCountChanged(MyWinUIControl sender, int oldValue, int newValue) { }
}
```

✅ **Correct code:**

```csharp
[OverrideMetadata<int>("Count", OnChanged = nameof(OnCountChanged))]
public partial class MyWinUIControl : UserControl
{
    // Solution: Change the signature to only receive the new value.
    private void OnCountChanged(MyWinUIControl sender, int newValue) { }
}
```

---

### DPG0007: Unsupported Callback Signature

The generator found a method matching the `partial void On{PropertyName}Changed(...)` naming convention, but its argument signature is invalid (for example, it uses the generic `DependencyObject`).

❌ **Incorrect code:**

```csharp
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // Error: Using generic DependencyObject and RoutedEventArgs.
    partial void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
}
```

✅ **Correct code:**

```csharp
[DependencyProperty<string>("Text")]
public partial class MyControl : UserControl
{
    // Solution: Use strongly-typed arguments.
    partial void OnTextChanged(string? oldValue, string? newValue);

    // Or include the sender:
    // partial void OnTextChanged(MyControl sender, string? oldValue, string? newValue);
}
```

---

### DPG0008: Invalid Default Value Expression

The C# string expression provided in `DefaultValueExpression` contains syntax errors, causing the Roslyn parser to fail.

❌ **Incorrect code:**

```csharp
// Error: Missing closing parenthesis, typo, etc.
[DependencyProperty<string>("Text", DefaultValueExpression = "new(123, ")]
public partial class MyControl : UserControl
{
}
```

✅ **Correct code:**

```csharp
// Solution: Provide a valid C# expression string.
[DependencyProperty<string>("Text", DefaultValueExpression = "new(123, 456)")]
public partial class MyControl : UserControl
{
}
```
