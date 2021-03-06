﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace PaintTool
{
    public class NewImage
    {
        private static WriteableBitmap instance;
        private static WriteableBitmap instanceCopy;
        public int width, height;

        public NewImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            instance = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            PaintBitmap(width, height, 255, 255, 255, 255);
        }

        public int Width
        {
            get
            {
                return width;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
        }
        public static WriteableBitmap Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
                instanceCopy = new WriteableBitmap(value);
            }
        }

        public static WriteableBitmap GetInstanceCopy()
        {
            if(instanceCopy == null)
            {
                instanceCopy = new WriteableBitmap(Instance);
            }
            return instanceCopy;
        }

        public void PaintBitmap(int width, int height, int blue, int green, int red, int alpha)
        {
            // Функция, которая принимает на вход значение цвета в формате Bgra32
            // и заливает поле данным цветом

            // Создаем WriteableBitmap поле            

            // Описание координат закрашиваемого прямоугольника
            Int32Rect rect = new Int32Rect(0, 0, width, height);

            int bytesPerPixel = (instance.Format.BitsPerPixel + 7) / 8; // general formula
            int stride = bytesPerPixel * width; // general formula valid for all PixelFormats

            //Width * height *  bytes per pixel aka(32/8)
            byte[] pixels = new byte[stride * height];

            // Закрашиваем наш прямоугольник нужным цветом
            for (int pixel = 0; pixel < pixels.Length; pixel += bytesPerPixel)
            {
                pixels[pixel] = (byte)blue;        // blue (depends normally on BitmapPalette)
                pixels[pixel + 1] = (byte)green;  // green (depends normally on BitmapPalette)
                pixels[pixel + 2] = (byte)red;    // red (depends normally on BitmapPalette)
                pixels[pixel + 3] = (byte)alpha;   // alpha (depends normally on BitmapPalette)
            }

            //int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            instance.WritePixels(rect, pixels, stride, 0);
            
            // Отрисовываем созданный WriteableBitmap в поле PaintField            
        }


    }
}
