#!/usr/bin/env python3
"""
Rename all .md files in docs/ (recursively) to lowercase, except index.md.
"""

from pathlib import Path

DOCS_DIR = Path("mkdocs")

def main():
    if not DOCS_DIR.exists():
        print("❌ docs/ directory not found.")
        return

    for file in DOCS_DIR.rglob("*.md"):
        if file.name.lower() == "index.md":
            continue
        lower_name = file.name.lower()
        if lower_name != file.name:
            new_path = file.with_name(lower_name)
            print(f"Renaming {file} -> {new_path}")
            file.rename(new_path)

if __name__ == "__main__":
    main()