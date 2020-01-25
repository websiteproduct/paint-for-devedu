using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace PaintTool.Strategy
{
    public class DrawByLine : DrawStrategy
    {
        public override void DrawLine(Point prev, Point position)
        {
            List<Point> temp = thicknessStrategy.GetPoints(prev);
            List<Point> temp2 = thicknessStrategy.GetPoints(position);
        }
    }
}
