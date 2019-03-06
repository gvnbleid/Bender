using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Bender.ClassLibrary
{
    public class VisualHost : FrameworkElement
    {
        private VisualCollection _children;
        private readonly Pen _color;

        public VisualHost(Pen color)
        {
            _children = new VisualCollection(this);
            _color = color;
        }

        public void AddLine(int[] line)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            DrawingContext dc = drawingVisual.RenderOpen();

            dc.DrawLine(_color, new Point(line[0], line[1]),
                new Point(line[2], line[3]));

            dc.Close();

            _children.Add(drawingVisual);
        }

        public void AddLines(IEnumerable<int[]> lines)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            DrawingContext dc = drawingVisual.RenderOpen();

            foreach (int[] line in lines)
            {
                dc.DrawLine(_color, new Point(line[0], line[1]),
                    new Point(line[2], line[3]));
            }

            dc.Close();

            _children.Add(drawingVisual);
        }

        public void AddPoint(Point point, int stepX, int stepY, Color color)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            DrawingContext dc = drawingVisual.RenderOpen();

            dc.DrawRectangle(new SolidColorBrush(color), null, new Rect(point, new Point(point.X + stepX, point.Y + stepY)));

            dc.Close();

            _children.Add(drawingVisual);
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
