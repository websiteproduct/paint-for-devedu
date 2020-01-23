using System;
using System.Collections.Generic;
using System.Text;

namespace PaintTool.Strategy
{
    abstract class DrawStrategy
    {
        Color currentColor;

        public virtual void DrawLine()
        {

        }

    }
}
