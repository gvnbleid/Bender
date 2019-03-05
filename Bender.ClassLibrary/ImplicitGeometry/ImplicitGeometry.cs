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
        public float Specular { get; set; }
        public float Diffuse { get; set; }
        public float Ambient { get; set; }
        public float Shininess { get; set; }

        public Matrix<float> EquationMatrix { get; protected set; }

        public int NumberOfPoints { get; set; }
        public bool CanContinue { get; set; }

        protected abstract bool TrySolveEquation(Vector<float> cameraVector, Vector<float> positionVector,
            out (float first, float second) valueTuple);

        protected ImplicitGeometry(string name, Vector<float> positionVector, Vector<float> rotationVector, Vector<float> scaleVector) : base(name, positionVector, rotationVector, scaleVector)
        {
            Specular = 1;
            Diffuse = 1;
            Ambient = 1;
            Shininess = 1;
        }
    }
}
