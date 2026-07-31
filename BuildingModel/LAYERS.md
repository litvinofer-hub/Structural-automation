## Dependency layers

A class may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

BuildingModel may use Utils. Utils must never use BuildingModel.

| Layer | Class | May depend on |
|---|---|---|
| 0 | `SubLevel` | Utils |
| 1 | `Level`, `Wall`, `Floor` | layer 0, Utils |
| 2 | `Building` | layers 0-1, Utils |
