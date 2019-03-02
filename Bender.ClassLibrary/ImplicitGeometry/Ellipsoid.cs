using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Bender.ClassLibrary.Geometry;
using Bender.ClassLibrary.Shaders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary.ImplicitGeometry
{
    public class Ellipsoid : ImplicitGeometry
    {
        private float _a;
        private float _b;
        private float _c;

        public Ellipsoid(string name, Vector<float> positionVector, Vector<float> rotationVector,
            Vector<float> scaleVector, float a, float b, float c) : base(name, positionVector, rotationVector,
            scaleVector)
        {
            _a = a;
            _b = b;
            _c = c;

            EquationMatrix = DenseMatrix.Create(4, 4, 0f);
            EquationMatrix.SetDiagonal(new[] {1 / (a * a), 1 / (b * b), 1 / (c * c), -1f});
        }

        protected override bool TrySolveEquation(Vector<float> cameraVector, Vector<float> positionVector,
            out (float first, float second) result)
        {
            float a = cameraVector[0] / _a;
            a = a * a;

            float b = cameraVector[1] / _b;
            b = b * b;

            float c = cameraVector[2] / _c;
            c = c * c;

            a = a + b + c;

            b = 2 * (positionVector[0] * cameraVector[0] / (_a * _a) + positionVector[1] * cameraVector[1] / (_b * _b) +
                     positionVector[2] * cameraVector[2] / (_c * _c));

            c = positionVector[0] * positionVector[0] / (_a * _a) + positionVector[1] * positionVector[1] / (_b * _b) +
                positionVector[2] * positionVector[2] / (_c * _c) - 1;

            return MathHelpers.TrySolveEquation(a, b, c, out result);
        }

        public override VisualHost Rasterize(Camera c)
        {
            Vector<float> versor = new DenseVector(new[] {1f, 1f, 1f, 1f});
            var versorByCamera = c.ViewMatrix * versor;
            float zCamera = versorByCamera[2];
            var versorByCameraByProjection = c.ProjectionMatrix * versorByCamera;

            VisualHost vh = new VisualHost(new Pen(Brushes.Yellow, 1f));

            PhongShader phongShader = c.Light;

            if (phongShader == null) return vh;

            for (int i = 0; i < (int) c.ScreenWidth; i += 2)
            {
                for (int j = 0; j < (int) c.ScreenHeight; j += 2)
                {
                    float screenX = (2 * i / c.ScreenWidth - 1);
                    float screenY = (2 * j / c.ScreenHeight - 1);

                    float x = screenX * (float) Math.Tan(c.FieldOfView / 2);
                    float y = screenY * (float) Math.Tan(c.FieldOfView / 2);

                    Vector<float> cameraVector = new DenseVector(new[] {x, y, -1, 1});

                    if (TrySolveEquation(cameraVector, c.PositionVector, out (float first, float second) result))
                    {
                        float z = result.first < result.second ? result.first : result.second;
                        vh.AddPoint(new Point(i, j), 2,
                            phongShader.GetColor(c, new DenseVector(new[] {x * z, y * z, z, 1f}),
                                new DenseVector(new[]
                                    {2 * x * z / (_a * _a), 2 * y * z / (_b * _b), 2 * z / (_c * _c), 0f}).Normalize(2),
                                Colors.Yellow));
                    }
                }
            }

            return vh;
        }
    }
}
