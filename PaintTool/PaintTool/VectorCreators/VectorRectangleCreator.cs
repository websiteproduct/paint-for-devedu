using System;
using System.Windows;
using System.Windows.Shapes;

namespace PaintTool.Creators
{
    class VectorRectangleCreator : VectorShapeCreator
    {
        public System.Windows.UIElement NewVectorShape(Point start, Point end, double thickness, System.Windows.Media.Brush color)
        {
            Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = color;
            rect.StrokeThickness = thickness;
            rect.HorizontalAlignment = HorizontalAlignment.Right;
            rect.VerticalAlignment = VerticalAlignment.Center;
            rect.Height = Math.Abs(end.Y - start.Y);
            rect.Width = Math.Abs(end.X - start.X);
            rect.Margin = new System.Windows.Thickness(start.X, start.Y, 0, 0);

            return rect;

        }

    }
}
