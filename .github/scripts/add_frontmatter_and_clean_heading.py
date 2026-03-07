#!/usr/bin/env python3
"""
Process MkDocs markdown files to produce clean page titles and headings.

For each file (except index.md and hand-crafted static content):

1. Frontmatter title
   - If no 'title' key exists: extract the first heading (# / ## / ###),
     strip any leading section-number prefix (e.g. "15.3. ") and set it
     as the frontmatter title.
   - Always strip bold markers (**...**) from the title value, whether the
     title was just set or already present from a previous run.

2. Top-level heading removal
   - If the first heading is plain (no **), remove it from the body to avoid
     duplicating the page title that the Material theme renders from frontmatter.
   - Bold headings (e.g. "### **Title**") are kept in the body so their
     formatting still renders on the page.

3. Sub-heading cleanup
   - Strip leading section-number prefixes from ### / #### / ##### headings
     in the body (e.g. "### 15.3. Some Title" → "### Some Title").
   - Headings inside fenced code blocks are never modified.
"""

import re
from pathlib import Path
import frontmatter

DOCS_DIR = Path("mkdocs")

# These folders contain hand-crafted static content — never touch them.
SKIP_DIRS = {
    DOCS_DIR / "architecture" / "solid",
    DOCS_DIR / "code-examples" / "samples",
    DOCS_DIR / "reference" / "api-doc",
}

# Matches a section-number prefix at the start of a string: "15.", "15.3.", "16.4.1.", …
SECTION_NUMBER_RE = re.compile(r'^[\d]+(?:\.[\d]+)*\.\s*')

# Matches ### / #### / ##### headings that start with a section-number prefix
SUBHEADING_NUMBER_RE = re.compile(r'^(#{3,5} )(\d+(?:\.\d+)*\.\s+)(.*)')


def should_skip(filepath):
    if filepath.name == "index.md":
        return True
    for skip_dir in SKIP_DIRS:
        try:
            filepath.relative_to(skip_dir)
            return True
        except ValueError:
            pass
    return False


def _is_code_fence(line):
    stripped = line.strip()
    return stripped.startswith('```') or stripped.startswith('~~~')


def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    try:
        post = frontmatter.loads(content)
        body = post.content
        metadata = post.metadata
    except Exception:
        body = content
        metadata = {}

    lines = body.splitlines()

    # ── Pass 1: find the first heading for the frontmatter title ────────────
    heading_line_idx = None
    heading_text = None
    in_code_block = False

    for i, line in enumerate(lines):
        if _is_code_fence(line):
            in_code_block = not in_code_block
            continue
        if in_code_block:
            continue
        stripped = line.strip()
        if stripped.startswith('# '):
            heading_text = stripped[2:].strip()
            heading_line_idx = i
            break
        elif stripped.startswith('## '):
            heading_text = stripped[3:].strip()
            heading_line_idx = i
            break
        elif stripped.startswith('### '):
            heading_text = stripped[4:].strip()
            heading_line_idx = i
            break

    if heading_text and not metadata.get('title'):
        metadata['title'] = SECTION_NUMBER_RE.sub('', heading_text)

    # Always strip bold markers from title (new or pre-existing)
    if metadata.get('title'):
        metadata['title'] = re.sub(r'\*\*(.*?)\*\*', r'\1', metadata['title'])

    # Remove plain top-level heading — bold headings are kept so they render
    if heading_line_idx is not None and not re.search(r'\*\*', heading_text or ''):
        del lines[heading_line_idx]

    # ── Pass 2: strip section-number prefixes from ### / #### / ##### headings
    in_code_block = False
    for i, line in enumerate(lines):
        if _is_code_fence(line):
            in_code_block = not in_code_block
            continue
        if in_code_block:
            continue
        m = SUBHEADING_NUMBER_RE.match(line)
        if m:
            lines[i] = m.group(1) + m.group(3)

    new_body = '\n'.join(lines).strip()
    new_post = frontmatter.Post(new_body, **metadata)
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(frontmatter.dumps(new_post))
    print(f"✅ Processed: {filepath}")


def main():
    if not DOCS_DIR.exists():
        print(f"❌ {DOCS_DIR} not found.")
        return

    for md_file in DOCS_DIR.rglob("*.md"):
        if should_skip(md_file):
            continue
        process_file(md_file)


if __name__ == "__main__":
    main()
