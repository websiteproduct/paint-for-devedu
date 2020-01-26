using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PaintTool.figures
{
    class Line : Shape
    {
        private byte[] colorData;
        private int size;

        public Line(byte[] colorData, int size, List<Point> points : base(points))
        {
            this.colorData = colorData;
            this.size = size;
        }

        public override void Draw(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            DrawLine(wb, prev, position, altBitmap);
        }

        private void DrawLine(Point prev, Point position, bool altBitmap = false)
        {
            int wth = Convert.ToInt32(Math.Abs(position.X - prev.X) + 1);
            int hght = Convert.ToInt32(Math.Abs(position.Y - prev.Y) + 1);
            if (ShapeList.SelectedItem == RectangleShape || ShapeList.SelectedItem == TriangleShape)
            {
                wth--;
                hght--;
            }

            int x0 = Convert.ToInt32(prev.X), y0 = Convert.ToInt32(prev.Y);
            List<Point> LineDots = new List<Point>();

            double k;

            if (hght >= wth)
            {
                k = wth * 1.0 / hght;

                if (position.X >= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(Convert.ToInt32(k * i + x0), y0 + i));
                    }
                }
                if (position.X <= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(-Convert.ToInt32(k * i - x0), y0 + i));
                    }
                }

                if (position.X >= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(Convert.ToInt32(k * i + x0), y0 - i));
                    }
                }

                if (position.X <= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(-Convert.ToInt32(k * i - x0), y0 - i));
                    }
                }

                for (int i = 0; i < hght; i++)
                {
                    SetPixel(LineDots[i], altBitmap);
                }
            }
            else
            {
                k = hght * 1.0 / wth;

                if (position.X >= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 + i, -Convert.ToInt32(k * i - y0)));
                    }
                }

                if (position.X <= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 - i, -Convert.ToInt32(k * i - y0)));
                    }
                }

                if (position.X >= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 + i, Convert.ToInt32(k * i + y0)));
                    }
                }

                if (position.X <= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 - i, Convert.ToInt32(k * i + y0)));
                    }
                }

                for (int i = 0; i < wth; i++)
                {
                    SetPixel(LineDots[i], altBitmap);
                }
            }

        }


        public void SetPixel(Point pxl, bool altBitmap)
        {

            if ((pxl.X < PaintField.Width && pxl.X > 0) && (pxl.Y < PaintField.Height && pxl.Y > 0))
            {
                Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(pxl.X),
                        Convert.ToInt32(pxl.Y),
                        1,
                        1);

                if ((bool)EraserToggleBtn.IsChecked)
                {
                    wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
                }
                else if (altBitmap)
                    wbCopy.WritePixels(rect, GetColor(), 4, 0);
                else
                    wb.WritePixels(rect, GetColor(), 4, 0);
            }
        }
        private byte[] GetColor()
        {
            return colorData;
        }
        
    }
}
