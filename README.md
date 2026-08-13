# Structural Automation

## Dependency tiers

A project may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Project | May depend on | References |
|---|---|---|---|
| 0 | `Utils` | nothing | — |
| 1 | `BuildingModel` | tier 0 | `Utils` |
| 2 | `AutoCadCommands` | tiers 0-1 | `Utils` |

Each project has its own tiers.
Each folder has its own namespaces and tiers.