using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary
{
    public abstract class Geometry
    {
        public Vector<float> PositionVector { get; protected set; }

        public Vector<float> RotationVector { get; protected set; }

        public Vector<float> ScaleVector { get; protected set; }

        public Matrix<float> WorldMatrix { get; protected set; }
        public Vector<float>[] Vertices { get; protected set; }
        public Edge[] Edges { get; protected set; }

        public string Name { get; protected set; }

        protected Geometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector, Vector<float>[] vertices = null, Edge[] edges = null)
        {
            Name = name;
            Vertices = vertices;
            Edges = edges;

            PositionVector = positionVector.Clone();
            RotationVector = rotationVector.Clone();
            ScaleVector = scaleVector.Clone();

            WorldMatrix = MathHelpers.CalculateTranslationMatrix(PositionVector) * MathHelpers.CalculateRotationMatrix(RotationVector);
        }

        public override string ToString()
        {
            return Name;
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
    }
}
