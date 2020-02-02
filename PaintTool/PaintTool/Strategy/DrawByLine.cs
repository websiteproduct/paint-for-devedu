using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.Thickness;
using PaintTool.Creators;
using PaintTool.figures;
using System.Diagnostics;

namespace PaintTool.Strategy
{
    public class DrawByLine : DrawStrategy
    {
        public DrawByLine()
        {
            currentColor = PaintColor.ColorData;
            thicknessStrategy = new DefaultStrategy();
        }
        public override void Draw(Point start, Point end)
        {
            List<Point> temp = thicknessStrategy.GetPoints(start);
            List<Point> temp2 = thicknessStrategy.GetPoints(end);

            for (int i = 0; i < temp.Count; i++)
            {
                DrawLine(temp[i], temp2[i]);
            }
        }


        public void DrawLine(Point start, Point end)
            {
            int wth = Convert.ToInt32(Math.Abs(end.X - start.X));
            int hght = Convert.ToInt32(Math.Abs(end.Y - start.Y));

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
            for (int i = 0; i < LineDots.Count; i++)
            {
                new DotCreator().CreateShape(LineDots[i], LineDots[i]);
            }

        }

    }
 
}
