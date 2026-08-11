## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `Session` | nothing here |
| 1 | `CreateLayersCommand`, `DeleteLayersCommand`, `FloorPlanBboxCommand`, `OriginCommand` | tier 0 |

Every command is handed a `Session` and asks it for what it needs, so a new command is
one more class at tier 1 and nothing else.
