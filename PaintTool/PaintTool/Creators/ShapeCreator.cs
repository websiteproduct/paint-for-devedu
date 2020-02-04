
using PaintTool.figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool.Creators
{
    interface ShapeCreator
    {
        public Shape CreateShape(Point start, Point end);
    }
}
