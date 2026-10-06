#!/usr/bin/env python3
"""Player-facing text lives in the localization files, never in the code, and both languages carry the same keys.

Fails if a C# string literal in ShinobiPrototype/ holds Chinese (outside comments; a line marked "text-check: allow"
is let through, e.g. reading old save data), or if zh-Hans and en-US differ in keys or in {0}-style placeholders.
"""
import pathlib, re, sys

import hjson

ROOT = pathlib.Path(__file__).resolve().parents[1] / "ShinobiPrototype"
CJK = re.compile(r"[一-鿿]")
LITERAL = re.compile(r'\$?@?"(?:[^"\\]|\\.)*"')

def code_problems():
    for path in sorted(ROOT.rglob("*.cs")):
        if {"obj", "bin"} & set(path.parts):
            continue
        for number, line in enumerate(path.read_text(encoding="utf-8").splitlines(), 1):
            if "text-check: allow" in line or line.lstrip().startswith("//"):
                continue
            for literal in LITERAL.findall(line):
                if CJK.search(literal):
                    yield f"{path.relative_to(ROOT.parent)}:{number}: Chinese in code; move it to Localization: {literal[:60]}"

def flat(tree, prefix=""):
    out = {}
    for key, value in tree.items():
        if isinstance(value, dict):
            out.update(flat(value, f"{prefix}{key}."))
        else:
            out[prefix + key] = value
    return out

def text_problems():
    loc = ROOT / "Localization"
    zh = flat(hjson.load(open(loc / "zh-Hans_Mods.ShinobiPrototype.hjson", encoding="utf-8")))
    en = flat(hjson.load(open(loc / "en-US_Mods.ShinobiPrototype.hjson", encoding="utf-8")))
    for key in sorted(set(zh) - set(en)):
        yield f"en-US is missing {key}"
    for key in sorted(set(en) - set(zh)):
        yield f"zh-Hans is missing {key}"
    for key in sorted(set(zh) & set(en)):
        a, b = set(re.findall(r"\{\d+\}", str(zh[key]))), set(re.findall(r"\{\d+\}", str(en[key])))
        if a != b:
            yield f"{key}: placeholders differ (zh {sorted(a)}, en {sorted(b)})"
        if CJK.search(str(en[key])):
            yield f"{key}: Chinese in the English text"

problems = list(code_problems()) + list(text_problems())
for problem in problems:
    print(problem)
print(f"text check: {len(problems)} problem(s)")
sys.exit(1 if problems else 0)
