# Structural Automation

## Dependency tiers

A project may reference lower tiers only — never its own or higher. Keeps the graph
acyclic.

| Tier | Project | May depend on | References |
|---|---|---|---|
| 0 | `Utils` | nothing | — |
| 1 | `BuildingModel` | tier 0 | `Utils` |
| 2 | `AutoCadCommands` | tiers 0-1 | `Utils` |

Each project has its own tiers.
Each folder has its own namespaces and tiers.

## Equality

A `Utils.Geometry` instance is its geometry, so it compares by geometry: two instances
are equal when they fill the same space, whichever corner each was built from and
whichever way round its edges run.

A `BuildingModel` instance is an element of the building, so it compares by `Id`: two
elements built over the same space are two elements, and one can be taken out without
the other going with it.