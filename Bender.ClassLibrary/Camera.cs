using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Storage;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary
{
    public class Camera : Geometry
    {
        private Matrix<float> _viewMatrix;
        public Matrix<float> ViewMatrix
        {
            get => WorldMatrix.Inverse();
            private set => _viewMatrix = value;
        }

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

            ViewMatrix = CalculateViewMatrix();
            ProjectionMatrix = CalculateProjectionMatrix(FieldOfView, Aspect, NearClippingPlane, FarClippingPlane);
        }

        public IEnumerable<LineGeometry> GeometryToRasterSpace(Geometry geometry)
        {
            var vertices = ModelToWorld(geometry);

            vertices = WorldToCameraSpace(vertices);

            vertices = CameraToProjectionSpace(vertices);

            return LinesToBeDrawn(vertices, geometry.Edges);

        }

        private Vector<float>[] ModelToWorld(Geometry geometry)
        {
            return geometry.Vertices.Select(x => geometry.WorldMatrix * x).ToArray();
        }

        public IEnumerable<LineGeometry> LinesToBeDrawn(Vector<float>[] vertices, Edge[] topology)
        {
            List<int[]> lines = new List<int[]>();
            foreach (Edge edge in topology)
            { 

                if (CohenSutherland.TryClipLine(vertices[edge.Beginning], vertices[edge.End], out float[] line))
                {
                    int beginningX = (int)((line[0] + 1) * 0.5 * ScreenWidth);
                    int beginningY = (int)((line[1] + 1) * 0.5 * ScreenHeight);
                    int endX = (int)((line[2] + 1) * 0.5 * ScreenWidth);
                    int endY = (int)((line[3] + 1) * 0.5 * ScreenHeight);

                    yield return new LineGeometry(new Point(beginningX, beginningY), new Point(endX, endY));
                }

            }
        }

        public Vector<float>[] WorldToCameraSpace(Vector<float>[] vertices)
        {
            return vertices.Select(x => ViewMatrix * x).ToArray();
        }

        public Vector<float>[] CameraToProjectionSpace(Vector<float>[] vertices)
        {  
            var v = vertices.Select(x => ProjectionMatrix * x).ToArray();
            return v.Select(x => x.Divide(Math.Abs(x[3]))).ToArray();
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

        private Matrix<float> CalculateViewMatrix()
        {
            return WorldMatrix.Inverse();
        }

        private Matrix<float> CalculateProjectionMatrix(float fieldOfView, float aspect, float nearClippingPlane, float farClippingPlane)
        {
            float top = (float) Math.Tan(Trig.DegreeToRadian(fieldOfView) / 2) * nearClippingPlane;
            float bottom = -top;
            float right = top * Aspect;
            float left = -right;

            Matrix<float> projectionMatrix = new DenseMatrix(4, 4);

            projectionMatrix[0, 0] = 2 * nearClippingPlane / (right - left);
            projectionMatrix[0, 2] = (right + left) / (right - left);
            projectionMatrix[1, 1] = 2 * nearClippingPlane / (top - bottom);
            projectionMatrix[1, 2] = (top + bottom) / (top - bottom);
            projectionMatrix[2, 2] = -(farClippingPlane + nearClippingPlane) / (farClippingPlane - nearClippingPlane);
            projectionMatrix[2, 3] = -2*farClippingPlane * nearClippingPlane / (farClippingPlane - nearClippingPlane);
            projectionMatrix[3, 2] = -1;

            return projectionMatrix;
        }

        public void UpdateProjectionMatrix(float fieldOfView, float aspect, float nearClippingPlane, float farClippingPlane)
        {
            ProjectionMatrix = CalculateProjectionMatrix(fieldOfView, aspect, nearClippingPlane, farClippingPlane);
        }

        public override void Transform(Vector<float> transformVector)
        {
            var m = MathHelpers.CalculateTranslationMatrix(transformVector);
            PositionVector += transformVector;
            WorldMatrix *= m;
        }

        public override void Rotate(Vector<float> rotationVector)
        {
            var m = MathHelpers.CalculateRotationMatrix(rotationVector);
            RotationVector += rotationVector;
            WorldMatrix *= m;
        }

        public override void PreScale(Vector<float> scaleVector)
        {
            Vector<float> newScaleVector = ScaleVector + scaleVector;
            Vector<float> relativeScaleVector = new DenseVector(new[] { newScaleVector[0] / ScaleVector[0], newScaleVector[1] / ScaleVector[1], newScaleVector[2] / ScaleVector[2], 0f });
            var m = MathHelpers.CalculateScaleMatrix(relativeScaleVector);
            ScaleVector = newScaleVector;
            WorldMatrix *= m;
        }


    }
}
