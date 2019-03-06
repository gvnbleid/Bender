using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Vector = System.Windows.Vector;

namespace Bender.ClassLibrary.Geometry
{
    public abstract class Geometry : IPositionable
    {
        public Vector<float> PositionVector { get; set; }
        public Vector<float> RotationVector { get; set; }
        public Vector<float> ScaleVector { get; set; }

        public Matrix<float> WorldMatrix { get; protected set; }
        public Vector<float>[] Vertices { get; protected set; }
        public Edge[] Edges { get; protected set; }

        public string Name { get; set; }

        protected Geometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector)
        {
            Name = name;

            PositionVector = positionVector.Clone();
            RotationVector = rotationVector.Clone();
            ScaleVector = scaleVector.Clone();

            WorldMatrix = MathHelpers.CalculateTranslationMatrix(PositionVector) * MathHelpers.CalculateRotationMatrix(RotationVector);
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

            WorldMatrix = MathHelpers.CalculateTranslationMatrix(PositionVector) * MathHelpers.CalculateRotationMatrix(RotationVector);
        }

        public void Transform(Vector<float> transformVector)
        {
            var m = MathHelpers.CalculateTranslationMatrix(transformVector);
            PositionVector += transformVector;
            WorldMatrix *= m;
        }

        public void Rotate(Vector<float> rotationVector)
        {
            var m = MathHelpers.CalculateRotationMatrix(rotationVector);
            RotationVector += rotationVector;
            WorldMatrix *= m;
        }

        public override string ToString()
        {
            return Name;
        }

        public abstract void Rasterize(Camera c);

        public abstract VisualHost GetDataForDrawing();
    }
}
