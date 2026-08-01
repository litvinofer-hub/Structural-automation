## Dependency layers

A class may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Namespace | May depend on |
|---|---|---|
| 0 | `SystemParams` | nothing |
| 1 | `Geometry` | layer 0 |

Geometry has its own layering — see [Geometry/LAYERS.md](Geometry/LAYERS.md).
