
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
// Aliased, since WinForms brings in an Application of its own.
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(Structural_Automation.AutoCadCommands.Commands))]

namespace Structural_Automation.AutoCadCommands
{
    /// <summary>
    /// Every command AutoCAD offers, and nothing else. Each one is a thin entry point:
    /// the drawing work belongs to the classes it calls, and all that happens here is
    /// naming the command, asking the user, and saying what it did.
    /// </summary>
    public class Commands
    {
        /// <summary>The palette every command draws in. AutoCAD holds one per document.</summary>
        private readonly SaLayerColors _colors = new();

        /// <summary>The layers holding our own marks rather than the building.</summary>
        private readonly SaAnnotations _annotations = new();

        [CommandMethod("SA_CREATELAYERS")]
        public void CreateLayers()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            LayerReport report = Layers(drawing).Create();

            drawing.Editor.WriteMessage($"\n{report.Applied.Count} layer(s) created.");

            if (report.Skipped.Count > 0)
            {
                drawing.Editor.WriteMessage($"\nWarning: {report.Skipped.Count} layer(s) were already in the drawing and "
                    + $"were left as they are, so their colour may not be the one we expect: "
                    + $"{string.Join(", ", report.Skipped)}");
            }
        }

        [CommandMethod("SA_DELETELAYERS")]
        public void DeleteLayers()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            LayerReport report = Layers(drawing).Delete();

            drawing.Editor.WriteMessage($"\n{report.Applied.Count} layer(s) deleted.");

            if (report.Skipped.Count > 0)
            {
                drawing.Editor.WriteMessage($"\nWarning: {report.Skipped.Count} layer(s) are still in use and were kept - "
                    + $"move or erase what is drawn on them first: {string.Join(", ", report.Skipped)}");
            }
        }

        [CommandMethod("SA_FLOORPLANBBOX")]
        public void FloorPlanBoundingBox()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            Prompts prompts = new(drawing.Editor);
            FloorPlanBbox plan = new(drawing, Layers(drawing), _annotations);

            ObjectId? box = AskForBox();
            if (box is null)
            {
                return;
            }

            int elements = plan.CentreOnContent(box.Value);

            string? name = prompts.AskText("\nEnter the floor plan name");
            if (name is null)
            {
                return;
            }

            plan.Label(box.Value, name);
            drawing.Editor.WriteMessage($"\nFloor plan '{name}' boxed around {elements} element(s).");

            // The choice comes first, so picking a point only ever means drawing a new box.
            ObjectId? AskForBox()
            {
                string? choice = prompts.AskKeyword(
                    "\nDraw a new floor plan box or select an existing one", ["Draw", "Select"], "Draw");

                if (choice is null)
                {
                    return null;
                }

                if (choice == "Select")
                {
                    return prompts.AskEntityOn(
                        "\nSelect the bounding box", SaLayer.SA_FLOOR_PLAN_BBOX, typeof(Polyline));
                }

                Point3d? corner = prompts.AskPoint("\nSpecify first corner", allowFinish: false);
                if (corner is null)
                {
                    return null;
                }

                Point3d? opposite = prompts.AskCorner("\nSpecify opposite corner", corner.Value);

                return opposite is null ? null : plan.Draw(corner.Value, opposite.Value);
            }
        }

        /// <summary>
        /// Asks for one origin circle per floor plan, then insists on exactly one in each:
        /// the plans left empty are asked for again by name, and a plan holding several is
        /// settled by the user saying which to keep.
        /// </summary>
        [CommandMethod("SA_ORIGIN")]
        public void Origin()
        {
            Drawing drawing = new(AcadApplication.DocumentManager.MdiActiveDocument);
            Editor editor = drawing.Editor;
            Prompts prompts = new(editor);
            FloorPlans plans = new(drawing);
            FloorPlanOrigin origins = new(drawing, Layers(drawing), plans);

            List<FloorPlan> all = plans.All();
            if (all.Count == 0)
            {
                editor.WriteMessage("\nNo floor plans found. Run SA_FLOORPLANBBOX first.");
                return;
            }

            if (!DrawMany())
            {
                return;
            }

            while (true)
            {
                List<FloorPlan> empty = [];
                List<FloorPlan> crowded = [];

                foreach (FloorPlan plan in all)
                {
                    int count = origins.In(plan).Count;
                    if (count == 0)
                    {
                        empty.Add(plan);
                    }
                    else if (count > 1)
                    {
                        crowded.Add(plan);
                    }
                }

                if (empty.Count == 0 && crowded.Count == 0)
                {
                    break;
                }

                if (!Thin(crowded) || !Fill(empty))
                {
                    return;
                }
            }

            editor.WriteMessage($"\n{all.Count} floor plan(s), one origin each.");

            // Free drawing: a circle wherever the user picks, until they say they are done.
            bool DrawMany()
            {
                while (true)
                {
                    Point3d? centre = prompts.AskPoint(
                        "\nDraw an origin in each floor plan, or press Enter when finished", allowFinish: true);

                    if (centre is null)
                    {
                        return true;
                    }

                    FloorPlan? plan = plans.Containing(centre.Value);
                    if (plan is null)
                    {
                        editor.WriteMessage("\nThat point is not inside any floor plan.");
                        continue;
                    }

                    origins.Place(centre.Value, plan);
                }
            }

            // Drawing another circle cannot fix a plan that holds too many, so the user says
            // which one is the origin and the rest go.
            bool Thin(List<FloorPlan> crowded)
            {
                foreach (FloorPlan plan in crowded)
                {
                    List<ObjectId> found = origins.In(plan);
                    editor.WriteMessage($"\nWarning: '{plan.Name}' has {found.Count} origins, it must have exactly one.");

                    ObjectId? keep = prompts.AskEntityOn(
                        $"\nPick the origin to keep in '{plan.Name}'", SaLayer.SA_FLOOR_PLAN_ORIGIN, typeof(Circle));

                    if (keep is null)
                    {
                        return false;
                    }

                    if (!found.Contains(keep.Value))
                    {
                        editor.WriteMessage($"\nThat origin is not in '{plan.Name}'.");
                        return false;
                    }

                    editor.WriteMessage($"\n{origins.KeepOnly(keep.Value, found)} origin(s) erased.");
                }

                return true;
            }

            bool Fill(List<FloorPlan> empty)
            {
                foreach (FloorPlan plan in empty)
                {
                    editor.WriteMessage($"\nWarning: '{plan.Name}' has no origin.");

                    Point3d? centre = prompts.AskPoint($"\nDraw the origin in '{plan.Name}'", allowFinish: false);
                    if (centre is null)
                    {
                        return false;
                    }

                    FloorPlan? holder = plans.Containing(centre.Value);
                    if (holder is null || holder.Box != plan.Box)
                    {
                        editor.WriteMessage($"\nThat point is not inside '{plan.Name}'.");
                        continue;
                    }

                    origins.Place(centre.Value, plan);
                }

                return true;
            }
        }

        private SaLayerTable Layers(Drawing drawing)
        {
            return new SaLayerTable(drawing.Database, _colors);
        }
    }
}
