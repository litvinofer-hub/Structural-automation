## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `FloorPlans`, `FloorPlanBbox` | nothing here |
| 1 | `FloorPlanOrigin` | tier 0 |

`FloorPlanOrigin` asks `FloorPlans` which plan a circle belongs to, since an origin
belongs to one plan alone. `FloorPlanBbox` works on a rectangle it is handed and needs
no such answer.
