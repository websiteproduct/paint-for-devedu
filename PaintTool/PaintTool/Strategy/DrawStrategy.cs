using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.Thickness;
using PaintTool.Surface;


namespace PaintTool.Strategy
{
    public abstract class DrawStrategy
    {
        public SurfaceStrategy ss;
        public virtual void Draw(Point prev, Point position)
        {
        }
        
    }
}
