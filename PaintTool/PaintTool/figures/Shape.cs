using System.Collections.Generic;
using PaintTool.Strategy;
using System.Drawing;

namespace PaintTool.figures
{
    public abstract class Shape
    {
        public List<Point> FigurePoints;
        public DrawStrategy ds;
        public virtual void Draw() {
            for (int i = 0; i < FigurePoints.Count; i++)
            {
                //new ds.DrawLine();
                ds = new DrawByLine();
            }
        }

        public Shape(List<Point> points)
        {
            FigurePoints = points;
        }
        
    }
}
