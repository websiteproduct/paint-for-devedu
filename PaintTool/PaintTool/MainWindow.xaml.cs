using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using PaintTool.Creators;
using PaintTool.figures;
using PaintTool.Strategy;
using Shape = PaintTool.figures.Shape;

namespace PaintTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public class FileDialog
    {

    }
    public partial class MainWindow : Window

    {
        // Инициализируем WriteableBitmap
        WriteableBitmap wb, copyUndo, copyRedo, wbCopy;
        // Инициализируем переменную для хранения цвета в формате Bgra32
        byte[] colorData = { 0, 0, 0, 255 };
        bool drawingBrokenLine = false;

        //Структуры для хранения кооординат
        System.Drawing.Point prev, position, tempBrokenLine, startBrokenLine, circleStart, CenterPolygon;

        // Создаем два стека. Один хранит состояние битмапа до отмены действия(undoStack), другой
        // хранит состояние битмапа после отмены(redoStack)
        Stack<WriteableBitmap> undoStack = new Stack<WriteableBitmap>();
        Stack<WriteableBitmap> redoStack = new Stack<WriteableBitmap>();
        // Переменные, которые являются промежуточными. В них записывается клон битмапа, а потом они записываются в свои стеки.

        ShapeEnum currentShape;

        public MainWindow()
        {
            // Конструктор, который строит и отрисовывает интерфейс из MainWindow.xaml файла
            InitializeComponent();
            SetGridSize(640,480);
            Paint(255, 255, 255, 255);
            BrushToggleBtn.IsChecked = true;

            currentShape = ShapeEnum.Line;
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
            PaintGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
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

            int x0 = Convert.ToInt32(prev.X), y0 = Convert.ToInt32(prev.Y);
            List<Point> LineDots = new List<Point>();
 
            double k;

            if (hght >= wth)
            { 
                k = wth * 1.0 / hght;

                if (position.X >= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(Convert.ToInt32(k * i + x0), y0 + i));
                    }
                }
                if (position.X <= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(-Convert.ToInt32(k * i - x0), y0 + i));
                    }
                }

                if (position.X >= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(Convert.ToInt32(k * i + x0), y0 - i));
                    }
                }

                if (position.X <= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < hght; i++)
                    {
                        LineDots.Add(new Point(-Convert.ToInt32(k * i - x0), y0 - i));
                    }
                }

                for (int i = 0; i < hght; i++)
                {
                    SetPixel(LineDots[i], altBitmap);
                }
            }
            else 
            {
                k = hght * 1.0 / wth;

                if (position.X >= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 + i, - Convert.ToInt32(k * i - y0)));
                    }
                }

                if (position.X <= prev.X && position.Y <= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 - i, -Convert.ToInt32(k * i - y0)));
                    }
                }

                if (position.X >= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 + i, Convert.ToInt32(k * i + y0)));
                    }
                }

                if (position.X <= prev.X && position.Y >= prev.Y)
                {
                    for (int i = 0; i < wth; i++)
                    {
                        LineDots.Add(new Point(x0 - i, Convert.ToInt32(k * i + y0)));
                    }
                }

                for (int i = 0; i < wth; i++)
                {
                    SetPixel(LineDots[i], altBitmap);
                }
            }

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

        #endregion

        #region Методы Рисования Фигур
        public void DrawingRectangle(object sender, MouseEventArgs e)
        {
            List<Point> rectangleDots = new List<Point>();
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);

            if (isShiftPressed)
            {

                double length = position.X - prev.X;
                if (position.X > prev.X)
                {
                    if (position.Y > prev.Y) rectangleDots = DrawingSquare(length, length);
                    else rectangleDots = DrawingSquare(length, -length);
                }
                else
                {
                    if (position.Y > prev.Y) rectangleDots = DrawingSquare(length, -length);
                    else rectangleDots = DrawingSquare(length, length);
                }

            }
            else
            {
                rectangleDots.Add(prev);
                rectangleDots.Add(new Point(position.X, prev.Y));
                rectangleDots.Add(position);
                rectangleDots.Add(new Point(prev.X, position.Y));
            }

            DrawLine(rectangleDots[0], rectangleDots[1], true);
            DrawLine(rectangleDots[1], rectangleDots[2], true);
            DrawLine(rectangleDots[2], rectangleDots[3], true);
            DrawLine(rectangleDots[3], rectangleDots[0], true);
        }

        private List<Point> DrawingSquare(double lengthX, double lengthY)
        {
            List<Point> tempDots = new List<Point>();
            tempDots.Add(prev);
            tempDots.Add(new Point(prev.X + lengthX, prev.Y));
            tempDots.Add(new Point(prev.X + lengthX, prev.Y + lengthY));
            tempDots.Add(new Point(prev.X, prev.Y + lengthY));
            return tempDots;
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
            List<Point> triangleDots = new List<Point>();
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            double rad = Math.Abs((prev.Y - position.Y)) / 2;
            if (isShiftPressed)
            {
                double temp = Math.Abs(position.X - prev.X);
                double offsetY = Math.Sqrt(3) / 2 * temp;
                if (position.X > prev.X)
                {
                    if (prev.Y > position.Y)
                        triangleDots = DrawingEquilateralTriangle(temp / 2, offsetY);
                    else
                        triangleDots = DrawingEquilateralTriangle(temp / 2, -offsetY);
                }
                else
                {
                    if (prev.Y > position.Y)
                        triangleDots = DrawingEquilateralTriangle(-temp / 2, offsetY);
                    else
                        triangleDots = DrawingEquilateralTriangle(-temp / 2, -offsetY);
                }

            }
            else
            {
                triangleDots.Add(prev);
                triangleDots.Add(new Point(prev.X + (position.X - prev.X) / 2, position.Y));
                triangleDots.Add(new Point(position.X, prev.Y));
            }

                DrawLine(triangleDots[0], triangleDots[1], true);
                DrawLine(triangleDots[1], triangleDots[2], true);
                DrawLine(triangleDots[2], triangleDots[0], true);
                
        }

        private List<Point> DrawingEquilateralTriangle(double offsetX, double offsetY)
        {
            List<Point> tempDots = new List<Point>();
            tempDots.Add(prev);
            tempDots.Add(new Point(prev.X + offsetX, prev.Y - offsetY));
            tempDots.Add(new Point(position.X, prev.Y));

            return tempDots;
        }
        public void DrawingCircle(object sender, MouseEventArgs e)
        {
            List<Point> circleDots = new List<Point>();
            double coeff = Math.Abs((circleStart.X - position.X) / (circleStart.Y - position.Y));
            if (isShiftPressed)
            {
                circleDots = DrawingCircleMethod(sender, e);
            }
            else
            {
                circleDots = DrawingCircleMethod(sender, e, coeff);
            }
            for (int i = 0; i < circleDots.Count - 4; i += 1)
            {
                DrawLine(circleDots[i], circleDots[i + 4], true);
            }
        }

        public List<Point> DrawingCircleMethod(object sender, MouseEventArgs e, double coeff = 1)
        {
            List<Point> tempDots = new List<Point>();
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            double y = Math.Abs(circleStart.Y - position.Y);
            double x = 0;
            double delta = 1 - 2 * y;
            double error = 0;
            while (y >= 0)
            {
                tempDots.Add(new Point(circleStart.X + coeff * x, circleStart.Y + y));
                tempDots.Add(new Point(circleStart.X + coeff * x, circleStart.Y - y));
                tempDots.Add(new Point(circleStart.X - coeff * x, circleStart.Y + y));
                tempDots.Add(new Point(circleStart.X - coeff * x, circleStart.Y - y));

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
            return tempDots;
        }

        public void DrawingPolygon(object sender, MouseEventArgs e, int numberOfSide = 7)
        {
            List<Point> polygonDots = new List<Point>();
            if (numberOfSide > 3)
            {
                CenterPolygon = prev;
                double radius;
                if (Math.Abs(CenterPolygon.X - position.X) > Math.Abs(CenterPolygon.Y - position.Y))
                    radius = Math.Abs(CenterPolygon.X - position.X);
                else
                    radius = Math.Abs(CenterPolygon.Y - position.Y);


                double z = Math.Atan(position.X/position.Y)*180/Math.PI;
                int i = 0;
                double angle = 360 / numberOfSide;

                //z -= angle ;


                while (i < numberOfSide)
                {
                    polygonDots.Add(new Point(CenterPolygon.X + (Math.Cos(z / 180 * Math.PI) * radius),
                                     CenterPolygon.Y - (Math.Sin(z / 180 * Math.PI) * radius)));
                    z -= angle;
                    i++;
                }
                for (int j = 0; j < numberOfSide; j++)
                {
                    if (j < numberOfSide-1) 
                        DrawLine(polygonDots[j], polygonDots[j+1],true);
                    else 
                        DrawLine(polygonDots[j], polygonDots[0], true);
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
                    if (size > 2)
                    {
                        if ((i != 1 && j != 1) || (i != size && j != 1) || (i != 1 && j != size) || (i != size && j != size))
                            DrawLine(new Point(prev.X + i, prev.Y + j), new Point(position.X + i, position.Y + j), true);
                    }
                    else
                    {
                        DrawLine(new Point(prev.X + i, prev.Y + j), new Point(position.X + i, position.Y + j), true);
                    }
                }
            }
        }

        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            position.X = e.GetPosition(PaintField).X;
            position.Y = e.GetPosition(PaintField).Y;
            // Метод для рисования при нажатой ЛКМ

            ShapeCreator currentCreator = null;
            NewImage.CopyInstance();

            switch (currentShape)
            {
                case ShapeEnum.Circle:
                    currentCreator = new CircleCreator(1.111108);
                    break;
                default:
                    currentCreator = new LineCreator();
                    break;
            }

            Shape createdShape = currentCreator.CreateShape(prev, position);
            createdShape.ds = new DrawByLine();
            createdShape.Draw();
            PaintField.Source = NewImage.GetInstanceCopy();




            ///////////////////




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
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    DrawingPolygon(sender, e);
                    PaintField.Source = wbCopy;
                }
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

            System.Windows.Media.Color clr = (System.Windows.Media.Color)ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());

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
        private object openFileDialog;

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
            int x = (int)e.GetPosition(PaintField).X;
            int y = (int)e.GetPosition(PaintField).Y;
            int xOld = x;
            int yOld = y;
            byte[] pixels = GetPixelArrayLength();
            wb.CopyPixels(pixels, GetStride(), 0);
            int currentPixel = (int)currentPoint.X * GetBytesPerPixel() + (int)currentPoint.Y * GetStride();
            byte[] firstColor = GetPixel(new Point(e.GetPosition(PaintField).X, e.GetPosition(PaintField).Y));
            byte[] currentColor = GetPixel(new Point(x, y));
            //Trace.WriteLine($"Current Color: {currentColor[0]}, {currentColor[1]}, {currentColor[2]}, {currentColor[3]}");
            //Trace.WriteLine($"Set Color: {colorData[0]}, {colorData[1]}, {colorData[2]}, {colorData[3]}");

            for(int i = y; y < PaintField.Height; i++)
            {
                while (currentColor.SequenceEqual(firstColor) && x > 0)
                {
                    currentColor = GetPixel(new Point(x, i));
                    SetPixel(new Point(x, i), false);
                    x--;
                }
            }            
            x = xOld +1;
            y = yOld;
            currentColor = GetPixel(new Point(x, y));
            for (int i = y; y > 0; i--)
            {
                while (currentColor.SequenceEqual(firstColor) && x < PaintField.Width)
                {
                    currentColor = GetPixel(new Point(x, i));
                    SetPixel(new Point(x, i), false);
                    x++;
                }
            }
            
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

            //Int32Rect rect = new Int32Rect(0, 0, (int)PaintField.Width, (int)PaintField.Height);
            //wb.WritePixels(rect, pixels, GetStride(), 0);
        }

        private byte[] GetPixel(Point point)
        {
            Point currentPoint = point;
            byte[] pixels = GetPixelArrayLength();
            wb.CopyPixels(pixels, GetStride(), 0);
            int currentPixel = (int)currentPoint.X * GetBytesPerPixel() + (int)currentPoint.Y * GetStride();
            byte[] color = new byte[] { pixels[currentPixel], pixels[currentPixel + 1], pixels[currentPixel + 2], 255 };
            return color;
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
            OpenFileDialog dlg = new OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Load Drawing 
                string filename = dlg.FileName;
                LoadDrawing(filename);
            }
            

        }


        private void SaveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and Save
            if (result == true)
            {
                // Save Drawing 
                string filename = dlg.FileName;
                SaveDrawing(filename);
            }
        }


            private void SaveDrawing(string filename)
        {
            //get the height and width of the PanelGrid
            int width = Convert.ToInt32(PaintGrid.Width);
            int height = Convert.ToInt32(PaintGrid.Height);
            
            //Render a bitmap of the PanelGrid
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(PaintGrid), null, new System.Windows.Rect(0, 0, width, height));
            }
            rtb.Render(dv);
            
            //Encode the render into a bitmap
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            //Save the bitmap using the given name
            using (var fs = System.IO.File.OpenWrite(filename))
            {
                encoder.Save(fs);
            }

        }

        private void LoadDrawing(string filename)
        {
            //Loads the given file image
            BitmapImage filefoto = new BitmapImage(new Uri(filename));
            var image = new Image { Source = filefoto };
            PaintGrid.Children.Add(image);

            // TODO: Test this after Menu Button is added

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
