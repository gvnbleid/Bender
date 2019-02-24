using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Storage;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary
{
    public class Camera : Geometry
    {
        public Matrix<float> ViewMatrix { get; private set; }
        public Matrix<float> ProjectionMatrix { get; private set; }
        public Vector<float> DirectionVector { get; private set; }
        public float NearClippingPlane { get; private set; }
        public float FarClippingPlane { get; private set; }
        public float FieldOfView { get; private set; }
        public float Aspect { get; private set; }
        public float ScreenWidth { get; private set; }
        public float ScreenHeight { get; private set; }

        public Camera(string name, Vector<float> positionVector, Vector<float> rotationVector, float nearClippingPlane,
            float farClippingPlane, float fieldOfView, float screenWidth, float screenHeight) :
            base(name, positionVector, rotationVector, new DenseVector(new[] {1f, 1f, 1f, 0f}))
        {
            ScaleVector = new DenseVector(new[] {1f, 1f, 1f, 0f});

            Update(positionVector, rotationVector, nearClippingPlane, farClippingPlane, fieldOfView, screenWidth,
                screenHeight, name);
        }

        public void Update(Vector<float> positionVector, Vector<float> rotationVector, float nearClippingPlane,
            float farClippingPlane, float fieldOfView, float screenWidth, float screenHeight, string name = null)
        {
            PositionVector = positionVector.Clone();
            RotationVector = rotationVector.Clone();

            Matrix<float> rotationMatrix = MathHelpers.CalculateRotationMatrix(rotationVector);
            DirectionVector = rotationMatrix * new DenseVector(new[] { 0f, 0f, -1f, 0f });
            NearClippingPlane = nearClippingPlane;
            FarClippingPlane = farClippingPlane;
            FieldOfView = fieldOfView;
            Aspect = screenWidth / screenHeight;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;

            ViewMatrix = CalculateViewMatrix(positionVector, rotationMatrix);
            ProjectionMatrix = CalculateProjectionMatrix(fieldOfView, Aspect, nearClippingPlane, farClippingPlane);

            if (name != null) Name = name;
        }

        public override void Update(Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector)
        {
            base.Update(positionVector, rotationVector, scaleVector);

            Matrix<float> rotationMatrix = MathHelpers.CalculateRotationMatrix(rotationVector);
            DirectionVector = rotationMatrix * new DenseVector(new[] { 0f, 0f, -1f, 0f });

            ViewMatrix = CalculateViewMatrix(positionVector, rotationMatrix);
            ProjectionMatrix = CalculateProjectionMatrix(FieldOfView, Aspect, NearClippingPlane, FarClippingPlane);
        }

        public void GeometryToRasterSpace(Geometry geometry, out Point[] pointsInRasterSpace, out bool[] clippedvertices)
        {
            //geometry.Geometry.Draw(_viewMatrix, _projectionMatrix, out Vector<float>[] verticesInScreenSpace, out topology);
            var vertices = WorldToCameraSpace(geometry.Vertices);

            Serializer.WriteVerticesToFile("torusTransformedToCameraSpace.txt", vertices);

            vertices = CameraToProjectionSpace(vertices);

            Serializer.WriteVerticesToFile("torusTransformedToProjectionSpaceBeforeDivide.txt", vertices);

            vertices = vertices.Select(x => new DenseVector(new[] {x[0] / x[3], x[1] / x[3], x[2] / x[3], x[3] / x[3]}))
                .ToArray();

            Serializer.WriteVerticesToFile("torusTransformedToProjectionSpace.txt", vertices);

            List<Point> pointsInRasterSpaceList = new List<Point>();
            clippedvertices = new bool[vertices.Length];

            for(int i=0;i<vertices.Length;i++)
            {
                if (vertices[i][0] < -1 || vertices[i][0] > 1 || vertices[i][1] < -1 || vertices[i][1] > 1 ||
                    vertices[i][2] < -1 || vertices[i][2] > 1)
                {
                    clippedvertices[i] = true;
                    //pointsInRasterSpaceList.Add(new Point(0, 0));
                    //continue;
                }

                int xRasterSpace = (int) ((vertices[i][0] + 1) * 0.5 * ScreenWidth);
                int yRasterSpace = (int) ((vertices[i][1] + 1) * 0.5 * ScreenHeight);

                pointsInRasterSpaceList.Add(new Point(xRasterSpace, yRasterSpace));
            }

            pointsInRasterSpace = pointsInRasterSpaceList.ToArray();

            Serializer.WriteVerticesToFile("torusVerticesInRasterSpace.txt", pointsInRasterSpace);
        }

        public IEnumerable<Vector<float>> LinesToBeDrawn(Vector<float>[] vertices, Edge[] topology)
        {
            foreach (Edge edge in topology)
            {
                
            }

            return null;
        }

        public Vector<float>[] WorldToCameraSpace(Vector<float>[] vertices)
        {
            return vertices.Select(x => ViewMatrix * x).ToArray();
        }

        public Vector<float>[] CameraToProjectionSpace(Vector<float>[] vertices)
        {
            return vertices.Select(x => ProjectionMatrix * x).ToArray();
        }

        private Matrix<float> CalculateViewMatrix(Vector<float> positionVector, Matrix<float> rotationMatrix)
        {
            if (positionVector.Count != 4)
            {
                throw new ArgumentException("Length of this vector has to be 3", nameof(positionVector));
            }

            Matrix<float> translationMatrix = MathHelpers.CalculateTranslationMatrix(positionVector);

            return (translationMatrix * rotationMatrix).Inverse();
        }

        private Matrix<float> CalculateProjectionMatrix(float fieldOfView, float aspect, float nearClippingPlane, float farClippingPlane)
        {
            float scale = 1f / (float)Math.Tan(fieldOfView / 2);

            Matrix<float> projectionMatrix = new DenseMatrix(4, 4);
            projectionMatrix[0, 0] = scale / aspect;
            projectionMatrix[1, 1] = scale;
            projectionMatrix[2, 2] = -(farClippingPlane + nearClippingPlane) / (farClippingPlane - nearClippingPlane);
            projectionMatrix[2, 3] = -2 * farClippingPlane * nearClippingPlane / (farClippingPlane - nearClippingPlane);
            projectionMatrix[3, 2] = -1;

            return projectionMatrix;
        }


    }
}
