#!/usr/bin/env python3
"""Regenerate Knowledge/EvidenceBundle from the local workspace. Standard library only.

    python Knowledge/tools/make_evidence_bundle.py

Reads sources without changing them, writes sanitized copies, and refuses to write anything
if a forbidden pattern survives. Raw logs, saves and per-object data are never included.
"""
import hashlib
import re
import sys
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


def self_test():
    sample = r"mr. wednesday, Ulfhedinn, 76561198000000000, C:\Users\someone\x, G:\STEAM\steamapps\common\Valheim"
    clean = sanitize(sample)
    assert not FORBIDDEN.search(clean), clean
    assert r"<steam-library>\Valheim" in clean, clean
    assert FORBIDDEN.search(sample), "forbidden pattern must catch the raw sample"


def build():
    outputs = {}
    for source, target in SOURCES.items():
        outputs[target] = (source, sanitize((WORKSPACE / source).read_text(encoding="utf-8")))
    source, target, keep = CENSUS
    lines = (WORKSPACE / source).read_text(encoding="utf-8").splitlines()
    outputs[target] = (source, sanitize("\n".join(l for l in lines if keep.match(l)) + "\n"))
    leaks = [target + ": " + m.group(0) for target, (_, text) in outputs.items() for m in FORBIDDEN.finditer(text)]
    if leaks:
        sys.exit("Refusing to write, forbidden content survived:\n  " + "\n  ".join(leaks))
    manifest = ["# sha256(source)  sha256(bundle)  bundle path. Regenerate with make_evidence_bundle.py."]
    for target, (source, text) in sorted(outputs.items()):
        path = BUNDLE / target
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text, encoding="utf-8", newline="\n")
        manifest.append(" ".join((hashlib.sha256((WORKSPACE / source).read_bytes()).hexdigest()[:16], hashlib.sha256(path.read_bytes()).hexdigest()[:16], target)))
    (BUNDLE / "MANIFEST.txt").write_text("\n".join(manifest) + "\n", encoding="utf-8", newline="\n")
    print("wrote " + str(len(outputs)) + " files to " + str(BUNDLE))


if __name__ == "__main__":
    self_test()
    build()
