using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Storage;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary
{
    public class Camera
    {
        public Matrix<float> _viewMatrix { get; private set; }
        public Matrix<float> ProjectionMatrix { get; private set; }
        public Vector<float> DirectionVector { get; private set; }
        public float NearClippingPlane { get; private set; }
        public float FarClippingPlane { get; private set; }
        public float FieldOfView { get; private set; }
        public float Aspect { get; private set; }
        public float ScreenWidth { get; private set; }
        public float ScreenHeight { get; private set; }

        public Camera(Vector<float> positionVector, Vector<float> rotationVector, float nearClippingPlane, float farClippingPlane, float fieldOfView, float screenWidth, float screenHeight)
        {
            Matrix<float> rotationMatrix = MathHelpers.CalculateRotationMatrix(rotationVector);
            DirectionVector = rotationMatrix * new DenseVector(new[] {0f, 0f, -1f, 0f});
            NearClippingPlane = nearClippingPlane;
            FarClippingPlane = farClippingPlane;
            FieldOfView = fieldOfView;
            Aspect = screenWidth / screenHeight;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;

            _viewMatrix = CalculateViewMatrix(positionVector, rotationVector);
            ProjectionMatrix = CalculateProjectionMatrix(fieldOfView, Aspect, nearClippingPlane, farClippingPlane);
        }

        public void GeometryToRasterSpace(IGeometry geometry, out Point[] pointsInRasterSpace)
        {
            //geometry.Geometry.Draw(_viewMatrix, _projectionMatrix, out Vector<float>[] verticesInScreenSpace, out topology);
            var vertices = WorldToCameraSpace(geometry.Geometry.Vertices);

            Serializer.WriteVerticesToFile("torusTransformedToCameraSpace.txt", vertices);

            vertices = CameraToProjectionSpace(vertices);

            Serializer.WriteVerticesToFile("torusTransformedToProjectionSpaceBeforeDivide.txt", vertices);

            vertices = vertices.Select(x => new DenseVector(new[] {x[0] / x[3], x[1] / x[3], x[2] / x[3], x[3] / x[3]}))
                .ToArray();

            Serializer.WriteVerticesToFile("torusTransformedToProjectionSpace.txt", vertices);

            List<Point> pointsInRasterSpaceList = new List<Point>();

            foreach (Vector<float> vector in vertices)
            {

                int xRasterSpace = (int) ((vector[0] + 1) * 0.5 * ScreenWidth);
                int yRasterSpace = (int) ((vector[1] + 1) * 0.5 * ScreenHeight);

                pointsInRasterSpaceList.Add(new Point(xRasterSpace, yRasterSpace));
            }

            pointsInRasterSpace = pointsInRasterSpaceList.ToArray();

            Serializer.WriteVerticesToFile("torusVerticesInRasterSpace.txt", pointsInRasterSpace);
        }

        public Vector<float>[] WorldToCameraSpace(Vector<float>[] vertices)
        {
            return vertices.Select(x => _viewMatrix * x).ToArray();
        }

        public Vector<float>[] CameraToProjectionSpace(Vector<float>[] vertices)
        {
            return vertices.Select(x => ProjectionMatrix * x).ToArray();
        }

        private Matrix<float> CalculateViewMatrix(Vector<float> positionVector, Vector<float> rotationVector)
        {
            if (positionVector.Count != 4)
            {
                throw new ArgumentException("Length of this vector has to be 3", nameof(positionVector));
            }

            if (rotationVector.Count != 4)
            {
                throw new ArgumentException("Length of this vector has to be 3", nameof(rotationVector));
            }

            Matrix<float> rotationMatrix = MathHelpers.CalculateRotationMatrix(rotationVector);
            Vector<float> preDirectionVector = rotationMatrix * new DenseVector(new[] { 0f, 0f, 1f, 0f });
            Vector<float> directionVector = new DenseVector(new[] { preDirectionVector[0], preDirectionVector[1], preDirectionVector[2] });

            Vector<float> upWorldVector = new DenseVector(new[] { 0f, 1f, 0f });
            Vector<float> rightVector = MathHelpers.CrossProduct(upWorldVector, directionVector).Normalize(2);
            Vector<float> upVector = MathHelpers.CrossProduct(directionVector, rightVector).Normalize(2);

            Matrix<float> viewMatrix = new DenseMatrix(4, 4);

            viewMatrix.SetRow(0, new[] { rightVector[0], rightVector[1], rightVector[2], -positionVector[0] }); //TODO: later check if rotation part is correct
            viewMatrix.SetRow(1, new[] { upVector[0], upVector[1], upVector[2], -positionVector[1] });
            viewMatrix.SetRow(2, new[] { directionVector[0], directionVector[1], directionVector[2], -positionVector[2] });
            viewMatrix.SetRow(3, new[] { 0, 0, 0, 1f });

            return viewMatrix;
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
