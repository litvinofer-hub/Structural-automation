## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `Drawing`, `Entities`, `Messages`, `Prompts`, `SaLayerTable` | nothing here, `Model/` |

Each class is one way of talking to the drawing — the database, the entities in it, the
layer table, and what the user is told and asked. None of them needs another, so they
all sit at the same tier and a new one joins them there.
