using System;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Json;
using GenerateEnvironmentalVisualGuide.Models;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino;

namespace GenerateEnvironmentalVisualGuide.Helpers
{
    internal class Helper
    {

        public static Camera[] GetCameraMovementDetails()
        {
            //todo read from file the coordinates
            Array[] arr = new Array[1]; //todo replace with file reading
            Camera[] cameraMovementDetails = new Camera[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                //todo refactor hardcoding with values from file
                Point3d position = Point3d.Origin;
                double yaw = 0; 
                double pitch = 0;
                double roll = 0;
                Quaternion orientation =  Quaternion.Rotation(pitch, Vector3d.XAxis) * Quaternion.Rotation(roll, Vector3d.YAxis) * Quaternion.Rotation(yaw, Vector3d.ZAxis);

                Camera camera = new Camera.Builder()
                    .Position(position)
                    .Rotation(orientation)
                    .Build();
                cameraMovementDetails[i] = camera;
            }
            return cameraMovementDetails;
        }

        public static Surface[] GetCuttingSurfacesFromCameraPositioning(Camera camera, Point3d startPoint, double viewingAngle)
        {
            Point3d position = Point3d.Add(camera.Position, startPoint); 
            Transform matrix = camera.Rotation.MatrixForm();

            //The second and third array are the relative y and z axis of the camera.
            Vector3d xAxis = new Vector3d(matrix.M31, matrix.M32, matrix.M33);
            Vector3d yAxis = new Vector3d(matrix.M21, matrix.M22, matrix.M23);

            var plane1 = new Plane(position, xAxis, yAxis);
            var plane2 = new Plane(position, xAxis, yAxis);
            //todo refactor to add rotation to Vector3D constructors instead
            plane1.Rotate(viewingAngle / 2, xAxis);
            plane2.Rotate(-viewingAngle / 2, xAxis);
            var surface1 = new PlaneSurface(plane1,new Interval(0, 1), new Interval(0, 1));
            var surface2 = new PlaneSurface(plane2,new Interval(0, 1), new Interval(0, 1));

            // Scale the surfaces (temporary measure)
            double scaleFactor = 1000;
            Transform scaleTransform = Transform.Scale(position, scaleFactor);
            surface1.Transform(scaleTransform);
            surface2.Transform(scaleTransform);

            var arr = new Surface[2];
            arr[0] = surface1;
            arr[1] = surface2;

            return arr;
        }

        public static Brep GetGeometrySlice(Brep sourceBrep, Surface[] cuttingPlanes, double tolerance)
        {
            Brep[] splitGeometries1 = sourceBrep.Split(cuttingPlanes[0].ToBrep(), tolerance);
            if (splitGeometries1.Length == 0)
            {
                throw new Exception("Exception occurred: Failed to split source geometry.");
            }
            Brep[] splitGeometries2 = splitGeometries1[1].Split(cuttingPlanes[1].ToBrep(), tolerance);
            return splitGeometries1.Length > 0 ? splitGeometries2[1] : splitGeometries1[1];
        }

        public static int generateLayer(string name, Color color, RhinoDoc doc)
        {
            Layer layer = new Layer();
            layer.Name = name;
            layer.Color = color;
            return doc.Layers.Add(layer);
        }

    }
}
