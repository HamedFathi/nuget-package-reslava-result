#!/usr/bin/env python3
"""
Clean up after mdsplit:
- Delete any file directly under docs/ named REslava.Result*.md (H1 file).
- Find a folder named REslava.Result* under docs/, move all its .md files up, then remove the folder.
"""

import shutil
from pathlib import Path

DOCS_DIR = Path("mkdocs")
PREFIX = "REslava.Result"  # The start of the H1 heading, used for both file and folder

def main():
    if not DOCS_DIR.exists():
        print("❌ docs/ directory not found.")
        return

    # 1. Delete the H1 file in the root
    for file in DOCS_DIR.glob(f"{PREFIX}*.md"):
        print(f"🗑️  Deleting H1 file: {file.name}")
        file.unlink()

    # 2. Find the folder starting with the same prefix
    folders = [item for item in DOCS_DIR.iterdir() if item.is_dir() and item.name.startswith(PREFIX)]
    if not folders:
        print("✅ No mdsplit folder found.")
        return

    # Assume only one such folder exists
    folder = folders[0]
    print(f"📂 Found folder: {folder.name}/")

    # Move all .md files from that folder to docs/ root
    moved = 0
    for md_file in folder.glob("*.md"):
        dest = DOCS_DIR / md_file.name
        # Avoid overwriting index.md (just in case)
        if dest.exists() and dest.name == "index.md":
            print(f"⚠️  Skipping {md_file.name} because {dest.name} already exists.")
            continue
        print(f"  Moving {md_file.name} -> docs/")
        shutil.move(str(md_file), str(dest))
        moved += 1

    if moved == 0:
        print("  No files moved (folder empty?).")
    else:
        print(f"  Moved {moved} file(s).")

    # Remove the now‑empty folder
    try:
        folder.rmdir()
        print(f"  Removed empty folder: {folder.name}/")
    except OSError:
        print(f"  ⚠️  Could not remove folder – maybe not empty?")
        # List remaining files for debugging
        remaining = list(folder.iterdir())
        if remaining:
            print(f"     Remaining items: {[f.name for f in remaining]}")

if __name__ == "__main__":
    main()