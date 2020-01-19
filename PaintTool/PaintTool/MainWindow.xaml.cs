using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        // Инициализируем переменную для хранения цвета в формате Bgra32
        byte[] colorData = { 0, 0, 0, 255 };

        //две структуры для хранения кооординат
        Point prev; Point position;

        // Создаем два стека. Один хранит состояние битмапа до отмены действия(undoStack), другой
        // хранит состояние битмапа после отмены(redoStack)
        Stack<WriteableBitmap> undoStack = new Stack<WriteableBitmap>();
        Stack<WriteableBitmap> redoStack = new Stack<WriteableBitmap>();
        // Переменные, которые являются промежуточными. В них записывается клон битмапа, а потом они записываются в свои стеки.
        WriteableBitmap copyUndo, copyRedo, wbCopy;

        public MainWindow()
        {
            // Конструктор, который строит и отрисовывает интерфейс из MainWindow.xaml файла
            InitializeComponent();
            SetGridSize(640, 480);
            Paint(255, 255, 255, 255);
            //BrushToggleBtn.IsChecked = true;
            Shapes.IsChecked = true;
            ShapeList.SelectedItem = RectangleShape;
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

        #region РАБОЧАЯ СВЯЗКА МЕТОДОВ ДЛЯ РИСОВАНИЯ ПРЯМОЙ
        private void DrawLine(Point prev, Point position, bool altBitmap = false)
        {
            int wth = Convert.ToInt32(Math.Abs(position.X - prev.X) + 1);
            int hght = Convert.ToInt32(Math.Abs(position.Y - prev.Y) + 1);
            if (ShapeList.SelectedItem == RectangleShape || ShapeList.SelectedItem == TriangleShape)
            {
                wth--;
                hght--;
            }
            int x0 = Convert.ToInt32(prev.X);
            int y0 = Convert.ToInt32(prev.Y);
            int x;
            int y;
            int[] xArr;
            int[] yArr;
            double k;
            int quarter = FindQuarter(prev, position);

            if (hght >= wth)
            {
                xArr = new int[hght];
                yArr = new int[hght];
                k = wth * 1.0 / hght;

                if (quarter == 4)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i + x0);
                        xArr[i] = x;
                        yArr[i] = y0 + i;
                    }
                }
                if (quarter == 3)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i - x0);
                        xArr[i] = -x >= 0 ? -x : 0;
                        yArr[i] = y0 + i;
                    }
                }

                if (quarter == 1)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i + x0);
                        xArr[i] = x;
                        yArr[i] = y0 - i;
                    }
                }

                if (quarter == 2)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = Convert.ToInt32(k * i - x0);
                        xArr[i] = -x;
                        yArr[i] = y0 - i;
                    }
                }

                for (int i = 0; i < hght; i++)
                {
                    prev.Y = yArr[i];
                    prev.X = xArr[i];
                    SetPixel(prev, altBitmap);
                }
            }
            else if (hght < wth)
            {
                xArr = new int[wth];
                yArr = new int[wth];
                k = hght * 1.0 / wth;

                if (quarter == 1)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i - y0);
                        yArr[i] = -y;
                        xArr[i] = x0 + i;
                    }
                }

                if (quarter == 2)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i - y0);
                        yArr[i] = -y;
                        xArr[i] = x0 - i;
                    }
                }

                if (quarter == 4)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i + y0);
                        yArr[i] = y;
                        xArr[i] = x0 + i;
                    }
                }

                if (quarter == 3)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = Convert.ToInt32(k * i + y0);
                        yArr[i] = y;
                        xArr[i] = x0 - i;
                    }
                }

                for (int i = 0; i < wth; i++)
                {
                    prev.Y = yArr[i];
                    prev.X = xArr[i];
                    SetPixel(prev, altBitmap);
                }
            }

        }

        public int FindQuarter(Point prev, Point position)
        {
            int quarter = 0;
            if (position.X >= prev.X && position.Y >= prev.Y)
            {
                quarter = 4;
            }
            if (position.X <= prev.X && position.Y <= prev.Y)
            {
                quarter = 2;
            }
            if (position.X >= prev.X && position.Y <= prev.Y)
            {
                quarter = 1;
            }
            if (position.X <= prev.X && position.Y >= prev.Y)
            {
                quarter = 3;
            }
            return quarter;
        }

        public void SetPixel(Point pxl, bool altBitmap)
        {

            if ((pxl.X < PaintField.Width && pxl.X > 0) && (pxl.Y < PaintField.Height && pxl.Y > 0))
            {
                Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(pxl.X),
                        Convert.ToInt32(pxl.Y),
                        1,
                        1);

                //int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
                //if ((pxl.X < 1 || pxl.X > PaintField.Width) || (pxl.Y < 1 || pxl.Y > PaintField.Height))
                //{
                //    return;
                //} else 
                if (altBitmap)
                    wbCopy.WritePixels(rect, GetColor(), 4, 0);
                else
                    wb.WritePixels(rect, GetColor(), 4, 0);
            }
        }
        public void SetPixel(int x, int y, bool altBitmap)
        {

            if (x <= PaintField.Width && y <= PaintField.Height)
            {
                Int32Rect rect = new Int32Rect(
                        Convert.ToInt32(x),
                        Convert.ToInt32(y),
                        1,
                        1);

                //int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
                if (altBitmap)
                    wbCopy.WritePixels(rect, GetColor(), 4, 0);
                else
                    wb.WritePixels(rect, GetColor(), 4, 0);
            }
        }

        #endregion

        #region Методы Рисования Фигур
        public void DrawingRectangle(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            //}

            //if (e.LeftButton == MouseButtonState.Released)
            //{
            if (isShiftPressed)
            {
                //DrawLine(prev, new Point(position.X, prev.Y), true);
                //Trace.WriteLine($"First point: {prev.X},{prev.Y}. Second Point: {position.X},{position.Y}");
                //DrawLine(prev, new Point(prev.X, position.X), true);

                Point diag; diag.X = position.X; diag.Y = position.X;
                double length = position.X - prev.X;
                if (position.X > prev.X)
                {
                    if (position.Y > prev.Y) DrawingSquare(length, length);
                    else DrawingSquare(length, -length);
                }
                else
                {
                    if (position.Y > prev.Y) DrawingSquare(length, -length);
                    else DrawingSquare(length, length);
                }
                
            }
            else
            {
                DrawLine(prev, new Point(position.X, prev.Y), true);
                DrawLine(new Point(position.X, prev.Y), position, true);
                DrawLine(position, new Point(prev.X, position.Y), true);
                DrawLine(new Point(prev.X, position.Y), prev, true);
            }
            //}
        }

        private void DrawingSquare(double lengthX, double lengthY)
        {
            DrawLine(prev, new Point(prev.X + lengthX, prev.Y), true);
            DrawLine(new Point(prev.X + lengthX, prev.Y), new Point(prev.X + lengthX, prev.Y + lengthY), true);
            DrawLine(new Point(prev.X + lengthX, prev.Y + lengthY), new Point(prev.X, prev.Y + lengthY), true);
            DrawLine(new Point(prev.X, prev.Y + lengthY), prev, true);
        }
        public void DrawingLineOnField(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                position.X = (int)(e.GetPosition(PaintField).X);
                position.Y = (int)(e.GetPosition(PaintField).Y);
            }

            if (e.LeftButton == MouseButtonState.Released)
            {
                DrawLine(prev, position);
            }
        }

        public void DrawingBrush(object sender, MouseEventArgs e)
        {
            DrawLine(prev, position);
            prev = position;
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
        }

        public void DrawingTriangle(object sender, MouseEventArgs e)
        {
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);

            DrawLine(prev, position, true);
            DrawLine(position, new Point(2 * position.X - prev.X, prev.Y), true);
            DrawLine(new Point(2 * position.X - prev.X, prev.Y), prev, true);
        }
        public void DrawingCircle(object sender, MouseEventArgs e)
        {
            int wth = Convert.ToInt32(Math.Abs(position.X - prev.X) + 1);
            int hght = Convert.ToInt32(Math.Abs(position.Y - prev.Y) + 1);
            int x0 = Convert.ToInt32(prev.X);
            int y0 = Convert.ToInt32(prev.Y);
            int x;
            int y;
            int[] xArr;
            int[] yArr;
            int quarter = FindQuarter(prev, position);

            Point center;
            center.X = (int)((Math.Abs(position.X - prev.X) + 1) / 2);
            center.Y = (int)((Math.Abs(position.Y - prev.Y) + 1) / 2);
            double r = Math.Sqrt(wth * wth + hght * hght);

            if (hght >= wth)
            {
                xArr = new int[hght];
                yArr = new int[hght];

                if (quarter == 4)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = (int)Math.Sqrt(r * r - i * i) - 1;
                        xArr[i] = (int)center.X + x;
                        yArr[i] = (int)center.Y + i;
                    }
                }
                if (quarter == 3)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = (int)Math.Sqrt(r * r - i * i) - 1;
                        xArr[i] = (int)center.X - x >= 0 ? -x : 0;
                        yArr[i] = (int)center.Y + i;
                    }
                }

                if (quarter == 1)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = (int)Math.Sqrt(r * r - i * i) - 1;
                        xArr[i] = (int)center.X + x;
                        yArr[i] = (int)center.Y - i;
                    }
                }

                if (quarter == 2)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        x = (int)Math.Sqrt(r * r - i * i) - 1;
                        xArr[i] = (int)center.X - x;
                        yArr[i] = (int)center.Y - i;
                    }
                }

                for (int i = 0; i < hght; i++)
                {
                    prev.Y = yArr[i];
                    prev.X = xArr[i];
                    //SetPixel(prev);
                }
            }
            else if (hght < wth)
            {
                xArr = new int[wth];
                yArr = new int[wth];


                if (quarter == 1)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = (int)Math.Sqrt(r * r - i * i) - 1;
                        yArr[i] = (int)center.Y - y;
                        xArr[i] = (int)center.X + i;
                    }
                }

                if (quarter == 2)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = (int)Math.Sqrt(r * r - i * i) - 1;
                        yArr[i] = (int)center.Y - y;
                        xArr[i] = (int)center.X - i;
                    }
                }

                if (quarter == 4)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = (int)Math.Sqrt(r * r - i * i) - 1;
                        yArr[i] = (int)center.Y + y;
                        xArr[i] = (int)center.X + i;
                    }
                }

                if (quarter == 3)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        y = (int)Math.Sqrt(r * r - i * i) - 1;
                        yArr[i] = (int)center.Y + y;
                        xArr[i] = (int)center.X - i;
                    }
                }

                for (int i = 0; i < wth; i++)
                {
                    prev.Y = yArr[i];
                    prev.X = xArr[i];
                    //SetPixel(prev);
                    Trace.WriteLine(prev);
                }
            }
        }

        //public void DrawingBrokenLine(object sender, MouseEventArgs e)
        //{
        //    Point start = prev;
        //    bool drawing = true;

        //    while (drawing == true)
        //    {
        //        if (e.LeftButton == MouseButtonState.Pressed)
        //        {
        //            position.X = (int)(e.GetPosition(PaintField).X);
        //            position.Y = (int)(e.GetPosition(PaintField).Y);
        //        }

        //        if (e.LeftButton == MouseButtonState.Released)
        //        {
        //            DrawLine(prev, position);
        //        }
        //        prev = position;
        //        if (e.LeftButton == this.MouseDoubleClick)
        //        {

        //        }
        //    }

        //}

        #endregion

        #region КНОПКИ
        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //запоминаем координаты в момент нажатия ЛКМ

            prev.X = (int)(e.GetPosition(PaintField).X);
            prev.Y = (int)(e.GetPosition(PaintField).Y);
            //position.X = prev.X;
            //position.Y = prev.Y;
            if (ShapeList.SelectedItem == Filling)
            {
                PixelFill(e);
            }
            if ((bool)Shapes.IsChecked)
            {
                wbCopy = wb;
            }
        }
        private void PaintField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //запоминаем координаты в момент отпускания ЛКМ

            //prev.X = 0;
            //prev.Y = 0;

            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);

            //if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == LineShape)
            //{
            //    if (Convert.ToInt32(SizeInput.Text) == 2)
            //    {
            //        Point test1 = new Point(prev.X + 1, prev.Y + 1);
            //        Point test2 = new Point(position.X + 1, position.Y + 1);
            //        DrawLine(test1, test2);
            //        test1 = new Point(prev.X - 1, prev.Y - 1);
            //        test2 = new Point(position.X - 1, position.Y - 1);
            //        DrawLine(test1, test2);
            //    }
            //    else DrawLine(prev, position);
            //}
            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == RectangleShape)
            {
                PaintField.Source = wbCopy;
                wb = wbCopy;
            }
            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == LineShape)
            {
                PaintField.Source = wbCopy;
                wb = wbCopy;
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == TriangleShape)
            {
                PaintField.Source = wbCopy;
                wb = wbCopy;
            }

            if ((bool)BrushToggleBtn.IsChecked) PutInUndoStack();
        }

        bool isShiftPressed = false;

        private void KeyDown_Event(object sender, KeyEventArgs e)
        {
            isShiftPressed = e.Key == Key.LeftShift ? true : false;
            //if (e.Key == Key.LeftShift)
            //{
            //    isShiftPressed = true;
            //} else 
        }
        private void KeyUp_Event(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                isShiftPressed = false;
            }
        }

        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            position.X = e.GetPosition(PaintField).X;
            position.Y = e.GetPosition(PaintField).Y;
            // Метод для рисования КИСТЬЮ при нажатой ЛКМ

            if ((bool)BrushToggleBtn.IsChecked && e.LeftButton == MouseButtonState.Pressed)
            {
                DrawingBrush(sender, e);
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == RectangleShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawingRectangle(sender, e);
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == TriangleShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawingTriangle(sender, e);
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == LineShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawLine(prev, position, true);
                    PaintField.Source = wbCopy;
                }
            }

            //if (isShiftPressed)
            //{
            //    Trace.WriteLine("shift is activated");
            //}
            //else Trace.WriteLine("poshel nah");
        }

        //else if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == LineShape)
        //{
        //    DrawingLineOnField(sender, e);
        //}

        //else if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == EllipseShape)
        //{
        //    DrawingCircle(sender, e);
        //}

        //else if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == TriangleShape)
        //{
        //    DrawingTriangle(sender, e);
        //}

        //else if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
        //{

        //    //DrawingBrokenLine(sender, e);
        //}

        private void AdditionalPanelToggler()
        {
            // При нажатии на кнопку BrushToggleBtn появление/скрытие панели с выбором цвета
            ColorsGrid.Visibility = (bool)BrushToggleBtn.IsChecked || (bool)Filling.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            SizePanel.Visibility = (bool)BrushToggleBtn.IsChecked || (bool)EraserToggleBtn.IsChecked || (bool)Shapes.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CurrentToggleBtn_IsCheckedChanged(object sender, RoutedEventArgs e)
        {
            ToggleButton currentToggleBtn = (ToggleButton)sender;
            currentToggleBtn.Background = (bool)currentToggleBtn.IsChecked ? Brushes.LightGray : Brushes.Transparent;
            if ((bool)Shapes.IsChecked)
            {
                ShapeList.Visibility = Visibility.Visible;
            }
            else ShapeList.Visibility = Visibility.Collapsed;
            AdditionalPanelToggler();
        }

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Появление значения позиции слайдера в окошке справа от него 
            SizeInput.Text = SizeSlider.Value.ToString();
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

        #region СТИРАНИЕ ПИКСЕЛЕЙ
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

        // Метод для рисования пикселей
        //private void PaintField_MouseMove(object sender, MouseEventArgs e)
        //{
        //    bool isMouseButtonPressed = false;
        //    isMouseButtonPressed = Convert.ToBoolean(MouseButtonState.Pressed) ? true : false;

        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        DrawPixel(e);
        //    }
        //    else if (e.RightButton == MouseButtonState.Pressed)
        //    {
        //        ErasePixel(e);
        //    }
        //}

        int[] p1 = new int[2], p2 = new int[2];

        private void DownDown(object sender, MouseEventArgs e)
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

        // закрашиваем пиксели

        private void PixelFill(MouseEventArgs e)
        {
            int bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8; // general formula
            int stride = bytesPerPixel * (int)PaintField.Width; // general formula valid for all PixelFormats

            //Width * height *  bytes per pixel aka(32/8)
            byte[] pixels = new byte[stride * (int)PaintField.Height];

            // Закрашиваем наш прямоугольник нужным цветом
            for (int pixel = 0; pixel < pixels.Length; pixel += bytesPerPixel)
            {
                //if (pixels[pixel] == (byte)255 && pixels[pixel + 1] == (byte)255 && pixels[pixel + 2] == (byte)255 && pixels[pixel + 3] == (byte)255)
                //{
                pixels[pixel] = 0;        // blue (depends normally on BitmapPalette)
                pixels[pixel + 1] = 0;  // green (depends normally on BitmapPalette)
                pixels[pixel + 2] = 255;    // red (depends normally on BitmapPalette)
                pixels[pixel + 3] = 255;   // alpha (depends normally on BitmapPalette)
                                           //}
            }

            Int32Rect rect = new Int32Rect(0, 0, (int)PaintField.Width, (int)PaintField.Height);

            //int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            wb.WritePixels(rect, pixels, stride, 0);
        }

        // Удаление пикселей (закрашивание в белый цвет)
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
            //PutInUndoStack();
            // Отрисовываем созданный WriteableBitmap в поле PaintField
            PaintField.Source = wb;
        }

        private void ShapeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == LineShape) Trace.WriteLine("zhopa");
        }


        #endregion

        #region Методы для Undo, Redo

        private void Undomethod()
        {
            if (undoStack.Count > 0)
            {
                PutInRedoStack();
                wb = undoStack.Pop();
                PaintField.Source = wb;
            }
            else if (undoStack.Count == 0)
            {
                return;
            }
        }
        private void PutInUndoStack()
        {
            copyUndo = wb.Clone();
            undoStack.Push(copyUndo);
        }
        private void PutInRedoStack()
        {
            copyRedo = wb.Clone();
            redoStack.Push(copyRedo);
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            Redomethod();
        }
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            Undomethod();
        }
        private void Redomethod()
        {
            if (redoStack.Count > 0)
            {
                wb = redoStack.Pop();
                PaintField.Source = wb;
            }
            else if (undoStack.Count == 0)
            {
                return;
            }
        }

        #endregion
    }
}
