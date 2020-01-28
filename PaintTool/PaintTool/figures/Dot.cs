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
            
            if ((points[0].X < 640 && points[0].X > 0) && (points[0].Y < 480 && points[0].Y > 0)) {
                
                Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(points[0].X),
                        Convert.ToInt32(points[0].Y),
                        1,
                        1);
                NewImage.Instance.WritePixels(rect, paintColor.GetColor(), 4, 0);
            }
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
