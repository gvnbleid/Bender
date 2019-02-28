using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bender.ClassLibrary;
using Bender.GUI.ViewModels;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Xceed.Wpf.Toolkit;
using Geometry = System.Windows.Media.Geometry;
using Vector = System.Windows.Vector;
using Window = System.Windows.Window;

namespace Bender.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Bender.ClassLibrary.Geometry> _geometryObjects = new ObservableCollection<Bender.ClassLibrary.Geometry>();
        private readonly ObservableCollection<string> _logs = new ObservableCollection<string>();
        private readonly List<Bender.ClassLibrary.Geometry> _figures = new List<Bender.ClassLibrary.Geometry>();
        private Camera _camera;
        private Point _mousePressPoint;
        private List<DecimalUpDown> upDowns = new List<DecimalUpDown>();
        private Torus _torus;
        private GeometryViewModel _cameraViewModel;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            //ComponentDispatcher.ThreadIdle += ComponentDispatcherOnThreadIdle;

            GeometryListBox.ItemsSource = _geometryObjects;

            upDowns.Add(PositionXUpDown);
            upDowns.Add(PositionYUpDown);
            upDowns.Add(PositionZUpDown);
            upDowns.Add(RotationXUpDown);
            upDowns.Add(RotationYUpDown);
            upDowns.Add(RotationZUpDown);
            upDowns.Add(ScaleXUpDown);
            upDowns.Add(ScaleYUpDown);
            upDowns.Add(ScaleZUpDown);
            upDowns.Add(AlphaDensDecUpDown);
            upDowns.Add(BetaDensDecUpDown);
        }

        private void RefreshCoordinates()
        {
            Bender.ClassLibrary.Geometry g = GeometryListBox.SelectedItem as Bender.ClassLibrary.Geometry;
            if (g == null) return;

            PositionXUpDown.Value = (decimal) g.PositionVector[0];
            PositionYUpDown.Value = - (decimal) g.PositionVector[1];
            PositionZUpDown.Value = (decimal) g.PositionVector[2];
                                     
            RotationXUpDown.Value = (decimal) g.RotationVector[0];
            RotationYUpDown.Value = (decimal) g.RotationVector[1];
            RotationZUpDown.Value = (decimal) g.RotationVector[2];

            ScaleXUpDown.Value = (decimal) g.ScaleVector[0];
            ScaleYUpDown.Value = (decimal) g.ScaleVector[1];
            ScaleZUpDown.Value = (decimal) g.ScaleVector[2];

            AlphaDensDecUpDown.Value = (decimal) _torus.AlphaDensity;
            BetaDensDecUpDown.Value = (decimal) _torus.BetaDensity;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Torus torus = new Torus("torus", 2f, 0.5f, 4f, 4f);
            _torus = torus;
            //Cube torus = new Cube("cube", 1f);
            _camera = new Camera(
                "camera",
                new DenseVector(new[] {0f, 0f, 5f, 1f}),
                new DenseVector(new[] {0f, 0f, 0f, 0f}),
                0.1f,
                10f,
                60f,
                (float) SceneCanvas.ActualWidth,
                (float) SceneCanvas.ActualHeight,
                _logs);

            //_cameraViewModel = new GeometryViewModel(_camera);

            var lines = _camera.GeometryToRasterSpace(torus);

            VisualHost vH = new VisualHost(new Pen(Brushes.Beige, 1));
            vH.AddLines(lines);

            SceneCanvas.Children.Add(vH);

            _geometryObjects.Add(torus);
            _geometryObjects.Add(_camera);

            _figures.Add(torus);

            upDowns.ForEach(x => x.ValueChanged += UpDownOnValueChanged);

            SceneCanvas.Focus();

        }

        private void UpDownOnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        { 
            Vector<float> positionVector = new DenseVector(new[]
            {
                (float) (PositionXUpDown.Value ?? 0), - (float) (PositionYUpDown.Value ?? 0), (float) (PositionZUpDown.Value ?? 0), 1f
            });

            Vector<float> rotationVector = new DenseVector(new[]
            {
                (float) (RotationXUpDown.Value ?? 0), (float) (RotationYUpDown.Value ?? 0), (float) (RotationZUpDown.Value ?? 0), 0f
            });

            Vector<float> scaleVector = new DenseVector(new[]
            {
                (float) (ScaleXUpDown.Value ?? 0), (float) (ScaleYUpDown.Value ?? 0), (float) (ScaleZUpDown.Value ?? 0), 0f
            });

            Bender.ClassLibrary.Geometry g = GeometryListBox.SelectedItem as ClassLibrary.Geometry;
            g.Update(positionVector, rotationVector, scaleVector);
            _torus.UpdateDensity((float) (AlphaDensDecUpDown.Value ?? 30), (float) (BetaDensDecUpDown.Value ?? 30));

            UpdateScreen();
        }

        private void DrawOnCanvas(int[] coordinates)
        {

            Line line = new Line();

            line.X1 = coordinates[0];
            line.Y1 = coordinates[1];

            line.X2 = coordinates[2];
            line.Y2 = coordinates[3];

            line.Stroke = Brushes.Beige;
            line.StrokeThickness = 0.5;

            SceneCanvas.Children.Add(line);
        }

        private void GeometryListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            upDowns.ForEach(x => x.ValueChanged -= UpDownOnValueChanged);

            if (GeometryListBox.SelectedItem is Camera)
            {
                CameraPanel.Visibility = Visibility.Visible;
                SelectedElementDockPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                CameraPanel.Visibility = Visibility.Collapsed;
                SelectedElementDockPanel.Visibility = Visibility.Visible;
            }

            var geometryListBox = sender as ListBox;
            Bender.ClassLibrary.Geometry geometry = geometryListBox.SelectedItem as Bender.ClassLibrary.Geometry;

            PositionXUpDown.Text = geometry.PositionVector[0].ToString(CultureInfo.InvariantCulture);
            PositionYUpDown.Text = (-geometry.PositionVector[1]).ToString(CultureInfo.InvariantCulture);
            PositionZUpDown.Text = geometry.PositionVector[2].ToString(CultureInfo.InvariantCulture);

            RotationXUpDown.Text = geometry.RotationVector[0].ToString(CultureInfo.InvariantCulture);
            RotationYUpDown.Text = geometry.RotationVector[1].ToString(CultureInfo.InvariantCulture);
            RotationZUpDown.Text = geometry.RotationVector[2].ToString(CultureInfo.InvariantCulture);

            ScaleXUpDown.Text = geometry.ScaleVector[0].ToString(CultureInfo.InvariantCulture);
            ScaleYUpDown.Text = geometry.ScaleVector[1].ToString(CultureInfo.InvariantCulture);
            ScaleZUpDown.Text = geometry.ScaleVector[2].ToString(CultureInfo.InvariantCulture);

            AlphaDensDecUpDown.Value = (decimal) _torus.AlphaDensity;
            BetaDensDecUpDown.Value = (decimal) _torus.BetaDensity;

            upDowns.ForEach(x => x.ValueChanged += UpDownOnValueChanged);
        }

        private void UpdateScreen()
        {
            upDowns.ForEach(x => x.ValueChanged -= UpDownOnValueChanged);
            RefreshCoordinates();
            upDowns.ForEach(x => x.ValueChanged += UpDownOnValueChanged);

            SceneCanvas.Children.Clear();

            foreach (ClassLibrary.Geometry figure in _figures)
            {
                var lines = _camera.GeometryToRasterSpace(figure);

                VisualHost vH = new VisualHost(new Pen(Brushes.Beige, 1));
                vH.AddLines(lines);

                SceneCanvas.Children.Add(vH);
            }
        }

        private void SceneCanvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var startPosition = e.GetPosition(SceneCanvas);
        }

        private void SceneCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.RightButton != MouseButtonState.Pressed) return;
            Vector v = e.GetPosition(SceneCanvas) - _mousePressPoint;
            Vector<float> newRotation = new DenseVector(new []{_camera.RotationVector[0] + (float) v.Y * 0.01f, _camera.RotationVector[1] + (float) v.X * 0.01f, _camera.RotationVector[2], _camera.RotationVector[3]});

            //_camera.Update(_camera.PositionVector, newRotation, _camera.ScaleVector);
            UpdateScreen();

            _mousePressPoint = e.GetPosition(SceneCanvas);
        }

        private void GeometryListBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            float speed = 0.1f;
            speed = Keyboard.IsKeyDown(Key.LeftShift) ? speed * 5 : speed;

            Vector<float> dirVector = _camera.DirectionVector;
            Vector<float> xVector = new DenseVector(new[] { speed, 0f, 0f, 0f });
            Vector<float> yVector = new DenseVector(new[] { 0f, speed, 0f, 0f });
            Vector<float> zVector = new DenseVector(new[] { 0f, 0f, -speed, 0f });

            Vector<float> yRotation = new DenseVector(new[] { 0f, speed * 20, 0f, 0f });
            Vector<float> xRotation = new DenseVector(new[] { -speed * 20, 0f, 0f, 0f });



            if (Keyboard.IsKeyDown(Key.A))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    _camera.Rotate(yRotation);
                }
                else
                {
                    _camera.Transform(-xVector);
                }

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    _camera.Rotate(-yRotation);
                }
                else
                {
                    _camera.Transform(xVector);
                }

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.W))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    _camera.Rotate(xRotation);
                }
                else
                {
                    _camera.Transform(zVector);
                }

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.S))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    _camera.Rotate(-xRotation);
                }
                else
                {
                    _camera.Transform(-zVector);
                }

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.Q))
            {
                _camera.Transform(yVector);

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.E))
            {
                _camera.Transform(-yVector);

                UpdateScreen();
            }
        }

        private void SceneCanvas_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SceneCanvas.Focus();
        }
    }
}
