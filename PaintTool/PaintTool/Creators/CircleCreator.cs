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
            int countOfDots = 0;



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
            countOfDots = tempDots.Count;

            for (int i = 0; i < countOfDots; i++)
            {
                for (int j = 2; j > 5; j++)
                {
                    if ((i+2)%j==0)
                    {
                        tempDots.Add(tempDots[i]);
                        tempDots.Remove(tempDots[i]);
                    }
                  
                } 

            }


            //int radius = (int)Math.Sqrt((Math.Pow((end.X - start.X), 2)) + Math.Sqrt(Math.Pow((end.Y - start.Y), 2)));
            //double a = Math.Sqrt(2) / 2;

            //for (int i = 0; i <= (int)(a*radius); i++)
            //{
            //    int newY1 = (int)(start.Y - Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(start.X + i, newY1));
            //}

            //for (int i = (int)(a * radius); i > 0; i--)
            //{
            //    int newX2 = (int)(start.X + Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(newX2, start.Y - i));
            //}

            //for (int i = 0; i <= (int)(a * radius); i++)
            //{
            //    int newX2 = (int)(start.X + Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(newX2, start.Y + i));
            //}

            //for (int i = (int)(a * radius); i > 0; i--)
            //{
            //    int newY2 = (int)(start.Y + Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(start.X + i, newY2));
            //}

            //for (int i = 0; i < (int)(a * radius); i++)
            //{
            //    int newY2 = (int)(start.Y + Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(start.X - i, newY2));
            //}

            //for (int i = (int)(a * radius); i > 0; i--)
            //{
            //    int newX1 = (int)(start.X + Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(newX1, start.Y + i));
            //}

            //for (int i = 0; i < (int)(a * radius); i++)
            //{
            //    int newX1 = (int)(start.X + Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(newX1, start.Y - i));
            //}

            //for (int i = (int)(a * radius); i > 0; i--)
            //{
            //    int newY1 = (int)(start.Y - Math.Sqrt(radius * radius - i * i));
            //    tempDots.Add(new Point(start.X - i, newY1));
            //}




            return new Circle(tempDots);
        }
    }
}
