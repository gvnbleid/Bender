using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary.ImplicitGeometry
{
    public abstract class ImplicitGeometry : Geometry.Geometry
    {
        public Matrix<float> EquationMatrix { get; protected set; }

        public int NumberOfPoints { get; set; }
        public bool CanContinue { get; set; }

        protected abstract bool TrySolveEquation(Vector<float> cameraVector, Vector<float> positionVector,
            out (float first, float second) valueTuple);

        protected ImplicitGeometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector) : base(name, positionVector, rotationVector, scaleVector)
        {
        }
    }
}
