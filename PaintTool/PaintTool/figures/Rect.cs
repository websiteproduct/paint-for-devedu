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

        public Rect(byte[] colorData, int size, bool isShiftPressed)
        {
            this.size = size;
            this.colorData = colorData;
            this.isShiftPressed = isShiftPressed;
            line = new Line(colorData, size);
        }

        public override void Draw(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            DrawRectangle(wb, prev, position, altBitmap);

        }

        private void DrawRectangle(WriteableBitmap wb, Point prev, Point position, bool altBitmap)
        {
            if (isShiftPressed)
            {

                double length = position.X - prev.X;
                if (position.X > prev.X)
                {
                    if (position.Y > prev.Y) DrawingSquare(wb, prev, length, length);
                    else DrawingSquare(wb, prev, length, -length);
                }
                else
                {
                    if (position.Y > prev.Y) DrawingSquare(wb, prev, length, -length);
                    else DrawingSquare(wb, prev, length, length);
                }

            }
            else
            {
                line.Draw(wb, prev, new Point(position.X, prev.Y), true);
                line.Draw(wb, new Point(position.X, prev.Y), position, true);
                line.Draw(wb, position, new Point(prev.X, position.Y), true);
                line.Draw(wb, new Point(prev.X, position.Y), prev, true);
            }
        }
        private void DrawingSquare(WriteableBitmap wb, Point prev, double lengthX, double lengthY)
        {
            line.Draw(wb, prev, new Point(prev.X + lengthX, prev.Y), true);
            line.Draw(wb, new Point(prev.X + lengthX, prev.Y), new Point(prev.X + lengthX, prev.Y + lengthY), true);
            line.Draw(wb, new Point(prev.X + lengthX, prev.Y + lengthY), new Point(prev.X, prev.Y + lengthY), true);
            line.Draw(wb, new Point(prev.X, prev.Y + lengthY), prev, true);
        }
    }
}
