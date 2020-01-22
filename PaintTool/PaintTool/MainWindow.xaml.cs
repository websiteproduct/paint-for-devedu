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
        WriteableBitmap wb, copyUndo, copyRedo, wbCopy;

        // Инициализируем переменную для хранения цвета в формате Bgra32
        byte[] colorData = { 0, 0, 0, 255 };
        bool drawingBrokenLine = false;

        //Структуры для хранения кооординат
        Point prev, position, tempBrokenLine, startBrokenLine, circleStart;

        // Создаем два стека. Один хранит состояние битмапа до отмены действия(undoStack), другой
        // хранит состояние битмапа после отмены(redoStack)
        Stack<WriteableBitmap> undoStack = new Stack<WriteableBitmap>();
        Stack<WriteableBitmap> redoStack = new Stack<WriteableBitmap>();
        // Переменные, которые являются промежуточными. В них записывается клон битмапа, а потом они записываются в свои стеки.

        public MainWindow()
        {
            // Конструктор, который строит и отрисовывает интерфейс из MainWindow.xaml файла
            InitializeComponent();
            SetGridSize(640, 480);
            Paint(255, 255, 255, 255);
            BrushToggleBtn.IsChecked = true;
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

        private int GetBytesPerPixel()
        {
            return (wb.Format.BitsPerPixel + 7) / 8;
        }

        private int GetStride()
        {
            return GetBytesPerPixel() * (int)PaintField.Width;
        }

        private byte[] GetPixelArrayLength()
        {
            int stride = GetStride();
            byte[] pixels = new byte[stride * (int)PaintField.Height];
            return pixels;
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

                if ((bool)EraserToggleBtn.IsChecked)
                {
                    wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
                }
                else if (altBitmap)
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
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);

            if (isShiftPressed)
            {
                double length;
                if (Math.Abs(position.X - prev.X) < Math.Abs(position.Y - prev.Y))
                {
                    length = Math.Abs(position.X - prev.X);
                }
                else
                {
                    length = Math.Abs(position.Y - prev.Y);
                }


                if (position.X == prev.X || position.Y == prev.Y)
                {
                    DrawingSquare(length, 0);
                }         
                if (position.X > prev.X)
                {
                    if (position.Y < prev.Y) DrawingSquare(length, -length);
                    else DrawingSquare(length, length);
                }
                else
                {
                    if (position.Y > prev.Y) DrawingSquare(-length, length);
                    else if (position.Y < prev.Y) DrawingSquare(-length, -length);
                }

            }
            else
            {
                DrawLine(prev, new Point(position.X, prev.Y), true);
                DrawLine(new Point(position.X, prev.Y), position, true);
                DrawLine(position, new Point(prev.X, position.Y), true);
                DrawLine(new Point(prev.X, position.Y), prev, true);
            }
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
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);

            DrawLine(prev, position);
        }

        public void DrawingBrush(object sender, MouseEventArgs e)
        {
            if (Convert.ToInt32(SizeInput.Text) > 1)
                SizeDrawer(prev, position);
            else
            {
                DrawLine(prev, position, false);
            }
            prev = position;
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
        }

        public void DrawingTriangle(object sender, MouseEventArgs e)
        {
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            double rad = Math.Abs((prev.Y - position.Y)) / 2;
            //if (Convert.ToInt32(SizeInput.Text) > 1)
            //{
            //    SizeDrawer(prev, position);
            //    SizeDrawer(position, new Point(2 * prev.X - position.X, position.Y));
            //    SizeDrawer(new Point(2 * prev.X - position.X, position.Y), prev);
            //}
            //else
            //{
            //    DrawLine(prev, position, true);
            //    DrawLine(position, new Point(2 * prev.X - position.X, position.Y), true);
            //    DrawLine(new Point(2 * prev.X - position.X, position.Y), prev, true);
            //}
           
            if (isShiftPressed)
            {
                DrawingPolygon(sender, e, 3, rad);
            }
            else
            {
                DrawLine(prev, position, true);
                DrawLine(position, new Point(2 * prev.X - position.X, position.Y), true);
                DrawLine(new Point(2 * prev.X - position.X, position.Y), prev, true);
            }
        }

        public void DrawingCircle(object sender, MouseEventArgs e)
        {
            double coeff = Math.Abs((circleStart.X - position.X) / (circleStart.Y - position.Y));
            if (isShiftPressed)
            {
                DrawingCircleMethod(sender, e);
            }
            else
            {
                DrawingCircleMethod(sender, e, coeff);
            }
        }
        
        public void DrawingCircleMethod(object sender, MouseEventArgs e, double coeff=1)
        {
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            double y = Math.Abs(circleStart.Y - position.Y);
            double x = 0;
            double delta = 1 - 2 * y;
            double error = 0;
            while (y >= 0)
            {
                SetPixel(new Point(circleStart.X + coeff * x, circleStart.Y + y), true);
                SetPixel(new Point(circleStart.X + coeff * x, circleStart.Y - y), true);
                SetPixel(new Point(circleStart.X - coeff * x, circleStart.Y + y), true);
                SetPixel(new Point(circleStart.X - coeff * x, circleStart.Y - y), true);
                error = 2 * (delta + y) - 1;
                if ((delta < 0) && (error <= 0))
                {
                    delta += 2 * ++x + 1;
                    continue;
                }
                if ((delta > 0) && (error > 0))
                {
                    delta -= 2 * --y + 1;
                    continue;
                }
                delta += 2 * (++x - y--);
            }
        }

        public void DrawingPolygon(object sender, MouseEventArgs e, int numberOfSide = 10, double radius=100)
        {
            if (numberOfSide > 3)
            {
                radius = 100;

                Point Center = position;
                Point tempPrev;
                Point tempNext;

                double z = 0;
                int i = 0;
                double angle = 360 / numberOfSide;

                tempPrev = new Point(Center.X + (int)(Math.Round(Math.Cos(z / 180 * Math.PI) * radius)),
                    Center.Y - (int)(Math.Round(Math.Sin(z / 180 * Math.PI) * radius)));
                z += angle;


                while (i < numberOfSide)
                {
                    tempNext = new Point(Center.X + (int)(Math.Round(Math.Cos(z / 180 * Math.PI) * radius)),
                    Center.Y - (int)(Math.Round(Math.Sin(z / 180 * Math.PI) * radius)));
                    DrawLine(tempPrev, tempNext, true);
                    tempPrev = tempNext;
                    z += angle;
                    i++;
                }
            }

        }

        public void DrawingBrokenLine(object sender, MouseEventArgs e)
        {
            position.X = e.GetPosition(PaintField).X;
            position.Y = e.GetPosition(PaintField).Y;
            DrawLine(tempBrokenLine, position, true);
        }

        private void EndOfBrokenLine(object sender, MouseButtonEventArgs e)
        {
            drawingBrokenLine = false;
            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            {
                DrawLine(tempBrokenLine, startBrokenLine);
            }

        }

        #endregion

        #region КНОПКИ
        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //запоминаем координаты в момент нажатия ЛКМLineShpe

            prev.X = (int)(e.GetPosition(PaintField).X);
            prev.Y = (int)(e.GetPosition(PaintField).Y);

            if ((bool)EraserToggleBtn.IsChecked)
            {
                ErasePixel(e);
            }

            if ((bool)Filling.IsChecked)
            {
                PixelFill(e);
            }
            if ((bool)Shapes.IsChecked)
            {
                wbCopy = wb;
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            {
                if (drawingBrokenLine)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawingBrokenLine(sender, e);
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == PolygonalShape)
            {

                wbCopy = new WriteableBitmap(wb);
                PaintField.Source = wb;
                DrawingPolygon(sender, e);
                PaintField.Source = wbCopy;

            }
        }
        private void PaintField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //запоминаем координаты в момент отпускания ЛКМ

            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            tempBrokenLine = position;

            if ((bool)Shapes.IsChecked)
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
        }
        private void KeyUp_Event(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                isShiftPressed = false;
            }
        }

        private void SizeDrawer(Point p1, Point p2)
        {
            int size = Convert.ToInt32(SizeInput.Text);
            for (int i = 1; i <= size; i++)
            {
                for (int j = 1; j <= size; j++)
                {
                    if ((i != 1 && j != 1) || (i != size && j != 1) || (i != 1 && j != size) || (i != size && j != size))
                        DrawLine(new Point(prev.X + i, prev.Y + j), new Point(position.X + i, position.Y + j), true);
                }
            }
        }

        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            position.X = e.GetPosition(PaintField).X;
            position.Y = e.GetPosition(PaintField).Y;
            // Метод для рисования при нажатой ЛКМ

            if (((bool)BrushToggleBtn.IsChecked || (bool)EraserToggleBtn.IsChecked) && e.LeftButton == MouseButtonState.Pressed)
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
                    if (Convert.ToInt32(SizeInput.Text) > 1)
                        SizeDrawer(prev, position);
                    else
                    {
                        DrawLine(prev, position, true);
                    }
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == EllipseShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawingCircle(sender, e);
                    PaintField.Source = wbCopy;
                }
                if (e.LeftButton == MouseButtonState.Released)
                {
                    circleStart = position;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (drawingBrokenLine == false)
                    {
                        startBrokenLine = tempBrokenLine = prev;
                    }
                    drawingBrokenLine = true;
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawingBrokenLine(sender, e);
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == PolygonalShape)
            {
                wbCopy = new WriteableBitmap(wb);
                PaintField.Source = wb;
                DrawingPolygon(sender, e);
                PaintField.Source = wbCopy;
            }
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
            ColorsGrid.Visibility = (bool)BrushToggleBtn.IsChecked || (bool)Filling.IsChecked || (bool)Shapes.IsChecked ? Visibility.Visible : Visibility.Collapsed;
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
            Point currentPoint = new Point(e.GetPosition(PaintField).X, e.GetPosition(PaintField).Y);
            byte[] pixels = GetPixelArrayLength();
            wb.CopyPixels(pixels, GetStride(), 0);
            // Закрашиваем наш прямоугольник нужным цветом
            //for (int pixel = 0; pixel < pixels.Length; pixel += GetBytesPerPixel())
            //{
            //    if (pixels[pixel] == 255 && pixels[pixel + 1] == 255 && pixels[pixel + 2] == 255 && pixels[pixel + 3] == 255)
            //    {
            //        pixels[pixel] = colorData[0];
            //        pixels[pixel + 1] = colorData[1];
            //        pixels[pixel + 2] = colorData[2];
            //        pixels[pixel + 3] = colorData[3];
            //    }
            //}
            // 1,0
            int currentPixel = (int)currentPoint.X * GetBytesPerPixel() + (GetStride() * (int)currentPoint.Y);
            if (pixels[currentPixel] != colorData[0] && pixels[currentPixel + 1] != colorData[1] && pixels[currentPixel + 2] != colorData[2]) SetPixel(currentPoint, false);
            Trace.WriteLine(colorData[0]);
            Trace.WriteLine(pixels[currentPixel]);

            //Int32Rect rect = new Int32Rect(0, 0, (int)PaintField.Width, (int)PaintField.Height);

            ////int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            //wb.WritePixels(rect, pixels, GetStride(), 0);
        }

        // Удаление пикселей (закрашивание в белый цвет)
        private void ErasePixel(MouseEventArgs e)
        {
            //// Удаление пикселей (закрашивание в белый цвет)
            //int bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8;
            //int stride = bytesPerPixel;
            //// умножаем на ширину пикселей
            ////int stride = bytesPerPixel * 5;

            //byte[] pixels = new byte[stride];
            //// количество пикселей, которые будем добавлять: умножаем на высота добавляемого участка
            ////byte[] pixels = new byte[stride * 5];

            //for (int pixel = 0; pixel < pixels.Length; pixel += bytesPerPixel)
            //{
            //    pixels[pixel] = 255;        // blue (depends normally on BitmapPalette)
            //    pixels[pixel + 1] = 255;  // green (depends normally on BitmapPalette)
            //    pixels[pixel + 2] = 255;    // red (depends normally on BitmapPalette)
            //    pixels[pixel + 3] = 255;   // alpha (depends normally on BitmapPalette)
            //}

            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(PaintGrid).X),
                    (int)(e.GetPosition(PaintGrid).Y),
                    1,
                    1);

            wb.WritePixels(rect, new byte[] { 255, 255, 255, 255 }, 4, 0);
            //Trace.WriteLine(stride);
        }
        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wb != null) Paint(255, 255, 255, 255);
        }

        private void ImportImageBtn_Click(object sender, RoutedEventArgs e)
        {

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
