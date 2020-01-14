using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        WriteableBitmap wb;
        int[] previousPoint = new int[2];
        int[] currentPoint = new int[2];
        
        byte[] colorData = { 0, 0, 0, 255 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private int CalculatePixelOffset(int x, int y)
        {
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

        private void CleaningField(object sender, RoutedEventArgs e)
        {
            Paint(255, 255, 255, 255);
        }

        public void Paint(int blue, int green, int red, int alpha)
        {
            wb = new WriteableBitmap((int)PaintField.Width, (int)PaintField.Height, 96, 96, PixelFormats.Bgra32, null);

            Int32Rect rect = new Int32Rect(0, 0, (int)PaintField.Width, (int)PaintField.Height);

            //Width * height *  bytes per pixel aka(32/8)
            byte[] pixels = new byte[(int)PaintField.Width * (int)PaintField.Height * (wb.Format.BitsPerPixel / 8)];

            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
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

        private void SetColor(byte blue, byte green, byte red, byte alpha = 255)
        {
            colorData = new byte[] { blue, green, red, alpha };
        }
        private byte[] GetColor()
        {
            return colorData;
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentSelectedComboBoxItem = (ComboBoxItem) ColorInput.SelectedItem;
            Grid currentSelectedComboBoxItemGrid = (Grid)currentSelectedComboBoxItem.Content;
            Rectangle colorRectangle = (Rectangle)currentSelectedComboBoxItemGrid.Children[0];

            Color clr = new Color();
            clr = (Color)ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());

            SetColor(clr.B, clr.G, clr.R);
        }

        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrawPixel(e);
            previousPoint = new int[] { (int)e.GetPosition(PaintField).X, (int)e.GetPosition(PaintField).Y };
            Trace.WriteLine(previousPoint[0] + ", " + previousPoint[1]);
        }

        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ErasePixel(e);
        }

        //private void DefineQuarter(int[] startPoint, int[] endPoint)
        //{
        //    if (endPoint[0] - startPoint[0] > endPoint[1] - startPoint[1])
        //    {

        //    }
        //}

            private void DrawLine()
        {
            int[] p1 = new int[] { 10, 10 };
            int[] p2 = new int[] { 50, 10 };

            // y = kx + b

            //int k = (p2[1]- p1[1]) / (p2[0] - p1[0]);
            //int b = p1[1] - (k * p1[0]);

            for (int i = p1[0]; i < p2[0]; i++)
            {
                DrawPixelTest(i);
            }
        }

        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            bool isMouseButtonPressed = false;
            isMouseButtonPressed = Convert.ToBoolean(MouseButtonState.Pressed) ? true : false;

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
                    (int)(e.GetPosition(PaintGrid).X),
                    (int)(e.GetPosition(PaintGrid).Y),
                    1,
                    1);

            wb.WritePixels(rect, colorData, 4, 0);
        }

        private void DrawPixelTest(int point)
        {
            Int32Rect rect = new Int32Rect(point, 10, 1, 1);
            int stride = 50 * (wb.Format.BitsPerPixel + 7 / 8);
            int bufferSize = 1 * stride;
            wb.WritePixels(rect, new byte[] { 0, 0, 0 }, bufferSize, 0);
        }

        private void DrawPixel(MouseEventArgs e)
        {
            //Trace.WriteLine(e.GetPosition(PaintField));
            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(PaintGrid).X),
                    (int)(e.GetPosition(PaintGrid).Y),
                    1,
                    1);
            //int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            if (e.GetPosition(PaintField).X >= Convert.ToInt32(PaintField.Width) && e.GetPosition(PaintField).Y >= Convert.ToInt32(PaintField.Height))
            {
                return;
            }
            else wb.WritePixels(rect, GetColor(), 4, 0);
            previousPoint = new int[] { (int)e.GetPosition(PaintGrid).X, (int)e.GetPosition(PaintGrid).Y };
            //Trace.WriteLine(previousPoint[0] + ", " + previousPoint[1]);
            //byte[] b = File.ReadAllBytes(PaintField.Source.ToString());
        }

        private void Test(object sender, MouseEventArgs e)
        {
            currentPoint = new int[] { (int)e.GetPosition(PaintGrid).X, (int)e.GetPosition(PaintGrid).Y };
            Trace.WriteLine(currentPoint[0] + ", " + currentPoint[1]);
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            Window dialog = new NewImageFileWindow();
            dialog.ShowDialog();
        }

        public void SetGridSize(int width, int height)
        {
            PaintGrid.Width = width;
            PaintGrid.Height = height;
            PaintField.Width = width;
            PaintField.Height = height;
            PaintGrid.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void BrushToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            BrushToggleBtn.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BrushToggleBtn.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wb != null) Paint(255, 255, 255, 255);
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            DrawLine();
        }
    }
}
