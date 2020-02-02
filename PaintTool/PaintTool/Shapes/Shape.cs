using System.Collections.Generic;
using PaintTool.Strategy;
using System.Drawing;
using PaintTool.FillStrategy;

namespace PaintTool.figures
{
    public abstract class Shape
    {
        public List<Point> FigurePoints;
        public DrawStrategy ds;
        public ShapeFillStrategy fs;
        public byte[] colorData;
        
        public virtual void Draw() {

            for (int i = 0; i < FigurePoints.Count - 1; i++)
            {
                ds.Draw(FigurePoints[i], FigurePoints[i + 1]);
            }
            ds.Draw(FigurePoints[FigurePoints.Count - 1], FigurePoints[0]);
            fs.ShapeFill(FigurePoints);
        }
        public Shape(List<Point> points)
        {
            FigurePoints = points;
        }

        
    }
}
