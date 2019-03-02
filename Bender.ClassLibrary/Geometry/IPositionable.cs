using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary.Geometry
{
    public interface IPositionable
    {
        Vector<float> PositionVector { get; set; }

        Vector<float> RotationVector { get; set; }

        Vector<float> ScaleVector { get; set; }

        string Name { get; set; }
    }
}
