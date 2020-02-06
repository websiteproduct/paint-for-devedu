using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PaintTool.Creators
{
    public class PolygonCreator : ShapeCreator
    {
        private int numberOfSide;

        public PolygonCreator(int numberOfSide)
        {
            this.numberOfSide = numberOfSide;
        }

        public Shape CreateShape(Point start, Point end)
        {
            List<Point> tempDots = new List<Point>();
            if (numberOfSide > 3)
            {
                Point CenterPolygon = start;
                double radius;
                if (Math.Abs(CenterPolygon.X - end.X) > Math.Abs(end.Y - end.Y))
                    radius = Math.Abs(CenterPolygon.X - end.X);
                else
                    radius = Math.Abs(CenterPolygon.Y - end.Y);

                if (end.Y == 0) end.Y = 1;
                double z = Math.Atan(end.X / end.Y) * 180 / Math.PI;
                int i = 0;
                double angle = 360 / numberOfSide;

                while (i < numberOfSide)
                {
                    tempDots.Add(new Point(CenterPolygon.X + (int)(Math.Cos(z / 180 * Math.PI) * radius),
                                     CenterPolygon.Y - (int)(Math.Sin(z / 180 * Math.PI) * radius)));
                    z -= angle;
                    i++;
                }
            }
            return new Polygon(tempDots, numberOfSide);

        }
    }
}