## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Namespace | May depend on |
|---|---|---|
| 0 | `SystemParams` | nothing |
| 1 | `Geometry` | tier 0 |

Geometry has its own tiers — see [Geometry/TIERS.md](Geometry/TIERS.md).
