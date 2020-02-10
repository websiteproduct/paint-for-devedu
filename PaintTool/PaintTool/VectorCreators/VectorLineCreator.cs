using System.Windows;
using System.Windows.Shapes;

namespace PaintTool.Creators
{
    public class VectorLineCreator : VectorShapeCreator
    {
        public System.Windows.UIElement NewVectorShape(Point start, Point end, double thickness, System.Windows.Media.Brush color)
        {
            Line line = new Line();

            line.Stroke = color;
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
            line.StrokeThickness = thickness;
            start = end;

            return line;
        }
    }
}
