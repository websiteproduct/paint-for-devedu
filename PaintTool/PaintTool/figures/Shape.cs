using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
                ds.DrawLine();
            }
        }

        public Shape(List<Point> points)
        {
            FigurePoints = points;
        }
        
    }
}
