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

        public Line(byte[] colorData, int size)
        {
            this.colorData = colorData;
            this.size = size;
        }

        public override void Draw(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            DrawLine(wb, prev, position, altBitmap);
        }

        private void DrawLine(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            int wth = Convert.ToInt32(Math.Abs(position.X - prev.X) + 1);
            int hght = Convert.ToInt32(Math.Abs(position.Y - prev.Y) + 1);
            //if (ShapeList.SelectedItem == RectangleShape || ShapeList.SelectedItem == TriangleShape)
            //{
            //    wth--;
            //    hght--;
            //}

            int x0 = Convert.ToInt32(prev.X);
            int y0 = Convert.ToInt32(prev.Y);
            int x;
            int y;
            int[] xArr;
            int[] yArr;
            double k;
            int quarter = FindQuarter(prev, position);

            if (hght >= wth)
            {
                xArr = new int[hght];
                yArr = new int[hght];
                k = wth * 1.0 / hght;

                if (quarter == 4)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i + x0);
                        xArr[i] = x;
                        yArr[i] = y0 + i;
                    }
                }
                if (quarter == 3)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i - x0);
                        xArr[i] = -x >= 0 ? -x : 0;
                        yArr[i] = y0 + i;
                    }
                }

                if (quarter == 1)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i + x0);
                        xArr[i] = x;
                        yArr[i] = y0 - i;
                    }
                }

                if (quarter == 2)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i - x0);
                        xArr[i] = -x;
                        yArr[i] = y0 - i;
                    }
                }

                for (int i = 0; i < hght; i++)
                {
                    prev.Y = yArr[i];
                    prev.X = xArr[i];
                    SetPixel(wb, prev, altBitmap);
                }
            }
            else if (hght < wth)
            {
                xArr = new int[wth];
                yArr = new int[wth];
                k = hght * 1.0 / wth;

                if (quarter == 1)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i - y0);
                        yArr[i] = -y;
                        xArr[i] = x0 + i;
                    }
                }

                if (quarter == 2)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i - y0);
                        yArr[i] = -y;
                        xArr[i] = x0 - i;
                    }
                }

                if (quarter == 4)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i + y0);
                        yArr[i] = y;
                        xArr[i] = x0 + i;
                    }
                }

                if (quarter == 3)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i + y0);
                        yArr[i] = y;
                        xArr[i] = x0 - i;
                    }
                }

                for (int i = 0; i < wth; i++)
                {
                    prev.Y = yArr[i];
                    prev.X = xArr[i];
                    SetPixel(wb, prev, altBitmap);
                }
            }
        }

        private int FindQuarter(Point prev, Point position)
        {
            int quarter = 0;
            if (position.X >= prev.X && position.Y >= prev.Y)
            {
                quarter = 4;
            }
            if (position.X <= prev.X && position.Y <= prev.Y)
            {
                quarter = 2;
            }
            if (position.X >= prev.X && position.Y <= prev.Y)
            {
                quarter = 1;
            }
            if (position.X <= prev.X && position.Y >= prev.Y)
            {
                quarter = 3;
            }
            return quarter;
        }

        private void SetPixel(WriteableBitmap wb, Point pxl, bool altBitmap)
        {

            //if ((pxl.X < PaintField.Width && pxl.X > 0) && (pxl.Y < PaintField.Height && pxl.Y > 0))
            //{ (сейчас не знает PaintField)
            Int32Rect rect = new Int32Rect(
            Convert.ToInt32(pxl.X),
            Convert.ToInt32(pxl.Y),
            1,
            1);

            ////if (EraserToggleBtnIsChecked)
            ////{
            //    wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
            ////}
            ////else if (altBitmap)
            ////{
            //    //wbCopy.WritePixels(rect, GetColor(), 4, 0);
            //}
            //else
                wb.WritePixels(rect, GetColor(), 4, 0);
            //}
        }
        private byte[] GetColor()
        {
            // Метод для возвращения значения цвета в Bgra32
            return colorData;
        }
    }
}
