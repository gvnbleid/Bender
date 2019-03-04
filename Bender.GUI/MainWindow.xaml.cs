using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;
using Bender.ClassLibrary;
using Bender.ClassLibrary.Geometry;
using Bender.ClassLibrary.ImplicitGeometry;
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
        private SceneViewModel _sceneViewModel;
        private GeometryMode _geometryMode = GeometryMode.Parametric;
        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Torus torus = new Torus("torus", 2f, 0.5f, 4, 4);
            //Camera camera = new Camera(
            //    "camera",
            //    new DenseVector(new[] { 0f, 0f, 5f, 1f }),
            //    new DenseVector(new[] { 0f, 0f, 0f, 0f }),
            //    0.1f,
            //    10f,
            //    60f,
            //    (float)SceneCanvas.ActualWidth,
            //    (float)SceneCanvas.ActualHeight
            //    );

            //Ellipsoid ellipsoid = new Ellipsoid(
            //    "ellipsoid",
            //    new DenseVector(new[] {0f, 0f, 0f, 1f}),
            //    new DenseVector(new[] {0f, 0f, 0f, 0f}),
            //    new DenseVector(new[] {0f, 0f, 0f, 0f}),
            //    1f, 1f, 1f);

            _sceneViewModel = new SceneViewModel(SceneCanvas);
            //_sceneViewModel.Add(camera);
            //_sceneViewModel.Add(ellipsoid);

            GeometryListBox.DataContext = _sceneViewModel;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.WorkerSupportsCancellation = true;

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_sceneViewModel.IsReadyToDraw)
            {
                _sceneViewModel.IsReadyToDraw = false;
                _sceneViewModel.DrawOnCanvas();
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if(_sceneViewModel.IsReadyToDraw)
            {
                _sceneViewModel.Refresh();
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy) return;
            backgroundWorker.RunWorkerAsync();
        }

        private void GeometryListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GeometryListBox.SelectedIndex == -1) return;

            var vm = _sceneViewModel.CreateViewModel(GeometryListBox.SelectedIndex);
            SelectedElemDockPanel.Children.Clear();
            SelectedElemDockPanel.Children.Add(vm.CreateView());
        }

        private void GeometryListBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            backgroundWorker.CancelAsync();
            decimal transformSpeed = 0.1M;
            decimal rotationSpeed = 1M;
            Views.Camera c = null;
            if (SelectedElemDockPanel.Children.Count > 0)
            {
                c = SelectedElemDockPanel.Children[0] as Views.Camera;
            }
            if (Keyboard.IsKeyDown(Key.A))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Rotate, Coordinate.Y, Direction.Forward);
                    else c.RotationYUpDown.Value += rotationSpeed;
                }
                else
                { 
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Transform, Coordinate.X, Direction.Backward);
                    else c.PositionXUpDown.Value -= transformSpeed;
                }
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Rotate, Coordinate.Y, Direction.Backward);
                    else c.RotationYUpDown.Value -= rotationSpeed;
                }
                else
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Transform, Coordinate.X, Direction.Forward);
                    else c.PositionXUpDown.Value += transformSpeed;
                }
            }

            if (Keyboard.IsKeyDown(Key.W))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Rotate, Coordinate.X, Direction.Backward);
                    else c.RotationXUpDown.Value -= rotationSpeed;
                }
                else
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Transform, Coordinate.Z, Direction.Backward);
                    else c.PositionZUpDown.Value -= transformSpeed;
                }

            }

            if (Keyboard.IsKeyDown(Key.S))
            {
                if (Keyboard.IsKeyDown(Key.Space))
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Rotate, Coordinate.X, Direction.Forward);
                    else c.RotationXUpDown.Value += rotationSpeed;
                }
                else
                {
                    if (c == null) _sceneViewModel.MoveScene(VectorKind.Transform, Coordinate.Z, Direction.Forward);
                    else c.PositionZUpDown.Value += transformSpeed;
                }


            }

            if (Keyboard.IsKeyDown(Key.Q))
            {
                if (c == null) _sceneViewModel.MoveScene(VectorKind.Transform, Coordinate.Y, Direction.Forward);
                else c.PositionYUpDown.Value += transformSpeed;
            }

            if (Keyboard.IsKeyDown(Key.E))
            {
                if (c == null) _sceneViewModel.MoveScene(VectorKind.Transform, Coordinate.Y, Direction.Backward);
                else c.PositionYUpDown.Value -= transformSpeed;
            }
        }

        private void SceneCanvas_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SceneCanvas.Focus();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _geometryMode = GeometryMode.Parametric;
            _sceneViewModel.GeometryMode = GeometryMode.Parametric;
            GeometryListBox.SelectedIndex = 0;
            _sceneViewModel.Clear();

            _sceneViewModel.Add(new Camera(
                "camera",
                new DenseVector(new[] {0f, 0f, 5f, 1f}),
                new DenseVector(new[] {0f, 0f, 0f, 0f}),
                0.1f,
                100f,
                60f,
                (float) SceneCanvas.ActualWidth,
                (float) SceneCanvas.ActualHeight
            ));

            _sceneViewModel.Add(new Torus(
                "torus",
                2f,
                1f,
                3,
                3));

            _sceneViewModel.IsReadyToDraw = true;
        }

        private void MenuItem2_OnClick(object sender, RoutedEventArgs e)
        {
            _geometryMode = GeometryMode.Implicit;
            _sceneViewModel.GeometryMode = GeometryMode.Implicit;
            GeometryListBox.SelectedIndex = 0;
            _sceneViewModel.Clear();

            _sceneViewModel.Add(new Camera(
                "camera",
                new DenseVector(new[] { 0f, 0f, 5f, 1f }),
                new DenseVector(new[] { 0f, 0f, 0f, 0f }),
                0.1f,
                100f,
                60f,
                (float)SceneCanvas.ActualWidth,
                (float)SceneCanvas.ActualHeight
            ));

            _sceneViewModel.Add(new Ellipsoid(
                "ellipsoid",
                new DenseVector(new[] { 0f, 0f, 0f, 1f }),
                new DenseVector(new[] { 0f, 0f, 0f, 0f }),
                new DenseVector(new[] { 0f, 0f, 0f, 0f }),
                1f, 1f, 1f));

            _sceneViewModel.IsReadyToDraw = true;
        }

        private void GeometryListBox_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (_geometryMode)
            {
                case GeometryMode.Implicit:
                    TorusMenuItem.Visibility = Visibility.Hidden;
                    EllipsoidMenuItem.Visibility = Visibility.Visible;
                    break;
                case GeometryMode.Parametric:
                    TorusMenuItem.Visibility = Visibility.Visible;
                    EllipsoidMenuItem.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
