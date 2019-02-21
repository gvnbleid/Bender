using System;
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

namespace Bender.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static WriteableBitmap writeableBitmap;
        public MainWindow()
        {
            InitializeComponent();

            writeableBitmap = new WriteableBitmap(
                (int)SceneImage.ActualWidth,
                (int)SceneImage.ActualHeight,
                96,
                96,
                PixelFormats.Bgra32,
                null);

            SceneImage.Source = writeableBitmap;

            SceneImage.Stretch = Stretch.None;
            SceneImage.HorizontalAlignment = HorizontalAlignment.Left;
            SceneImage.VerticalAlignment = VerticalAlignment.Top;

            Torus torus = new Torus(10, 2, 0.5f, 0.5f);
            Camera camera = new Camera(
                new DenseVector(new[] {0f, 0f, -20f}),
                new DenseVector(new[] {0f, 0f, 0f}),
                0.1f,
                30f,
                (float) MathNet.Numerics.Trig.DegreeToRadian(120));

            camera.Draw(torus, writeableBitmap);
        }
    }
}
