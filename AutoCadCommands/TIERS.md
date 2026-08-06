## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic. Tiers are ours; layers are AutoCAD's, and the two mean different things here.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `SaLayer`, `SaLayerColors`, `SaAnnotations`, `LayerReport`, `FloorPlan` | nothing |
| 1 | `Drawing`, `Prompts`, `Entities`, `SaLayerTable` | tier 0 |
| 2 | `FloorPlans` | tiers 0-1 |
| 3 | `FloorPlanBbox`, `FloorPlanOrigin` | tiers 0-2 |
| 4 | `Commands` | tiers 0-3 |

`Commands` holds every command AutoCAD offers and no drawing work. Tier 3 holds one
class per operation, tier 2 what the operations agree on, tier 1 the plumbing they all
need, so a new command is an entry point at the top and a method or two below it.
