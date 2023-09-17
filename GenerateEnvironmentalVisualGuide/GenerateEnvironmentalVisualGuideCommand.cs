using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GenerateEnvironmentalVisualGuide.Helpers;
using GenerateEnvironmentalVisualGuide.Models;

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
            try
            {
                //Get start point for movement
                Point3d startPoint;
                Result pointResult = RhinoGet.GetPoint("Select starting view point.", false, out startPoint);

                //Get source geometry to use for guide geometry generation
                ObjRef sourceGeometryObjRef;
                Result solidResult = RhinoGet.GetOneObject("Select solid to generate guide from.", false, ObjectType.Brep, out sourceGeometryObjRef);
                if (solidResult != Result.Success)
                {
                    RhinoApp.WriteLine("An error occurred selecting source geometry.");
                    return solidResult;
                }

                //Box sourceGeometry;
                //Result solidResult = RhinoGet.GetBox(out sourceGeometry);
                
                RhinoApp.WriteLine("did we get here");

                //Get Boundary Representation to enable geometry manipulation
                Brep sourceGeometryBrep = sourceGeometryObjRef.Brep();

                Camera[] viewCoordinates = Helper.GetCameraMovementDetails();

                foreach (Camera camera in viewCoordinates)
                {
                    double viewingAngle = Math.PI / 3; //60 degrees
                    Surface[] cuttingSurfaces =  Helper.GetCuttingSurfacesFromCameraPositioning(camera,viewingAngle);
                    foreach (Surface surfaceRepresentation in cuttingSurfaces)
                    {

                        doc.Objects.AddSurface(surfaceRepresentation);
                    }
                    Brep slicedGeometry = Helper.GetGeometrySlice(sourceGeometryBrep, cuttingSurfaces, doc.ModelAbsoluteTolerance);

                }

                RhinoApp.WriteLine("Movement guide generated successfully.");
                doc.Views.Redraw();
                return Result.Success;
            } catch (Exception ex)
            {
                RhinoApp.WriteLine("Exception occurred: " + ex.Message);
                doc.Views.EnableRedraw(true, true, true);
                doc.Views.Redraw();
                return Result.Failure;
            }
        }
    }
}
