using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            throw new NotImplementedException();
        }
    }
}
