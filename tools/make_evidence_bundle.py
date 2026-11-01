#!/usr/bin/env python3
"""Maintain Knowledge/EvidenceBundle, a sanitized snapshot of reports. Standard library only.

    python Knowledge/tools/make_evidence_bundle.py           regenerate
    python Knowledge/tools/make_evidence_bundle.py --check   read-only, exit 1 if stale

Reads sources without changing them. Refuses to write if a forbidden pattern survives.
Raw logs, saves and per-object data are never included.
"""
import hashlib
import re
import sys
import tempfile
from pathlib import Path

KNOWLEDGE = Path(__file__).resolve().parents[1]
WORKSPACE = KNOWLEDGE.parent
BUNDLE = KNOWLEDGE / "EvidenceBundle"

# source path -> path inside the bundle
SOURCES = {
    "REPORT.md": "reports/REPORT.md",
    "EVIDENCE.md": "reports/EVIDENCE.md",
    "COVERAGE.md": "reports/COVERAGE.md",
    "PROGRESS.md": "reports/PROGRESS.md",
    "patches/dist/SHA256SUMS.txt": "release/SHA256SUMS.txt",
    "patches/PoVPlus.Compat/package/manifest.json": "release/manifest.json",
    "patches/PoVPlus.Compat/package/CHANGELOG.md": "release/CHANGELOG.md",
    "evidence/acceptance-1.0.1/final.txt": "results/acceptance-1.0.1-final.txt",
    "evidence/fdcheck/steps.txt": "results/fdcheck-1.0.1-steps.txt",
}
# Counts only: the full census holds per-object coordinates and saved field data.
CENSUS = ("evidence/ops/census-locations.txt", "results/wraeclast-location-counts.txt", re.compile(r"^(world=|UNKNOWN |LOC |containersScanned=)"))
# These stay exact apart from redaction: their own contents are checksums and metadata.
UNLABELLED = ("release/SHA256SUMS.txt", "release/manifest.json")
BANNER = {".md": "> **Sanitized snapshot, not raw evidence.** Generated from `{source}`. See `README.md` in this bundle.\n\n",
          ".txt": "# Sanitized snapshot, not raw evidence. Generated from {source}.\n"}
HEADER = ("# Sanitized snapshot, not raw evidence. Regenerate with tools/make_evidence_bundle.py.\n"
          "# Columns: SHA-256 of the source, SHA-256 of the bundled file, bundled path.\n"
          "# The two hashes differ by design: bundled files are redacted and labelled. Source hashes identify provenance only.\n")

REDACTIONS = (
    (re.compile(r"7656\d{13}"), "<steamid>"),
    (re.compile(r"ulfhedinn", re.I), "<real-character-1>"),
    (re.compile(r"mr\. wednesday", re.I), "<real-character-2>"),
    (re.compile(r"claudevtwo|claudesoloproj", re.I), "<test-character>"),
    (re.compile(r"saigremor", re.I), "<player-name>"),
    (re.compile(r"G:\\STEAM\\steamapps\\common", re.I), "<steam-library>"),
    (re.compile(r"C:\\Users\\[^\\\s`|)]+", re.I), "<user-home>"),
)
FORBIDDEN = re.compile(r"7656\d{13}|ulfhedinn|wednesday|saigremor|claudevtwo|claudesolo|donaldson|@gmail|C:\\Users|gh[pousr]_\w{20}|Bearer [A-Za-z0-9]", re.I)


def sanitize(text):
    for pattern, replacement in REDACTIONS:
        text = pattern.sub(replacement, text)
    return text


def sha(data):
    return hashlib.sha256(data).hexdigest()


def label(target, source, text):
    if target in UNLABELLED:
        return text
    return BANNER[Path(target).suffix].format(source=source) + text


def render():
    """Return {bundle path: text}, including the manifest. Exits if sanitization failed."""
    outputs, sources = {}, {}
    for source, target in SOURCES.items():
        outputs[target], sources[target] = label(target, source, sanitize((WORKSPACE / source).read_text(encoding="utf-8"))), source
    source, target, keep = CENSUS
    lines = (WORKSPACE / source).read_text(encoding="utf-8").splitlines()
    outputs[target], sources[target] = label(target, source, sanitize("\n".join(l for l in lines if keep.match(l)) + "\n")), source
    leaks = [t + ": " + m.group(0) for t, text in outputs.items() for m in FORBIDDEN.finditer(text)]
    if leaks:
        sys.exit("Refusing to continue, forbidden content survived:\n  " + "\n  ".join(leaks))
    rows = [" ".join((sha((WORKSPACE / sources[t]).read_bytes()), sha(outputs[t].encode("utf-8")), t)) for t in sorted(outputs)]
    outputs["MANIFEST.txt"] = HEADER + "\n".join(rows) + "\n"
    return outputs


def stale(outputs, bundle):
    """Return differences between expected outputs and the bundle folder. README.md is hand-written."""
    problems = []
    for target, text in outputs.items():
        path = bundle / target
        if not path.is_file():
            problems.append("missing " + target)
        elif path.read_bytes() != text.encode("utf-8"):
            problems.append("stale " + target)
    known = set(outputs) | {"README.md"}
    for path in bundle.rglob("*"):
        if path.is_file() and path.relative_to(bundle).as_posix() not in known:
            problems.append("unexpected " + path.relative_to(bundle).as_posix())
    return problems


def write(outputs, bundle):
    for target, text in outputs.items():
        path = bundle / target
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(text.encode("utf-8"))


def sources_present():
    return all((WORKSPACE / s).is_file() for s in list(SOURCES) + [CENSUS[0]])


def self_test():
    sample = r"mr. wednesday, Ulfhedinn, 76561198000000000, C:\Users\someone\x, G:\STEAM\steamapps\common\Valheim"
    clean = sanitize(sample)
    assert not FORBIDDEN.search(clean), clean
    assert r"<steam-library>\Valheim" in clean, clean
    assert FORBIDDEN.search(sample), "forbidden pattern must catch the raw sample"
    assert "Sanitized snapshot" in label("reports/x.md", "x.md", "body") and label("release/manifest.json", "m", "{}") == "{}"
    with tempfile.TemporaryDirectory() as tmp:
        bundle, outputs = Path(tmp), {"a/one.txt": "one\n", "two.txt": "two\n"}
        write(outputs, bundle)
        assert stale(outputs, bundle) == [], "a matching bundle must pass"
        (bundle / "two.txt").write_bytes(b"changed\n")
        (bundle / "a/one.txt").unlink()
        (bundle / "extra.txt").write_bytes(b"x")
        assert sorted(stale(outputs, bundle)) == ["missing a/one.txt", "stale two.txt", "unexpected extra.txt"], stale(outputs, bundle)


def main():
    self_test()
    if "--check" in sys.argv:
        if not sources_present():
            print("bundle: skipped, workspace sources are not here")
            return 0
        problems = stale(render(), BUNDLE)
        for line in problems:
            print("bundle: " + line)
        print("bundle: " + ("STALE, run tools/make_evidence_bundle.py" if problems else "current"))
        return 1 if problems else 0
    outputs = render()
    write(outputs, BUNDLE)
    print("wrote " + str(len(outputs)) + " files to " + str(BUNDLE))
    return 0


if __name__ == "__main__":
    sys.exit(main())
