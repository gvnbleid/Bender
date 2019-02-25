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
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
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
        private List<TextBox> textboxes = new List<TextBox>();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            ComponentDispatcher.ThreadIdle += ComponentDispatcherOnThreadIdle;

            GeometryListBox.ItemsSource = _geometryObjects;
            ConsoleListBox.ItemsSource = _logs;

            textboxes.Add(PositionXTextBox);
            textboxes.Add(PositionYTextBox);
            textboxes.Add(PositionZTextBox);
            textboxes.Add(RotationXTextBox);
            textboxes.Add(RotationYTextBox);
            textboxes.Add(RotationZTextBox);
            textboxes.Add(ScaleXTextBox);
            textboxes.Add(ScaleYTextBox);
            textboxes.Add(ScaleZTextBox);
        }

        private void ComponentDispatcherOnThreadIdle(object sender, EventArgs e)
        {

            Vector<float> dirVector = _camera.DirectionVector;
            Vector<float> xVector = new DenseVector(new []{-dirVector[2], 0f, dirVector[0], 0f});
            Vector<float> zVector = new DenseVector(new[] {-dirVector[0], 0f, -dirVector[2], 0f});

            if (Keyboard.IsKeyDown(Key.A))
            {
                Vector<float> newPositionVector = _camera.PositionVector - xVector * 0.01f;
                _camera.Update(newPositionVector, _camera.RotationVector, _camera.ScaleVector);

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                //Vector<float> newPositionVector = new DenseVector(new[] { _camera.PositionVector[0] + 0.01f, _camera.PositionVector[1], _camera.PositionVector[2], _camera.PositionVector[3] });
                Vector<float> newPositionVector = _camera.PositionVector + xVector * 0.01f;
                _camera.Update(newPositionVector, _camera.RotationVector, _camera.ScaleVector);

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.W))
            {
                //Vector<float> newPositionVector = new DenseVector(new[] { _camera.PositionVector[0], _camera.PositionVector[1], _camera.PositionVector[2] - 0.01f, _camera.PositionVector[3]  });
                Vector<float> newPositionVector = _camera.PositionVector - zVector * 0.01f;
                _camera.Update(newPositionVector, _camera.RotationVector, _camera.ScaleVector);

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.S))
            {
                //Vector<float> newPositionVector = new DenseVector(new[] { _camera.PositionVector[0], _camera.PositionVector[1], _camera.PositionVector[2] + 0.01f, _camera.PositionVector[3] });
                Vector<float> newPositionVector = _camera.PositionVector + zVector * 0.01f;
                _camera.Update(newPositionVector, _camera.RotationVector, _camera.ScaleVector);

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.Q))
            {
                Vector<float> newPositionVector = new DenseVector(new[] { _camera.PositionVector[0], _camera.PositionVector[1] + 0.01f, _camera.PositionVector[2], _camera.PositionVector[3] });
                _camera.Update(newPositionVector, _camera.RotationVector, _camera.ScaleVector);

                UpdateScreen();
            }

            if (Keyboard.IsKeyDown(Key.E))
            {
                Vector<float> newPositionVector = new DenseVector(new[] { _camera.PositionVector[0], _camera.PositionVector[1] - 0.01f, _camera.PositionVector[2], _camera.PositionVector[3] });
                _camera.Update(newPositionVector, _camera.RotationVector, _camera.ScaleVector);

                UpdateScreen();
            }

        }

        private void RefreshCoordinates()
        {
            Bender.ClassLibrary.Geometry g = GeometryListBox.SelectedItem as Bender.ClassLibrary.Geometry;
            if (g == null) return;

            PositionXTextBox.Text = g.PositionVector[0].ToString(CultureInfo.InvariantCulture);
            PositionYTextBox.Text = g.PositionVector[1].ToString(CultureInfo.InvariantCulture);
            PositionZTextBox.Text = g.PositionVector[2].ToString(CultureInfo.InvariantCulture);

            RotationXTextBox.Text = g.RotationVector[0].ToString(CultureInfo.InvariantCulture);
            RotationYTextBox.Text = g.RotationVector[1].ToString(CultureInfo.InvariantCulture);
            RotationZTextBox.Text = g.RotationVector[2].ToString(CultureInfo.InvariantCulture);

            ScaleXTextBox.Text = g.ScaleVector[0].ToString(CultureInfo.InvariantCulture);
            ScaleYTextBox.Text = g.ScaleVector[1].ToString(CultureInfo.InvariantCulture);
            ScaleZTextBox.Text = g.ScaleVector[2].ToString(CultureInfo.InvariantCulture);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Torus torus = new Torus("torus", 2f, 0.5f, 0.5f, 0.5f);
            //Cube cube = new Cube("cube", 1

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

            var lines = _camera.GeometryToRasterSpace(torus);
            foreach (int[] line in lines)
            {
                DrawOnCanvas(line);
            }

            _geometryObjects.Add(torus);
            _geometryObjects.Add(_camera);

            _figures.Add(torus);


            textboxes.ForEach(x => x.TextChanged += CoordinateTextBox_OnTextChanged);

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
            var geometryListBox = sender as ListBox;
            Bender.ClassLibrary.Geometry geometry = geometryListBox.SelectedItem as Bender.ClassLibrary.Geometry;

            PositionXTextBox.Text = geometry.PositionVector[0].ToString(CultureInfo.InvariantCulture);
            PositionYTextBox.Text = geometry.PositionVector[1].ToString(CultureInfo.InvariantCulture);
            PositionZTextBox.Text = geometry.PositionVector[2].ToString(CultureInfo.InvariantCulture);

            RotationXTextBox.Text = geometry.RotationVector[0].ToString(CultureInfo.InvariantCulture);
            RotationYTextBox.Text = geometry.RotationVector[1].ToString(CultureInfo.InvariantCulture);
            RotationZTextBox.Text = geometry.RotationVector[2].ToString(CultureInfo.InvariantCulture);

            ScaleXTextBox.Text = geometry.ScaleVector[0].ToString(CultureInfo.InvariantCulture);
            ScaleYTextBox.Text = geometry.ScaleVector[1].ToString(CultureInfo.InvariantCulture);
            ScaleZTextBox.Text = geometry.ScaleVector[2].ToString(CultureInfo.InvariantCulture);
        }

        private void CoordinateTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!float.TryParse(PositionXTextBox.Text.Replace('.', ','), out float positionX)) return;
            if (!float.TryParse(PositionYTextBox.Text.Replace('.', ','), out float positionY)) return;
            if (!float.TryParse(PositionZTextBox.Text.Replace('.', ','), out float positionZ)) return;

            if (!float.TryParse(RotationXTextBox.Text.Replace('.', ','), out float rotationX)) return;
            if (!float.TryParse(RotationYTextBox.Text.Replace('.', ','), out float rotationY)) return;
            if (!float.TryParse(RotationZTextBox.Text.Replace('.', ','), out float rotationZ)) return;

            if (!float.TryParse(ScaleXTextBox.Text.Replace('.', ','), out float scaleX)) return;
            if (!float.TryParse(ScaleYTextBox.Text.Replace('.', ','), out float scaleY)) return;
            if (!float.TryParse(ScaleZTextBox.Text.Replace('.', ','), out float scaleZ)) return;
            Vector<float> positionVector = new DenseVector(new[]
            {
                positionX, -positionY, positionZ, 1f
            });

            Vector<float> rotationVector = new DenseVector(new[]
            {
                (float) rotationX, (float) rotationY,
                (float) rotationZ, 0f
            });

            Vector<float> scaleVector = new DenseVector(new[]
            {
                scaleX, scaleY, scaleZ, 0f
            });

            Bender.ClassLibrary.Geometry g = GeometryListBox.SelectedItem as ClassLibrary.Geometry;
            g.Update(positionVector, rotationVector, scaleVector);

            UpdateScreen();
        }

        private void UpdateScreen()
        {
            textboxes.ForEach(x => x.TextChanged -= CoordinateTextBox_OnTextChanged);
            RefreshCoordinates();
            textboxes.ForEach(x => x.TextChanged += CoordinateTextBox_OnTextChanged);

            SceneCanvas.Children.Clear();

            foreach (ClassLibrary.Geometry figure in _figures)
            {
                var lines = _camera.GeometryToRasterSpace(figure);
                foreach (int[] line in lines)
                {
                    DrawOnCanvas(line);
                }
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

            _camera.Update(_camera.PositionVector, newRotation, _camera.ScaleVector);
            UpdateScreen();

            _mousePressPoint = e.GetPosition(SceneCanvas);
        }
    }
}
