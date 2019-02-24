using System;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public static class MathHelpers
    {
        public static Matrix<float> CalculateRotationMatrix(Vector<float> rotationVector)
        {
            rotationVector[0] = (float) Trig.DegreeToRadian(rotationVector[0]);
            rotationVector[1] = (float)Trig.DegreeToRadian(rotationVector[1]);
            rotationVector[2] = (float)Trig.DegreeToRadian(rotationVector[2]);

            float cosX = (float) Math.Cos(rotationVector[0]);
            float sinX = (float) Math.Sin(rotationVector[0]);
            float cosY = (float) Math.Cos(rotationVector[1]);
            float sinY = (float) Math.Sin(rotationVector[1]);
            float cosZ = (float) Math.Cos(rotationVector[2]);
            float sinZ = (float) Math.Sin(rotationVector[2]);

            Matrix xRotationMatrix = new DenseMatrix(4, 4);
            xRotationMatrix.SetRow(0, new []{1f, 0f, 0f, 0f});
            xRotationMatrix.SetRow(1, new []{0f, cosX, -sinX, 0f});
            xRotationMatrix.SetRow(2, new []{0f, sinX, cosX, 0f});
            xRotationMatrix.SetRow(3, new []{0f, 0f, 0f, 1f});

            Matrix yRotationMatrix = new DenseMatrix(4, 4);
            yRotationMatrix.SetRow(0, new[] { cosY, 0f, sinY, 0f });
            yRotationMatrix.SetRow(1, new[] { 0f, 1f, 0f, 0f });
            yRotationMatrix.SetRow(2, new[] { -sinY, 0f, cosY, 0f });
            yRotationMatrix.SetRow(3, new[] { 0f, 0f, 0f, 1f });

            Matrix zRotationMatrix = new DenseMatrix(4, 4);
            zRotationMatrix.SetRow(0, new[] { cosZ, -sinZ, 0f, 0f });
            zRotationMatrix.SetRow(1, new[] { sinZ, cosZ, 0f, 0f });
            zRotationMatrix.SetRow(2, new[] { 0f, 0f, 1f, 0f });
            zRotationMatrix.SetRow(3, new[] { 0f, 0f, 0f, 1f });

            return zRotationMatrix * yRotationMatrix * xRotationMatrix;
        }

        public static Matrix<float> CalculateTranslationMatrix(Vector<float> positionVector)
        {
            Matrix<float> translationMatrix = new DenseMatrix(4,4);
            translationMatrix.SetDiagonal(new[] {1f, 1f, 1f, 1f});
            translationMatrix.SetColumn(3, positionVector);

            return translationMatrix;
        }

        public static Vector<float> CrossProduct(Vector<float> u, Vector<float> v)
        {
            if (u.Count != v.Count)
            {
                throw new ArgumentException("Vectors have to be the same length");
            }

            if (3 != u.Count)
            {
                throw new ArgumentOutOfRangeException("u", "Vector has to be 3-dimensional");
            }

            Vector product = new DenseVector(new [] {
                (u[1] * v[2]) - (u[2] * v[1]),
                (u[2] * v[0]) - (u[0] * v[2]),
                (u[0] * v[1]) - (u[1] * v[0])
            });

            return product;
        }
    }
}
