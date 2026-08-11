## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

Folders group by role and the namespaces follow them, so a tier is readable from the
path a file sits at.

| Tier | Folder | Class | May depend on |
|---|---|---|---|
| 0 | `Model/` | `SaLayer`, `SaLayerColors`, `SaAnnotations`, `LayerReport`, `FloorPlan` | nothing |
| 1 | `Acad/` | `Drawing`, `Prompts`, `Entities`, `Messages`, `SaLayerTable` | tier 0 |
| 2 | `Plans/` | the floor plans, and the work done to them | tiers 0-1 |
| 3 | `Commands/` | one class per command, and what they all start from | tiers 0-2 |
| 4 | root | `SaCommands` | tiers 0-3 |
