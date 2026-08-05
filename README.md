# Structural Automation

## Dependency tiers

A project may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Project | May depend on | References |
|---|---|---|---|
| 0 | `Utils` | nothing | — |
| 0 | `AutoCadCommands` | nothing | — |
| 1 | `BuildingModel` | tier 0 | `Utils` |

Each project has its own tiers — see [Utils/TIERS.md](Utils/TIERS.md),
[BuildingModel/TIERS.md](BuildingModel/TIERS.md) and
[AutoCadCommands/TIERS.md](AutoCadCommands/TIERS.md).

The references are the `ProjectReference` entries in each `.csproj`.

`AutoCadCommands` is the AutoCAD plugin, holding the commands. It references the
managed AutoCAD assemblies straight from the install directory, and a build deploys it
into a bundle AutoCAD reads on startup — see the comments in
[AutoCadCommands.csproj](AutoCadCommands/AutoCadCommands.csproj).