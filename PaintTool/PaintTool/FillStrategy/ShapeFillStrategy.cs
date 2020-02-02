using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.FillStrategy
{
    public abstract class ShapeFillStrategy
    {
        public byte[] fillColor;
        public virtual void ShapeFill(List<Point> points)
        {
        }

    }
}
