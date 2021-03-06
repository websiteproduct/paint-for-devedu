﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PaintTool.figures;

namespace PaintTool.Creators
{
    public class RectCreator : ShapeCreator
    {
        private bool isShiftPressed;
        //private byte[] colorData;
        //int size;
        //private Shape line;
        //bool isShiftPressed;

        public RectCreator(bool isShiftPressed)
        {
            this.isShiftPressed = isShiftPressed;
        }

        public Shape CreateShape(Point start, Point end)
        {
             List<Point> tempDots = new List<Point>();

            if (isShiftPressed)
            {
                int length = end.X - start.X;
                if (end.X > start.X)
                {
                    if (end.Y > start.Y) tempDots = DrawingSquare(start, length, length);
                    else tempDots = DrawingSquare(start, length, -length);
                }
                else
                {
                    if (end.Y > start.Y) tempDots = DrawingSquare(start, length, -length);
                    else tempDots = DrawingSquare(start, length, length);
                }

            }
            else
            {
                tempDots.Add(start);
                tempDots.Add(new Point(end.X, start.Y));
                tempDots.Add(end);
                tempDots.Add(new Point(start.X, end.Y));
            }

            return new Rect(tempDots);
        }
        private List<Point> DrawingSquare(Point start, int lengthX, int lengthY)
        {
            List<Point> tempDots = new List<Point>();
            tempDots.Add(start);
            tempDots.Add(new Point(start.X + lengthX, start.Y));
            tempDots.Add(new Point(start.X + lengthX, start.Y + lengthY));
            tempDots.Add(new Point(start.X, start.Y + lengthY));
            return tempDots;
        }
    }

}
