const fs = require('fs');
const path = require('path');

const SPEC_DIR = 'spec';
const OUTPUT_DIR = 'wiki_dist';

const FILE_MAP = {
  // English
  'en/intro.md': 'EN-Intro.md',
  'en/01_faq_and_rationale.md': 'EN-01-FAQ-and-Rationale.md',
  'en/02_foundation_and_domain.md': 'EN-02-Foundation-and-Domain.md',
  'en/03_pipeline_architecture.md': 'EN-03-Pipeline-Architecture.md',
  'en/04_framework_strategies.md': 'EN-04-Framework-Strategies.md',
  'en/05_synthesis_and_performance.md': 'EN-05-Synthesis-and-Performance.md',
  'en/06_mathematical_model.md': 'EN-06-Mathematical-Complexity.md',
  'en/07_test_specification.md': 'EN-07-Test-Specification.md',
  'en/08_diagnostics_reference.md': 'EN-08-Diagnostics-Reference.md',
  // Japanese
  'ja/intro.md': 'JA-Intro.md',
  'ja/01_faq_and_rationale.md': 'JA-01-FAQ-and-Rationale.md',
  'ja/02_foundation_and_domain.md': 'JA-02-Foundation-and-Domain.md',
  'ja/03_pipeline_architecture.md': 'JA-03-Pipeline-Architecture.md',
  'ja/04_framework_strategies.md': 'JA-04-Framework-Strategies.md',
  'ja/05_synthesis_and_performance.md': 'JA-05-Synthesis-and-Performance.md',
  'ja/06_mathematical_model.md': 'JA-06-Mathematical-Complexity.md',
  'ja/07_test_specification.md': 'JA-07-Test-Specification.md',
  'ja/08_diagnostics_reference.md': 'JA-08-Diagnostics-Reference.md',
};

const LINK_REPLACEMENTS = [
  // Inside spec/en/
  { from: 'en', src: './intro.md', dest: 'EN-Intro' },
  { from: 'en', src: './01_faq_and_rationale.md', dest: 'EN-01-FAQ-and-Rationale' },
  { from: 'en', src: './02_foundation_and_domain.md', dest: 'EN-02-Foundation-and-Domain' },
  { from: 'en', src: './03_pipeline_architecture.md', dest: 'EN-03-Pipeline-Architecture' },
  { from: 'en', src: './04_framework_strategies.md', dest: 'EN-04-Framework-Strategies' },
  { from: 'en', src: './05_synthesis_and_performance.md', dest: 'EN-05-Synthesis-and-Performance' },
  { from: 'en', src: './06_mathematical_model.md', dest: 'EN-06-Mathematical-Complexity' },
  { from: 'en', src: './07_test_specification.md', dest: 'EN-07-Test-Specification' },
  { from: 'en', src: './08_diagnostics_reference.md', dest: 'EN-08-Diagnostics-Reference' },
  { from: 'en', src: '../ja/intro.md', dest: 'JA-Intro' },
  { from: 'en', src: '../ja/01_faq_and_rationale.md', dest: 'JA-01-FAQ-and-Rationale' },
  { from: 'en', src: '../ja/02_foundation_and_domain.md', dest: 'JA-02-Foundation-and-Domain' },
  { from: 'en', src: '../ja/03_pipeline_architecture.md', dest: 'JA-03-Pipeline-Architecture' },
  { from: 'en', src: '../ja/04_framework_strategies.md', dest: 'JA-04-Framework-Strategies' },
  { from: 'en', src: '../ja/05_synthesis_and_performance.md', dest: 'JA-05-Synthesis-and-Performance' },
  { from: 'en', src: '../ja/06_mathematical_model.md', dest: 'JA-06-Mathematical-Complexity' },
  { from: 'en', src: '../ja/07_test_specification.md', dest: 'JA-07-Test-Specification' },
  { from: 'en', src: '../ja/08_diagnostics_reference.md', dest: 'JA-08-Diagnostics-Reference' },

  // Inside spec/ja/
  { from: 'ja', src: './intro.md', dest: 'JA-Intro' },
  { from: 'ja', src: './01_faq_and_rationale.md', dest: 'JA-01-FAQ-and-Rationale' },
  { from: 'ja', src: './02_foundation_and_domain.md', dest: 'JA-02-Foundation-and-Domain' },
  { from: 'ja', src: './03_pipeline_architecture.md', dest: 'JA-03-Pipeline-Architecture' },
  { from: 'ja', src: './04_framework_strategies.md', dest: 'JA-04-Framework-Strategies' },
  { from: 'ja', src: './05_synthesis_and_performance.md', dest: 'JA-05-Synthesis-and-Performance' },
  { from: 'ja', src: './06_mathematical_model.md', dest: 'JA-06-Mathematical-Complexity' },
  { from: 'ja', src: './07_test_specification.md', dest: 'JA-07-Test-Specification' },
  { from: 'ja', src: './08_diagnostics_reference.md', dest: 'JA-08-Diagnostics-Reference' },
  { from: 'ja', src: '../en/intro.md', dest: 'EN-Intro' },
  { from: 'ja', src: '../en/01_faq_and_rationale.md', dest: 'EN-01-FAQ-and-Rationale' },
  { from: 'ja', src: '../en/02_foundation_and_domain.md', dest: 'EN-02-Foundation-and-Domain' },
  { from: 'ja', src: '../en/03_pipeline_architecture.md', dest: 'EN-03-Pipeline-Architecture' },
  { from: 'ja', src: '../en/04_framework_strategies.md', dest: 'EN-04-Framework-Strategies' },
  { from: 'ja', src: '../en/05_synthesis_and_performance.md', dest: 'EN-05-Synthesis-and-Performance' },
  { from: 'ja', src: '../en/06_mathematical_model.md', dest: 'EN-06-Mathematical-Complexity' },
  { from: 'ja', src: '../en/07_test_specification.md', dest: 'EN-07-Test-Specification' },
  { from: 'ja', src: '../en/08_diagnostics_reference.md', dest: 'EN-08-Diagnostics-Reference' },

  // Root spec/README.md links (Home.md)
  { from: 'root', src: './en/intro.md', dest: 'EN-Intro' },
  { from: 'root', src: './en/01_faq_and_rationale.md', dest: 'EN-01-FAQ-and-Rationale' },
  { from: 'root', src: './en/02_foundation_and_domain.md', dest: 'EN-02-Foundation-and-Domain' },
  { from: 'root', src: './en/03_pipeline_architecture.md', dest: 'EN-03-Pipeline-Architecture' },
  { from: 'root', src: './en/04_framework_strategies.md', dest: 'EN-04-Framework-Strategies' },
  { from: 'root', src: './en/05_synthesis_and_performance.md', dest: 'EN-05-Synthesis-and-Performance' },
  { from: 'root', src: './en/06_mathematical_model.md', dest: 'EN-06-Mathematical-Complexity' },
  { from: 'root', src: './en/07_test_specification.md', dest: 'EN-07-Test-Specification' },
  { from: 'root', src: './en/08_diagnostics_reference.md', dest: 'EN-08-Diagnostics-Reference' },
  { from: 'root', src: './ja/intro.md', dest: 'JA-Intro' },
  { from: 'root', src: './ja/01_faq_and_rationale.md', dest: 'JA-01-FAQ-and-Rationale' },
  { from: 'root', src: './ja/02_foundation_and_domain.md', dest: 'JA-02-Foundation-and-Domain' },
  { from: 'root', src: './ja/03_pipeline_architecture.md', dest: 'JA-03-Pipeline-Architecture' },
  { from: 'root', src: './ja/04_framework_strategies.md', dest: 'JA-04-Framework-Strategies' },
  { from: 'root', src: './ja/05_synthesis_and_performance.md', dest: 'JA-05-Synthesis-and-Performance' },
  { from: 'root', src: './ja/06_mathematical_model.md', dest: 'JA-06-Mathematical-Complexity' },
  { from: 'root', src: './ja/07_test_specification.md', dest: 'JA-07-Test-Specification' },
  { from: 'root', src: './ja/08_diagnostics_reference.md', dest: 'JA-08-Diagnostics-Reference' },
];

function transformContent(content, context) {
  let result = content;
  // Convert main repository relative links (../../path) to full GitHub URLs
  result = result.replace(/\(\.\.\/\.\.\/([^)]+)\)/g, '(https://github.com/kassyi/DependencyPropertyGenerator/tree/main/$1)');

  for (const item of LINK_REPLACEMENTS) {
    if (item.from === context) {
      result = result.split(`(${item.src})`).join(`(${item.dest})`);
    }
  }
  return result;
}

function generateSidebar() {
  return `### 📖 Specifications

* [[Home]]

#### 🇺🇸 English
* [[Introduction|EN-Intro]]
* [[01. FAQ & Design Rationale|EN-01-FAQ-and-Rationale]]
* [[02. Foundation & Domain|EN-02-Foundation-and-Domain]]
* [[03. Pipeline Architecture|EN-03-Pipeline-Architecture]]
* [[04. Framework Strategies|EN-04-Framework-Strategies]]
* [[05. Synthesis & Performance|EN-05-Synthesis-and-Performance]]
* [[06. Mathematical Complexity|EN-06-Mathematical-Complexity]]
* [[07. Test Specification|EN-07-Test-Specification]]
* [[08. Diagnostics Reference|EN-08-Diagnostics-Reference]]

#### 🇯🇵 日本語
* [[概要|JA-Intro]]
* [[01. 建築設計思想と FAQ|JA-01-FAQ-and-Rationale]]
* [[02. 基盤とドメイン|JA-02-Foundation-and-Domain]]
* [[03. パイプライン構造|JA-03-Pipeline-Architecture]]
* [[04. フレームワーク別生成仕様|JA-04-Framework-Strategies]]
* [[05. コード生成と最適化|JA-05-Synthesis-and-Performance]]
* [[06. 計算量モデル|JA-06-Mathematical-Complexity]]
* [[07. テスト仕様書|JA-07-Test-Specification]]
* [[08. 診断機能リファレンス|JA-08-Diagnostics-Reference]]

---
* [📦 GitHub Repository](https://github.com/kassyi/DependencyPropertyGenerator)
`;
}

function generateFooter() {
  return `---
*This wiki is automatically synchronized from [\`spec/\`](https://github.com/kassyi/DependencyPropertyGenerator/tree/main/spec) in the repository.*
`;
}

function main() {
  if (fs.existsSync(OUTPUT_DIR)) {
    fs.rmSync(OUTPUT_DIR, { recursive: true, force: true });
  }
  fs.mkdirSync(OUTPUT_DIR, { recursive: true });

  // 1. Home.md
  const homeRaw = fs.readFileSync(path.join(SPEC_DIR, 'README.md'), 'utf-8');
  fs.writeFileSync(path.join(OUTPUT_DIR, 'Home.md'), transformContent(homeRaw, 'root'), 'utf-8');

  // 2. Sidebar & Footer
  fs.writeFileSync(path.join(OUTPUT_DIR, '_Sidebar.md'), generateSidebar(), 'utf-8');
  fs.writeFileSync(path.join(OUTPUT_DIR, '_Footer.md'), generateFooter(), 'utf-8');

  // 3. Chapters
  for (const [srcRel, targetName] of Object.entries(FILE_MAP)) {
    const lang = srcRel.startsWith('en') ? 'en' : 'ja';
    const srcPath = path.join(SPEC_DIR, srcRel);
    const content = fs.readFileSync(srcPath, 'utf-8');
    fs.writeFileSync(path.join(OUTPUT_DIR, targetName), transformContent(content, lang), 'utf-8');
  }

  console.log(`Successfully generated ${Object.keys(FILE_MAP).length + 3} wiki pages into '${OUTPUT_DIR}'.`);
}

main();
