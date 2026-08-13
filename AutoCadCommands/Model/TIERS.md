## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `SaLayer`, `LayerReport`, `FloorPlan` | nothing here |
| 1 | `SaLayerColors`, `SaAnnotations` | tier 0 |
