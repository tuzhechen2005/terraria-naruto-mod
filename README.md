<p align="center">
  <img src="docs/images/mod-icon.png" alt="Konoha leaf emblem" width="160">
</p>

<h1 align="center">Terraria Naruto Mod</h1>

<p align="center">Naruto Part I, from the Land of Waves to the Chunin Exams.</p>

<p align="center">
  <a href="README.md"><img src="https://img.shields.io/badge/English-f08c28?style=for-the-badge" alt="English"></a>
  <a href="README.zh-CN.md"><img src="https://img.shields.io/badge/%E4%B8%AD%E6%96%87-586069?style=for-the-badge" alt="切换为中文"></a>
</p>

<p align="center">
  <a href="https://github.com/tuzhechen2005/terraria-naruto-mod/actions/workflows/tests.yml"><img src="https://github.com/tuzhechen2005/terraria-naruto-mod/actions/workflows/tests.yml/badge.svg" alt="CI"></a>
  <a href="https://github.com/tuzhechen2005/terraria-naruto-mod/stargazers"><img src="https://img.shields.io/github/stars/tuzhechen2005/terraria-naruto-mod?style=flat&logo=github&label=stars&color=f08c28" alt="GitHub stars"></a>
  <img src="https://img.shields.io/badge/version-0.4.0-e05a2b" alt="version 0.4.0">
  <img src="https://img.shields.io/badge/tModLoader-1.4.4-4e7d32" alt="tModLoader 1.4.4">
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-3a6ea5" alt="MIT license"></a>
</p>

---

Shinobi Prototype is a Naruto fan mod for tModLoader. It follows the story of Part I alongside Terraria's existing progression, with all vanilla bosses retained. You start in the Hidden Leaf Village, travel to the Land of Waves with Kakashi's guidance, and return for the Chunin Exams after completing the mission.

## Game content

Development has reached the Chunin Exams arc. The Land of Waves is playable from beginning to end: you meet Tazuna by the coast, survive an ambush by the Demon Brothers, free Kakashi from a water prison at the lake, and face Zabuza and Haku on the unfinished bridge. The arc closes with an epilogue after both bosses fall.

The Chunin Exams begin with Ibiki's written test. After passing, you enter the Forest of Death to collect the Heaven and Earth Scrolls, fight Dosu in the Central Tower's preliminary round, and face Gaara in the finals outside the village walls. The main sequence is implemented and is being refined through playtesting, including adjustments to combat, guidance, and the exam sites. Some characters still use placeholder art.

The mod adds a separate chakra resource and a substitution technique, bound to F by default and activated just before an attack lands. Sharingan, Eight Gates, and Byakugan are available as style cores that can be used with vanilla classes. The Hidden Leaf Village is generated at spawn in new worlds and serves as a base between missions.

## Playing and building

The mod runs on tModLoader for Terraria 1.4.4. **Start a new world** when playing for the first time: the village, coastal bridge, and exam sites are placed during world generation and are not added to existing worlds. Playtesting has focused on single player; multiplayer has not been verified. The Ninja Handbook and Kakashi's dialogue provide clues about where to go next.

To build from source, place or symlink the repository's `ShinobiPrototype/` directory in tModLoader's `ModSources/` directory. Open **Workshop → Develop Mods** in the game and select **Build + Reload**.

The project is developed on macOS. Setup instructions are in [DEVELOPMENT_MAC.md](DEVELOPMENT_MAC.md), written in Chinese. You can also run `./scripts/verify-mac.sh` from the repository root to run the rule tests and build the mod package. Close the game before packaging with the script, since it holds the mod file open while running. Use Build + Reload for rebuilding in the game.

<details>
<summary>Single-player debug commands</summary>

Enter `/m0` to see the full command list. These commands let you jump between story stages, watch the epilogue, or visit the exam sites during playtesting.

| Command | Effect |
| --- | --- |
| `/m0 god on` / `/m0 god off` | Enable or disable invincibility |
| `/m0 story 1` through `/m0 story 5` | Jump to a Land of Waves mission stage |
| `/m0 epilogue zabuza` | Play Zabuza and Haku's epilogue |
| `/m0 exam ForestGate` | Set progress to just after the written test |
| `/m0 exam gate` / `/m0 exam tower` / `/m0 exam stadium` | Teleport to the forest entrance, Central Tower, or finals stadium |
| `/m0 exam rebuild` | Rebuild the Forest of Death site in the current world |

</details>

## Development

The mod source and textures are in `ShinobiPrototype/`, and the arc designs are in `specs/`. The `tests/` directory contains rule tests and in-game acceptance checklists. Logic for enemy spawns, boss move selection, and site layouts is kept independent of Terraria so it can be tested on its own. GitHub Actions runs these tests on pushes to `main` and on pull requests.

Art requests and deliveries are in `art/`. Build scripts, world generation checks, and pixel art tools are in `scripts/`; earlier documents are archived in `docs/archive/`. See [CONTRIBUTING.md](CONTRIBUTING.md) for playtesting feedback, bug reports, and contributions. The design documents and contribution guide are currently written in Chinese.

## License

This is an unofficial fan project. The code and original art are released under the [MIT license](LICENSE). Naruto's characters, names, and designs belong to Masashi Kishimoto, Shueisha, and their respective rights holders; Terraria belongs to Re-Logic. The public repository contains no anime soundtracks, voice recordings, or official images. See [NOTICE](NOTICE) for the rights statement.
