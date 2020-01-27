using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PaintTool.figures
{
    class Dot : Shape
    {
        
        public Dot(List<System.Drawing.Point> points) : base(points)
        {
            PaintColor paintColor = new PaintColor();
            WriteableBitmap wb = NewImage.Instance;

                Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(points[0].X),
                        Convert.ToInt32(points[0].Y),
                        1,
                        1);
                wb.WritePixels(rect, paintColor.GetColor(), 4, 0);
            }

        //if ((bool) EraserToggleBtn.IsChecked)
        //{
        //    wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
        //}
        //else if (altBitmap)
        //    wbCopy.WritePixels(rect, GetColor(), 4, 0);
        //else

    }
}
