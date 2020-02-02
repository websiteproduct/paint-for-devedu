using PaintTool.Creators;
using PaintTool.Thickness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Surface
{
    class DrawOnBitmap : SurfaceStrategy
    {
        public override void DrawLine(Point start, Point end)
        {
            DotCreation dot = new DotCreation();
            dot.DotDrawing(start);
        }
    }
}
