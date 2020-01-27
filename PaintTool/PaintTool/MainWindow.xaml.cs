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
using PaintTool.Actions;
using System.Drawing;

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
        PaintColor paintColor = new PaintColor();
        // Инициализируем WriteableBitmap
        WriteableBitmap wb, wbCopy;
        // Инициализируем переменную для хранения цвета в формате Bgra32
        byte[] colorData = { 0, 0, 0, 255 };
        bool drawingBrokenLine = false;

        //Структуры для хранения кооординат
        System.Drawing.Point prev, position, tempBrokenLine, startBrokenLine, circleStart, CenterPolygon;

        // Создаем два стека. Один хранит состояние битмапа до отмены действия(undoStack), другой
        // хранит состояние битмапа после отмены(redoStack)
        Undo newUndo = new Undo();
        Redo newRedo = new Redo();
        
        // Переменные, которые являются промежуточными. В них записывается клон битмапа, а потом они записываются в свои стеки.

        ShapeEnum currentShape;

        public MainWindow()
        {
            // Конструктор, который строит и отрисовывает интерфейс из MainWindow.xaml файла
            InitializeComponent();
            //wb = new NewImage(640,480);
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
  


        public void SetPixel(System.Drawing.Point pxl, bool altBitmap)
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
                    wbCopy.WritePixels(rect, paintColor.GetColor(), 4, 0);
                else
                    wb.WritePixels(rect, paintColor.GetColor(), 4, 0);
            }
        }

        public void DrawingBrush(object sender, MouseEventArgs e)
        {
            //if (Convert.ToInt32(SizeInput.Text) > 1)
            //    SizeDrawer(prev, position);
            //else
            //{
               new LineCreator().CreateShape(prev, position);
            //}
            prev = position;
            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
        }

        //public void DrawingBrokenLine(object sender, MouseEventArgs e)
        //{
        //    position.X = e.GetPosition(PaintField).X;
        //    position.Y = e.GetPosition(PaintField).Y;
        //    DrawLine(tempBrokenLine, position, true);
        //}

        //private void EndOfBrokenLine(object sender, MouseButtonEventArgs e)
        //{
        //    drawingBrokenLine = false;
        //    if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
        //    {
        //        DrawLine(tempBrokenLine, startBrokenLine);
        //    }

        //}
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

            //if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            //{
            //    if (drawingBrokenLine)
            //    {
            //        wbCopy = new WriteableBitmap(wb);
            //        PaintField.Source = wb;
            //        DrawingBrokenLine(sender, e);
            //        PaintField.Source = wbCopy;
            //    }
            //}

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == PolygonalShape)
            {

                NewImage.GetInstanceCopy();
                new PolygonCreator(5).CreateShape(prev, position);
                PaintField.Source = wbCopy;

                //createdShape.ds = new DrawByLine();
                //createdShape.Draw();
                PaintField.Source = NewImage.GetInstanceCopy();

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

            if ((bool)BrushToggleBtn.IsChecked) newUndo.PutInUndoStack(NewImage.GetInstanceCopy());
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

        //private void SizeDrawer(Point p1, Point p2)
        //{
        //    int size = Convert.ToInt32(SizeInput.Text);
        //    for (int i = 1; i <= size; i++)
        //    {
        //        for (int j = 1; j <= size; j++)
        //        {
        //            if (size > 2)
        //            {
        //                if ((i != 1 && j != 1) || (i != size && j != 1) || (i != 1 && j != size) || (i != size && j != size))
        //                    DrawLine(new Point(prev.X + i, prev.Y + j), new Point(position.X + i, position.Y + j), true);
        //            }
        //            else
        //            {
        //                DrawLine(new Point(prev.X + i, prev.Y + j), new Point(position.X + i, position.Y + j), true);
        //            }
        //        }
        //    }
        //}

        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            position.X = (int)e.GetPosition(PaintField).X;
            position.Y = (int)e.GetPosition(PaintField).Y;
            // Метод для рисования при нажатой ЛКМ

            ShapeCreator currentCreator = null;
            NewImage.GetInstanceCopy();

            switch (currentShape)
            {
                case ShapeEnum.Circle:
                    currentCreator = new CircleCreator(1.111108);
                    break;
                case ShapeEnum.Line:
                    break;
                case ShapeEnum.Rect:
                    break;
                case ShapeEnum.Triangle:
                    break;
                case ShapeEnum.Polygone:
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
                    NewImage.GetInstanceCopy();
                    PaintField.Source = wb;
                    new RectCreator(false).CreateShape(prev, position);
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == TriangleShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    new TriangleCreator(false).CreateShape(prev, position);
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == LineShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    //if (Convert.ToInt32(SizeInput.Text) > 1)
                    //    SizeDrawer(prev, position);
                    //else
                    //{
                        new LineCreator().CreateShape(prev, position);
                    //}
                    PaintField.Source = wbCopy;
                }
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == EllipseShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    new CircleCreator(1).CreateShape(prev, position); 
                    PaintField.Source = wbCopy;
                }
                if (e.LeftButton == MouseButtonState.Released)
                {
                    circleStart = position;
                }
            }

            //if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            //{
            //    if (e.LeftButton == MouseButtonState.Pressed)
            //    {
            //        if (drawingBrokenLine == false)
            //        {
            //            startBrokenLine = tempBrokenLine = prev;
            //        }
            //        drawingBrokenLine = true;
            //        wbCopy = new WriteableBitmap(wb);
            //        PaintField.Source = wb;
            //        DrawingBrokenLine(sender, e);
            //        PaintField.Source = wbCopy;
            //    }
            //}

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == PolygonalShape)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    wbCopy = new WriteableBitmap(wb);
                    PaintField.Source = wb;
                    new PolygonCreator(5).CreateShape(prev, position);
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

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Метод для выбора цвета кисти в комбобоксе
            // Смотрим текстовое значение цвета в поле Fill у выбранного цвета в комбобоксе 
            // и переделываем его в RGB, а затем передаем в метод SetColor для изменения цвета
            ComboBoxItem currentSelectedComboBoxItem = (ComboBoxItem)ColorInput.SelectedItem;
            Grid currentSelectedComboBoxItemGrid = (Grid)currentSelectedComboBoxItem.Content;
            System.Windows.Shapes.Rectangle colorRectangle = (System.Windows.Shapes.Rectangle)currentSelectedComboBoxItemGrid.Children[0];

            System.Windows.Media.Color clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());
            // Создали новый цвет и добавили в него значение. 
            PaintColor newColor = new PaintColor();
            newColor.SetColor(clr.B, clr.G, clr.R);
        }
        

        #region СТИРАНИЕ ПИКСЕЛЕЙ
        private void CleaningField(object sender, RoutedEventArgs e)
        {
            // Очистка поля, точнее все заливается белым цветом
            Paint(255, 255, 255, 255);
        }
        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
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
            wb.WritePixels(rect, paintColor.GetColor(), 4, 0);
        }

        // закрашиваем пиксели

        private void PixelFill(MouseEventArgs e)
        {
            System.Drawing.Point currentPoint = new System.Drawing.Point((int)e.GetPosition(PaintField).X, (int)e.GetPosition(PaintField).Y);
            int x = (int)e.GetPosition(PaintField).X;
            int y = (int)e.GetPosition(PaintField).Y;
            int xOld = x;
            int yOld = y;
            byte[] pixels = GetPixelArrayLength();
            wb.CopyPixels(pixels, GetStride(), 0);
            int currentPixel = (int)currentPoint.X * GetBytesPerPixel() + (int)currentPoint.Y * GetStride();
            byte[] firstColor = GetPixel(new System.Drawing.Point((int)e.GetPosition(PaintField).X, (int)e.GetPosition(PaintField).Y));
            byte[] currentColor = GetPixel(new System.Drawing.Point(x, y));
            //Trace.WriteLine($"Current Color: {currentColor[0]}, {currentColor[1]}, {currentColor[2]}, {currentColor[3]}");
            //Trace.WriteLine($"Set Color: {colorData[0]}, {colorData[1]}, {colorData[2]}, {colorData[3]}");

            for(int i = y; y < PaintField.Height; i++)
            {
                while (currentColor.SequenceEqual(firstColor) && x > 0)
                {
                    currentColor = GetPixel(new System.Drawing.Point(x, i));
                    SetPixel(new System.Drawing.Point(x, i), false);
                    x--;
                }
            }            
            x = xOld +1;
            y = yOld;
            currentColor = GetPixel(new System.Drawing.Point(x, y));
            for (int i = y; y > 0; i--)
            {
                while (currentColor.SequenceEqual(firstColor) && x < PaintField.Width)
                {
                    currentColor = GetPixel(new System.Drawing.Point(x, i));
                    SetPixel(new System.Drawing.Point(x, i), false);
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

        private byte[] GetPixel(System.Drawing.Point point)
        {
            System.Drawing.Point currentPoint = point;
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

        #region Кнопки для методов Undo, Redo    

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            newRedo.RedoMethod();
        }
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            newUndo.UndoMethod(NewImage.GetInstanceCopy());
        }


        #endregion
    }
}
