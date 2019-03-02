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
        public ParametricGeometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector) : base(name, positionVector, rotationVector, scaleVector)
        {
        }

        public override VisualHost Rasterize(Camera c)
        {
            var vertices = c.WorldToCameraSpace(Vertices);
            vertices = c.CameraToProjectionSpace(vertices);

            VisualHost vH = new VisualHost(new Pen(Brushes.Beige, 1));
            vH.AddLines(c.LinesToBeDrawn(vertices, Edges));

            return vH;
        }

    }
}
