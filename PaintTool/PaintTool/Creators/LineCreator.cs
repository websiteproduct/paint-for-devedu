using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    public class LineCreator : ShapeCreator
    {
        public Shape CreateShape(Point start, Point end)
        {
            List<Point> LineDots = new List<Point> { start, end };

            return new Line(LineDots);
        }
    }
}
