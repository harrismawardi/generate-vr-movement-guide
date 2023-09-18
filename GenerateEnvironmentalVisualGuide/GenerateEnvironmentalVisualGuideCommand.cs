using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using System;
using GenerateEnvironmentalVisualGuide.Helpers;
using GenerateEnvironmentalVisualGuide.Models;
using System.Runtime.Serialization.Json;
using System.Drawing;

namespace GenerateEnvironmentalVisualGuide
{
    [CommandStyle(Style.ScriptRunner)]
    public class GenerateEnvironmentalVisualGuideCommand : Command
    {
        public GenerateEnvironmentalVisualGuideCommand()
        {
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static GenerateEnvironmentalVisualGuideCommand Instance { get; private set; }

        public override string EnglishName => "GenerateEnvironmentalVisualGuideCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            doc.Views.EnableRedraw(true, true, true);
            double viewingAngle = Math.PI / 3; //60 degrees
            try
            {
                //Get start point for movement
                Point3d startPoint;
                Result pointResult = RhinoGet.GetPoint("Select starting view point.", false, out startPoint);
                if (pointResult != Result.Success)
                {
                    RhinoApp.WriteLine("An error occurred selecting movement start point.");
                    return pointResult;
                }

                //Get source geometry to use for guide geometry generation
                ObjRef sourceGeometryObjRef;
                Result solidResult = RhinoGet.GetOneObject("Select solid to generate guide from.", false, ObjectType.Brep, out sourceGeometryObjRef);
                if (solidResult != Result.Success)
                {
                    RhinoApp.WriteLine("An error occurred selecting source geometry.");
                    return solidResult;
                }
               
                //Get Boundary Representation to enable geometry manipulation
                Brep sourceGeometryBrep = sourceGeometryObjRef.Brep();

                //Layers to organise scaffold geometries (e.g. cutting planes) and guide geometries
                int scaffoldLayerIndex = Helper.generateLayer("Scaffold Objects", Color.Aqua, doc);
                int guideLayerIndex = Helper.generateLayer("Movement Guide", Color.Orange, doc);

                Camera[] viewCoordinates = Helper.GetCameraMovementDetails();
                foreach (Camera camera in viewCoordinates)
                {
                    Surface[] cuttingSurfaces =  Helper.GetCuttingSurfacesFromCameraPositioning(camera, startPoint, viewingAngle);
                    foreach (Surface surfaceRepresentation in cuttingSurfaces)
                    {
                        var scaffoldOa = new ObjectAttributes();
                        scaffoldOa.LayerIndex = scaffoldLayerIndex;
                        doc.Objects.AddSurface(surfaceRepresentation, scaffoldOa);
                    }
                    
                    Brep slicedGeometry = Helper.GetGeometrySlice(sourceGeometryBrep, cuttingSurfaces, doc.ModelAbsoluteTolerance);
                    var guideOa = new ObjectAttributes();
                    guideOa.LayerIndex = guideLayerIndex;
                    doc.Objects.AddBrep(slicedGeometry, guideOa);
                }

                RhinoApp.WriteLine("Movement guide generated successfully.");
                doc.Views.Redraw();
                return Result.Success;
            } catch (Exception ex)
            {
                RhinoApp.WriteLine("Exception occurred: " + ex.Message);
                doc.Views.Redraw();
                return Result.Failure;
            }
        }
    }
}
