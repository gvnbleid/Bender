using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bender.ClassLibrary;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Vector = System.Windows.Vector;

namespace Bender.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Torus torus = new Torus(2f, 0.5f, 0.5f, 0.5f);
            Cube cube = new Cube(1f);
            Serializer.WriteVerticesToFile("torusModel.txt", cube.Geometry.Vertices);

            Camera camera = new Camera(
                new DenseVector(new[] {0f, 0f, 1f, 0f}),
                new DenseVector(new[] {0f, 0f, 0f, 0f}),
                0.1f,
                5f,
                (float) MathNet.Numerics.Trig.DegreeToRadian(120),
                (float) SceneCanvas.ActualWidth,
                (float) SceneCanvas.ActualHeight);

            camera.GeometryToRasterSpace(cube, out Point[] verticesToDraw);

            DrawOnCanvas(verticesToDraw, cube.Geometry.Edges);
        }

        private void DrawOnCanvas(Point[] vertices, Edge[] topology)
        {
            foreach (Edge edge in topology)
            {
                Line line = new Line();

                line.X1 = vertices[edge.Beginning].X;
                line.Y1 = vertices[edge.Beginning].Y;

                line.X2 = vertices[edge.End].X;
                line.Y2 = vertices[edge.End].Y;

                line.Stroke = Brushes.Beige;
                line.StrokeThickness = 0.1;

                SceneCanvas.Children.Add(line);
            }
        }
    }
}
