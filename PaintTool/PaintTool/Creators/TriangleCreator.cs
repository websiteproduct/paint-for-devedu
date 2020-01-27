using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    public class TriangleCreator : ShapeCreator
    {
        private bool isShiftPressed;
        public TriangleCreator(bool isShiftPressed)
        {
            this.isShiftPressed = isShiftPressed;
        }

        public Shape CreateShape(Point start, Point end)
        {
        List<Point> triangleDots = new List<Point>();

            double rad = Math.Abs((start.Y - end.Y)) / 2;
            if (isShiftPressed)
            {
                double temp = Math.Abs(end.X - start.X);
                double offsetY = Math.Sqrt(3) / 2 * temp;
                if (end.X > start.X)
                {
                    if (start.Y > end.Y)
                        triangleDots = DrawingEquilateralTriangle(temp / 2, offsetY, start, end);
                    else
                        triangleDots = DrawingEquilateralTriangle(temp / 2, -offsetY, start, end);
                 }
                else
                {
                    if (start.Y > end.Y)
                        triangleDots = DrawingEquilateralTriangle(-temp / 2, offsetY, start, end);
                    else
                        triangleDots = DrawingEquilateralTriangle(-temp / 2, -offsetY, start, end);
                }

            }
            else
            {
                triangleDots.Add(start);
                triangleDots.Add(new Point(start.X + (end.X - start.X) / 2, end.Y));
                triangleDots.Add(new Point(end.X, start.Y));
            }
            return new Triangle(triangleDots);
        }
        private List<Point> DrawingEquilateralTriangle(double offsetX, double offsetY, Point start, Point end)
        {
            List<Point> tempDots = new List<Point>();
            tempDots.Add(start);
            tempDots.Add(new Point(start.X + (int)offsetX, start.Y - (int)offsetY));
            tempDots.Add(new Point(end.X, start.Y));

            return tempDots;
        }
    }
}
