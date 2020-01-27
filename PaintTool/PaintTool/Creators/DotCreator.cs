using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    public class DotCreator : ShapeCreator
    {
        public Shape CreateShape(Point dot, Point tempDot)
        {
            List<Point> dots = new List<Point> { dot };
            return new Dot(dots);
        }
    }
}
