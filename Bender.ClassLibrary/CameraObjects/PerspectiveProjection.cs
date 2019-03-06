using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary.CameraObjects
{
    public class PerspectiveProjection : IProjection
    {
        public PerspectiveProjection(float nearClippingPlane, float farClippingPlane, float fieldOfView, float screenWidth, float screenHeight)
        {
            NearClippingPlane = nearClippingPlane;
            FarClippingPlane = farClippingPlane;
            FieldOfView = fieldOfView;
            Aspect = screenWidth / screenHeight;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public float NearClippingPlane { get; }
        public float FarClippingPlane { get; }
        public float FieldOfView { get; }
        public float Aspect { get; }

        public float ScreenWidth { get; }
        public float ScreenHeight { get; }

        public Matrix<float> PerspectiveProjectionMatrix
        {
            get
            {
                float top = (float)Math.Tan(Trig.DegreeToRadian(FieldOfView) / 2) * NearClippingPlane;
                float bottom = -top;
                float right = top * Aspect;
                float left = -right;

                Matrix<float> projectionMatrix = new DenseMatrix(4, 4);

                projectionMatrix[0, 0] = 2 * NearClippingPlane / (right - left);
                projectionMatrix[0, 2] = (right + left) / (right - left);
                projectionMatrix[1, 1] = 2 * NearClippingPlane / (top - bottom);
                projectionMatrix[1, 2] = (top + bottom) / (top - bottom);
                projectionMatrix[2, 2] = -(FarClippingPlane + NearClippingPlane) / (FarClippingPlane - NearClippingPlane);
                projectionMatrix[2, 3] = -2 * FarClippingPlane * NearClippingPlane / (FarClippingPlane - NearClippingPlane);
                projectionMatrix[3, 2] = -1;

                return projectionMatrix;
            }
        }

        

        public IEnumerable<LineGeometry> Project(IEnumerable<Vector<float>> vertices, IEnumerable<Edge> edges)
        {
            var matrix = PerspectiveProjectionMatrix;
            var v = vertices.Select(x => matrix * x).ToArray();
            v = v.Select(x => x.Divide(Math.Abs(x[3]))).ToArray();

            foreach (var e in edges)
            {
                if (CohenSutherland.TryClipLine(v[e.Beginning], v[e.End], out float[] line))
                {
                    int beginningX = (int)((line[0] + 1) * 0.5 * ScreenWidth);
                    int beginningY = (int)((line[1] + 1) * 0.5 * ScreenHeight);
                    int endX = (int)((line[2] + 1) * 0.5 * ScreenWidth);
                    int endY = (int)((line[3] + 1) * 0.5 * ScreenHeight);

                    yield return new LineGeometry(new Point(beginningX, beginningY), new Point(endX, endY));
                }
            }
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
    }
}
