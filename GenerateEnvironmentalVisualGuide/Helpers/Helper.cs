using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GenerateEnvironmentalVisualGuide.Models;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Runtime;

namespace GenerateEnvironmentalVisualGuide.Helpers
{
    internal class Helper
    {

        public static Camera[] GetCameraMovementDetails()
        {
            //todo read from file the coordinates
            //create list
     

            Array[] arr = new Array[1]; //todo replace with file reading
            Camera[] cameraMovementDetails = new Camera[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                Point3d position = Point3d.Origin; //todo replace with reading from array
                double yaw = Math.PI / 2; //todo replace with reading from array
                double pitch = Math.PI / 6;
                double roll = Math.PI / 3;

                Quaternion orientation =  Quaternion.Rotation(pitch, Vector3d.XAxis) * Quaternion.Rotation(roll, Vector3d.YAxis) * Quaternion.Rotation(yaw, Vector3d.ZAxis);

                Camera camera = new Camera.Builder()
                    .Position(position)
                    .Rotation(orientation)
                    .Build();

                cameraMovementDetails[i] = camera;
            }
            return cameraMovementDetails;
        }

        public static Surface[] GetCuttingSurfacesFromCameraPositioning(Camera camera, double viewingAngle)
        {
            Point3d position = camera.Position;
            Transform matrix = camera.Rotation.MatrixForm();

            //The second and third array are the relative y and z axis of the camera.
            Vector3d xAxis = new Vector3d(matrix.M31, matrix.M32, matrix.M33);
            Vector3d yAxis = new Vector3d(matrix.M21, matrix.M22, matrix.M23);

            var plane1 = new Plane(position, xAxis, yAxis);
            var plane2 = new Plane(position, xAxis, yAxis);

            RhinoApp.WriteLine("cutting plane length:" + plane1.XAxis.Length);

            plane1.Rotate(viewingAngle / 2, xAxis);
            plane2.Rotate(-viewingAngle / 2, xAxis);

            var surface1 = new PlaneSurface(plane1,new Interval(0, 1), new Interval(0, 1));
            var surface2 = new PlaneSurface(plane2,new Interval(0, 1), new Interval(0, 1));

            //temporary measure 
            surface1.Scale(1000);
            surface2.Scale(1000);

            var arr = new Surface[2];
            arr[0] = surface1;
            arr[1] = surface2;

            return arr;
        }

        public static Brep GetGeometrySlice(Brep sourceBrep, Surface[] cuttingPlanes, double tolerance)
        {
            BoundingBox boundingBox = sourceBrep.GetBoundingBox(false);
            //Brep cuttingBrep1 = PlaneSurface.CreateThroughBox(cuttingPlanes[0], boundingBox).ToBrep();
            //Brep cuttingBrep2 = PlaneSurface.CreateThroughBox(cuttingPlanes[1], boundingBox).ToBrep();
            Brep[] splitGeometries1 = sourceBrep.Split(cuttingPlanes[0].ToBrep(), tolerance);
            if (splitGeometries1.Length == 0)
            {
                throw new Exception("source geometry not split");
            }
            Brep[] splitGeometries2 = splitGeometries1[1].Split(cuttingPlanes[1].ToBrep(), tolerance); ;
            return splitGeometries2[0]; //TODO: check what needs to be returned
        }

    }
}
