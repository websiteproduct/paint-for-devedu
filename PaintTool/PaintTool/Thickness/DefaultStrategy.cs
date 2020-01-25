using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Thickness
{
    public class DefaultStrategy : ThicknessS
    {
        public List<Point> GetPoints(Point point)
        {
            return new List<Point> { point };
        }
    }
}
