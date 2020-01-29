using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.Thickness;
using PaintTool.Creators;


namespace PaintTool.Strategy
{
    public class DrawByLine : DrawStrategy
    {
        public DrawByLine()
        {
            currentColor = new PaintColor();
            thicknessStrategy = new VeryBigStrategy();
        }
        public override void DrawLine(Point start, Point end)
        {
            List<Point> temp = thicknessStrategy.GetPoints(start);
            List<Point> temp2 = thicknessStrategy.GetPoints(end);

            //for (int i = 0; i < temp.Count; i++)
            //{
            //    new LineCreator().CreateShape(temp[i], temp2[i]);
            //}

        }
    }
}
