using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace PaintTool.tools
{
	class Filling
	{
		struct XLine
		{
			public int startX;
			public int endX;
		}
		int Width = 640;
		int Height = 480;

		private XLine FillLineHorizontal(ref byte[][][] canvasMem, int x, int y, byte[] oldColor)
		{
			XLine rs;
			rs.startX = x;
			rs.endX = x;

			int lY = y;

			if (!canvasMem[lY][x].SequenceEqual(oldColor))
				return rs;

			Array.Copy(PaintColor.ColorData, 0, canvasMem[lY][rs.startX], 0, 4);

			rs.startX = x - 1;
			rs.endX = x + 1;

			while (rs.startX > 0 && canvasMem[lY][rs.startX].SequenceEqual(oldColor))
			{
				Array.Copy(PaintColor.ColorData, 0, canvasMem[lY][rs.startX], 0, 4);
				//canvasMem[lY][rs.startX] = oldColor;
				rs.startX--;
			}
			//rs.startX++;

			while (rs.endX < Width - 1 && (canvasMem[lY][rs.endX]).SequenceEqual(oldColor))
			{
				Array.Copy(PaintColor.ColorData, 0, canvasMem[lY][rs.endX], 0, 4);
				//canvasMem[lY][rs.endX] = oldColor;
				rs.endX++;
			}
			//rs.endX--;

			SetPixelLine(++rs.startX, rs.endX, lY);


			//System.Threading.Thread.Sleep(5);
			//Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
			//                              new Action(delegate { }));
			return rs;
		}

		private void RecLineTop(ref byte[][][] canvasMem, XLine rs, int y, byte[] oldColor)
		{
			int lY = y - 1;
			if (lY == -1) return;
			for (int i = rs.startX; i < rs.endX; i++)
			{
				//while (canvasMem[lY][i].SequenceEqual(oldColor))
				//{
				XLine sub = FillLineHorizontal(ref canvasMem, i, lY, oldColor);
				i = sub.endX;

				RecLineTop(ref canvasMem, sub, lY, oldColor);

				if (sub.endX > rs.endX)
				{

					sub.startX = rs.endX;

					RecLineBottom(ref canvasMem, sub, lY, oldColor);


				}
				if (sub.startX < rs.startX)
				{
					sub.endX = rs.startX;
					//Trace.WriteLine("BOTTOM: " + sub.startX + " " + sub.endX + " Y: " + lY);
					RecLineBottom(ref canvasMem, sub, lY, oldColor);
				}
				// }
			}
		}

		private void RecLineBottom(ref byte[][][] canvasMem, XLine rs, int y, byte[] oldColor)
		{

			int lY = y + 1;
			if (lY == (int)Height - 1) return;

			for (int i = rs.startX; i < rs.endX; i++)
			{
				//while (canvasMem[lY][i].SequenceEqual(oldColor))
				//{
				XLine sub = FillLineHorizontal(ref canvasMem, i, lY, oldColor);
				i = sub.endX;

				RecLineBottom(ref canvasMem, sub, lY, oldColor);


				if (sub.endX > rs.endX)
				{

					sub.startX = rs.endX;

					RecLineTop(ref canvasMem, sub, lY, oldColor);


				}

				if (sub.startX < rs.startX)
				{
					sub.endX = rs.startX;
					//Trace.WriteLine("BOTTOM: " + sub.startX + " " + sub.endX + " Y: " + lY);
					RecLineTop(ref canvasMem, sub, lY, oldColor);
				}
				//}
			}
		}


		private void Fill(int x, int y, byte[] oldColor, int recursionDepth = 0)
		{
			byte[] oldColor1 = new byte[4];//= oldColor;
			Array.Copy(oldColor, 0, oldColor1, 0, 4);

			if (recursionDepth > 1) return;
			//recursionDepth++;
			byte[] pixels = GetPixelArrayLength();
			NewImage.Instance.CopyPixels(pixels, GetStride(), 0);


			byte[][][] canvasMem = new byte[(int)Height][][];
			//[(int)PaintField.Height][4];

			int m = 0;

			for (int i = 0; i < Height; i++)
			{
				canvasMem[i] = new byte[(int)Width][];

				for (int j = 0; j < Width; j++)
				{
					canvasMem[i][j] = new byte[4];

					for (int k = 0; k < 4; k++)
					{
						canvasMem[i][j][k] = pixels[m + k];
					}
					m += 4;
				}
			}

			XLine rs = FillLineHorizontal(ref canvasMem, x, y, oldColor1);

			RecLineTop(ref canvasMem, rs, y, oldColor1);

			RecLineBottom(ref canvasMem, rs, y, oldColor1);


		}

		private void PixelFill(MouseEventArgs e)
		{
			byte[] currentColor = GetPixel(new Point(x, y));
			Fill(x, y, currentColor);

		}
		#region  GetPixel, GetBytesPerPixel, GetStride, GetPixelArrayLength
		private byte[] GetPixel(Point point)
		{
			Point currentPoint = point;
			byte[] pixels = GetPixelArrayLength();
			NewImage.Instance.CopyPixels(pixels, GetStride(), 0);
			int currentPixel = (int)currentPoint.X * GetBytesPerPixel() + (int)currentPoint.Y * GetStride();
			byte[] color = new byte[] { pixels[currentPixel], pixels[currentPixel + 1], pixels[currentPixel + 2], 255 };
			return color;
		}
		private int GetBytesPerPixel()
		{
			return (NewImage.Instance.Format.BitsPerPixel + 7) / 8;
		}

		private int GetStride()
		{
			return GetBytesPerPixel() * (int)Width;
		}

		private byte[] GetPixelArrayLength()
		{
			int stride = GetStride();
			byte[] pixels = new byte[stride * (int)Height];
			return pixels;
		}
		#endregion
		#region SetPixelLine для X и Y
		public void SetPixelLine(int xStart, int xEnd, int y)
		{
			if ((xStart < Width && xStart > 0) && (xEnd < Width && xEnd > 0) && (y < Height && y > 0))
			{
				Int32Rect rect = new Int32Rect(
						xStart,
						y,
						xEnd - xStart,
						1);

				byte[] ColorData = new byte[4 * (xEnd - xStart)];
				byte[] nColor = PaintColor.ColorData;

				for (int i = 0; i < 4 * (xEnd - xStart); i += 4)
				{

					Array.Copy(nColor, 0, ColorData, i, 4);
				}

				NewImage.Instance.WritePixels(rect, ColorData, 4 * (xEnd - xStart), 0);
			}
		}

		public void SetPixelLineY(int yStart, int yEnd, int x)
		{
			if ((yStart < Height && yStart > 0) && (yEnd < Height && yEnd > 0) && (x < Width && x > 0))
			{
				Int32Rect rect = new Int32Rect(
						x,
						yStart,
						1,
						yEnd - yStart);

				byte[] ColorData = new byte[4 * (yEnd - yStart)];
				byte[] nColor = PaintColor.ColorData;

				for (int i = 0; i < 4 * (yEnd - yStart); i += 4)
				{
					Array.Copy(nColor, 0, ColorData, i, 4);
				}

				NewImage.Instance.WritePixels(rect, ColorData, 4, 0);
			}
		}
        #endregion
    }
}
