using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintTool.Creators
{
    public class VectorPolygonCreator : VectorShapeCreator
    {

        private int numberOfSide;

        public VectorPolygonCreator(int numberOfSide)
        {
            this.numberOfSide = numberOfSide;
        }

        public System.Windows.UIElement NewVectorShape(Point start, Point end, double thickness, System.Windows.Media.Brush color)
        {
            Polygon polygon = new Polygon();

            polygon.Stroke = color;
            polygon.StrokeThickness = thickness;

            PointCollection tempDots = new PointCollection();

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
            polygon.Points = tempDots;
            return polygon;
        }
    }
}
