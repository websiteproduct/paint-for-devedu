using System.Collections.Generic;
using System.Drawing;
using PaintTool.Creators;

namespace PaintTool.figures
{
    class Line : Shape
    {
        private byte[] colorData;
        private int size;

        public Line(List<Point> points) : base(points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                new DotCreator().CreateShape(points[i], points[i]);
            }
        }

        //public Line(byte[] colorData, int size, List<Point> points : base(points))
        //{
        //    this.colorData = colorData;
        //    this.size = size;
        //}

           


        //public void SetPixel(Point pxl, bool altBitmap)
        //{

        //    if ((pxl.X < PaintField.Width && pxl.X > 0) && (pxl.Y < PaintField.Height && pxl.Y > 0))
        //    {
        //        Int32Rect rect = new Int32Rect(
        //                Convert.ToInt32(pxl.X),
        //                Convert.ToInt32(pxl.Y),
        //                1,
        //                1);

        //        if ((bool)EraserToggleBtn.IsChecked)
        //        {
        //            wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
        //        }
        //        else if (altBitmap)
        //            wbCopy.WritePixels(rect, GetColor(), 4, 0);
        //        else
        //            wb.WritePixels(rect, GetColor(), 4, 0);
        //    }
        //}
        private byte[] GetColor()
        {
            return colorData;
        }
        
    }
}
