using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Bender.ClassLibrary;
using Bender.ClassLibrary.Geometry;
using Bender.ClassLibrary.ImplicitGeometry;
using Bender.ClassLibrary.Shaders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Geometry = Bender.ClassLibrary.Geometry.Geometry;

namespace Bender.GUI.ViewModels
{
    public class SceneViewModel : ViewModelBase
    {
        public ObservableCollection<IPositionable> GeometryList { get; }

        private readonly List<Geometry> _figures;
        private readonly Canvas _canvas;
        private Camera _camera;
        private PhongShader _light;

        private Vector<float> xVector = new DenseVector(new[] { 0.1f, 0f, 0f, 0f });
        private Vector<float> yVector = new DenseVector(new[] { 0f, 0.1f, 0f, 0f });
        private Vector<float> zVector = new DenseVector(new[] { 0f, 0f, 0.1f, 0f });
        private Vector<float> yRotation = new DenseVector(new[] { 0f, 0.1f * 20, 0f, 0f });
        private Vector<float> xRotation = new DenseVector(new[] { 0.1f * 20, 0f, 0f, 0f });
        private Vector<float> zRotation = new DenseVector(new[] { 0f, 0f, 0.1f * 20, 0f });

        private List<VisualHost> visualHosts;

        public GeometryMode GeometryMode { get; set; }

        internal SceneViewModel(Canvas c) : base(null)
        {
            _canvas = c;
            GeometryList = new ObservableCollection<IPositionable>();
            _figures = new List<Geometry>();
        }

        internal int Count => GeometryList.Count;

        internal void Add(IPositionable g)
        {
            GeometryList.Add(g);
            if(g.GetType() == typeof(Camera)) _camera = g as Camera;
            else if (g.GetType() == typeof(PhongShader)) _light = g as PhongShader;
            else _figures.Add(g as Geometry);

            if (g.GetType() == typeof(Ellipsoid))
            {
                _light = new PhongShader("light", new DenseVector(new[] {3f, 3f, 3f, 1f}), Colors.Black, Colors.White);
                GeometryList.Add(_light);
            }

            if (_camera != null) Refresh();
        }

        internal ViewModelBase CreateViewModel(int index)
        {
            Type t = GeometryList[index].GetType();

            if(t == typeof(Camera)) return new CameraViewModel((Camera) GeometryList[index], this);
            if(t == typeof(Torus)) return new TorusViewModel((Torus) GeometryList[index], this);
            if (t == typeof(PhongShader)) return new LightViewModel((PhongShader) GeometryList[index], this);
            if(t == typeof(Ellipsoid)) return new EllipsoidViewModel((Ellipsoid) GeometryList[index], this);

            throw new Exception("Geometry element is not properly handled in GeometryListViewModel");
        }

        internal void Refresh()
        {
            //_canvas.Children.Clear();
            visualHosts = new List<VisualHost>();
            if (_camera == null) return;

            _camera.Light = _light;
            if (GeometryMode == GeometryMode.Parametric)
            {
                foreach (Geometry figure in _figures)
                {
                    figure.Rasterize(_camera);
                    //visualHosts.Add(vh);
                    //var a =_canvas.Dispatcher.InvokeAsync(() => { _canvas.Children.Add(vh); });
                    //_canvas.Children.Add(vh);
                    //VisualHost vh = null;

                    //var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    //var t = new Task<VisualHost>(() => figure.Rasterize(_camera));
                    ////t.ContinueWith(r => _canvas.Children.Add(r.Result), scheduler);
                    //t.Start(scheduler);
                    //_tasks.Add(t);

                    //Task.Run(async () => await t.ContinueWith(r => _canvas.Children.Add(r.Result), scheduler));

                    //Task<VisualHost>.Factory.StartNew(() => { return figure.Rasterize(_camera);}, Taskcre)

                    //Application.Current.Dispatcher.Invoke(() => { _canvas.Children.Add(vh); });

                }
            }
            else
            {
                foreach (ImplicitGeometry figure in _figures)
                {
                    //_canvas.Children.Clear();
                        figure.Rasterize(_camera);
                        //visualHosts.Add(vh);
                        //_canvas.Children.Add(vh);
                        //VisualHost vh = null;
                        //var thread = new Thread(
                        //    () => { vh = figure.Rasterize(_camera); });
                        //thread.SetApartmentState(ApartmentState.STA);

                        //thread.Start();
                        //thread.Join();

                        //var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                        //var t = new Task<VisualHost>(() => figure.Rasterize(_camera));
                        ////t.ContinueWith(r => _canvas.Children.Add(r.Result), scheduler);
                        //t.Start(scheduler);
                        //_tasks.Add(t);

                        //Task.Run(async () => await t.ContinueWith(r => _canvas.Children.Add(r.Result), scheduler));

                        //Task<VisualHost>.Factory.StartNew(() => { return figure.Rasterize(_camera);}, Taskcre)

                        //Application.Current.Dispatcher.Invoke(() => { _canvas.Children.Add(vh); });
                    
                }
            }
            
        }

        internal void DrawOnCanvas()
        {
            _canvas.Children.Clear();
            foreach (Geometry figure in _figures)
            {
                _canvas.Children.Add(figure.GetDataForDrawing());
            }
            
            //visualHosts?.ForEach(v => _canvas.Children.Add(v));
        }

        internal void Clear()
        {
            GeometryList.Clear();
            _figures.Clear();
        }

        public void MoveScene(VectorKind k, Coordinate c, Direction d)
        {
            int i = d == Direction.Backward ? -1 : 1;
            switch (k)
            {
                case VectorKind.Rotate:
                    switch (c)
                    {
                        case Coordinate.X:
                            _camera.Rotate(i * xRotation);
                            break;
                        case Coordinate.Y:
                            _camera.Rotate(i * yRotation);
                            break;
                        case Coordinate.Z:
                            _camera.Rotate(i * zRotation);
                            break;
                    }

                    break;
                case VectorKind.Transform:
                    switch (c)
                    {
                        case Coordinate.X:
                            _camera.Transform(i * xVector);
                            break;
                        case Coordinate.Y:
                            _camera.Transform(i * yVector);
                            break;
                        case Coordinate.Z:
                            _camera.Transform(i * zVector);
                            break;
                    }

                    break;
            }

            Refresh();
        }

        public override UserControl CreateView()
        {
            throw new NotImplementedException();
        }
    }
}
