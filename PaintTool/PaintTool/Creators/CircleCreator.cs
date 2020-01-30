using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    public class CircleCreator : ShapeCreator
    {
        private bool isShiftPressed;

        public CircleCreator(bool isShiftPressed)
        {
            this.isShiftPressed = isShiftPressed;
        }

        public Shape CreateShape(Point start, Point end)
        {
            List<Point> tempDots = new List<Point>();
            double y = Math.Abs(start.Y - end.Y);
            double x = 0;
            double delta = 1 - 2 * y;
            double coeff;


            
            if (start.Y - end.Y != 0)
            {
                coeff = Math.Abs((start.X - end.X) / (start.Y - end.Y));
            }
            else
            {
                 coeff = Math.Abs(start.X - end.X);
            }


            if (isShiftPressed)
            {
                coeff = 1;
            }

            while (y >= 0)
            {
                tempDots.Add(new Point(Convert.ToInt32(start.X + coeff * x), start.Y + (int)y));
                tempDots.Add(new Point(Convert.ToInt32(start.X + coeff * x), start.Y - (int)y));
                tempDots.Add(new Point(Convert.ToInt32(start.X - coeff * x), start.Y + (int)y));
                tempDots.Add(new Point(Convert.ToInt32(start.X - coeff * x), start.Y - (int)y));

                if ((delta < 0))
                {
                    x++;
                    delta = delta + 2 * x + 1;
                    continue;
                }
                if ((delta > 0))
                {
                    y--;
                    delta = delta - 2 * y + 1;
                    continue;
                }
                x++;
                delta = delta + 2 * (x - y);
                y--;
            }

            return new Circle(tempDots);
        }
    }
}
