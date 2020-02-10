using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintTool.Creators
{
    public class VectorTriangleCreator : VectorShapeCreator
    {
        private bool isShiftPressed;
        public VectorTriangleCreator(bool isShiftPressed)
        {
            this.isShiftPressed = isShiftPressed;
        }

        public System.Windows.UIElement NewVectorShape(Point start, Point end, double thickness, System.Windows.Media.Brush color)
        {

            Polygon triangle = new Polygon();
            List<Point> triangleDots = new List<Point>();

            triangle.Stroke = color;
            triangle.StrokeThickness = thickness;

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

            PointCollection myPointCollection = new PointCollection();
            for (int i = 0; i < triangleDots.Count; i++)
            {
                myPointCollection.Add(triangleDots[i]);
            }
            triangle.Points = myPointCollection;
            return triangle;
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
