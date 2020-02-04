using PaintTool.Thickness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Surface
{
    public abstract class SurfaceStrategy
    {
        public byte[] currentColor;
        public static ThicknessS thicknessStrategy { get; set; }

        public virtual void DrawLine(Point start, Point end)
        {
        }

    }
}
