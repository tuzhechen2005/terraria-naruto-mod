#!/usr/bin/env python3
"""Wait on Codex art jobs (scripts/art_bridge.py): resubmit a job that failed because the model was at capacity
(after a pause, one at a time), submit queued ids as running jobs drop below --max, and exit printing the ids as soon
as any job is delivered or fails for another reason.

Usage: python3 scripts/art_wait.py ID [ID ...] [--max 2]
"""
import argparse, json, pathlib, shutil, subprocess, sys, time

root = pathlib.Path(__file__).resolve().parent.parent
p = argparse.ArgumentParser()
p.add_argument("ids", nargs="+")
p.add_argument("--max", type=int, default=2)
a = p.parse_args()

def status(i):
    out = subprocess.run([sys.executable, str(root / "scripts/art_bridge.py"), "status", i], capture_output=True, text=True, cwd=root).stdout
    try:
        return json.loads(out)["status"]
    except Exception:
        return "unknown"

def submit(i):
    shutil.rmtree(root / "art/deliveries" / i, ignore_errors=True)
    subprocess.run([sys.executable, str(root / "scripts/art_bridge.py"), "submit", i], capture_output=True, cwd=root)

started = set()
while True:
    states = {i: status(i) for i in a.ids}
    for i, s in states.items():
        if s == "running":
            started.add(i)
    finished = [f"{i}:{s}" for i, s in states.items() if i in started and s in ("delivered",)]
    broken = []
    for i, s in states.items():
        if i in started and s == "failed":
            log = (root / ".art-bridge" / f"{i}.log")
            if log.exists() and "at capacity" in log.read_text(errors="ignore"):
                time.sleep(90)
                submit(i)
            else:
                broken.append(f"{i}:failed")
    if finished or broken:
        print(" ".join(finished + broken), flush=True)
        break
    running = sum(1 for s in states.values() if s == "running")
    for i in a.ids:
        if running >= a.max:
            break
        if i not in started:
            submit(i)
            started.add(i)
            running += 1
    time.sleep(30)
