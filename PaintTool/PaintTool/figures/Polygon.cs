using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.figures
{
    class Polygon : Shape
    {
        public Polygon(List<Point> points) : base(points)
        {
            for (int j = 0; j < numberOfSide; j++)
            {
                if (j < numberOfSide - 1)
                    DrawLine(polygonDots[j], polygonDots[j + 1], true);
                else
                    DrawLine(polygonDots[j], polygonDots[0], true);
            }
        }
    }
}


 