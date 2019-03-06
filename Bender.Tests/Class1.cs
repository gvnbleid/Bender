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

        [Fact]
        public void Camera_WorldToCameraSpace_InFrontOfCamera()
        {
            Camera c = new Camera("camera", new DenseVector(new[] { 0f, 0f, 2f, 1f }), new DenseVector(new[] { 0f, 0f, 0f, 0f }), 0.1f, 10f, (float)Math.PI, 500, 500);
            Cube singlePoint = new Cube("cube", 1f);

            var points = c.WorldToCameraSpace(singlePoint.Vertices);

            Assert.Equal(new DenseVector(new float[] { -0.5f, -0.5f, -1.5f, 1 }), points[4]);
        }

        [Fact]
        public void Camera_WorldToCameraSpace_BehindCamera()
        {
            Camera c = new Camera("camera", new DenseVector(new[] { 0f, 0f, 0f, 1f }), new DenseVector(new[] { 0f, 0f, 0f, 0f }), 0.1f, 10f, (float)Math.PI, 500, 500);
            Cube singlePoint = new Cube("cube", 1f);

            var points = c.WorldToCameraSpace(singlePoint.Vertices);

            Assert.Equal(new DenseVector(new float[] { -0.5f, -0.5f, 0.5f, 1 }), points[4]);
        }

        [Fact]
        public void MatrixInversion()
        {
            Camera c = new Camera("camera", new DenseVector(new[] { 0f, 0f, 2f, 1f }), new DenseVector(new[] { 0f, 0f, 0f, 0f }), 0.1f, 10f, (float)Math.PI, 500, 500);
            Matrix<float> m = new DenseMatrix(4, 4);
            m.SetDiagonal(new[] {1f, 1f, 1f, 1f});
            m[2, 3] = -2;
            Assert.Equal(m, c.ViewMatrix);
        }

        [Fact]
        public void Camera_WorldToCameraAndProjection()
        {
            Camera c = new Camera("camera", new DenseVector(new[] { 0f, 0f, 0f, 1f }), new DenseVector(new[] { 0f, 0f, 0f, 0f }), 0.1f, 0.5f, (float)Math.PI, 500, 500);
            Cube singlePoint = new Cube("cube", 1f);

            Assert.Equal(new DenseVector(new float[] {-0.5f, -0.5f, -0.5f, 1}), singlePoint.Vertices[0]);

            var points = c.WorldToCameraSpace(singlePoint.Vertices);

            Assert.Equal(new DenseVector(new float[] { -0.5f, -0.5f, -0.5f, 1 }), points[0]);

            points = c.CameraToProjectionSpace(points);

            Assert.Equal(1, points[0][2]);
        }

        [Fact]
        public void Camera_WorldToCameraAndProjection2()
        {
            Camera c = new Camera("camera", new DenseVector(new[] { 0f, 0f, 0f, 1f }), new DenseVector(new[] { 0f, 0f, 0f, 0f }), 0.01f, 0.02f, (float)Math.PI, 500, 500);
            Cube singlePoint = new Cube("cube", 0.2f);

            Assert.Equal(new DenseVector(new float[] { -0.1f, -0.1f, -0.1f, 1 }), singlePoint.Vertices[0]);

            var points = c.WorldToCameraSpace(singlePoint.Vertices);

            //Assert.Equal(new DenseVector(new float[] { -0.1f, -0.1f, 0.05f, 1 }), points[0]);

            points = c.CameraToProjectionSpace(points);

            Assert.Equal(-1, points[0][2]);
        }

        [Fact]
        public void Camera_WorldToCameraAndProjectionAndClip()
        {
            Camera c = new Camera("camera", new DenseVector(new[] { 0f, 0f, 0f, 1f }), new DenseVector(new[] { 0f, 0f, 0f, 0f }), 0.1f, 10f, (float)Math.PI, 500, 500);
            Cube singlePoint = new Cube("cube", 1f);

            Assert.Equal(new DenseVector(new float[] { -0.5f, -0.5f, 0.5f, 1 }), singlePoint.Vertices[4]);

            var points = c.WorldToCameraSpace(singlePoint.Vertices);
            points = c.CameraToProjectionSpace(points);
            

            Assert.Equal(new DenseVector(new float[] { -0.5f, -0.5f, -0.5f, 1 }), points[4]);
        }
    }
}
