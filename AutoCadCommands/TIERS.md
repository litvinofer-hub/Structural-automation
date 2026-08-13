## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

A row is a folder or a class sitting in the root. A folder is tiered as a whole — what
its own classes may depend on is its business, not this table's.

| Tier | Folder or Class | May depend on |
|---|---|---|
| 0 | `Model/` | nothing |
| 1 | `Acad/` | tier 0 |
| 2 | `Plans/` | tiers 0-1 |
| 3 | `Commands/` | tiers 0-2 |
| 4 | `SaCommands` | tiers 0-3 |

Each folder has its own namespaces and tiers.