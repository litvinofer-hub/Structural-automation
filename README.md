# Structural Automation

## Dependency layers

A project may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Project | May depend on | References |
|---|---|---|---|
| 0 | `Utils` | nothing | — |
| 0 | `AutoCadCommands` | nothing | — |
| 1 | `BuildingModel` | layer 0 | `Utils` |

Each project has its own layering — see [Utils/LAYERS.md](Utils/LAYERS.md),
[BuildingModel/LAYERS.md](BuildingModel/LAYERS.md) and
[AutoCadCommands/LAYERS.md](AutoCadCommands/LAYERS.md).

The references are the `ProjectReference` entries in each `.csproj`.

`AutoCadCommands` is the AutoCAD plugin, holding the commands. It references the
managed AutoCAD assemblies straight from the install directory, and a build deploys it
into a bundle AutoCAD reads on startup — see the comments in
[AutoCadCommands.csproj](AutoCadCommands/AutoCadCommands.csproj).