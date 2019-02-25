using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private ObservableCollection<string> _logs;
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
            float farClippingPlane, float fieldOfView, float screenWidth, float screenHeight, ObservableCollection<string> logs) :
            base(name, positionVector, rotationVector, new DenseVector(new[] {1f, 1f, 1f, 0f}))
        {
            ScaleVector = new DenseVector(new[] {1f, 1f, 1f, 0f});

            Update(positionVector, rotationVector, nearClippingPlane, farClippingPlane, fieldOfView, screenWidth,
                screenHeight, name);

            _logs = logs;
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

        public IEnumerable<int[]> GeometryToRasterSpace(Geometry geometry)
        {
            //geometry.Geometry.Draw(_viewMatrix, _projectionMatrix, out Vector<float>[] verticesInScreenSpace, out topology);
            var vertices = WorldToCameraSpace(geometry.Vertices);

            Serializer.WriteVerticesToFile("torusTransformedToCameraSpace.txt", vertices);

            vertices = CameraToProjectionSpace(vertices);

            Serializer.WriteVerticesToFile("torusTransformedToProjectionSpaceBeforeDivide.txt", vertices);

            //vertices = vertices.Select(x => new DenseVector(new[] {x[0] / x[3], x[1] / x[3], x[2] / x[3], x[3] / x[3]}))
            //    .ToArray();

            return LinesToBeDrawn(vertices, geometry.Edges);

            //Serializer.WriteVerticesToFile("torusTransformedToProjectionSpace.txt", vertices);

            //List<Point> pointsInRasterSpaceList = new List<Point>();
            //clippedvertices = new bool[vertices.Length];

            //for(int i=0;i<vertices.Length;i++)
            //{
            //    if (vertices[i][0] < -1 || vertices[i][0] > 1 || vertices[i][1] < -1 || vertices[i][1] > 1 ||
            //        vertices[i][2] < -1 || vertices[i][2] > 1)
            //    {
            //        clippedvertices[i] = true;
            //        //pointsInRasterSpaceList.Add(new Point(0, 0));
            //        //continue;
            //    }

            //    int xRasterSpace = (int) ((vertices[i][0] + 1) * 0.5 * ScreenWidth);
            //    int yRasterSpace = (int) ((vertices[i][1] + 1) * 0.5 * ScreenHeight);

            //    pointsInRasterSpaceList.Add(new Point(xRasterSpace, yRasterSpace));
            //}

            //pointsInRasterSpace = pointsInRasterSpaceList.ToArray();

            //Serializer.WriteVerticesToFile("torusVerticesInRasterSpace.txt", pointsInRasterSpace);
        }

        public IEnumerable<int[]> LinesToBeDrawn(Vector<float>[] vertices, Edge[] topology)
        {
            foreach (Edge edge in topology)
            {
                bool ret = false;
                if (vertices[edge.Beginning][3] < NearClippingPlane &&
                    vertices[edge.End][3] < NearClippingPlane) continue;
                if (vertices[edge.Beginning][3] > FarClippingPlane &&
                    vertices[edge.End][3] > FarClippingPlane) continue;

                if(!MathHelpers.VectorInFrustum(vertices[edge.Beginning]) && !MathHelpers.VectorInFrustum(vertices[edge.End])) continue;

                Vector<float> beginning = vertices[edge.Beginning];
                Vector<float> end = vertices[edge.End];

                if (vertices[edge.Beginning][3] >= NearClippingPlane && vertices[edge.End][3] < NearClippingPlane)
                {
                    _logs.Add(
                        $"{vertices[edge.End][0]} {vertices[edge.End][1]} {vertices[edge.End][2]} {vertices[edge.End][3]} {vertices[edge.Beginning][0]} {vertices[edge.Beginning][1]} {vertices[edge.Beginning][2]} {vertices[edge.Beginning][3]}");
                    float n = (vertices[edge.Beginning][3] - NearClippingPlane) /
                              (vertices[edge.Beginning][3] - vertices[edge.End][3]);
                    float xc = (n * vertices[edge.Beginning][0]) + ((1 - n) * vertices[edge.End][0]);
                    float yc = (n * vertices[edge.Beginning][1]) + ((1 - n) * vertices[edge.End][1]);
                    float zc = (n * vertices[edge.Beginning][2]) + ((1 - n) * vertices[edge.End][2]);
                    float wc = (n * vertices[edge.Beginning][3]) + ((1 - n) * vertices[edge.End][3]);

                    end = new DenseVector(new[] { xc, yc, zc, wc });
                    ret = true;
                }

                //if (vertices[edge.End][3] >= NearClippingPlane && vertices[edge.Beginning][3] < NearClippingPlane)
                //{
                //    _logs.Add($"{vertices[edge.Beginning][0]} {vertices[edge.Beginning][1]} {vertices[edge.Beginning][2]} {vertices[edge.Beginning][3]} {vertices[edge.End][0]} {vertices[edge.End][1]} {vertices[edge.End][2]} {vertices[edge.End][3]}");
                //    float n = (vertices[edge.End][3] - NearClippingPlane) /
                //              (vertices[edge.End][3] - vertices[edge.Beginning][3]);
                //    float xc = (n * vertices[edge.End][0]) + ((1 - n) * vertices[edge.Beginning][0]);
                //    float yc = (n * vertices[edge.End][1]) + ((1 - n) * vertices[edge.Beginning][1]);
                //    float zc = (n * vertices[edge.End][2]) + ((1 - n) * vertices[edge.Beginning][2]);
                //    float wc = (n * vertices[edge.End][3]) + ((1 - n) * vertices[edge.Beginning][3]);

                //    beginning = new DenseVector(new[] {xc, yc, zc, wc});
                //    ret = true;
                //}

                //if (Math.Abs(beginning[3] - 1f) > float.Epsilon)
                //{
                //    beginning[0] /= beginning[3];
                //    beginning[1] /= beginning[3];
                //    beginning[2] /= beginning[3];
                //    beginning[3] = 1f;
                //}

                //if (Math.Abs(end[3] - 1f) > float.Epsilon)
                //{
                //    end[0] /= end[3];
                //    end[1] /= end[3];
                //    end[2] /= end[3];
                //    end[3] = 1f;
                //}

                //if (vertices[edge.Beginning][0] < -1 || vertices[edge.Beginning][0] > 1 ||
                //    vertices[edge.Beginning][1] < -1 || vertices[edge.Beginning][1] > 1 ||
                //    vertices[edge.Beginning][2] < -1 || vertices[edge.Beginning][2] > 1)
                //    continue;

                //if (vertices[edge.End][0] < -1 || vertices[edge.End][0] > 1 ||
                //    vertices[edge.End][1] < -1 || vertices[edge.End][1] > 1 ||
                //    vertices[edge.End][2] < -1 || vertices[edge.End][2] > 1)
                //    continue;

                int beginningX = (int) ((beginning[0] + 1) * 0.5 * ScreenWidth);
                int beginningY = (int) ((beginning[1] + 1) * 0.5 * ScreenHeight);
                int endX = (int) ((end[0] + 1) * 0.5 * ScreenWidth);
                int endY = (int) ((end[1] + 1) * 0.5 * ScreenHeight);

                yield return new[] {beginningX, beginningY, endX, endY};

            }
        }

        public Vector<float>[] WorldToCameraSpace(Vector<float>[] vertices)
        {
            return vertices.Select(x => ViewMatrix * x).ToArray();
        }

        public Vector<float>[] CameraToProjectionSpace(Vector<float>[] vertices)
        {
            var v = vertices.Select(x => ProjectionMatrix * x).ToArray();
            return v.Select(x => x.Divide(x[3])).ToArray();
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
            float s = 1f / (float)Math.Tan(Trig.DegreeToRadian(fieldOfView) / 2);
            float top = (float) Math.Tan(Trig.DegreeToRadian(fieldOfView) / 2) * nearClippingPlane;
            float bottom = -top;
            float right = top * Aspect;
            float left = -right;

            Matrix<float> projectionMatrix = new DenseMatrix(4, 4);
            //projectionMatrix[0, 0] = s;
            //projectionMatrix[1, 1] = s;
            //projectionMatrix[2, 2] = -farClippingPlane / (farClippingPlane - nearClippingPlane);
            //projectionMatrix[2, 3] = -farClippingPlane * nearClippingPlane / (farClippingPlane - nearClippingPlane);
            //projectionMatrix[3, 2] = -1;

            projectionMatrix[0, 0] = 2 * nearClippingPlane / (right - left);
            projectionMatrix[0, 2] = (right + left) / (right - left);
            projectionMatrix[1, 1] = 2 * nearClippingPlane / (top - bottom);
            projectionMatrix[1, 2] = (top + bottom) / (top - bottom);
            projectionMatrix[2, 2] = -(farClippingPlane + nearClippingPlane) / (farClippingPlane - nearClippingPlane);
            projectionMatrix[2, 3] = -2*farClippingPlane * nearClippingPlane / (farClippingPlane - nearClippingPlane);
            projectionMatrix[3, 2] = -1;

            return projectionMatrix;
        }


    }
}
