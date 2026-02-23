#!/usr/bin/env python3
"""
Scans all markdown files in docs/ and fixes broken internal links.
- Converts all internal links to absolute paths (starting with /).
- Builds a map of all existing .md files and their headings (with anchors).
- For each link pointing to a missing .md file, attempts to find a better target.
- If link text matches a heading in another file, links to that file + anchor.
- Otherwise, falls back to a default file (like advanced-patterns.md) and warns.
- Also replaces external references (CONTRIBUTING.md, LICENSE, etc.) with absolute GitHub URLs.
- Dry-run mode by default; use --apply to actually modify files.
"""

import sys
import io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

import re
from pathlib import Path

DOCS_DIR = Path("mkdocs")
# Known external files that should point to GitHub (use exact target strings)
EXTERNAL_MAP = {
    "CONTRIBUTING.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/CONTRIBUTING.md",
    "CONTRIBUTORS.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/CONTRIBUTING.md",
    "LICENSE": "https://github.com/reslava/nuget-package-reslava-result/blob/main/LICENSE.txt",
    "docs/how-to-create-custom-generator.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/how-to-create-custom-generator.md",
    "docs/getting-started.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/getting-started.md",
    "docs/aspnet-integration.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/aspnet-integration.md",
    "docs/oneof-extensions.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/oneof-extensions.md",
    "docs/source-generator.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/source-generator.md",
    "docs/functional-programming.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/functional-programming.md",
    "samples/FastMinimalAPI.REslava.Result.Demo/README.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/samples/FastMinimalAPI.REslava.Result.Demo/README.md",
    "samples/FastMvcAPI.REslava.Result.Demo/": "https://github.com/reslava/nuget-package-reslava-result/tree/main/samples/FastMvcAPI.REslava.Result.Demo",
    "samples/REslava.Result.Samples.Console/README.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/samples/REslava.Result.Samples.Console/README.md",
    "samples/ASP.NET/README.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/samples/ASP.NET/README.md",
    "docs/uml/UML-v1.12.1-core.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/uml/UML-v1.12.1-core.md",
    "docs/uml/UML-v1.12.1-generators.md": "https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/uml/UML-v1.12.1-generators.md",
}

# Fallback mapping for common missing files (if heading matching fails)
FALLBACK_MAP = {
    "result-pattern.md": "/core-concepts/reslava.result-core-library.md#core-operations",
    "functional-composition.md": "/core-concepts/reslava.result-core-library.md#functional-composition",
    "async-patterns.md": "/core-concepts/reslava.result-core-library.md#async-patterns-whenall-retry-timeout",
    "linq-integration.md": "/core-concepts/reslava.result-core-library.md#-linq-integration",
    "maybe.md": "/core-concepts/advanced-patterns.md#-maybe---safe-null-handling",
    "oneof.md": "/core-concepts/advanced-patterns.md#-oneof---discriminated-unions",
    "validation.md": "/core-concepts/advanced-patterns.md#-validation-framework",
    "domain-errors.md": "/core-concepts/advanced-patterns.md#️-domain-error-hierarchy-v1200",
    "error-context.md": "/core-concepts/advanced-patterns.md#️-rich-error-context",
    "performance.md": "/core-concepts/advanced-patterns.md#-performance-patterns",
    "minimal-api.md": "/aspnet/asp.net-integration.md#-resulttoiresult-extensions",
    "mvc.md": "/aspnet/asp.net-integration.md#-resulttoactionresult-extensions-mvc-support--v1210",
    "smartendpoints.md": "/aspnet/asp.net-integration.md#-smartendpoints---zero-boilerplate-fast-apis",
    "openapi.md": "/aspnet/asp.net-integration.md#-enhanced-smartendpoints--openapi-metadata-new",
    "authorization.md": "/aspnet/asp.net-integration.md#-smartendpoints-authorization--policy-support",
    "problem-details.md": "/aspnet/asp.net-integration.md#-problem-details-integration",
    "generators.md": "/architecture/complete-architecture.md#-source-generators-reslavaresultsourcegenerators",
    "scenarios.md": "/getting-started/quick-start-scenarios.md",
    "transformation.md": "/getting-started/the-transformation-70-90-less-code.md",
    "test-suite.md": "/testing/testing--quality-assurance.md#-comprehensive-test-suite",
    "ci-cd.md": "/testing/testing--quality-assurance.md#-cicd-pipeline",
}

def extract_heading_anchors(filepath):
    """Return a dict mapping heading text (lowercase) to anchor."""
    anchors = {}
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    heading_pattern = re.compile(r'^(#{1,6})\s+(.+)$', re.MULTILINE)
    for match in heading_pattern.finditer(content):
        heading = match.group(2).strip()
        heading = re.sub(r'\*\*(.*?)\*\*', r'\1', heading)
        anchor = re.sub(r'[^\w\s-]', '', heading).lower().replace(' ', '-')
        anchors[heading.lower()] = anchor
    return anchors

def build_heading_map():
    heading_map = {}
    for md_file in DOCS_DIR.rglob("*.md"):
        rel_path = md_file.relative_to(DOCS_DIR)
        anchors = extract_heading_anchors(md_file)
        for heading_text, anchor in anchors.items():
            heading_map[heading_text] = (str(rel_path).replace('\\', '/'), anchor)
    return heading_map

def fix_links_in_file(filepath, heading_map, dry_run=True):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    modified = False
    link_pattern = re.compile(r'\[([^\]]+)\]\(([^)]+)\)')
    new_content = content

    for match in link_pattern.finditer(content):
        link_text = match.group(1)
        target = match.group(2)

        # Skip external URLs
        if target.startswith(('http://', 'https://', 'ftp://')):
            continue
        # Skip anchors within the same page
        if target.startswith('#'):
            continue

        # Strip anchor from target to check the base
        base_target = target.split('#')[0]
        if not base_target:
            continue

        # --- IMPORTANT: Check EXTERNAL_MAP FIRST ---
        # This must be done before any .md filtering so that non-.md targets like LICENSE are caught.
        if target in EXTERNAL_MAP:          # exact match on full target (including anchor if any)
            new_target = EXTERNAL_MAP[target]
            print(f"In {filepath.relative_to(DOCS_DIR)}: replacing external link '{link_text}' -> '{new_target}'")
            new_content = new_content.replace(match.group(0), f'[{link_text}]({new_target})')
            modified = True
            continue

        # If the link points to a directory (no .md), skip (handled by MkDocs)
        if not base_target.endswith('.md'):
            continue

        # Now handle .md files
        target_path = DOCS_DIR / base_target
        if target_path.exists():
            # Make it absolute (starting with /)
            if not base_target.startswith('/'):
                new_target = '/' + base_target
                # Re-add anchor if present
                if '#' in target:
                    new_target += '#' + target.split('#')[1]
                print(f"In {filepath.relative_to(DOCS_DIR)}: making link absolute: '{target}' -> '{new_target}'")
                new_content = new_content.replace(match.group(0), f'[{link_text}]({new_target})')
                modified = True
            continue

        # Broken link: base_target .md file doesn't exist.
        print(f"In {filepath.relative_to(DOCS_DIR)}: broken link '{link_text}' -> '{target}'")

        # Check if the base filename is in FALLBACK_MAP
        target_filename = base_target.split('/')[-1]
        if target_filename in FALLBACK_MAP:
            new_target = FALLBACK_MAP[target_filename]
            print(f"  ↳ Using fallback: {new_target}")
            new_content = new_content.replace(match.group(0), f'[{link_text}]({new_target})')
            modified = True
            continue

        # Try to match link text to a heading in heading_map
        link_text_lower = link_text.lower()
        if link_text_lower in heading_map:
            file_path, anchor = heading_map[link_text_lower]
            new_target = f"/{file_path}#{anchor}"
            print(f"  ↳ Found matching heading in {new_target}")
            new_content = new_content.replace(match.group(0), f'[{link_text}]({new_target})')
            modified = True
            continue

        # If all else fails, suggest manual review
        print(f"  ⚠️  No automatic fix found for '{target}'. Please check manually.")

    if modified and not dry_run:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"✅ Updated {filepath.relative_to(DOCS_DIR)}")
    elif modified and dry_run:
        print(f"🔍 Dry run: would update {filepath.relative_to(DOCS_DIR)}")
    return modified

def main():
    dry_run = True
    if len(sys.argv) > 1 and sys.argv[1] == '--apply':
        dry_run = False
        print("🔧 Applying changes (modifying files)...")
    else:
        print("🔍 Dry-run mode (no files modified). Use --apply to apply changes.")

    print("Building heading map...")
    heading_map = build_heading_map()
    print(f"Found {len(heading_map)} headings across {len(list(DOCS_DIR.rglob('*.md')))} files.")

    any_modified = False
    for md_file in DOCS_DIR.rglob("*.md"):
        if fix_links_in_file(md_file, heading_map, dry_run):
            any_modified = True

    if not any_modified:
        print("No broken links found.")
    else:
        if dry_run:
            print("\nDry-run completed. Run with --apply to actually modify files.")
        else:
            print("\nAll fixes applied. Please review changes with git diff and test locally.")

if __name__ == "__main__":
    main()