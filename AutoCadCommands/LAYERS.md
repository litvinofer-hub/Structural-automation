## Dependency layers

A class may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Class | May depend on |
|---|---|---|
| 0 | `SaLayer`, `SaLayerColors`, `LayerReport` | nothing |
| 1 | `SaLayerTable` | layer 0 |
| 2 | `Commands` | layers 0-1 |

`Commands` holds every command AutoCAD offers and no drawing work. The layer below it
does the work and reports it back, so a new command is an entry point here and a
method there.
