using System.Collections.Generic;
using System.Drawing;
using PaintTool.Creators;

namespace PaintTool.figures
{
    class Polygon : Shape
    {
        public Polygon(List<Point> points, int numberOfSide) : base(points)
        {
            for (int i = 0; i < numberOfSide; i++)
            {
                if (i < numberOfSide - 1)
                    new LineCreator().CreateShape(points[i], points[i + 1]);
                else
                    new LineCreator().CreateShape(points[i], points[0]);
            }
        }
    }
}


 