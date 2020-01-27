using System.Collections.Generic;
using System.Drawing;
using PaintTool.Creators;

namespace PaintTool.figures
{
    class Triangle : Shape
    {
        //private byte[] colorData;
        //int size;
        //private Shape line;

        //public Triangle(byte[] colorData, int size)
        //{
        //    this.size = size;
        //    this.colorData = colorData;
        //    line = new Line(colorData, size);
        //}


        public Triangle(List<Point> points) : base(points)
        {
            new LineCreator().CreateShape(points[0], points[1]);
            new LineCreator().CreateShape(points[1], points[2]);
            new LineCreator().CreateShape(points[2], points[0]);
        }
    }
}
