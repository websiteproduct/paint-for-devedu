using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintTool.Creators
{
    public class VectorCircleCreator : VectorShapeCreator
    {
        public System.Windows.UIElement NewVectorShape(Point start, Point end)
        {
            Line line = new Line();

            line.Stroke = Brushes.Black;
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
            line.StrokeThickness = 4;
            start = end;

            return line;
        }
    }
}
