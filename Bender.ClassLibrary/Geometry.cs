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
    public abstract class Geometry
    {
        public Vector<float> PositionVector { get; protected set; }
        public Vector<float> RotationVector { get; protected set; }
        public Vector<float> ScaleVector { get; protected set; }
        public Vector<float>[] Vertices { get; protected set; }
        public Edge[] Edges { get; protected set; }

        public string Name { get; protected set; }

        protected Geometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector, Vector<float>[] vertices = null, Edge[] edges = null)
        {
            Name = name;
            Vertices = vertices;
            Edges = edges;

            PositionVector = positionVector;
            RotationVector = rotationVector;
            ScaleVector = scaleVector;
        }

        public override string ToString()
        {
            return Name;
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

        public virtual void Update(Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector)
        {
            PositionVector = positionVector.Clone();
            RotationVector = rotationVector.Clone();
            ScaleVector = scaleVector.Clone();
        }
    }
}
