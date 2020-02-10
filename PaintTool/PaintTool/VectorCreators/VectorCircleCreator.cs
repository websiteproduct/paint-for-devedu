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
        public System.Windows.UIElement NewVectorShape(Point start, Point end, double thickness, System.Windows.Media.Brush color)
        {
            Ellipse elipse = new Ellipse();
            double marginX, marginY;

            elipse.Stroke = color;
            elipse.StrokeThickness = thickness;
            //elipse.HorizontalAlignment = HorizontalAlignment.Left;
            //elipse.VerticalAlignment = VerticalAlignment.Center;
            elipse.Height = Math.Abs(end.Y - start.Y);
            elipse.Width = Math.Abs(end.X - start.X);
  
            elipse.Margin = new System.Windows.Thickness(start.X, start.Y, 0, 0);

            return elipse;
        }
    }
}
