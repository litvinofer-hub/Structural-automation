## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `LengthUnit` | nothing |
| 1 | `Units` | tier 0 |

The unit a model is drawn in is the one thing everything else is measured against, so
it sits under the whole project and depends on none of it.
