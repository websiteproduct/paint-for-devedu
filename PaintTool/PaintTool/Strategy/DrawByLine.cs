using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace PaintTool.Strategy
{
    public class DrawByLine : DrawStrategy
    {
        public override void DrawLine(Point start, Point end)
        {
            List<Point> temp = thicknessStrategy.GetPoints(start);
            List<Point> temp2 = thicknessStrategy.GetPoints(end);
        }
    }
}
