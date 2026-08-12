## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

BuildingModel may use Utils. Utils must never use BuildingModel.

Folders group by role and the namespaces follow them, so a tier is readable from the
path a file sits at.

| Tier | Folder | Class | May depend on |
|---|---|---|---|
| 0 | `Params/` | `WallBorders` | Utils |
| 0 | root | `SubLevel`, `IFlattenable` | Utils |
| 1 | root | `Level`, `Floor`, `Opening` | tier 0, Utils |
| 2 | root | `Wall` | tiers 0-1, Utils |
| 3 | root | `Building` | tiers 0-2, Utils |

`Params/` holds the parameters building elements are given, the defaults they start
with and the units those depend on. Nothing in it knows what an element is, so it sits
at the bottom alongside the pieces that carry no dependencies of their own.
