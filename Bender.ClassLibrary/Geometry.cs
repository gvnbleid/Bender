using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary
{
    public class Geometry
    {
        public Vector<float>[] Vertices { get; }
        public Edge[] Edges { get; }

        public Geometry(Vector<float>[] vertices, Edge[] edges)
        {
            Vertices = vertices;
            Edges = edges;
        }

        public void Draw(Matrix<float> viewMatrix, Matrix<float> projectionMatrix, out Vector<float>[] verticesInScreenSpace, out IEnumerable<Edge> topology)
        {
            foreach (var vertex in Vertices)
            {
                var xprim = viewMatrix * vertex;
                var xprimprim = projectionMatrix * xprim;


            }

            verticesInScreenSpace = Vertices.Select(x => (projectionMatrix * viewMatrix * x).Divide(x[2])).ToArray();
            topology = Edges;
        }
    }
}
