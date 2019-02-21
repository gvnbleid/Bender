using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Bender.ClassLibrary
{
    public class Camera
    {
        private Matrix<float> _viewMatrix;
        private Matrix<float> _projectionMatrix;
        private Vector<float> _directionVector;
        private float _nearClippingPlane;
        private float _farClippingPlane;
        private float _fieldOfView;
        private float _aspect;

        public Camera(Vector<float> positionVector, Vector<float> rotationVector, float nearClippingPlane, float farClippingPlane, float fieldOfView)
        {
            Matrix<float> rotationMatrix = MathHelpers.CalculateRotationMatrix(rotationVector);
            _directionVector = rotationMatrix * new DenseVector(new[] {0f, 0f, -1f, 0f});
            _nearClippingPlane = nearClippingPlane;
            _farClippingPlane = farClippingPlane;
            _fieldOfView = fieldOfView;
            _aspect = 1;

            _viewMatrix = CalculateViewMatrix(positionVector, rotationVector);
            _projectionMatrix = CalculateProjectionMatrix(fieldOfView, _aspect, nearClippingPlane, farClippingPlane);
        }

        private Matrix<float> CalculateViewMatrix(Vector<float> positionVector, Vector<float> rotationVector)
        {
            if (positionVector.Count != 3)
            {
                throw new ArgumentException("Length of this vector has to be 3", nameof(positionVector));
            }

            if (rotationVector.Count != 3)
            {
                throw new ArgumentException("Length of this vector has to be 3", nameof(rotationVector));
            }

            Matrix<float> rotationMatrix = MathHelpers.CalculateRotationMatrix(rotationVector);
            Vector<float> preDirectionVector = rotationMatrix * new DenseVector(new[] { 0f, 0f, -1f, 0f });
            Vector<float> directionVector = new DenseVector(new[] {preDirectionVector[0], preDirectionVector[1], preDirectionVector[2]});

            Vector<float> upWorldVector = new DenseVector(new []{0f, 1f, 0f});
            Vector<float> rightVector = MathHelpers.CrossProduct(upWorldVector, directionVector).Normalize(2);
            Vector<float> upVector = MathHelpers.CrossProduct(directionVector, rightVector).Normalize(2);

            Matrix<float> viewMatrix = new DenseMatrix(4, 4);
            viewMatrix.SetColumn(0, new[] {rightVector[0], rightVector[1], rightVector[2], 0f});
            viewMatrix.SetColumn(1, new[] {upVector[0], upVector[1], upVector[2], 0f});
            viewMatrix.SetColumn(2, new[] {directionVector[0], directionVector[1], directionVector[2], 0f});
            viewMatrix.SetColumn(3, new[] {positionVector[0], positionVector[1], positionVector[2], 1f});

            return viewMatrix;
        }

        private Matrix<float> CalculateProjectionMatrix(float fieldOfView, float aspect, float nearClippingPlane, float farClippingPlane)
        {
            float scale = 1f / (float) Math.Tan(fieldOfView / 2);

            Matrix<float> projectionMatrix = new DenseMatrix(4, 4);
            projectionMatrix[0, 0] = scale / aspect;
            projectionMatrix[1, 1] = scale;
            projectionMatrix[2, 2] = -(farClippingPlane + nearClippingPlane) / (farClippingPlane - nearClippingPlane);
            projectionMatrix[2, 3] = -2 * farClippingPlane * nearClippingPlane / (farClippingPlane - nearClippingPlane);
            projectionMatrix[3, 2] = -1;

            return projectionMatrix;
        }

        public void Draw(Torus torus, WriteableBitmap sceneCanvas)
        {
            throw new NotImplementedException();
        }
    }
}
