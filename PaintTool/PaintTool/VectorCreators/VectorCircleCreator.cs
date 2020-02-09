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
            Ellipse elipse = new Ellipse();

            elipse.Stroke = System.Windows.Media.Brushes.Black;
            elipse.StrokeThickness = 3;
            elipse.HorizontalAlignment = HorizontalAlignment.Left;
            elipse.VerticalAlignment = VerticalAlignment.Center;
            elipse.Margin = new System.Windows.Thickness(start.X, start.Y, 0, 0);
            elipse.Height = Math.Abs(end.Y - start.Y);
            elipse.Width = Math.Abs(end.X - start.X);

            return elipse;
        }
    }
}
