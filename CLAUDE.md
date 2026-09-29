# Claude Code project guidance

This is a Terraria tModLoader mod. Work in this repository and read `DEVELOPMENT_MAC.md` plus the relevant spec before changing code. Run `./scripts/verify-mac.sh` for rules and packaging. The current Wave Country fight is the M9 duo encounter; some older README and acceptance text is stale.

When bitmap art is needed, communicate directly with Codex through the local bridge. Write a request using `art/requests/TEMPLATE.md` as `art/requests/<asset-id>.md`, then run `python3 scripts/art_bridge.py submit <asset-id>`. This launches a separate Codex CLI image worker using the built-in imagegen tool. Continue other code work while it runs. Poll `python3 scripts/art_bridge.py status <asset-id>` until it reports `delivered` or `failed`. Read `art/deliveries/<asset-id>/DELIVERY.md` and inspect the PNGs before integrating. If it fails, read `.art-bridge/<asset-id>.log` and report the concrete problem. Do not ask the user to relay prompts between Claude and Codex.

Keep your edits out of `art/deliveries/<asset-id>/` while its job is running. Codex only delivers assets there; you own integration into `ShinobiPrototype/Content/`, code changes, game build, and in-game verification. For revisions, create a new request ID such as `haku-mirror-v2` so previous assets remain reviewable.
