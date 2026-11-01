#!/usr/bin/env python3
"""Maintain EvidenceBundle, a sanitized snapshot. Standard library only.

    python tools/make_evidence_bundle.py --verify   any clone: check bundled files against MANIFEST.tsv
    python tools/make_evidence_bundle.py --check    also compare with local originals, if present
    python tools/make_evidence_bundle.py            regenerate; needs the original workspace

--verify and --check only read. Regenerating overwrites the bundle, so review what changed first.
Private terms come from an untracked local file, never from this repository.
"""
import hashlib
import re
import sys
import tempfile
from pathlib import Path

KNOWLEDGE = Path(__file__).resolve().parents[1]
WORKSPACE = KNOWLEDGE.parent
BUNDLE = KNOWLEDGE / "EvidenceBundle"
TERMS = WORKSPACE / "tools" / "bundle-redactions.local"  # regex<TAB>replacement, one per line

COMPAT = "yes, in PoVPlus.Compat"
LOCAL = "no, local only"
FD_CLIENT = r"^\S+ (FDCLOCK|FDRULE|FDSTATION|FDCONV|FDEXPIRE|FDSTAMP|CONVT|PICKALL|CMD rem)"
FD_SERVER = r"^\S+ (RULE|CONV|CONVT|STAMP|EXPIRE|---)"
# source, bundle path, original available to reviewers, optional line filter
ENTRIES = (
    ("REPORT.md", "reports/REPORT.md", LOCAL, None),
    ("EVIDENCE.md", "reports/EVIDENCE.md", LOCAL, None),
    ("COVERAGE.md", "reports/COVERAGE.md", LOCAL, None),
    ("PROGRESS.md", "reports/PROGRESS.md", LOCAL, None),
    ("patches/dist/SHA256SUMS.txt", "release/SHA256SUMS.txt", LOCAL, None),
    ("patches/PoVPlus.Compat/package/manifest.json", "release/manifest.json", COMPAT, None),
    ("patches/PoVPlus.Compat/package/CHANGELOG.md", "release/CHANGELOG.md", COMPAT, None),
    ("evidence/acceptance-1.0.1/final.txt", "results/acceptance-1.0.1-final.txt", LOCAL, None),
    ("evidence/acceptance/final.txt", "results/acceptance-1.0.0-final.txt", LOCAL, None),
    ("evidence/fdcheck/steps.txt", "results/fdcheck-1.0.1-steps.txt", LOCAL, None),
    ("evidence/fdcheck/probe-log.txt", "results/fdcheck-1.0.1-client.txt", LOCAL, FD_CLIENT),
    ("evidence/fdcheck/server-probe.txt", "results/fdcheck-1.0.1-server.txt", LOCAL, FD_SERVER),
    ("evidence/fdcheck/run2-baseline/probe-log.txt", "results/fdcheck-1.0.0-client.txt", LOCAL, FD_CLIENT),
    ("evidence/fdcheck/run2-baseline/server-probe.txt", "results/fdcheck-1.0.0-server.txt", LOCAL, FD_SERVER),
    ("evidence/ops/graceful.txt", "results/graceful-shutdown.txt", LOCAL, None),
    ("evidence/compat/chips-toggle.txt", "results/map-chips-toggle.txt", LOCAL, None),
    ("evidence/compat/smelter-boost-start.txt", "results/smelter-boost-start.txt", LOCAL, None),
    ("evidence/ops/census-locations.txt", "results/wraeclast-location-counts.txt", LOCAL, r"^(world=|UNKNOWN |LOC |containersScanned=)"),
    ("tools/ClientProbe/ClientProbe.cs", "harness/ClientProbe/ClientProbe.cs", LOCAL, None),
    ("tools/ClientProbe/ClientProbe.csproj", "harness/ClientProbe/ClientProbe.csproj", LOCAL, None),
    ("tools/FdServerProbe/Plugin.cs", "harness/FdServerProbe/Plugin.cs", LOCAL, None),
    ("tools/FdServerProbe/FdServerProbe.csproj", "harness/FdServerProbe/FdServerProbe.csproj", LOCAL, None),
    ("tools/WorldCensus/Plugin.cs", "harness/WorldCensus/Plugin.cs", LOCAL, None),
    ("tools/WorldCensus/WorldCensus.csproj", "harness/WorldCensus/WorldCensus.csproj", LOCAL, None),
    ("tools/fdcheck.ps1", "harness/fdcheck.ps1", LOCAL, None),
    ("tools/finalrun.ps1", "harness/finalrun.ps1", LOCAL, None),
    ("tools/compatsetup.ps1", "harness/compatsetup.ps1", LOCAL, None),
    ("tools/server.ps1", "harness/server.ps1", LOCAL, None),
    ("tools/joinserver.ps1", "harness/joinserver.ps1", LOCAL, None),
    ("tools/snapshot.ps1", "harness/snapshot.ps1", LOCAL, None),
    ("tools/missingprefab.ps1", "harness/missingprefab.ps1", LOCAL, None),
    ("tools/memlog.ps1", "harness/memlog.ps1", LOCAL, None),
    ("tools/ui.ps1", "harness/ui.ps1", LOCAL, None),
    ("tools/pc.ps1", "harness/pc.ps1", LOCAL, None),
)
BANNER = {".md": "> **Sanitized snapshot, not raw evidence.** Generated from `{source}`. See `README.md` in this bundle.\n\n",
          ".txt": "# Sanitized snapshot, not raw evidence. Generated from {source}.\n",
          ".ps1": "# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from {source}.\n",
          ".cs": "// Sanitized snapshot for reading. Generated from {source}.\n"}
UNLABELLED = ("release/SHA256SUMS.txt", "release/manifest.json")  # their contents are checksums and metadata
MANIFEST = "MANIFEST.tsv"
HEADER = "\t".join(("source_id", "source_sha256", "bundle_path", "bundle_sha256", "transformation", "original_available"))

GENERIC = ((re.compile(r"7656\d{13}"), "<steamid>"), (re.compile(r"[A-Z]:\\Users\\[^\\\s`|)'\"]+", re.I), "<user-home>"))
FORBIDDEN = re.compile(r"7656\d{13}|[A-Z]:\\Users\\[A-Za-z]|gh[pousr]_\w{20}|Bearer [A-Za-z0-9]{8}|[\w.+-]+@gmail\.")


def sha(data):
    return hashlib.sha256(data).hexdigest()


def load_terms():
    if not TERMS.is_file():
        sys.exit("Refusing to regenerate: " + str(TERMS) + " is missing.")
    terms = []
    for line in TERMS.read_text(encoding="utf-8").splitlines():
        if line.strip():
            pattern, replacement = line.split("\t")
            terms.append((re.compile(pattern), replacement))
    return terms


def sanitize(text, terms):
    for pattern, replacement in tuple(terms) + GENERIC:
        text = pattern.sub(replacement, text)
    return text


def label(target, source, text):
    suffix = Path(target).suffix
    return text if target in UNLABELLED or suffix not in BANNER else BANNER[suffix].format(source=source) + text


def render():
    """Return {bundle path: text}, including the manifest. Exits if sanitization failed."""
    terms = load_terms()
    outputs, rows = {}, [HEADER]
    for source, target, available, keep in sorted(ENTRIES, key=lambda e: e[1]):
        raw = (WORKSPACE / source).read_bytes()
        text = raw.decode("utf-8").replace("\r\n", "\n")
        steps = []
        if keep:
            pattern = re.compile(keep)
            text = "\n".join(l for l in text.split("\n") if pattern.match(l)) + "\n"
            steps.append("line filter")
        clean = sanitize(text, terms)
        steps.append("redacted" if clean != text else "no redactions needed")
        final = label(target, source, clean)
        if final != clean:
            steps.append("banner")
        outputs[target] = final
        rows.append("\t".join((source, sha(raw), target, sha(final.encode("utf-8")), "; ".join(steps), available)))
    outputs[MANIFEST] = "\n".join(rows) + "\n"
    leaks = [t + ": " + m.group(0)[:12] for t, text in outputs.items() for m in FORBIDDEN.finditer(text)]
    leaks += [t + ": private term" for t, text in outputs.items() for pattern, _ in terms if pattern.search(text)]
    if leaks:
        sys.exit("Refusing to continue, forbidden content survived:\n  " + "\n  ".join(leaks))
    return outputs


def stale(outputs, bundle):
    """Return differences between expected outputs and the folder. README.md is hand-written."""
    problems = []
    for target, text in outputs.items():
        path = bundle / target
        if not path.is_file():
            problems.append("missing " + target)
        elif path.read_bytes() != text.encode("utf-8"):
            problems.append("stale " + target)
    known = set(outputs) | {"README.md"}
    problems += ["unexpected " + p.relative_to(bundle).as_posix() for p in bundle.rglob("*") if p.is_file() and p.relative_to(bundle).as_posix() not in known]
    return problems


def verify(bundle):
    """Check bundled files against the manifest. Needs no original files."""
    path = bundle / MANIFEST
    if not path.is_file():
        return ["missing " + MANIFEST]
    problems, listed = [], {MANIFEST, "README.md"}
    for row in path.read_text(encoding="utf-8").splitlines()[1:]:
        fields = row.split("\t")
        if len(fields) != 6:
            problems.append("malformed manifest row: " + row[:40])
            continue
        target = bundle / fields[2]
        listed.add(fields[2])
        if not target.is_file():
            problems.append("missing " + fields[2])
        elif sha(target.read_bytes()) != fields[3]:
            problems.append("hash mismatch " + fields[2])
    problems += ["unlisted " + p.relative_to(bundle).as_posix() for p in bundle.rglob("*") if p.is_file() and p.relative_to(bundle).as_posix() not in listed]
    for p in bundle.rglob("*"):
        if p.is_file() and p.name != "README.md":
            if FORBIDDEN.search(p.read_text(encoding="utf-8", errors="replace")):
                problems.append("forbidden pattern in " + p.relative_to(bundle).as_posix())
    return problems


def write(outputs, bundle):
    for target, text in outputs.items():
        path = bundle / target
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(text.encode("utf-8"))


def sources_present():
    return TERMS.is_file() and all((WORKSPACE / e[0]).is_file() for e in ENTRIES)


def self_test():
    terms = [(re.compile("Fictional Name", re.I), "<name>")]
    sample = r"fictional name, 76561198000000000, C:\Users\someone\x"
    clean = sanitize(sample, terms)
    assert not FORBIDDEN.search(clean) and "<name>" in clean, clean
    assert FORBIDDEN.search(sample), "forbidden pattern must catch the raw sample"
    assert "Sanitized snapshot" in label("reports/x.md", "x.md", "body") and label("release/manifest.json", "m", "{}") == "{}"
    with tempfile.TemporaryDirectory() as tmp:
        bundle, outputs = Path(tmp), {"a/one.txt": "one\n", "two.txt": "two\n"}
        rows = [HEADER] + ["\t".join(("s", "0", t, sha(v.encode()), "x", LOCAL)) for t, v in outputs.items()]
        outputs[MANIFEST] = "\n".join(rows) + "\n"
        write(outputs, bundle)
        assert stale(outputs, bundle) == [] and verify(bundle) == [], "a matching bundle must pass"
        (bundle / "two.txt").write_bytes(b"changed\n")
        assert verify(bundle) == ["hash mismatch two.txt"], verify(bundle)
        (bundle / "a/one.txt").unlink()
        (bundle / "extra.txt").write_bytes(b"x")
        assert sorted(stale(outputs, bundle)) == ["missing a/one.txt", "stale two.txt", "unexpected extra.txt"], stale(outputs, bundle)
        assert "unlisted extra.txt" in verify(bundle), verify(bundle)


def main():
    self_test()
    if "--verify" in sys.argv or "--check" in sys.argv:
        problems = verify(BUNDLE)
        for line in problems:
            print("bundle: " + line)
        print("bundle manifest: " + ("FAILED" if problems else "ok, hashes match"))
        if "--check" in sys.argv:
            if sources_present():
                changed = stale(render(), BUNDLE)
                for line in changed:
                    print("bundle: " + line)
                print("bundle originals: " + ("STALE, review the changes, then regenerate" if changed else "current"))
                problems += changed
            else:
                print("bundle originals: not checked, they are not on this machine")
        return 1 if problems else 0
    write(render(), BUNDLE)
    print("regenerated " + str(BUNDLE) + ". Review the diff before committing.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
