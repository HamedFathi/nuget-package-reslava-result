#!/usr/bin/env python3
"""
Remove leading emojis from level‑2 headings (##) in all .md files under mkdocs/.
Emoji ranges: U+1F000–U+1F9FF, U+2600–U+26FF, U+2700–U+27BF.
Dry‑run mode by default; use --apply to modify files.
"""

import argparse
from pathlib import Path

DOCS_DIR = Path("mkdocs")

# Unicode ranges for emojis and common symbols (inclusive)
EMOJI_RANGES = [
    (0x1F000, 0x1F9FF),  # Miscellaneous Symbols and Pictographs, Emoticons, etc.
    (0x2600, 0x26FF),    # Miscellaneous Symbols
    (0x2700, 0x27BF),    # Dingbats
]

def is_emoji(char):
    """Return True if the character's code point falls in any emoji range."""
    code = ord(char)
    return any(start <= code <= end for start, end in EMOJI_RANGES)

def process_file(filepath, dry_run=True):
    """Process a single file: remove emoji prefixes from ## headings."""
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    modified = False
    new_lines = []

    for line in lines:
        if line.startswith('## '):
            # Look at the part after "## "
            rest = line[3:]
            i = 0
            # Count consecutive emoji characters
            while i < len(rest) and is_emoji(rest[i]):
                i += 1
            # If we found at least one emoji and the next character is a space
            if i > 0 and i < len(rest) and rest[i] == ' ':
                # Remove the emoji run and the following space
                new_line = '## ' + rest[i+1:]
                new_lines.append(new_line)
                modified = True
                print(f"In {filepath}: removed emoji from: {line.strip()}")
            else:
                new_lines.append(line)
        else:
            new_lines.append(line)

    if modified and not dry_run:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.writelines(new_lines)
        print(f"✅ Updated {filepath}")
    elif modified and dry_run:
        print(f"🔍 Dry run: would update {filepath}")
    return modified

def main():
    parser = argparse.ArgumentParser(description="Remove leading emojis from ## headings.")
    parser.add_argument(
        "--dry-run",
        action="store_true",
        default=True,
        help="Preview changes (default: true)"
    )
    parser.add_argument(
        "--no-dry-run",
        action="store_false",
        dest="dry_run",
        help="Actually modify files (use with caution!)"
    )
    args = parser.parse_args()

    if not DOCS_DIR.exists():
        print(f"❌ Directory {DOCS_DIR} not found.")
        return

    all_files = list(DOCS_DIR.rglob("*.md"))
    if not all_files:
        print(f"No .md files found in {DOCS_DIR}.")
        return

    any_modified = False
    for md_file in all_files:
        if process_file(md_file, args.dry_run):
            any_modified = True

    if not any_modified:
        print("No headings needed cleaning.")
    else:
        if args.dry_run:
            print("\nDry‑run completed. Run with --no-dry-run to apply changes.")
        else:
            print("\nAll changes applied. Please review with git diff.")

if __name__ == "__main__":
    main()