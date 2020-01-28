using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Thickness
{
    class BigStrategy
    {
        public List<Point> GetPoints(Point point)
        {
            return new List<Point> {
                point,
                new Point(point.X, point.Y + 1),
                new Point(point.X, point.Y - 1),
                new Point(point.X + 1, point.Y),
                new Point(point.X - 1, point.Y)
            };
        }
    }
}
