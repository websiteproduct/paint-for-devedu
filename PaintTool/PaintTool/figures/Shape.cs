using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PaintTool.Strategy;

namespace PaintTool.figures
{
    abstract class Shape
    {
        List<Point> FigurePoints;
        DrawStrategy ds;
        public virtual void Draw(WriteableBitmap wb, Point prev, Point position, bool altBitmap) { }
    }
}
