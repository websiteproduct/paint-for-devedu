using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace PaintTool.Creators
{
    class VectorRectangleCreator : VectorShapeCreator
    {
        public System.Windows.UIElement NewVectorShape(Point start, Point end)
        {
            Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = System.Windows.Media.Brushes.Black;
            rect.Fill = System.Windows.Media.Brushes.SkyBlue;
            rect.HorizontalAlignment = HorizontalAlignment.Right;
            rect.VerticalAlignment = VerticalAlignment.Center;
            rect.Height = Math.Abs(end.Y - start.Y);
            rect.Width = Math.Abs(end.X - start.X);
            rect.Margin = new System.Windows.Thickness(start.X, start.Y, 0, 0);
            //rect.Margin.Left = (char)5;
            //rect.Margin = "5";

            return rect;

        }

    }
}
