using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.Thickness;


namespace PaintTool.Strategy
{
    public class DrawByLine : DrawStrategy
    {
        public DrawByLine()
        {
            currentColor = new PaintColor();
            thicknessStrategy = new DefaultStrategy();
        }
        public override void DrawLine(Point start, Point end)
        {
            List<Point> temp = thicknessStrategy.GetPoints(start);
            List<Point> temp2 = thicknessStrategy.GetPoints(end);

        }
    }
}
