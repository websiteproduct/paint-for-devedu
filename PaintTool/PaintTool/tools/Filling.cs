using System;
using System.Linq;
using System.Windows;

namespace PaintTool.tools
{
    public class Filling
    {
        struct XLine
        {
            public int startX;
            public int endX;
        }

        public void SetPixelLineInline(int xStart, int xEnd, int y)
        {
            if ((xStart <= MainWindow.PWidth && xStart >= 0) && (xEnd <= MainWindow.PWidth && xEnd > 0) && (y < MainWindow.PHeight && y >= 0))
            {
                Int32Rect rect = new Int32Rect(
                        xStart,
                        y,
                        xEnd - xStart,
                        1);

                byte[] ColorData = new byte[4 * (xEnd - xStart)];
                byte[] nColor = FillColor.ColorData;

                for (int i = 0; i < 4 * (xEnd - xStart); i += 4)
                {
                    Array.Copy(nColor, 0, ColorData, i, 4);
                }

                NewImage.Instance.WritePixels(rect, ColorData, 4 * (xEnd - xStart), 0);
            }
        }

        private XLine FillLineHorizontalInline(ref byte[] canvasMem, int x, int y, byte[] oldColor)
        {
            XLine rs = new XLine { startX = x, endX = x };

            if (!canvasMem.Skip((y * (int)MainWindow.PWidth + x) * 4).Take(4).SequenceEqual(oldColor))
                return rs;

            Array.Copy(PaintColor.ColorData, 0, canvasMem, (y * (int)MainWindow.PWidth + rs.startX) * 4, 4);

            rs.startX--;
            while (rs.startX >= 0 && canvasMem.Skip((y * (int)MainWindow.PWidth + rs.startX) * 4).Take(4).SequenceEqual(oldColor))
            {
                Array.Copy(PaintColor.ColorData, 0, canvasMem, (y * (int)MainWindow.PWidth + rs.startX) * 4, 4);
                rs.startX--;
            }

            rs.endX++;
            while (rs.endX < MainWindow.PWidth && canvasMem.Skip((y * (int)MainWindow.PWidth + rs.endX) * 4).Take(4).SequenceEqual(oldColor))
            {
                Array.Copy(PaintColor.ColorData, 0, canvasMem, (y * (int)MainWindow.PWidth + rs.endX) * 4, 4);
                rs.endX++;
            }

            SetPixelLineInline(++rs.startX, rs.endX, y);

            // delay for paint 

            //System.Threading.Thread.Sleep(5);
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
            //new Action(delegate { }));

            return rs;
        }

        private void RecLineTopInline(ref byte[] canvasMem, XLine rs, int y, byte[] oldColor)
        {
            if (y - 1 == -1) return;

            for (int i = rs.startX; i < rs.endX; i++)
            {
                XLine sub = FillLineHorizontalInline(ref canvasMem, i, y - 1, oldColor);
                i = sub.endX;

                RecLineTopInline(ref canvasMem, sub, y - 1, oldColor);


                XLine next = sub;
                if (next.startX < rs.startX)
                {
                    next.endX = rs.startX;
                    RecLineBottomInline(ref canvasMem, next, y - 1, oldColor);
                }

                next = sub;
                if (next.endX > rs.endX)
                {
                    next.startX = rs.endX;
                    RecLineBottomInline(ref canvasMem, next, y - 1, oldColor);
                }

            }
        }

        private void RecLineBottomInline(ref byte[] canvasMem, XLine rs, int y, byte[] oldColor)
        {
            if (y + 1 == (int)MainWindow.PHeight) return;

            for (int i = rs.startX; i < rs.endX; i++)
            {
                XLine sub = FillLineHorizontalInline(ref canvasMem, i, y + 1, oldColor);
                i = sub.endX;

                RecLineBottomInline(ref canvasMem, sub, y + 1, oldColor);

                XLine next = sub;
                if (next.startX < rs.startX)
                {
                    next.endX = rs.startX;
                    RecLineTopInline(ref canvasMem, next, y + 1, oldColor);
                }

                next = sub;
                if (next.endX > rs.endX)
                {
                    next.startX = rs.endX;
                    RecLineTopInline(ref canvasMem, next, y + 1, oldColor);
                }


            }
        }
        private void Fill(int x, int y, byte[] oldColor, int recursionDepth = 0)
        {
            if (oldColor.SequenceEqual(PaintColor.ColorData)) return;

            byte[] oldColor1 = new byte[4];//= oldColor;
            Array.Copy(oldColor, 0, oldColor1, 0, 4);

            if (recursionDepth > 1) return;
            //recursionDepth++;
            byte[] pixels = GetPixelArrayLength();
            NewImage.Instance.CopyPixels(pixels, GetStride(), 0);

            XLine rs = FillLineHorizontalInline(ref pixels, x, y, oldColor1);

            RecLineTopInline(ref pixels, rs, y, oldColor1);
            RecLineBottomInline(ref pixels, rs, y, oldColor1);

            pixels = null;

        }

        public void PixelFill(int x, int y)
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
            return GetBytesPerPixel() * MainWindow.PWidth;
        }

        private byte[] GetPixelArrayLength()
        {
            int stride = GetStride();
            byte[] pixels = new byte[stride * MainWindow.PHeight];
            return pixels;
        }
        #endregion
        #region SetPixelLine для X и Y
        public void SetPixelLine(int xStart, int xEnd, int y)
        {
            if ((xStart < MainWindow.PWidth && xStart > 0) && (xEnd < MainWindow.PWidth && xEnd > 0) && (y < MainWindow.PHeight && y > 0))
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
			if ((yStart < MainWindow.PHeight && yStart > 0) && (yEnd < MainWindow.PHeight && yEnd > 0) && (x < MainWindow.PWidth && x > 0))
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
