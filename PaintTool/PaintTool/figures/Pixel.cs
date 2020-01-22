using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PaintTool.figures
{
    class Pixel
    {
        private byte[] colorData;
        private int size;
        bool EraserToggleBtnIsChecked;

        public Pixel(byte[] colorData, int size, bool EraserToggleBtnIsChecked)
        {
            this.colorData = colorData;
            this.size = size;
            this.EraserToggleBtnIsChecked = EraserToggleBtnIsChecked;
        }
        public void SetPixel(WriteableBitmap wb, Point pxl, bool altBitmap)
        {

            //if ((pxl.X < PaintField.Width && pxl.X > 0) && (pxl.Y < PaintField.Height && pxl.Y > 0))
            //{ (сейчас не знает PaintField)
                        Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(pxl.X),
                        Convert.ToInt32(pxl.Y),
                        1,
                        1);

                if (EraserToggleBtnIsChecked)
                {
                    wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
                }
                else if (altBitmap)
                {
                    //wbCopy.WritePixels(rect, GetColor(), 4, 0);
                }
                else
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
