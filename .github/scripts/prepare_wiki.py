import os
import re
import shutil

SPEC_DIR = "spec"
OUTPUT_DIR = "wiki_dist"

FILE_MAP = {
    # English
    ("en", "intro.md"): "EN-Intro.md",
    ("en", "01_foundation_and_domain.md"): "EN-01-Foundation-and-Domain.md",
    ("en", "02_pipeline_architecture.md"): "EN-02-Pipeline-Architecture.md",
    ("en", "03_synthesis_and_performance.md"): "EN-03-Synthesis-and-Performance.md",
    ("en", "04_mathematical_model.md"): "EN-04-Mathematical-Complexity.md",
    ("en", "05_test_specification.md"): "EN-05-Test-Specification.md",
    ("en", "06_framework_strategies.md"): "EN-06-Framework-Strategies.md",
    ("en", "07_diagnostics_reference.md"): "EN-07-Diagnostics-Reference.md",
    # Japanese
    ("ja", "intro.md"): "JA-Intro.md",
    ("ja", "01_foundation_and_domain.md"): "JA-01-Foundation-and-Domain.md",
    ("ja", "02_pipeline_architecture.md"): "JA-02-Pipeline-Architecture.md",
    ("ja", "03_synthesis_and_performance.md"): "JA-03-Synthesis-and-Performance.md",
    ("ja", "04_mathematical_model.md"): "JA-04-Mathematical-Complexity.md",
    ("ja", "05_test_specification.md"): "JA-05-Test-Specification.md",
    ("ja", "06_framework_strategies.md"): "JA-06-Framework-Strategies.md",
    ("ja", "07_diagnostics_reference.md"): "JA-07-Diagnostics-Reference.md",
}

LINK_REPLACEMENTS = {
    # Relative links inside spec/en/
    ("./intro.md", "en"): "EN-Intro",
    ("./01_foundation_and_domain.md", "en"): "EN-01-Foundation-and-Domain",
    ("./02_pipeline_architecture.md", "en"): "EN-02-Pipeline-Architecture",
    ("./03_synthesis_and_performance.md", "en"): "EN-03-Synthesis-and-Performance",
    ("./04_mathematical_model.md", "en"): "EN-04-Mathematical-Complexity",
    ("./05_test_specification.md", "en"): "EN-05-Test-Specification",
    ("./06_framework_strategies.md", "en"): "EN-06-Framework-Strategies",
    ("./07_diagnostics_reference.md", "en"): "EN-07-Diagnostics-Reference",
    ("../ja/intro.md", "en"): "JA-Intro",
    ("../ja/01_foundation_and_domain.md", "en"): "JA-01-Foundation-and-Domain",
    ("../ja/02_pipeline_architecture.md", "en"): "JA-02-Pipeline-Architecture",
    ("../ja/03_synthesis_and_performance.md", "en"): "JA-03-Synthesis-and-Performance",
    ("../ja/04_mathematical_model.md", "en"): "JA-04-Mathematical-Complexity",
    ("../ja/05_test_specification.md", "en"): "JA-05-Test-Specification",
    ("../ja/06_framework_strategies.md", "en"): "JA-06-Framework-Strategies",
    ("../ja/07_diagnostics_reference.md", "en"): "JA-07-Diagnostics-Reference",

    # Relative links inside spec/ja/
    ("./intro.md", "ja"): "JA-Intro",
    ("./01_foundation_and_domain.md", "ja"): "JA-01-Foundation-and-Domain",
    ("./02_pipeline_architecture.md", "ja"): "JA-02-Pipeline-Architecture",
    ("./03_synthesis_and_performance.md", "ja"): "JA-03-Synthesis-and-Performance",
    ("./04_mathematical_model.md", "ja"): "JA-04-Mathematical-Complexity",
    ("./05_test_specification.md", "ja"): "JA-05-Test-Specification",
    ("./06_framework_strategies.md", "ja"): "JA-06-Framework-Strategies",
    ("./07_diagnostics_reference.md", "ja"): "JA-07-Diagnostics-Reference",
    ("../en/intro.md", "ja"): "EN-Intro",
    ("../en/01_foundation_and_domain.md", "ja"): "EN-01-Foundation-and-Domain",
    ("../en/02_pipeline_architecture.md", "ja"): "EN-02-Pipeline-Architecture",
    ("../en/03_synthesis_and_performance.md", "ja"): "EN-03-Synthesis-and-Performance",
    ("../en/04_mathematical_model.md", "ja"): "EN-04-Mathematical-Complexity",
    ("../en/05_test_specification.md", "ja"): "EN-05-Test-Specification",
    ("../en/06_framework_strategies.md", "ja"): "EN-06-Framework-Strategies",
    ("../en/07_diagnostics_reference.md", "ja"): "EN-07-Diagnostics-Reference",

    # Root spec/README.md links (Home.md)
    ("./en/intro.md", "root"): "EN-Intro",
    ("./en/01_foundation_and_domain.md", "root"): "EN-01-Foundation-and-Domain",
    ("./en/02_pipeline_architecture.md", "root"): "EN-02-Pipeline-Architecture",
    ("./en/03_synthesis_and_performance.md", "root"): "EN-03-Synthesis-and-Performance",
    ("./en/04_mathematical_model.md", "root"): "EN-04-Mathematical-Complexity",
    ("./en/05_test_specification.md", "root"): "EN-05-Test-Specification",
    ("./en/06_framework_strategies.md", "root"): "EN-06-Framework-Strategies",
    ("./en/07_diagnostics_reference.md", "root"): "EN-07-Diagnostics-Reference",
    ("./ja/intro.md", "root"): "JA-Intro",
    ("./ja/01_foundation_and_domain.md", "root"): "JA-01-Foundation-and-Domain",
    ("./ja/02_pipeline_architecture.md", "root"): "JA-02-Pipeline-Architecture",
    ("./ja/03_synthesis_and_performance.md", "root"): "JA-03-Synthesis-and-Performance",
    ("./ja/04_mathematical_model.md", "root"): "JA-04-Mathematical-Complexity",
    ("./ja/05_test_specification.md", "root"): "JA-05-Test-Specification",
    ("./ja/06_framework_strategies.md", "root"): "JA-06-Framework-Strategies",
    ("./ja/07_diagnostics_reference.md", "root"): "JA-07-Diagnostics-Reference",
}

def transform_content(content, context):
    for (src, ctx), dest in LINK_REPLACEMENTS.items():
        if ctx == context:
            content = content.replace(f"({src})", f"({dest})")
    return content

def generate_sidebar():
    return """### 📖 Specifications

* [[Home]]

#### 🇺🇸 English
* [[Introduction|EN-Intro]]
* [[01. Foundation & Domain|EN-01-Foundation-and-Domain]]
* [[02. Pipeline Architecture|EN-02-Pipeline-Architecture]]
* [[03. Synthesis & Performance|EN-03-Synthesis-and-Performance]]
* [[04. Mathematical Complexity|EN-04-Mathematical-Complexity]]
* [[05. Test Specification|EN-05-Test-Specification]]
* [[06. Framework Strategies|EN-06-Framework-Strategies]]
* [[07. Diagnostics Reference|EN-07-Diagnostics-Reference]]

#### 🇯🇵 日本語
* [[概要|JA-Intro]]
* [[01. 基盤とドメイン|JA-01-Foundation-and-Domain]]
* [[02. パイプライン構造|JA-02-Pipeline-Architecture]]
* [[03. コード生成と最適化|JA-03-Synthesis-and-Performance]]
* [[04. 計算量モデル|JA-04-Mathematical-Complexity]]
* [[05. テスト仕様書|JA-05-Test-Specification]]
* [[06. フレームワーク別生成仕様|JA-06-Framework-Strategies]]
* [[07. 診断機能リファレンス|JA-07-Diagnostics-Reference]]

---
* [📦 GitHub Repository](https://github.com/kassyi/DependencyPropertyGenerator)
"""

def generate_footer():
    return """---
*This wiki is automatically synchronized from [`spec/`](https://github.com/kassyi/DependencyPropertyGenerator/tree/main/spec) in the repository.*
"""

def main():
    if os.path.exists(OUTPUT_DIR):
        shutil.rmtree(OUTPUT_DIR)
    os.makedirs(OUTPUT_DIR, exist_ok=True)

    # 1. Home.md
    with open(os.path.join(SPEC_DIR, "README.md"), "r", encoding="utf-8") as f:
        home_content = transform_content(f.read(), "root")
    with open(os.path.join(OUTPUT_DIR, "Home.md"), "w", encoding="utf-8") as f:
        f.write(home_content)

    # 2. Sidebar & Footer
    with open(os.path.join(OUTPUT_DIR, "_Sidebar.md"), "w", encoding="utf-8") as f:
        f.write(generate_sidebar())
    with open(os.path.join(OUTPUT_DIR, "_Footer.md"), "w", encoding="utf-8") as f:
        f.write(generate_footer())

    # 3. All Chapter files
    for (lang, src_filename), target_filename in FILE_MAP.items():
        src_path = os.path.join(SPEC_DIR, lang, src_filename)
        dest_path = os.path.join(OUTPUT_DIR, target_filename)
        with open(src_path, "r", encoding="utf-8") as f:
            content = transform_content(f.read(), lang)
        with open(dest_path, "w", encoding="utf-8") as f:
            f.write(content)

    print(f"Successfully generated {len(FILE_MAP) + 3} wiki pages into '{OUTPUT_DIR}'.")

if __name__ == "__main__":
    main()
