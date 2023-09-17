using Rhino.Geometry;

namespace GenerateEnvironmentalVisualGuide.Models
{
    internal class Camera
    {
        public Point3d Position { get; set; }
        public Quaternion Rotation { get; set; }


        public class Builder
        {
            private readonly Camera _camera;
            public Builder() 
            { 
                _camera = new Camera();
            }

            public Builder Position(Point3d position)
            {
                _camera.Position = position;
                return this;
            }

            public Builder Rotation(Quaternion rotation)
            {
                _camera.Rotation = rotation;
                return this;
            }

            public Camera Build() => _camera;
        }
    }
}
