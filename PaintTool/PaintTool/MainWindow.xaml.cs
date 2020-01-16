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
        // Инициализируем WriteableBitmap
        WriteableBitmap wb;
        // Инициализируем переменнух для хранения цвета в формате Bgra32
        byte[] colorData = { 0, 0, 0, 255 };

        public MainWindow()
        {
            // Конструктор, который строит и отрисовывает интерфейс из MainWindow.xaml файла
            InitializeComponent();
        }

        private int CalculatePixelOffset(int x, int y)
        {
            return ((x + (wb.PixelWidth * y)) * (wb.Format.BitsPerPixel / 8));
        }


        // Делает видимым панель с выбором цвета, как понял это прошлый метод
        // Ниже новая версия, эту позже удалим
        private void Brush_Btn_Click(object sender, RoutedEventArgs e)
        {
            ColorsGrid.Visibility = Visibility.Visible;
        }

        // При нажатии на кнопку BrushToggleBtn появление/скритие панели с выбором цвета
        private void BrushToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorsGrid.Visibility = (bool)BrushToggleBtn.IsChecked ? Visibility.Visible : Visibility.Hidden;
        }


        // Появление значения позиции слайдера в окошке справа от него 
        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SizeInput.Text = SizeSlider.Value.ToString();
        }

        // Очистка поля, точнее все заливается белым цветом
        private void CleaningField(object sender, RoutedEventArgs e)
        {
            Paint(255, 255, 255, 255);
        }

        // Функция, которая принимает на вход значение цвета в формате Bgra32
        // и заливает поле данным цветом
        public void Paint(int blue, int green, int red, int alpha)
        {
            // Создаем WriteableBitmap поле
            wb = new WriteableBitmap(
           (int)PaintField.Width, (int)PaintField.Height, 96, 96, PixelFormats.Bgra32, null);


            // Описание координат закрашиваемого прямоугольника
            Int32Rect rect = new Int32Rect(0, 0, (int)PaintField.Width, (int)PaintField.Height);

            int bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8; // general formula
            int stride = bytesPerPixel * (int)PaintField.Width; // general formula valid for all PixelFormats

            //Width * height *  bytes per pixel aka(32/8)
            byte[] pixels = new byte[stride * (int)PaintField.Height];

            // Закрашиваем наш прямоугольник нужным цветом
            for (int pixel = 0; pixel < pixels.Length; pixel += bytesPerPixel)
            {
                pixels[pixel] = (byte)blue;        // blue (depends normally on BitmapPalette)
                pixels[pixel + 1] = (byte)green;  // green (depends normally on BitmapPalette)
                pixels[pixel + 2] = (byte)red;    // red (depends normally on BitmapPalette)
                pixels[pixel + 3] = (byte)alpha;   // alpha (depends normally on BitmapPalette)
            }

            //int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            wb.WritePixels(rect, pixels, stride, 0);

            // Отрисовываем созданный WriteableBitmap в поле PaintField
            PaintField.Source = wb;
        }
        //private void DrawLine()
        //{
        //    int[] p1 = new int[] { 10, 10 };
        //    int[] p2 = new int[] { 110, 10 };

        //    // y = kx + b

        //    //int k = (p2[1]- p1[1]) / (p2[0] - p1[0]);
        //    //int b = p1[1] - (k * p1[0]);

        //    for (int i = p1[0]; i < p2[0]; i++)
        //    {
        //        DrawPixelTest(i);
        //    }
        //}

        // Метод для выбора цвета в Bgra32
        private void SetColor(byte blue, byte green, byte red, byte alpha = 255)
        {
            colorData = new byte[] { blue, green, red, alpha };
        }

        // Метод для возвращения значения цвета в Bgra32
        private byte[] GetColor()
        {
            return colorData;
        }


        // Метод для выбора цвета кисти в комбобоксе
        // Смотрим текстовое значение цвета в поле Fill у выбранного цвета в комбобоксе 
        // и переделываем его в RGB, а затем передаем в метод SetColor для изменения цвета
        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentSelectedComboBoxItem = (ComboBoxItem)ColorInput.SelectedItem;
            Grid currentSelectedComboBoxItemGrid = (Grid)currentSelectedComboBoxItem.Content;
            Rectangle colorRectangle = (Rectangle)currentSelectedComboBoxItemGrid.Children[0];

            Color clr = (Color)ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());

            SetColor(clr.B, clr.G, clr.R);
        }

        // При нажатии на левую кнопку мышки - рисуем пиксели
        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrawPixel(e);
            //previousPoint = new int[] { (int)e.GetPosition(PaintField).X, (int)e.GetPosition(PaintField).Y };
            //Trace.WriteLine(previousPoint[0] + ", " + previousPoint[1]);
        }
        //При нажатии на правую кнопку мышки - стираем пиксели
        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ErasePixel(e);
        }


        // Метод для рисования пикселей
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

        int[] p1 = new int[2], p2 = new int[2];

        private void Down(object sender, MouseEventArgs e)
        {
            p1[0] = (int)e.GetPosition(PaintField).X;
            p1[1] = (int)e.GetPosition(PaintField).Y;
        }

        private void Up(object sender, MouseEventArgs e)
        {
            p2[0] = (int)e.GetPosition(PaintField).X;
            p2[1] = (int)e.GetPosition(PaintField).Y;
            Trace.WriteLine(p1[0] + "," + p1[1]);
            Trace.WriteLine(p2[0] + "," + p2[1]);
            Dr(p1, p2);
        }

        private double GetC(int[] p1, int[] p2, int w, int h)
        {
            double realK = (double)w / (double)h;

            if (p1[0] < p2[0] && p1[1] < p2[1])
            {
                return realK;
            }
            else if (p1[0] < p2[0] && p1[1] > p2[1])
            {
                return realK * -1;
            }
            else if (p1[0] > p2[0] && p1[1] > p2[1])
            {
                return (double)h / (double)w;
            }
            else if (p1[0] > p2[0] && p1[1] < p2[1])
            {
                return realK * -1;
            }
            else return 1;
        }

        private void Dr(int[] p1, int[] p2)
        {
            double k = 0;

            if (Math.Abs(p2[0] - p1[0]) > Math.Abs(p2[1] - p1[1]))
            {
                int h = Math.Abs(p2[0] - p1[0]) + 1;
                int w = Math.Abs(p2[1] - p1[1]) + 1;

                k = GetC(p1, p2, w, h);

                //k = (double)w / (double)h;

                for (int i = p1[0]; i < p2[0]; i++)
                {
                    int y = (int)(k * i + p1[1]);
                    TestPixel(i, y);
                }
            }
            else
            {
                int w = Math.Abs(p2[0] - p1[0]) + 1;
                int h = Math.Abs(p2[1] - p1[1]) + 1;

                k = (double)w / (double)h;

                for (int i = p1[1]; i < p2[1]; i++)
                {
                    int x = (int)(k * i + p1[0]);
                    TestPixel(x, i);
                }
            }
        }

        private void TestPixel(int x, int y)
        {
            Int32Rect rect = new Int32Rect(x, y, 1, 1);

            wb.WritePixels(rect, GetColor(), 4, 0);
        }

        // Удаление пикселей (закрашивание в белый цвет)
        private void ErasePixel(MouseEventArgs e)
        {
            int bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8;
            int stride = bytesPerPixel;
            // умножаем на ширину пикселей
            //int stride = bytesPerPixel * 5;

            byte[] pixels = new byte[stride];
            // количество пикселей, которые будем добавлять: умножаем на высота добавляемого участка
            //byte[] pixels = new byte[stride * 5];

            for (int pixel = 0; pixel < pixels.Length; pixel += bytesPerPixel)
            {
                pixels[pixel] = 255;        // blue (depends normally on BitmapPalette)
                pixels[pixel + 1] = 255;  // green (depends normally on BitmapPalette)
                pixels[pixel + 2] = 255;    // red (depends normally on BitmapPalette)
                pixels[pixel + 3] = 255;   // alpha (depends normally on BitmapPalette)
            }

            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(PaintGrid).X),
                    (int)(e.GetPosition(PaintGrid).Y),
                    1,
                    1);

            wb.WritePixels(rect, pixels, stride, 0);
            //Trace.WriteLine(stride);
        }


        // Рисование пикселей с выбранным цветом
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
            //previousPoint = new int[] { (int)e.GetPosition(PaintGrid).X, (int)e.GetPosition(PaintGrid).Y };
            //Trace.WriteLine(previousPoint[0] + ", " + previousPoint[1]);
            //byte[] b = File.ReadAllBytes(PaintField.Source.ToString());
        }


        // Создание файла 
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

        //private void Redo_Click(object sender, RoutedEventArgs e)
        //{
        //    DrawLine();
        //}
    }
}
