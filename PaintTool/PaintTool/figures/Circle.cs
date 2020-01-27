using System.Collections.Generic;
using System.Drawing;
using PaintTool.Creators;

namespace PaintTool.figures
{
    public class Circle : Shape
    {
        public Circle(List<Point> points) : base(points)
        {
            for (int i = 0; i < points.Count - 4; i++)
            {
                new LineCreator().CreateShape(points[i], points[i + 4]);
            }
        }
    }
}
