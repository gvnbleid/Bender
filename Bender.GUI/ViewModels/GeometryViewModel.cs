using System.ComponentModel;
using System.Windows.Controls;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Geometry = Bender.ClassLibrary.Geometry.Geometry;

namespace Bender.GUI.ViewModels
{
    public abstract partial class GeometryViewModel : ViewModelBase
    {
        protected Geometry Geometry;

        private float _positionX;
        private float _positionY;
        private float _positionZ;
        private float _rotationX;
        private float _rotationY;
        private float _rotationZ;
        private float _scaleX;
        private float _scaleY;
        private float _scaleZ;

        protected GeometryViewModel() : base(null)
        {
            _positionX = 0;
            _positionY = 0;
            _positionZ = 0;
            _rotationX = 0;
            _rotationY = 0;
            _rotationZ = 0;
            _scaleX = 0;
            _scaleY = 0;
            _scaleZ = 0;
        }

        protected GeometryViewModel(Geometry g, SceneViewModel swm) : base(swm)
        {
            Geometry = g;

            _positionX = g.PositionVector[0];
            _positionY = g.PositionVector[1];
            _positionZ = g.PositionVector[2];

            _rotationX = g.RotationVector[0];
            _rotationY = g.RotationVector[1];
            _rotationZ = g.RotationVector[2];

            _scaleX = g.ScaleVector[0];
            _scaleY = g.ScaleVector[1];
            _scaleZ = g.ScaleVector[2];
        }

        public decimal PositionX
        {
            get => (decimal) _positionX;
            set => UpdateGeometryAndSetProperty(VectorKind.Transform, Coordinate.X, (float) value);
        }

        public decimal PositionY
        {
            get => (decimal) _positionY;
            set => UpdateGeometryAndSetProperty(VectorKind.Transform, Coordinate.Y, (float) value);
        }

        public decimal PositionZ
        {
            get => (decimal) _positionZ;
            set => UpdateGeometryAndSetProperty(VectorKind.Transform, Coordinate.Z, (float) value);
        }

        public decimal RotationX
        {
            get => (decimal) _rotationX;
            set => UpdateGeometryAndSetProperty(VectorKind.Rotate, Coordinate.X, (float) value);
        }

        public decimal RotationY
        {
            get => (decimal) _rotationY;
            set => UpdateGeometryAndSetProperty(VectorKind.Rotate, Coordinate.Y, (float) value);
        }

        public decimal RotationZ
        {
            get => (decimal) _rotationZ;
            set => UpdateGeometryAndSetProperty(VectorKind.Rotate, Coordinate.Z, (float) value);
        }

        public decimal ScaleX
        {
            get => (decimal) _scaleX;
            set => UpdateGeometryAndSetProperty(VectorKind.Scale, Coordinate.X, (float) value);
        }

        public decimal ScaleY
        {
            get => (decimal) _scaleY;
            set => UpdateGeometryAndSetProperty(VectorKind.Scale, Coordinate.Y, (float) value);
        }

        public decimal ScaleZ
        {
            get => (decimal) _scaleZ;
            set => UpdateGeometryAndSetProperty(VectorKind.Scale, Coordinate.Z, (float) value);
        }

        private void UpdateGeometryAndSetProperty(VectorKind k, Coordinate c, float value)
        {
            var v = CreateVector(k);
            v = new DenseVector(new[]
            {
                c == Coordinate.X ? value : v[0],
                c == Coordinate.Y ? value : v[1],
                c == Coordinate.Z ? value : v[2],
                0f
            });

            v = v - CreateVector(k);

            UpdateGeometry(k, v);

            SetProperty(c, k, value);

            SceneViewModel.Refresh();
        }

        private void SetProperty(Coordinate c, VectorKind k, float value)
        {
            switch (c)
            {
                case Coordinate.X:
                    switch (k)
                    {
                        case VectorKind.Rotate:
                            SetProperty(ref _rotationX, value);
                            break;
                        case VectorKind.Scale:
                            SetProperty(ref _scaleX, value);
                            break;
                        case VectorKind.Transform:
                            SetProperty(ref _positionX, value);
                            break;
                    }

                    break;
                case Coordinate.Y:
                    switch (k)
                    {
                        case VectorKind.Rotate:
                            SetProperty(ref _rotationY, value);
                            break;
                        case VectorKind.Scale:
                            SetProperty(ref _scaleY, value);
                            break;
                        case VectorKind.Transform:
                            SetProperty(ref _positionY, value);
                            break;
                    }

                    break;
                case Coordinate.Z:
                    switch (k)
                    {
                        case VectorKind.Rotate:
                            SetProperty(ref _rotationZ, value);
                            break;
                        case VectorKind.Scale:
                            SetProperty(ref _scaleZ, value);
                            break;
                        case VectorKind.Transform:
                            SetProperty(ref _positionZ, value);
                            break;
                    }

                    break;
            }
        }

        private Vector<float> CreateVector(VectorKind k)
        {
            switch (k)
            {
                case VectorKind.Rotate:
                    return new DenseVector(new[] {_rotationX, _rotationY, _rotationZ, 0f});
                case VectorKind.Scale:
                    return new DenseVector(new[] {_scaleX, _scaleY, _scaleZ, 0f});
                case VectorKind.Transform:
                    return new DenseVector(new[] {_positionX, _positionY, _positionZ, 0f});
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void UpdateGeometry(VectorKind k, Vector<float> v)
        {
            switch (k)
            {
                case VectorKind.Rotate:
                    Geometry.Rotate(v);
                    break;
                case VectorKind.Scale:
                    break;
                case VectorKind.Transform:
                    Geometry.Transform(v);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
