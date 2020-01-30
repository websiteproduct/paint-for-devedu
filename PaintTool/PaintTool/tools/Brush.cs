using PaintTool.Creators;
using PaintTool.figures;
using PaintTool.Strategy;
using PaintTool.Thickness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool
{
    public class Brush 
    {
        public List<Point> FigurePoints;
        public void DrawingBrush(Point prev, Point position, ThicknessS currentStrategy)
        {
            new DotCreator().CreateShape(prev, position);
            Shape createdShape = new LineCreator().CreateShape(prev, position);
            createdShape.ds = new DrawByLine();
            DrawStrategy.thicknessStrategy = currentStrategy;
            createdShape.Draw();

        }
    }
}
