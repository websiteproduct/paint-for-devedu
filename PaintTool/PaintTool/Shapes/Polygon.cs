using System.Collections.Generic;
using System.Drawing;
using PaintTool.Creators;

namespace PaintTool.figures
{
    class Polygon : Shape
    {
        public Polygon(List<Point> points, int numberOfSide) : base(points)
        { }
    }
}


 