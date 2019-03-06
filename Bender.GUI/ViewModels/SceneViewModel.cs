using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Bender.ClassLibrary;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Geometry = Bender.ClassLibrary.Geometry;

namespace Bender.GUI.ViewModels
{
    public class SceneViewModel : ViewModelBase
    {
        public ObservableCollection<Geometry> GeometryList { get; }

        private readonly List<Geometry> _figures;
        private readonly Canvas _canvas;
        private Camera _camera;

        private Vector<float> xVector = new DenseVector(new[] { 1f, 0f, 0f, 0f });
        private Vector<float> yVector = new DenseVector(new[] { 0f, 1f, 0f, 0f });
        private Vector<float> zVector = new DenseVector(new[] { 0f, 0f, 1f, 0f });
        private Vector<float> yRotation = new DenseVector(new[] { 0f, 1f, 0f, 0f });
        private Vector<float> xRotation = new DenseVector(new[] { 1f, 0f, 0f, 0f });
        private Vector<float> zRotation = new DenseVector(new[] { 0f, 0f, 1f, 0f });

        internal SceneViewModel(Canvas c)
        {
            _canvas = c;
            GeometryList = new ObservableCollection<Geometry>();
            _figures = new List<Geometry>();
        }

        internal int Count => GeometryList.Count;

        internal void Add(Geometry g)
        {
            GeometryList.Add(g);
            if (g.GetType() != typeof(Camera)) _figures.Add(g);
            else _camera = g as Camera;

            if(_camera != null) Refresh();
        }

        internal GeometryViewModel CreateViewModel(int index)
        {
            Type t = GeometryList[index].GetType();

            if(t == typeof(Camera)) return new CameraViewModel((Camera) GeometryList[index], this);
            if(t == typeof(Torus)) return new TorusViewModel((Torus) GeometryList[index], this);

            throw new Exception("Geometry element is not properly handled in GeometryListViewModel");
        }

        internal void Refresh()
        {
            _canvas.Children.Clear();

            foreach (Geometry figure in _figures)
            {
                var lines = _camera.GeometryToRasterSpace(figure);
                PathGeometry pathGeometry = new PathGeometry();
                foreach (var line in lines)
                {
                    pathGeometry.AddGeometry(line);
                }

                Path p = new Path() { Stroke = Brushes.Beige, StrokeThickness = 1 };
                p.Data = pathGeometry;
                _canvas.Children.Add(p);
            }
        }

        public void MoveScene(VectorKind k, Coordinate c, Direction d, float speed)
        {
            int i = d == Direction.Backward ? -1 : 1;
            switch (k)
            {
                case VectorKind.Rotate:
                    switch (c)
                    {
                        case Coordinate.X:
                            _camera.Rotate(i * xRotation * speed);
                            break;
                        case Coordinate.Y:
                            _camera.Rotate(i * yRotation * speed);
                            break;
                        case Coordinate.Z:
                            _camera.Rotate(i * zRotation * speed);
                            break;
                    }

                    break;
                case VectorKind.Transform:
                    switch (c)
                    {
                        case Coordinate.X:
                            _camera.Transform(i * xVector * speed);
                            break;
                        case Coordinate.Y:
                            _camera.Transform(i * yVector * speed);
                            break;
                        case Coordinate.Z:
                            _camera.Transform(i * zVector * speed);
                            break;
                    }

                    break;
            }

            Refresh();
        }
    }
}
