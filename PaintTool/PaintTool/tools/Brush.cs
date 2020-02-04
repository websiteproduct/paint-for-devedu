using PaintTool.Creators;
using PaintTool.figures;
using PaintTool.Strategy;
using PaintTool.Thickness;
using System.Collections.Generic;
using System.Drawing;
using PaintTool.Surface;
using PaintTool.FillStrategy;

namespace PaintTool
{
    public class Brush 
    {
        public List<Point> FigurePoints;
        public void DrawingBrush(Point prev, Point position, ThicknessS currentStrategy)
        {
            DrawOnBitmap draw = new DrawOnBitmap();
            draw.DrawLine(prev, position);
            Shape createdShape = new LineCreator().CreateShape(prev, position);
            createdShape.ds = new DrawByLine();
            createdShape.fs = new NoFillStrategy();
            SurfaceStrategy.thicknessStrategy = currentStrategy;
            createdShape.Draw();

        }
    }
}
