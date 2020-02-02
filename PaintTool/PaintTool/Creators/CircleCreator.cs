using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
//using System.Windows;
//using System.Windows.Threading;

namespace PaintTool.Creators
{
    public class CircleCreator : ShapeCreator
    {
        private bool isShiftPressed;

        public CircleCreator(bool isShiftPressed)
        {
            this.isShiftPressed = isShiftPressed;
        }

        public Shape CreateShape(System.Drawing.Point start, System.Drawing.Point end)
        {
            List<System.Drawing.Point> tempDots = new List<System.Drawing.Point>();
            double y = Math.Abs(start.Y - end.Y);
            double x = 0;
            double delta = 1 - 2 * y;
            double coeff;
            int countOfDots = 0;



            if (start.Y - end.Y != 0)
            {
                coeff = Math.Abs((start.X - end.X) / ((double)start.Y - end.Y));
            }
            else
            {
                coeff = Math.Abs((double)start.X - end.X);
            }


            if (isShiftPressed)
            {
                coeff = 1;
            }

            while (y >= 0)
            {
                double tempX = start.X + coeff * x;
                tempDots.Add(new Point(Convert.ToInt32(tempX), start.Y + (int)y));

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
            countOfDots = tempDots.Count;

            for (int i = countOfDots-1; i > 0; i--) 
            {
                tempDots.Add(new Point(tempDots[i].X, 2 * start.Y - tempDots[i].Y));
            }
            for (int i = 0; i < countOfDots; i++) 
            {
                tempDots.Add(new Point(2 * start.X - tempDots[i].X, 2 * start.Y - tempDots[i].Y));
            }
            for (int i = countOfDots - 1; i > 0; i--) 
            {
                tempDots.Add(new Point(2 * start.X - tempDots[i].X, tempDots[i].Y));
            }

            return new Circle(tempDots);
        }
    }
}
