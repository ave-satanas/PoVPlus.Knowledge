#!/usr/bin/env python3
"""Read-only checks for the PoVPlus knowledge base. Standard library only.

    python Knowledge/tools/validate.py                  self-test, then vault
    python Knowledge/tools/validate.py --applies EpicLoot   records to review after that mod updates

Failures: missing metadata, bad values, duplicate ids, broken links, unknown related or
superseded records, missing entry points. Review information never fails the run.
It checks structure only. It cannot find semantic contradictions or prove compatibility.
"""
import argparse
import re
import sys
import tempfile
from pathlib import Path

REQUIRED = ("id", "type", "status", "created", "updated")
TYPES = {"index", "state", "schema", "system", "mod", "compat", "investigation", "decision",
         "operation", "diagnostic", "session", "inventory", "historical"}
STATUSES = {"active", "open", "resolved", "bounded", "retired", "superseded", "deferred", "blocked", "historical"}
ENTRY_POINTS = ("INDEX.md", "CURRENT-STATE.md", "SCHEMA.md")
LINK_FIELDS = ("related", "supersedes", "superseded_by")
SKIP_DIRS = {"Templates", "tools", ".obsidian"}
DATE = re.compile(r"^\d{4}-\d{2}-\d{2}$")
WIKILINK = re.compile(r"!?\[\[([^\]]+)\]\]")
FENCE = re.compile(r"^(```|~~~).*?^\1\s*$", re.M | re.S)
CODE_SPAN = re.compile(r"`[^`\n]*`")


def parse(text):
    """Return (metadata, body), or (None, text) without frontmatter. Handles the flat keys and lists SCHEMA.md allows."""
    if not text.startswith("---\n"):
        return None, text
    end = text.find("\n---", 4)
    if end < 0:
        return None, text
    meta, key = {}, None
    for line in text[4:end].splitlines():
        if not line.strip() or line.lstrip().startswith("#"):
            continue
        if line.startswith("  - ") and key:
            meta[key].append(line[4:].strip().strip('"'))
            continue
        name, _, value = line.partition(":")
        key, value = name.strip(), value.strip()
        if value == "":
            meta[key] = []
        elif value.startswith("[") and value.endswith("]"):
            meta[key] = [v.strip().strip('"') for v in value[1:-1].split(",") if v.strip()]
        else:
            meta[key] = value.strip('"')
    return meta, text[end + 4:].lstrip("\n")


def headings(body):
    return {re.sub(r"\s+", " ", m.group(1)).strip().lower() for m in re.finditer(r"^#{1,6}\s+(.+?)\s*$", body, re.M)}


def as_list(value):
    return value if isinstance(value, list) else [value]


def load(root):
    notes = []
    for path in sorted(root.rglob("*.md")):
        if SKIP_DIRS & set(path.relative_to(root).parts[:-1]):
            continue
        meta, body = parse(path.read_text(encoding="utf-8"))
        notes.append({"path": path, "rel": path.relative_to(root).as_posix(), "meta": meta, "body": body})
    return notes


def validate(root):
    errors, review = [], []
    for name in ENTRY_POINTS:
        if not (root / name).is_file():
            errors.append("missing entry point: " + name)
    notes = load(root)
    by_id, by_stem = {}, {n["path"].stem: n for n in notes}
    for note in notes:
        rel, meta = note["rel"], note["meta"]
        if meta is None:
            errors.append(rel + ": no frontmatter")
            continue
        for field in REQUIRED:
            if not meta.get(field):
                errors.append(rel + ": missing " + field)
        if meta.get("type") and meta["type"] not in TYPES:
            errors.append(rel + ": unknown type " + str(meta["type"]))
        if meta.get("status") and meta["status"] not in STATUSES:
            errors.append(rel + ": unknown status " + str(meta["status"]))
        for field in ("created", "updated", "last_verified"):
            if meta.get(field) and not DATE.match(str(meta[field])):
                errors.append(rel + ": " + field + " is not an ISO date")
        note_id = meta.get("id")
        if note_id:
            if note_id in by_id:
                errors.append("duplicate id " + note_id + ": " + by_id[note_id]["rel"] + ", " + rel)
            by_id[note_id] = note
            if note_id != note["path"].stem:
                errors.append(rel + ": id does not match filename")
    linked = set()
    for note in notes:
        meta = note["meta"] or {}
        for field in LINK_FIELDS:
            for target in as_list(meta.get(field, [])):
                if target and target not in by_id:
                    errors.append(note["rel"] + ": " + field + " names unknown record " + target)
                linked.add(target)
        text = CODE_SPAN.sub("", FENCE.sub("", note["body"]))
        for raw in WIKILINK.findall(text):
            target = raw.split("|")[0].strip()
            name, _, heading = target.partition("#")
            name = name.strip()
            if name.endswith(".md"):
                name = name[:-3]
            destination = by_stem.get(name) if name else note
            if destination is None:
                errors.append(note["rel"] + ": broken link [[" + target + "]]")
                continue
            linked.add(destination["path"].stem)
            if heading and re.sub(r"\s+", " ", heading).strip().lower() not in headings(destination["body"]):
                errors.append(note["rel"] + ": no heading in [[" + target + "]]")
    for note in notes:
        meta = note["meta"] or {}
        if meta.get("type") == "investigation" and meta.get("status") in ("open", "bounded"):
            review.append("unresolved (" + meta["status"] + "): " + note["rel"])
        if note["path"].stem not in linked and note["rel"] not in ENTRY_POINTS:
            review.append("orphan: " + note["rel"])
    return notes, errors, review


def self_test():
    good = "---\nid: {i}\ntype: decision\nstatus: active\ncreated: 2026-01-01\nupdated: 2026-01-01\nrelated: [{r}]\n---\n# Head\n{b}\n"
    entry = {"INDEX": "---\nid: INDEX\ntype: index\nstatus: active\ncreated: 2026-01-01\nupdated: 2026-01-01\n---\n[[A]] [[B|alias]] [[A#Head]] [[CURRENT-STATE]] [[SCHEMA]]\n",
             "CURRENT-STATE": "---\nid: CURRENT-STATE\ntype: state\nstatus: active\ncreated: 2026-01-01\nupdated: 2026-01-01\n---\n",
             "SCHEMA": "---\nid: SCHEMA\ntype: schema\nstatus: active\ncreated: 2026-01-01\nupdated: 2026-01-01\n---\n"}

    def run(extra):
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            for name, text in entry.items():
                (root / (name + ".md")).write_text(text, encoding="utf-8")
            (root / "A.md").write_text(good.format(i="A", r="B", b="`[[not-a-link]]` C:\\Users\\x\\file.md"), encoding="utf-8")
            (root / "B.md").write_text(good.format(i="B", r="A", b="[[A]]"), encoding="utf-8")
            for name, text in extra.items():
                (root / name).parent.mkdir(exist_ok=True)
                (root / name).write_text(text, encoding="utf-8")
            return validate(root)[1]

    assert run({}) == [], "a valid linked vault must pass: " + repr(run({}))
    cases = {"duplicate id": {"sub/C.md": good.format(i="A", r="", b="")},
             "missing status": {"C.md": "---\nid: C\ntype: decision\ncreated: 2026-01-01\nupdated: 2026-01-01\n---\n"},
             "broken link": {"C.md": good.format(i="C", r="", b="[[Nope]]")},
             "no heading": {"C.md": good.format(i="C", r="", b="[[A#Missing]]")},
             "unknown record": {"C.md": good.format(i="C", r="Ghost", b="")}}
    for label, extra in cases.items():
        found = " ".join(run(extra))
        assert found, label + " was not detected"
        assert label.split()[-1] in found or label == "unknown record" and "unknown record" in found, label + ": " + found
    return len(cases) + 1


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--applies", help="list records whose applies_to names this mod")
    args = parser.parse_args()
    if args.applies:
        needle = args.applies.lower()
        for note in load(args.root):
            hits = [v for v in as_list((note["meta"] or {}).get("applies_to", [])) if needle in v.lower()]
            if hits:
                print(note["rel"] + "  [" + ", ".join(hits) + "]  status=" + note["meta"].get("status", "?"))
        return 0
    print("self-test: " + str(self_test()) + " cases passed")
    notes, errors, review = validate(args.root)
    print("records: " + str(len(notes)))
    for line in review:
        print("review: " + line)
    for line in errors:
        print("ERROR: " + line)
    print("result: " + ("FAILED, " + str(len(errors)) + " error(s)" if errors else "ok"))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
