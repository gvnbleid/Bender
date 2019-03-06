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
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary.ImplicitGeometry
{
    public class Ellipsoid : ImplicitGeometry
    {
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }

        private List<(Point p, Color c)> _pointsWithColors;
        private int _stepX;
        private int _stepY;

        public Ellipsoid(string name, Vector<float> positionVector, Vector<float> rotationVector,
            Vector<float> scaleVector, float a, float b, float c) : base(name, positionVector, rotationVector,
            scaleVector)
        {
            A = a;
            B = b;
            C = c;

            EquationMatrix = DenseMatrix.Create(4, 4, 0f);
            EquationMatrix.SetDiagonal(new[] {1 / (a * a), 1 / (b * b), 1 / (c * c), -1f});

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

            b = 2 * (positionVector[0] * cameraVector[0] / (A * A) + positionVector[1] * cameraVector[1] / (B * B) +
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

                    if (Math.Abs(screenX) < 0.01f && Math.Abs(screenY) < 0.01f)
                    {
                        int adasd = 5;
                    }

                    Vector<float> screenVector = new DenseVector(new []{screenX, screenY, 1, 1});
                    var v = c.ProjectionMatrix.Inverse() * screenVector;

                    //float x = screenX * (float) Math.Tan(c.FieldOfView / 2);
                    //float y = screenY * (float) Math.Tan(c.FieldOfView / 2);

                    //Vector<float> cameraVector = new DenseVector(new[] {x, y, -1, 1});

                    if (TrySolveEquation(v, c.PositionVector, out (float first, float second) result))
                    {
                        float z = result.first < result.second ? result.first : result.second;

                        v = v * z;
                        v = c.WorldMatrix * v;

                        _pointsWithColors.Add((new Point(i, j), phongShader.GetColor(c, v,
                            new DenseVector(new[]
                                {2 * v[0] / (A * A), 2 * v[1] / (B * B), 2 * v[2] / (C * C), 0f}).Normalize(2),
                            Colors.Yellow)));

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
            _pointsWithColors.ForEach(p => vh.AddPoint(p.p, _stepX, _stepY, p.c));

            return vh;
        }
    }
}
