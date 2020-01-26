using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PaintTool.Creators
{
    class PolygonCreator
    {
        public Shape CreateShape(Point start, Point end)
        {
            List<Point> tempDots = new List<Point>();
            if (numberOfSide > 3)
            {
                CenterPolygon = start;
                double radius;
                if (Math.Abs(CenterPolygon.X - end.X) > Math.Abs(end.Y - end.Y))
                    radius = Math.Abs(CenterPolygon.X - end.X);
                else
                    radius = Math.Abs(CenterPolygon.Y - end.Y);


                double z = Math.Atan(end.X / end.Y) * 180 / Math.PI;
                int i = 0;
                double angle = 360 / numberOfSide;

                while (i < numberOfSide)
                {
                    tempDots.Add(new Point(CenterPolygon.X + (Math.Cos(z / 180 * Math.PI) * radius),
                                     CenterPolygon.Y - (Math.Sin(z / 180 * Math.PI) * radius)));
                    z -= angle;
                    i++;
                }


                return new Line(tempDots);
        }
    }
}
