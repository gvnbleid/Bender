using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MathNet.Numerics.LinearAlgebra;

namespace Bender.ClassLibrary.CameraObjects
{
    public interface IProjection
    {
        float ScreenWidth { get; }
        float ScreenHeight { get; }
        IEnumerable<LineGeometry> Project(IEnumerable<Vector<float>> vertices, IEnumerable<Edge> edges);
    }
}