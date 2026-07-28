## Dependency layers

A class may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Class | May depend on |
|---|---|---|
| 0 | `LengthTolerance`, `AngleTolerance` | nothing |
| 1 | `Point3d` | layer 0 |
| 2 | `Vector3d` | layers 0-1 |
| 3 | `LineSegment` | layers 0-2 |
| 4 | `Rectangle` | layers 0-3 |

`Vector3d` sits above `Point3d` because it owns every point-and-vector operation:
the `Vector3d(Point3d, Point3d)` constructor and `Translate(Point3d)`.

Not every permitted dependency is used, which is fine — the table says what a class
*may* reach for, not what it must.