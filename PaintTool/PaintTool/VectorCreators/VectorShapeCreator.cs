using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PaintTool.Creators
{
    public interface VectorShapeCreator
    {
        public System.Windows.UIElement NewVectorShape (Point start, Point end);
    }
}
