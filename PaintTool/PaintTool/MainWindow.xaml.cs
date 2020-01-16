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
        // Инициализируем переменнух для хранения цвета в формате Bgra32        byte[] colorData = { 0, 0, 0, 255 };

        //два интовых поля для хранения координат
        int xTemp1, yTemp1;
        int xTemp2, yTemp2;

        public MainWindow()
        {
            // Конструктор, который строит и отрисовывает интерфейс из MainWindow.xaml файла
            InitializeComponent(); 
        }

        #region СОЗДАНИЕ НОВОГО ФАЙЛА
        private void NewFile(object sender, RoutedEventArgs e)
        {
            // Создание файла 
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
        #endregion

        #region РИСОВАНИЕ ПИКСЕЛЕЙ
        private void DrawPixel(MouseEventArgs e)
        {
            // Рисование пикселя с выбранным цветом
            int currentX = (int)(e.GetPosition(PaintField).X);
            int currentY = (int)(e.GetPosition(PaintField).Y);

            if (currentX <= Convert.ToInt32(PaintField.Width) && currentY <= Convert.ToInt32(PaintField.Height))
            {
                Int32Rect rect = new Int32Rect(
                        currentX,
                        currentY,
                        1,
                        1);

                int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);

                wb.WritePixels(rect, GetColor(), stride, 0);
            }
        }
        private void DrawPixel(int currentX, int currentY)
        {
            // Перегрузка метода для координат

            if (currentX <= Convert.ToInt32(PaintField.Width) && currentY <= Convert.ToInt32(PaintField.Height))
            {
                Int32Rect rect = new Int32Rect(
                        currentX,
                        currentY,
                        1,
                        1);

                int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);

                wb.WritePixels(rect, GetColor(), stride, 0);
            }
        }

        private void DrawLine(int x1, int y1, int x2, int y2)
        {

            double k; //коэффициент, который выражает отношение абсциссы к ординате -- или наоборот -- в зависимости от того, чья дельта больше

            if(Math.Abs(x2-x1)> Math.Abs(y2 - y1))
            {
                
                int h = Math.Abs(x2 - x1) + 1;
                int w = Math.Abs(y2 - y1) + 1;

                k = (double)w / (double)h;

                for (int i = x1; i < x2; i++)//здесь i это ордината каждой точки нашей линии
                {
                    int y = (int)(k * i + y1); //так мы высчитываем абсциссу
                    DrawPixel(i, y); //здесь должен быть метод, который проглатывает координаты(i,y) и рисует пиксель
                }
            }
            else
            {
                int w = Math.Abs(x2 - x1) + 1;
                int h = Math.Abs(y2 - y1) + 1;

                k = (double)w / (double)h;

                for (int i = y1; i < y2; i++)// в этом случае i это абсцисса
                {
                    int x = (int)(k * i + x1); //так считаем ординату
                    DrawPixel(x, i); //здесь координаты(x,i)
                }
            }
        }
        private int GetCoefficient(int x1, int x2, int y1, int y2, int h, int w)
        {
            if (x1 < x2 && y1 < y2)
            {
                return w / h;
            }
            else if (x1 < x2 && y1 > y2)
            {
                return w / h * -1;
            }
            else if (x1 > x2 && y1 > y2)
            {
                return h / w;
            }
            else if (x1 > x2 && y1 < y2)
            {
                return w / h * -1;
            }
            else return 1;
        }
        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            // Метод для рисования пикселей
            bool isMouseButtonPressed = false;
            isMouseButtonPressed = Convert.ToBoolean(MouseButtonState.Pressed) ? true : false;

            if ((bool)BrushToggleBtn.IsChecked)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DrawPixel(e);
                    //DrawLine(xTemp1, yTemp1, xTemp2, yTemp2);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    ErasePixel(e);
                }                                            
              // xTemp1 = xTemp2;                             //
              // yTemp1 = yTemp2;                             // это должно работать, если пофиксить DrawLine
              // xTemp2 = (int)(e.GetPosition(PaintField).X); // 
              // yTemp2 = (int)(e.GetPosition(PaintField).Y); //
            }
        }
        

        #endregion

        #region КНОПКИ
        private void BrushToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            // При нажатии на кнопку BrushToggleBtn появление/скрытие панели с выбором цвета
            ColorsGrid.Visibility = (bool)BrushToggleBtn.IsChecked ? Visibility.Visible : Visibility.Hidden;
        }
        private void BrushToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            BrushToggleBtn.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BrushToggleBtn.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Появление значения позиции слайдера в окошке справа от него 
            SizeInput.Text = SizeSlider.Value.ToString();
        }
        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // При нажатии на левую кнопку мышки - рисуем пиксель и запоминаем координаты
            if((bool)BrushToggleBtn.IsChecked) DrawPixel(e);

            xTemp1 = (int)(e.GetPosition(PaintField).X);
            yTemp1 = (int)(e.GetPosition(PaintField).Y);
        }
        private void PaintField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //запоминаем координаты
            xTemp2 = (int)(e.GetPosition(PaintField).X);
            yTemp2 = (int)(e.GetPosition(PaintField).Y);

            
            DrawLine(xTemp1, yTemp1, xTemp2, yTemp2);
        }
        #endregion

        #region ЦВЕТА
        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Метод для выбора цвета кисти в комбобоксе
            // Смотрим текстовое значение цвета в поле Fill у выбранного цвета в комбобоксе 
            // и переделываем его в RGB, а затем передаем в метод SetColor для изменения цвета
            ComboBoxItem currentSelectedComboBoxItem = (ComboBoxItem)ColorInput.SelectedItem;
            Grid currentSelectedComboBoxItemGrid = (Grid)currentSelectedComboBoxItem.Content;
            Rectangle colorRectangle = (Rectangle)currentSelectedComboBoxItemGrid.Children[0];

            Color clr = (Color)ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());

            SetColor(clr.B, clr.G, clr.R);
        }
        private void SetColor(byte blue, byte green, byte red, byte alpha = 255)
        {
            colorData = new byte[] { blue, green, red, alpha };
        }
        private byte[] GetColor()
        {
            // Метод для возвращения значения цвета в Bgra32
            return colorData;
        }
        #endregion

        #region СТИРАНИЕ ПИКСЛЕЙЙ
        private void CleaningField(object sender, RoutedEventArgs e)
        {
            // Очистка поля, точнее все заливается белым цветом
            Paint(255, 255, 255, 255);
        }
        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //При нажатии на правую кнопку мышки - стираем пиксели
            ErasePixel(e);
        }
        private void ErasePixel(MouseEventArgs e) 
        {
            // Удаление пикселей (закрашивание в белый цвет)
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
        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wb != null) Paint(255, 255, 255, 255);
        }
        #endregion





        #region ЭКСПЕРИМЕНТАЛЬНЫЕ, ВРЕМЕННЫЕ МЕТОДЫ И ПРОЧЕЕ
        public void Paint(int blue, int green, int red, int alpha)
        {
            // Функция, которая принимает на вход значение цвета в формате Bgra32
            // и заливает поле данным цветом

            // Создаем WriteableBitmap поле
            wb = new WriteableBitmap((int)PaintField.Width, (int)PaintField.Height, 96, 96, PixelFormats.Bgra32, null);


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
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
           
        }
        #endregion
    }
}
