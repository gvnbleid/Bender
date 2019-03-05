using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary.Geometry
{
    public abstract class ParametricGeometry : Geometry
    {

        private int[][] _lines;
        public ParametricGeometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector) : base(name, positionVector, rotationVector, scaleVector)
        {

        }

        public override void Rasterize(Camera c)
        {
            var vertices = c.WorldToCameraSpace(Vertices);
            vertices = c.CameraToProjectionSpace(vertices);

            _lines = c.LinesToBeDrawn(vertices, Edges).ToArray();

        }

        public override VisualHost GetDataForDrawing()
        {
            VisualHost vH = new VisualHost(new Pen(Brushes.Beige, 1));
            vH.AddLines(_lines);

            return vH;
        }
    }
}
