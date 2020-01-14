using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //Создаем массив пикселей, используемый для обновления изображения
            byte[] pixels =
            new byte[(int)PaintField.Width * (int)PaintField.Height * (wb.Format.BitsPerPixel / 8)];

            // Закрашиваем наш прямоугольник нужным цветом
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

            // Шаг области обновления пикселей
            int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);

            // Обновляет пиксели растрового изображения
            // Принимает на вход координаты, массив пикселей, шаг и смещение входного буфера 
            wb.WritePixels(rect, pixels, stride, 0);

            // Отрисовываем созданный WriteableBitmap в поле PaintField
            PaintField.Source = wb;

        }

        // Метод для выбора цвета в Bgra32
        private void SetColor(byte blue, byte green, byte red, byte alpha = 255)
        {
            colorData = new byte[] { blue, green, red, alpha };
        }

        // Метод для возвращения значения цвета в Bgra32
        private byte[] GetColor()
        {
            //return new byte[] { colorData[3], colorData[0], colorData[1], colorData[2] };
            return colorData;
        }


        // Метод для выбора цвета кисти в комбобоксе
        // Смотрим текстовое значение цвета в поле Fill у выбранного цвета в комбобоксе 
        // и переделываем его в RGB, а затем передаем в метод SetColor для изменения цвета
        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem currentSelectedComboBoxItem = (ComboBoxItem) ColorInput.SelectedItem;
            Grid currentSelectedComboBoxItemGrid = (Grid)currentSelectedComboBoxItem.Content;
            Rectangle colorRectangle = (Rectangle)currentSelectedComboBoxItemGrid.Children[0];

            Color clr = new Color();
            clr = (Color)ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());

            SetColor(clr.B, clr.G, clr.R);
        }

        // При нажатии на левую кнопку мышки - рисуем пиксели
        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrawPixel(e);
        }
        //При нажатии на правую кнопку мышки - стираем пиксели
        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ErasePixel(e);
        }


        // Метод для рисования пикселей
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

        // Удаление пикселей (закрашивание в белый цвет)
        private void ErasePixel(MouseEventArgs e) 
        {
            byte[] colorData = { 255, 255, 255, 255 }; 

            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(PaintGrid).X),
                    (int)(e.GetPosition(PaintGrid).Y),
                    1,
                    1);

            wb.WritePixels(rect, colorData, 4, 0);
            
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
    }
}
