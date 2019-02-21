using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public class Geometry
    {
        private Vector4[] _vertices;
        private Edge[] _edges;

        public Geometry(Vector4[] vertices, Edge[] edges)
        {
            _vertices = vertices;
            _edges = edges;
        }

        public void Draw()
        {
            Matrix4x4 perspectiveMatrix = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 0, 0,
                0, 0, 0.1f, 1);
            throw new NotImplementedException();
        }
    }
}
