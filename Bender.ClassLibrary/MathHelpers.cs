using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public static class MathHelpers
    {
        public static Matrix<float> CalculateRotationMatrix(Vector<float> rotationVector)
        {
            var rad = rotationVector.Storage.AsArray().Select(x => (float) Trig.DegreeToRadian(x)).ToArray();
            var radianRotation = new DenseVector(rad);

            float cosX = (float) Math.Cos(radianRotation[0]);
            float sinX = (float) Math.Sin(radianRotation[0]);
            float cosY = (float) Math.Cos(radianRotation[1]);
            float sinY = (float) Math.Sin(radianRotation[1]);
            float cosZ = (float) Math.Cos(radianRotation[2]);
            float sinZ = (float) Math.Sin(radianRotation[2]);

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
            translationMatrix[3, 3] = 1;

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

        public static bool PointInFrustum(Vector<float> vector)
        {
            if (vector[0] < -1 || vector[0] > 1) return false;
            if (vector[1] < -1 || vector[1] > 1) return false;
            if (vector[2] < -1 || vector[2] > 1) return false;

            return true;
        }

        public static bool TrySolveEquation(float a, float b, float c, out (float first, float second) result)
        {
            result = (0f, 0f);

            float delta = b * b - 4 * a * c;

            if (delta < 0) return false;

            float sqrtDelta = (float) Math.Sqrt(delta);

            float first = (-b - sqrtDelta) * 0.5f / a;
            float second = (-b + sqrtDelta) * 0.5f / a;

            result = (first, second);

            return true;
        }
    }
}
