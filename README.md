# Structural Automation

## Dependency layers

A project may reference lower layers only — never its own or higher. Keeps the graph
acyclic.

| Layer | Project | May depend on | References |
|---|---|---|---|
| 0 | `Utils` | nothing | — |
| 0 | `AutoCadCommands` | nothing | — |
| 1 | `BuildingModel` | layer 0 | `Utils` |

Each project has its own layering — see [Utils/LAYERS.md](Utils/LAYERS.md),
[BuildingModel/LAYERS.md](BuildingModel/LAYERS.md) and
[AutoCadCommands/LAYERS.md](AutoCadCommands/LAYERS.md).

The references are the `ProjectReference` entries in each `.csproj`.

`AutoCadCommands` stands alone: it depends on no project, and no project depends on
it. It is the AutoCAD plugin, holding the commands, and it references the managed
AutoCAD assemblies straight from the install directory.

## The plugin

Two paths drive it, both in [Directory.Build.props](Directory.Build.props) and both
overridable — `dotnet build -p:SaBundleDir=D:\Somewhere\`:

| Property | Default | What it is |
|---|---|---|
| `AutoCadDir` | `C:\Program Files\Autodesk\AutoCAD 2027\` | The install we compile against |
| `SaBundleDir` | `%APPDATA%\Autodesk\ApplicationPlugins\StructuralAutomation.bundle\` | Where the plugin is deployed |

Building deploys: the `DeployBundle` target copies the dll, its pdb and
[PackageContents.xml](AutoCadCommands/PackageContents.xml) into the bundle. AutoCAD
reads every bundle under `ApplicationPlugins` on startup, so the commands are simply
there — no `NETLOAD`. Start AutoCAD, open a drawing, and run `SA_CREATELAYERS`.

AutoCAD holds the dll open while it runs, so a build with AutoCAD open cannot replace
it. The target warns rather than leaving a stale dll behind: close AutoCAD and build
again.

F5 in Visual Studio builds, deploys, starts AutoCAD and attaches the debugger, so
breakpoints bind as soon as a command runs. It needs `AutoCadCommands` set as the
startup project — a class library cannot be started, and `RunCommand` in the csproj
is what points the run at `acad.exe` instead.