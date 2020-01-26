using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.figures
{
    public class Circle : Shape
    {
        public Circle(List<Point> points) : base(points)
        {
            for (int i = 0; i < points.Count - 4; i += 1)
            {
                DrawLine(points[i], points[i + 4], true);
            }
        }
    }
}
