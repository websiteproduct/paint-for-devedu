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

namespace PaintTool.figures
{
    class Rect : Shape
    {
        private byte[] colorData;
        int size;
        private Shape line;
        bool isShiftPressed;

        public Rect(List<Point> points : base(points), byte[] colorData, int size, bool isShiftPressed)
        {
            DrawLine(points[0], points[1], true);
            DrawLine(points[1], points[2], true);
            DrawLine(points[2], points[3], true);
            DrawLine(points[3], points[0], true);
        }
    }
}
