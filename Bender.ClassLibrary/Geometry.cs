using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

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

            WorldMatrix = MathHelpers.CalculateTranslationMatrix(PositionVector) * MathHelpers.CalculateRotationMatrix(RotationVector) * MathHelpers.CalculateScaleMatrix(ScaleVector);
        }

        public virtual void Transform(Vector<float> transformVector)
        {
            var m = MathHelpers.CalculateTranslationMatrix(transformVector);
            PositionVector += transformVector;
            WorldMatrix  = m * WorldMatrix;
        }

        public virtual void Rotate(Vector<float> rotationVector)
        {
            var m = MathHelpers.CalculateRotationMatrix(rotationVector);
            RotationVector += rotationVector;
            WorldMatrix = m * WorldMatrix;
        }

        public virtual void PreScale(Vector<float> scaleVector)
        {
            Vector<float> newScaleVector = ScaleVector + scaleVector;
            Vector<float> relativeScaleVector = new DenseVector(new[] {newScaleVector[0] / ScaleVector[0], newScaleVector[1] / ScaleVector[1], newScaleVector[2] / ScaleVector[2], 0f});
            var m = MathHelpers.CalculateScaleMatrix(relativeScaleVector);
            ScaleVector = newScaleVector;
            WorldMatrix = m * WorldMatrix;
        }
    }
}
