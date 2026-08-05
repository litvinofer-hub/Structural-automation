## Dependency layers

A class may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Class | May depend on |
|---|---|---|
| 0 | `SaLayer`, `SaLayerColors` | nothing |
| 1 | `Commands` | layer 0 |
