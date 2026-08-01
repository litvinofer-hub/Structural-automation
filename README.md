# Structural Automation

## Dependency layers

A project may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Project | May depend on | References |
|---|---|---|---|
| 0 | `Utils` | nothing | — |
| 0 | `AutoCadCommands` | nothing | — |
| 1 | `BuildingModel` | layer 0 | `Utils` |

Each project has its own layering — see [Utils/LAYERS.md](Utils/LAYERS.md) and
[BuildingModel/LAYERS.md](BuildingModel/LAYERS.md).

The references are the `ProjectReference` entries in each `.csproj`.

`AutoCadCommands` stands alone: it depends on no project, and no project depends on
it. It is the AutoCAD plugin — a dll loaded by AutoCAD with NETLOAD, holding the
commands. It references the managed AutoCAD assemblies straight from the install
directory, overridable with the `AutoCadDir` property.