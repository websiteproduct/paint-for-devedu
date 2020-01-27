using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    public class CircleCreator : ShapeCreator
    {
        private double coeff;

        public CircleCreator(double coeff)
        {
            this.coeff = coeff;
        }

        public Shape CreateShape(Point start, Point end)
        {
            List<Point> tempDots = new List<Point>();
            int y = Math.Abs(start.Y - end.Y);
            int x = 0;
            int delta = 1 - 2 * y;

            while (y >= 0)
            {
                tempDots.Add(new Point(Convert.ToInt32(start.X + coeff * x), start.Y + y));
                tempDots.Add(new Point(Convert.ToInt32(start.X + coeff * x), start.Y - y));
                tempDots.Add(new Point(Convert.ToInt32(start.X - coeff * x), start.Y + y));
                tempDots.Add(new Point(Convert.ToInt32(start.X - coeff * x), start.Y - y));
                
                if ((delta < 0))
                {
                    x++;
                    delta = delta + 2 * x + 1;
                    continue;
                }
                if ((delta > 0))
                {
                    y++;
                    delta =delta - 2 * y + 1;
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
