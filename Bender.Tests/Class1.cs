using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Bender.ClassLibrary;
using Xunit;

namespace Bender.Tests
{
    public class Class1
    {
        [Fact]
        public void MathHelpers_MultiplyMatrixByVector_IdentityByOnes()
        {
            Matrix4x4 m = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            Vector4 v = new Vector4(3, 5, 7, 9);

            Assert.Equal(v, MathHelpers.MultiplyMatrixByVector(m, v));
        }
    }
}
