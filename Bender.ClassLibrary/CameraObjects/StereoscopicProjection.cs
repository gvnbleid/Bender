using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary.CameraObjects
{
    public class StereoscopicProjection : IProjection
    {
        public StereoscopicProjection(float screenWidth, float screenHeight, float distanceBetweenEyes, float projectionDistance)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            DistanceBetweenEyes = distanceBetweenEyes;
            ProjectionDistance = projectionDistance;
        }

        public Matrix<float> LeftProjectionMatrix
        {
            get
            {
                Matrix<float> matrix = new DenseMatrix(4, 4);
                matrix[0, 0] = 1f;
                matrix[0, 2] = -DistanceBetweenEyes / (4 * ProjectionDistance);
                matrix[1, 1] = 1f;
                matrix[3, 2] = 1f / ProjectionDistance;
                matrix[3, 3] = 1f;

                return matrix;
            }
        }

        public Matrix<float> RightProjectionMatrix
        {
            get
            {
                Matrix<float> matrix = new DenseMatrix(4, 4);
                matrix[0, 0] = 1f;
                matrix[0, 2] = DistanceBetweenEyes / (4 * ProjectionDistance);
                matrix[1, 1] = 1f;
                matrix[3, 2] = 1f / ProjectionDistance;
                matrix[3, 3] = 1f;

                return matrix;
            }
        }

        public float DistanceBetweenEyes { get; }
        public float ProjectionDistance { get; }

        public float ScreenWidth { get; }
        public float ScreenHeight { get; }
        public IEnumerable<LineGeometry> Project(IEnumerable<Vector<float>> vertices, IEnumerable<Edge> edges)
        {
            var leftMatrix = LeftProjectionMatrix;
            var rightMatrix = RightProjectionMatrix;

            var v = vertices.Select(x => leftMatrix * x).ToArray();
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

            v = vertices.Select(x => rightMatrix * x).ToArray();
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
    }
}
