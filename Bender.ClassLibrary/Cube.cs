using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public class Cube : IGeometry
    {
        public Geometry Geometry { get; }

        public Cube(float size)
        {
            List<Vector<float>> vertices = new List<Vector<float>>();
            List<Edge> edges = new List<Edge>();

            vertices.Add(new DenseVector(new[] {-size / 2, -size / 2, -size / 2, 1f}));
            vertices.Add(new DenseVector(new[] {size / 2, -size / 2, -size / 2, 1f}));
            vertices.Add(new DenseVector(new[] {size / 2, size / 2, -size / 2, 1f}));
            vertices.Add(new DenseVector(new[] {-size / 2, size / 2, -size / 2, 1f}));
            vertices.Add(new DenseVector(new[] { -size / 2, -size / 2, size / 2, 1f}));
            vertices.Add(new DenseVector(new[] { size / 2, -size / 2, size / 2, 1f }));
            vertices.Add(new DenseVector(new[] { size / 2, size / 2, size / 2, 1f }));
            vertices.Add(new DenseVector(new[] { -size / 2, size / 2, size / 2, 1f }));

            edges.Add(new Edge(0, 1));
            edges.Add(new Edge(1, 2));
            edges.Add(new Edge(2, 3));
            edges.Add(new Edge(3, 0));
            edges.Add(new Edge(4, 5));
            edges.Add(new Edge(5, 6));
            edges.Add(new Edge(6, 7));
            edges.Add(new Edge(7, 4));
            edges.Add(new Edge(0, 4));
            edges.Add(new Edge(1, 5));
            edges.Add(new Edge(2, 6));
            edges.Add(new Edge(3, 7));

            Geometry = new Geometry(vertices.ToArray(), edges.ToArray());
        }
    }
}
