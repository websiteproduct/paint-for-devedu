using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;


namespace PaintTool.Strategy
{
    abstract class DrawStrategy
    {
        Color currentColor;

        public virtual void DrawLine(Point prev, Point position)
        {

        }

    }
}
