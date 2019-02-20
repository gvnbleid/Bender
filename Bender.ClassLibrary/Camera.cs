using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public class Camera
    {
        private Matrix _viewMatrix;
        private Matrix _perspectiveMatrix;
        private Vector _directionVector;
        private float _nearClippingPlane;
        private float _farClippingPlane;
        private float _fieldOfView;
        private float _aspect;

    }
}
