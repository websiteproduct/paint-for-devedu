using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    public class LineCreator : ShapeCreator
    {
        public Shape CreateShape(Point start, Point end)
        {
            int wth = Convert.ToInt32(Math.Abs(end.X - start.X) + 1);
            int hght = Convert.ToInt32(Math.Abs(end.Y - start.Y) + 1);
            //if (ShapeList.SelectedItem == RectangleShape || ShapeList.SelectedItem == TriangleShape)
            //{
            //    wth--;
            //    hght--;
            //}

            int x0 = Convert.ToInt32(start.X), y0 = Convert.ToInt32(start.Y);
            List<Point> LineDots = new List<Point>();

            double k;

            if (hght >= wth)
            {
                k = wth * 1.0 / hght;

                if (end.X >= start.X && end.Y >= start.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(Convert.ToInt32(k * i + x0), y0 + i));
                    }
                }
                if (end.X <= start.X && end.Y >= start.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(-Convert.ToInt32(k * i - x0), y0 + i));
                    }
                }

                if (end.X >= start.X && end.Y <= start.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(Convert.ToInt32(k * i + x0), y0 - i));
                    }
                }

                if (end.X <= start.X && end.Y <= start.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(-Convert.ToInt32(k * i - x0), y0 - i));
                    }
                }

            }
            else
            {
                k = hght * 1.0 / wth;

                if (end.X >= start.X && end.Y <= start.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 + i, -Convert.ToInt32(k * i - y0)));
                    }
                }

                if (end.X <= start.X && end.Y <= start.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 - i, -Convert.ToInt32(k * i - y0)));
                    }
                }

                if (end.X >= start.X && end.Y >= start.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 + i, Convert.ToInt32(k * i + y0)));
                    }
                }

                if (end.X <= start.X && end.Y >= start.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 - i, Convert.ToInt32(k * i + y0)));
                    }
                }

            }

            return new Line(LineDots);
        }
    }
}
