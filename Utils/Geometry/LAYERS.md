## Dependency layers

A class may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Class | May depend on |
|---|---|---|
| 0 | `LengthTolerance`, `AngleTolerance` | nothing |
| 1 | `Point3d`, `LineSegment2d` | layer 0 |
| 2 | `Vector3d` | layers 0-1 |
| 3 | `LineSegment3d` | layers 0-2 |
| 4 | `Rectangle`, `Polygon` | layers 0-3 |