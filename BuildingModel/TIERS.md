## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

BuildingModel may use Utils. Utils must never use BuildingModel.

| Tier | Folder or Class | May depend on |
|---|---|---|
| 0 | `Params/` | Utils |
| 1 | `SubLevel`, `IFlattenable` | tier 0, Utils |
| 2 | `Level`, `Floor`, `Opening` | tiers 0-1, Utils |
| 3 | `Wall` | tiers 0-2, Utils |
| 4 | `Building` | tiers 0-3, Utils |

Each folder has its own namespaces and tiers.