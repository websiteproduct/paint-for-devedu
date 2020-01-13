﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        WriteableBitmap wb { get; set; }
        byte[] colorData = { 0, 0, 0, 255 };

        public MainWindow()
        {
            InitializeComponent();
        }
        private void WBExample_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WriteableBitmap wb = new WriteableBitmap((int)PaintGrid.ActualWidth, (int)PaintGrid.ActualHeight, 96, 96, PixelFormats.Bgra32, null);

            byte blue = 100;
            byte green = 50;
            byte red = 50;
            byte alpha = 255;
            //Match the order to the pixel type
            byte[] colorData = { blue, green, red, alpha };

            //colordata takes four bytes so stride is 4
            for (int i = 0; i < 120; i++)
            {
                //Use an Int32Rect to choose the rectangular region to edit
                //xy of top left corner plus width and height of edited region
                Int32Rect rect = new Int32Rect(i, i, 1, 1);
                wb.WritePixels(rect, colorData, 4, 0);
            }

            PaintField.Source = wb;
        }



        private int CalculatePixelOffset(int x, int y)
        { //pixel with is the length of a row
          //mulitply it by what row you want to be on then add the remaining pixel to move to the right
            return ((x + (wb.PixelWidth * y)) * (wb.Format.BitsPerPixel / 8));
        }



        private void Brush_Btn_Click(object sender, RoutedEventArgs e)
        {
            ColorsGrid.Visibility = Visibility.Visible;
        }

        private void BrushToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorsGrid.Visibility = (bool)BrushToggleBtn.IsChecked ? Visibility.Visible : Visibility.Hidden;
        }

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SizeInput.Text = SizeSlider.Value.ToString();
        }

        private void TestPaintBlue(object sender, RoutedEventArgs e)
        {
            wb = new WriteableBitmap(
           (int)PaintGrid.ActualWidth, (int)PaintGrid.ActualHeight, 96, 96, PixelFormats.Bgra32, null);



            Int32Rect rect = new Int32Rect(0, 0, (int)PaintGrid.ActualWidth, (int)PaintGrid.ActualHeight);

            //Width * height *  bytes per pixel aka(32/8)
            byte[] pixels =
            new byte[(int)PaintGrid.ActualWidth * (int)PaintGrid.ActualHeight * (wb.Format.BitsPerPixel / 8)];

            Random rand = new Random();
            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {

                    int blue = 255;
                    int green = 0;
                    int red = 0;
                    int alpha = 255;

                    int pixelOffset = CalculatePixelOffset(x, y);
                    pixels[pixelOffset] = (byte)blue;
                    pixels[pixelOffset + 1] = (byte)green;
                    pixels[pixelOffset + 2] = (byte)red;
                    pixels[pixelOffset + 3] = (byte)alpha;
                }
            }

            int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            wb.WritePixels(rect, pixels, stride, 0);

            PaintField.Source = wb;
        }

        private void CleaningField(object sender, RoutedEventArgs e)
        {
            wb = new WriteableBitmap(
           (int)PaintGrid.ActualWidth, (int)PaintGrid.ActualHeight, 96, 96, PixelFormats.Bgra32, null);


            Int32Rect rect = new Int32Rect(0, 0, (int)PaintGrid.ActualWidth, (int)PaintGrid.ActualHeight);

            //Width * height *  bytes per pixel aka(32/8)
            byte[] pixels =
            new byte[(int)PaintGrid.ActualWidth * (int)PaintGrid.ActualHeight * (wb.Format.BitsPerPixel / 8)];

            Random rand = new Random();
            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {

                    int blue = 255;
                    int green = 255;
                    int red = 255;
                    int alpha = 255;

                    int pixelOffset = CalculatePixelOffset(x, y);
                    pixels[pixelOffset] = (byte)blue;
                    pixels[pixelOffset + 1] = (byte)green;
                    pixels[pixelOffset + 2] = (byte)red;
                    pixels[pixelOffset + 3] = (byte)alpha;
                }
            }

            int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            wb.WritePixels(rect, pixels, stride, 0);

            PaintField.Source = wb;

        }

        private void SetColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            colorData = new byte[] { red, green, blue, alpha };
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBox2.SelectedItem)
            {
                case 0:
                    SetColor(0, 0, 0);
                    break;
                case 1:
                    SetColor(169, 169, 169);
                    break;
                case 2:
                    SetColor(255, 0, 0);
                    break;
                case 3:
                    SetColor(0, 0, 255);
                    break;
                case 4:
                    SetColor(0, 128, 0);
                    break;
                case 5:
                    SetColor(165, 42, 42);
                    break;
                case 6:
                    SetColor(128, 0, 128);
                    break;
                case 7:
                    SetColor(211, 211, 211);
                    break;
                case 8:
                    SetColor(144, 238, 144);
                    break;
                case 9:
                    SetColor(173, 216, 230);
                    break;
                case 10:
                    SetColor(0, 255, 255);
                    break;
                case 11:
                    SetColor(255, 165, 0);
                    break;
                case 12:
                    SetColor(255, 255, 0);
                    break;
                case 13:
                    SetColor(210, 180, 140);
                    break;
                case 14:
                    SetColor(255, 192, 203);
                    break;
                case 15:
                    SetColor(255, 255, 255);
                    break;
            }

        }

        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            DrawPixel(e);
        }

        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            ErasePixel(e);
        }

        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DrawPixel(e);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                ErasePixel(e);
            }
        }
        private void ErasePixel(MouseEventArgs e)
        {            
            byte[] colorData = { 255, 255, 255, 255 }; // White color(default)!

            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(PaintField).X),
                    (int)(e.GetPosition(PaintField).Y),
                    1,
                    1);

            wb.WritePixels(rect, colorData, 4, 0);
        }

        private void DrawPixel(MouseEventArgs e)
        {            
            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(PaintField).X),
                    (int)(e.GetPosition(PaintField).Y),
                    1,
                    1);

            wb.WritePixels(rect, colorData, 4, 0);
        }
    }
}
