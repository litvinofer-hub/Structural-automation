## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic. Tiers are ours; layers are AutoCAD's, and the two mean different things here.

Folders group by role and the namespaces follow them, so a tier is readable from the
path a file sits at.

| Tier | Folder | Class | May depend on |
|---|---|---|---|
| 0 | `Layers/` | `SaLayer`, `SaLayerColors`, `SaAnnotations`, `LayerReport` | nothing |
| 0 | `Plans/` | `FloorPlan` | nothing |
| 1 | `Acad/` | `Drawing`, `Prompts`, `Entities`, `Messages` | tier 0 |
| 1 | `Layers/` | `SaLayerTable` | tier 0 |
| 2 | `Plans/` | `FloorPlans` | tiers 0-1 |
| 3 | `Plans/` | `FloorPlanBbox`, `FloorPlanOrigin` | tiers 0-2 |
| 4 | `Commands/` | `Session` | tiers 0-2 |
| 5 | `Commands/` | one class per command | tiers 0-4 |
| 6 | root | `SaCommands` | tier 5 |

`SaCommands` declares the commands AutoCAD offers and nothing else. Each hands off to
a class in `Commands/`, which asks the user and reports, leaning on `Plans/` for the
drawing work and `Acad/` for the plumbing under it. So a new command is a declaration
at the top and a class below it.
