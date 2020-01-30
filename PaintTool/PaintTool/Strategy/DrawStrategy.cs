using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.Thickness;


namespace PaintTool.Strategy
{
    public abstract class DrawStrategy
    {
        public byte[] currentColor;
        public static ThicknessS thicknessStrategy { get; set; }

        public virtual void Draw(Point prev, Point position)
        {

        }
        
    }
}
