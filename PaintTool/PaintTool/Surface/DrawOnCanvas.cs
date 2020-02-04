using PaintTool.Creators;
using PaintTool.Thickness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintTool.Surface
{
    class DrawOnCanvas : SurfaceStrategy
    {
        public override void DrawLine(Point start, Point end)
        {         
            
            Line line = new Line();

            line.Stroke = Brushes.White;
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
            line.StrokeThickness = 1;
            start = end;
            
            //newCanvas.Children.Add(line);
        }
    }    
}
