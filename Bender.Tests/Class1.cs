using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Bender.ClassLibrary;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Xunit;

namespace Bender.Tests
{
    public class Class1
    {
        [Fact]
        public void Camera_WorldToCameraSpace_OnePointCheck()
        {
            Camera c = new Camera("camera", new DenseVector(new []{0f, 0f, 1f, 0f}), new DenseVector(new []{0f, 0f, 0f, 0f}), 0.1f, 10f, (float) Math.PI, 500, 500);
            Cube singlePoint = new Cube("cube", 2f);

            var points = c.WorldToCameraSpace(singlePoint.Vertices);

            Assert.Equal(new DenseVector(new float[]{-1, -1, -2, 1}), points[0]);
        }
    }
}
