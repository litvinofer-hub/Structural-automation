## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic. Tiers are ours; layers are AutoCAD's, and the two mean different things here.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `SaLayer`, `SaLayerColors`, `LayerReport` | nothing |
| 1 | `SaLayerTable` | tier 0 |
| 2 | `Commands` | tiers 0-1 |

`Commands` holds every command AutoCAD offers and no drawing work. The tier below it
does the work and reports it back, so a new command is an entry point here and a
method there.
