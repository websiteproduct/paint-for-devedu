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
    class Triangle : Shape
    {
        private byte[] colorData;
        int size;
        private Shape line;

        public Triangle(byte[] colorData, int size)
        {
            this.size = size;
            this.colorData = colorData;
            line = new Line(colorData, size);
        }

        public override void Draw(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            DrawTriangle(wb, prev, position, altBitmap);

        }

        private void DrawTriangle(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            line.Draw(wb, prev, position, altBitmap);
            line.Draw(wb, position, new Point(2 * prev.X - position.X, position.Y), altBitmap);
            line.Draw(wb, new Point(2 * prev.X - position.X, position.Y), prev, altBitmap);
        }
    }
}
