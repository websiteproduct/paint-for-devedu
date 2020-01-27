using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PaintTool.Creators;

namespace PaintTool.figures
{
    class Rect : Shape
    {
        private byte[] colorData;
        int size;
        private Shape line;

        public Rect(List<Point> points, byte[] colorData, int size) : base(points)
        {
            new LineCreator().CreateShape(points[0], points[1]);
            new LineCreator().CreateShape(points[1], points[2]);
            new LineCreator().CreateShape(points[2], points[3]);
            new LineCreator().CreateShape(points[3], points[0]);
        }
    }
}
