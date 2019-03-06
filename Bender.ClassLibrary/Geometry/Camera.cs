using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bender.ClassLibrary.Shaders;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.RootFinding;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary.Geometry
{
    public class Camera : Geometry
    {
        private Matrix<float> _viewMatrix;
        public Matrix<float> ViewMatrix
        {
            get => WorldMatrix.Inverse();
            private set => _viewMatrix = value;
        }

        public PhongShader Light { get; set; }

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
            base(name, positionVector, rotationVector, new DenseVector(new[] { 1f, 1f, 1f, 0f }))
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

            //ViewMatrix = CalculateViewMatrix(positionVector, rotationMatrix);
            ViewMatrix = CalculateViewMatrix();
            ProjectionMatrix = CalculateProjectionMatrix(FieldOfView, Aspect, NearClippingPlane, FarClippingPlane);
        }

        public override void Rasterize(Camera c)
        {
            throw new NotImplementedException();
        }

        public override VisualHost GetDataForDrawing()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int[]> Rasterize(Geometry geometry)
        {

            var vertices = WorldToCameraSpace(geometry.Vertices);

            Serializer.WriteVerticesToFile("torusTransformedToCameraSpace.txt", vertices);

            vertices = CameraToProjectionSpace(vertices);

            Serializer.WriteVerticesToFile("torusTransformedToProjectionSpaceBeforeDivide.txt", vertices);

            return LinesToBeDrawn(vertices, geometry.Edges);

        }

        //public IEnumerable<int[]> ImplicitGeometryToRasterSpace(ImplicitGeometry.ImplicitGeometry geometry)
        //{
        //    Vector<float> versor = new DenseVector(new[] {1f, 1f, 1f, 1f});
        //    var versorByCamera = ViewMatrix * versor;
        //    float zCamera = versorByCamera[2];
        //    var versorByCameraByProjection = ProjectionMatrix * versorByCamera;


        //    for (int i = 0; i < (int) ScreenWidth; i+=20)
        //    {
        //        for (int j = 0; j < (int) ScreenHeight; j+=20)
        //        {
        //            float screenX = (2 * i / ScreenWidth - 1);
        //            float screenY = (2 * j / ScreenHeight - 1);

        //            float x = screenX * (float) Math.Tan(FieldOfView / 2);
        //            float y = screenY * (float) Math.Tan(FieldOfView / 2);

        //            Vector<float> cameraVector = new DenseVector(new []{x, y, -1, 1});

        //            if (geometry.TrySolveEquation(cameraVector, PositionVector, out (float first, float second) result))
        //            {
        //                float z = result.first < result.second ? result.first : result.second;
        //                yield return new[] { i, j };
        //            }

        //            //Vector<float> worldVector = WorldMatrix * cameraVector;

        //            //var mu = geometry.EquationMatrix * worldVector;

        //            //Matrix<float> muMatrix = new DenseMatrix(4, 4);
        //            //muMatrix.SetDiagonal(mu);

        //            //Matrix<float> equation = worldVector.ToRowMatrix() * muMatrix;

        //            //if (MathHelpers.TrySolveEquation(equation[0, 0] + equation[0, 1] + equation[0, 2], 0f,
        //            //    equation[0, 3], out (float first, float second) result))
        //            //{
        //            //    float z = result.first < result.second ? result.first : result.second;
        //            //    yield return new[] { i, j };
        //            //}

        //            //Vector<float> u = new DenseVector(new[] {projectionX, projectionY, 1, 1});
        //            //u = ProjectionMatrix.Inverse() * u;
        //            //u = WorldMatrix * u;
        //            //Vector<float> mu = geometry.EquationMatrix * u;
        //            //Matrix<float> muMatrix = new DenseMatrix(4, 4);
        //            //muMatrix.SetDiagonal(mu);
        //            //Matrix<float> equation = u.ToRowMatrix() * muMatrix;

        //            //if (MathHelpers.TrySolveEquation(equation[0, 0] + equation[0, 1] + equation[0, 2], 0f,
        //            //    equation[0, 3], out (float first, float second) result))
        //            //{
        //            //    float z = result.first < result.second ? result.first : result.second;
        //            //    yield return new[] { i, j };
        //            //}

        //        }
        //    }
        //}        //public IEnumerable<int[]> ImplicitGeometryToRasterSpace(ImplicitGeometry.ImplicitGeometry geometry)
        //{
        //    Vector<float> versor = new DenseVector(new[] {1f, 1f, 1f, 1f});
        //    var versorByCamera = ViewMatrix * versor;
        //    float zCamera = versorByCamera[2];
        //    var versorByCameraByProjection = ProjectionMatrix * versorByCamera;


        //    for (int i = 0; i < (int) ScreenWidth; i+=20)
        //    {
        //        for (int j = 0; j < (int) ScreenHeight; j+=20)
        //        {
        //            float screenX = (2 * i / ScreenWidth - 1);
        //            float screenY = (2 * j / ScreenHeight - 1);

        //            float x = screenX * (float) Math.Tan(FieldOfView / 2);
        //            float y = screenY * (float) Math.Tan(FieldOfView / 2);

        //            Vector<float> cameraVector = new DenseVector(new []{x, y, -1, 1});

        //            if (geometry.TrySolveEquation(cameraVector, PositionVector, out (float first, float second) result))
        //            {
        //                float z = result.first < result.second ? result.first : result.second;
        //                yield return new[] { i, j };
        //            }

        //            //Vector<float> worldVector = WorldMatrix * cameraVector;

        //            //var mu = geometry.EquationMatrix * worldVector;

        //            //Matrix<float> muMatrix = new DenseMatrix(4, 4);
        //            //muMatrix.SetDiagonal(mu);

        //            //Matrix<float> equation = worldVector.ToRowMatrix() * muMatrix;

        //            //if (MathHelpers.TrySolveEquation(equation[0, 0] + equation[0, 1] + equation[0, 2], 0f,
        //            //    equation[0, 3], out (float first, float second) result))
        //            //{
        //            //    float z = result.first < result.second ? result.first : result.second;
        //            //    yield return new[] { i, j };
        //            //}

        //            //Vector<float> u = new DenseVector(new[] {projectionX, projectionY, 1, 1});
        //            //u = ProjectionMatrix.Inverse() * u;
        //            //u = WorldMatrix * u;
        //            //Vector<float> mu = geometry.EquationMatrix * u;
        //            //Matrix<float> muMatrix = new DenseMatrix(4, 4);
        //            //muMatrix.SetDiagonal(mu);
        //            //Matrix<float> equation = u.ToRowMatrix() * muMatrix;

        //            //if (MathHelpers.TrySolveEquation(equation[0, 0] + equation[0, 1] + equation[0, 2], 0f,
        //            //    equation[0, 3], out (float first, float second) result))
        //            //{
        //            //    float z = result.first < result.second ? result.first : result.second;
        //            //    yield return new[] { i, j };
        //            //}

        //        }
        //    }
        //}
 
        public IEnumerable<int[]> LinesToBeDrawn(Vector<float>[] vertices, Edge[] topology)
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

                    yield return new[] { beginningX, beginningY, endX, endY };
                }

                if (!MathHelpers.PointInFrustum(vertices[edge.Beginning]) && !MathHelpers.PointInFrustum(vertices[edge.End])) continue;

                Vector<float> beginning = vertices[edge.Beginning];
                Vector<float> end = vertices[edge.End];

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

        private Matrix<float> CalculateOrtographicMatrix(float fieldOfView, float aspect, float nearClippingPlane,
            float farClippingPlane)
        {
            float top = (float)Math.Tan(Trig.DegreeToRadian(fieldOfView) / 2) * nearClippingPlane;
            float bottom = -top;
            float right = top * Aspect;
            float left = -right;

            Matrix<float> projectionMatrix = new DenseMatrix(4, 4);

            projectionMatrix[0, 0] = 2 / (right - left);
            projectionMatrix[0, 3] = -(right + left) / (right - left);
            projectionMatrix[1, 1] = 2 / (top - bottom);
            projectionMatrix[1, 3] = - (top + bottom) / (top - bottom);
            projectionMatrix[2, 2] = -2 / (farClippingPlane - nearClippingPlane);
            projectionMatrix[2, 3] = -(farClippingPlane + nearClippingPlane) / (farClippingPlane - nearClippingPlane);
            projectionMatrix[3, 3] = 1;

            return projectionMatrix;
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


    }
}
