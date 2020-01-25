using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.Thickness;


namespace PaintTool.Strategy
{
    public abstract class DrawStrategy
    {
        public Color currentColor;
        public ThicknessS thicknessStrategy;

        public virtual void DrawLine(Point prev, Point position)
        {
        }

    }
}
