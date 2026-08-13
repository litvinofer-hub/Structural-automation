## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `LengthTolerance`, `AngleTolerance` | nothing |
| 1 | `Point3d`, `LineSegment2d` | tier 0 |
| 2 | `Vector3d` | tiers 0-1 |
| 3 | `LineSegment3d` | tiers 0-2 |
| 4 | `Rectangle`, `Polygon` | tiers 0-3 |
| 5 | `Box` | tiers 0-4 |
| 6 | `VerticalBox` | tiers 0-5 |