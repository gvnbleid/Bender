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
using Matrix = System.Windows.Media.Matrix;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary.ImplicitGeometry
{
    public class Ellipsoid : ImplicitGeometry
    {
        public float A
        {
            get => _a;
            set
            {
                _a = value;
                EquationMatrix.SetDiagonal(new[] { 1 / (A * A), 1 / (B * B), 1 / (C * C), -1f });
            }
        }

        public float B
        {
            get => _b;
            set
            {
                _b = value;
                EquationMatrix.SetDiagonal(new[] { 1 / (A * A), 1 / (B * B), 1 / (C * C), -1f });
            }
        }

        public float C
        {
            get => _c;
            set
            {
                _c = value;
                EquationMatrix.SetDiagonal(new[] { 1 / (A * A), 1 / (B * B), 1 / (C * C), -1f });
            }
        }

        private List<(Point p, Color c)> _pointsWithColors;
        private int _stepX;
        private int _stepY;
        private float _a;
        private float _b;
        private float _c;

        public Ellipsoid(string name, Vector<float> positionVector, Vector<float> rotationVector,
            Vector<float> scaleVector, float a, float b, float c) : base(name, positionVector, rotationVector,
            scaleVector)
        {
            EquationMatrix = DenseMatrix.Create(4, 4, 0f);

            A = a;
            B = b;
            C = c;

            NumberOfPoints = 2;
        }

        protected override bool TrySolveEquation(Vector<float> cameraVector, Vector<float> positionVector,
            out (float first, float second) result)
        {
            float a = cameraVector[0] / A;
            a = a * a;

            float b = cameraVector[1] / B;
            b = b * b;

            float c = cameraVector[2] / C;
            c = c * c;

            a = a + b + c;

            b = -2 * (positionVector[0] * cameraVector[0] / (A * A) + positionVector[1] * cameraVector[1] / (B * B) +
                     positionVector[2] * cameraVector[2] / (C * C));

            c = positionVector[0] * positionVector[0] / (A * A) + positionVector[1] * positionVector[1] / (B * B) +
                positionVector[2] * positionVector[2] / (C * C) - 1;

            return MathHelpers.TrySolveEquation(a, b, c, out result);
        }

        public override void Rasterize(Camera c)
        {
            CanContinue = true;
            Vector<float> versor = new DenseVector(new[] {1f, 1f, 1f, 1f});
            var versorByCamera = c.ViewMatrix * versor;
            float zCamera = versorByCamera[2];
            var versorByCameraByProjection = c.ProjectionMatrix * versorByCamera;

            _pointsWithColors = new List<(Point p, Color c)>();

            //VisualHost vh = new VisualHost(new Pen(Brushes.Yellow, 1f));

            PhongShader phongShader = c.Light;

            if (phongShader == null) return;

            int iinc = Math.Max((int) c.ScreenWidth / NumberOfPoints, 1);
            int jinc = Math.Max((int) c.ScreenHeight / NumberOfPoints, 1);

            _stepX = iinc;
            _stepY = jinc;

            if (iinc == 1 && jinc == 1)
            {
                CanContinue = false;
            }

            for (int i = (int) c.ScreenWidth/(2*NumberOfPoints); i < (int) c.ScreenWidth; i += iinc)
            {
                for (int j = (int) c.ScreenHeight/(2*NumberOfPoints); j < (int) c.ScreenHeight; j += jinc)
                {
                    float screenX = (2 * i / c.ScreenWidth - 1);
                    float screenY = (2 * j / c.ScreenHeight - 1);

                    Matrix<float> m = (c.ProjectionMatrix * c.ViewMatrix).Inverse();
                    Matrix<float> dPrim = m.Transpose() * EquationMatrix * m;

                    Vector<float> vector = new DenseVector(4);
                    vector[0] = dPrim[2, 2];
                    vector[1] = screenX * (dPrim[0, 2] + dPrim[2, 0]) + screenY * (dPrim[1, 2] + dPrim[2, 1]) +
                                dPrim[2, 3] + dPrim[3, 2];
                    vector[2] = screenX * screenX * dPrim[0, 0] + screenX * screenY * (dPrim[0, 1] + dPrim[1, 0]) +
                                screenX * (dPrim[0, 3] + dPrim[3, 0]) + screenY * screenY * dPrim[1, 1] +
                                screenY * (dPrim[1, 3] + dPrim[3, 1]) + dPrim[3, 3];


                    Vector<float> screenVector = new DenseVector(new []{screenX, screenY, 1, 1});
                    var v = c.ProjectionMatrix.Inverse() * screenVector;
                    var vv = v * c.ViewMatrix;
                    //float x = screenX * (float) Math.Tan(c.FieldOfView / 2);
                    //float y = screenY * (float) Math.Tan(c.FieldOfView / 2);

                    //Vector<float> cameraVector = new DenseVector(new[] {x, y, -1, 1});

                    if (MathHelpers.TrySolveEquation(vector[0], vector[1], vector[2], out (float first, float second) result))
                    {
                        float z = result.first < result.second ? result.first : result.second;

                        //v = v * z;
                        //v = c.WorldMatrix * v;

                        //if ((result.first < -1 || result.first > 1) && (result.second < -1 || result.second > 1))
                        //    continue;

                        var v1 = m * (new DenseVector(new []{screenX, screenY, result.first, 1}));
                        var v2 = m * (new DenseVector(new[] { screenX, screenY, result.second, 1 }));
                        var vvv = (v1 - c.PositionVector).L2Norm() < (v2 - c.PositionVector).L2Norm() ? v1 : v2;

                        _pointsWithColors.Add((new Point(i, j), phongShader.GetColor(c, vvv,
                            new DenseVector(new[]
                                {2 * vvv[0] / (A * A), 2 * vvv[1] / (B * B), 2 * vvv[2] / (C * C), 0f}).Normalize(2),
                            Colors.Yellow)));

                        //_pointsWithColors.Add((new Point(i, j), Colors.Yellow));

                        //vh.AddPoint(new Point(i, j), 2,
                        //    phongShader.GetColor(c, v,
                        //        new DenseVector(new[]
                        //            {2 * v[0] / (A * A), 2 * v[1] / (B * B), 2 * v[2] / (C * C), 0f}).Normalize(2),
                        //        Colors.Yellow));
                    }
                }
            }

            NumberOfPoints *= 2;
        }

        public override VisualHost GetDataForDrawing()
        {
            VisualHost vh = new VisualHost(new Pen(Brushes.Yellow, 0.1));
            vh.AddPoints(_pointsWithColors, _stepX, _stepY);

            return vh;
        }
    }
}
