using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;

namespace PaintTool
{
    public class DotCreation
    {
        public void DotDrawing(System.Drawing.Point start)
        {
            if ((start.X < 640 && start.X >= 0) && (start.Y < 480 && start.Y >= 0))
            {
                byte[] colorData = PaintColor.ColorData;
                Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(start.X),
                        Convert.ToInt32(start.Y),
                        1,
                        1);
                NewImage.Instance.WritePixels(rect, colorData, 4, 0);
            }
        }

    }
}
