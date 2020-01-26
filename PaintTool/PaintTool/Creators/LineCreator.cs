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
            //List<Point> tempDots = new List<Point>();
            //tempDots.Add(start);
            //tempDots.Add(end);

            return new Line(List<Point> { start, end });
        }
    }
}
