using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using PaintTool.figures;
using PaintTool.Creators;
using PaintTool.tools;

namespace PaintTool.FillStrategy
{
	public class SolidFillStrategy : ShapeFillStrategy
	{
		public override void ShapeFill(List<System.Drawing.Point> points)
		{
			double everageX = 0;
			double everageY = 0;
			int totalX = 0;
			int totalY = 0;

			for (int i = 0; i < points.Count; i++)
			{
				totalX += points[i].X;
				totalY += points[i].Y;
			}
			everageX = totalX / points.Count;
			everageY = totalY / points.Count;

			tools.Filling filling = new Filling();
			filling.PixelFill((int)everageX, (int)everageY);

		}
	}
}
