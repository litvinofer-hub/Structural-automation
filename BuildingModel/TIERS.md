## Dependency tiers

A class may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

BuildingModel may use Utils. Utils must never use BuildingModel.

| Tier | Class | May depend on |
|---|---|---|
| 0 | `SubLevel` | Utils |
| 1 | `Level`, `Wall`, `Floor` | tier 0, Utils |
| 2 | `Building` | tiers 0-1, Utils |
