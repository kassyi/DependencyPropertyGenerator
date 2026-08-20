const fs = require("fs");
const path = require("path");

const SPEC_DIR = "spec";
const OUTPUT_DIR = "wiki_dist";
const REPO_URL = "https://github.com/kassyi/DependencyPropertyGenerator/tree/main";

const CHAPTERS = [
  { name: "intro", en: "Introduction", ja: "概要" },
  { name: "01_faq_and_rationale", en: "01. FAQ & Design Rationale", ja: "01. 設計思想とFAQ" },
  { name: "02_foundation_and_domain", en: "02. Foundation & Domain", ja: "02. 基盤とドメイン" },
  { name: "03_pipeline_architecture", en: "03. Pipeline Architecture", ja: "03. パイプライン構造" },
  { name: "04_framework_strategies", en: "04. Framework Strategies", ja: "04. フレームワーク別生成仕様" },
  { name: "05_synthesis_and_performance", en: "05. Synthesis & Performance", ja: "05. コード生成と最適化" },
  { name: "06_mathematical_model", en: "06. Complexity Model", ja: "06. 計算量モデル" },
  { name: "07_test_specification", en: "07. Test Specification", ja: "07. テスト仕様書" },
  { name: "08_diagnostics_reference", en: "08. Diagnostics Reference", ja: "08. 診断機能リファレンス" },
].map((c) => ({
  ...c,
  file: `${c.name}.md`,
  wiki: c.wiki || c.name,
}));

const FILE_MAP = Object.fromEntries(
  CHAPTERS.flatMap((c) => [
    [`en/${c.file}`, `EN-${c.wiki}.md`],
    [`ja/${c.file}`, `JA-${c.wiki}.md`],
  ]),
);

const LINK_REPLACEMENTS = {
  en: CHAPTERS.flatMap((c) => [
    { src: `./${c.file}`, dest: `EN-${c.wiki}` },
    { src: `../ja/${c.file}`, dest: `JA-${c.wiki}` },
  ]),
  ja: CHAPTERS.flatMap((c) => [
    { src: `./${c.file}`, dest: `JA-${c.wiki}` },
    { src: `../en/${c.file}`, dest: `EN-${c.wiki}` },
  ]),
  root: CHAPTERS.flatMap((c) => [
    { src: `./en/${c.file}`, dest: `EN-${c.wiki}` },
    { src: `./ja/${c.file}`, dest: `JA-${c.wiki}` },
  ]),
};

function buildNavLine(chapter, lang) {
  if (chapter.name === "intro") {
    return "[🇺🇸 English](EN-intro) | [🇯🇵 日本語](JA-intro)";
  }
  const indexTarget = lang === "en" ? "EN-intro" : "JA-intro";
  return `[🇺🇸 English](EN-${chapter.wiki}) | [🇯🇵 日本語](JA-${chapter.wiki}) | [Introduction](${indexTarget})`;
}

function injectNavigation(content, navLine) {
  // Insert navigation line right after the first H1 header
  const withHeaderNav = content.replace(/^(#\s+[^\r\n]+)/m, `$1\n\n${navLine}`);
  // Strip any trailing horizontal rule and whitespace, then append footer
  const cleaned = withHeaderNav.replace(/(\r?\n\s*---\s*)*$/, "").trimEnd();
  return `${cleaned}\n\n---\n\n${navLine}\n`;
}

function transformContent(content, context, filename = null) {
  let result = content.replace(/\(\.\.\/\.\.\/([^)]+)\)/g, `(${REPO_URL}/$1)`);

  const replacements = LINK_REPLACEMENTS[context] || [];
  for (const { src, dest } of replacements) {
    const escapedSrc = src.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    const regex = new RegExp(`\\(${escapedSrc}(#[^)]*)?\\)`, "g");
    result = result.replace(regex, (match, anchor) => `(${dest}${anchor || ""})`);
  }

  if (filename && (context === "en" || context === "ja")) {
    const chapter = CHAPTERS.find((c) => c.file === filename);
    if (chapter) {
      const navLine = buildNavLine(chapter, context);
      result = injectNavigation(result, navLine);
    }
  }

  return result;
}

function generateSidebar() {
  const enLinks = CHAPTERS.map((c) => `* [[${c.en}|EN-${c.wiki}]]`).join("\n");
  const jaLinks = CHAPTERS.map((c) => `* [[${c.ja}|JA-${c.wiki}]]`).join("\n");

  return `### 📖 Specifications

* [[Home]]

#### 🇺🇸 English
${enLinks}

#### 🇯🇵 日本語
${jaLinks}

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
  const homeRaw = fs.readFileSync(path.join(SPEC_DIR, "README.md"), "utf-8");
  fs.writeFileSync(path.join(OUTPUT_DIR, "Home.md"), transformContent(homeRaw, "root"), "utf-8");

  // 2. Sidebar & Footer
  fs.writeFileSync(path.join(OUTPUT_DIR, "_Sidebar.md"), generateSidebar(), "utf-8");
  fs.writeFileSync(path.join(OUTPUT_DIR, "_Footer.md"), generateFooter(), "utf-8");

  // 3. Chapters
  for (const [srcRel, targetName] of Object.entries(FILE_MAP)) {
    const lang = srcRel.startsWith("en") ? "en" : "ja";
    const srcPath = path.join(SPEC_DIR, srcRel);
    const content = fs.readFileSync(srcPath, "utf-8");
    const transformed = transformContent(content, lang, path.basename(srcRel));
    fs.writeFileSync(path.join(OUTPUT_DIR, targetName), transformed, "utf-8");
  }

  console.log(`Successfully generated ${Object.keys(FILE_MAP).length + 3} wiki pages into '${OUTPUT_DIR}'.`);
}

main();
