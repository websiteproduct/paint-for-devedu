using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Thickness
{
    class X2Strategy
    {
        public List<Point> GetPoints(Point point)
        {
            return new List<Point> {
                new Point(point.X, point.Y - 1),
                new Point(point.X + 1, point.Y - 1),

                new Point(point.X - 1, point.Y),
                 point,
                new Point(point.X + 1, point.Y),
                new Point(point.X + 2, point.Y),

                new Point(point.X - 1, point.Y + 1),
                new Point(point.X, point.Y + 1),
                new Point(point.X + 1, point.Y + 1),
                new Point(point.X + 2, point.Y + 1),

                new Point(point.X, point.Y + 2),
                new Point(point.X + 1, point.Y + 2)
            };
        }
    }
}
