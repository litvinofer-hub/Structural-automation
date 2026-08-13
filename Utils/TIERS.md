## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

A row is a folder or a class sitting in the root. 
Each folder has its own tier file, and its own namespace.

| Tier | Folder or Class | May depend on |
|---|---|---|
| 0 | `SystemParams/` | nothing |
| 1 | `Geometry/` | tier 0 |