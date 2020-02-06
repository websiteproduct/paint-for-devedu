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
using System.Text.RegularExpressions;
using Shape = PaintTool.figures.Shape;
using System.Drawing;
using PaintTool.Creators;
using PaintTool.figures;
using PaintTool.Strategy;
using PaintTool.Thickness;
using PaintTool.Actions;
using PaintTool.tools;
using PaintTool.FillStrategy;
using PaintTool.Surface;

namespace PaintTool
{
    public partial class MainWindow : Window

    {
        NewImage newImage;
        PaintColor paintColor = new PaintColor();
        int numberOfSize, polygonNumberOfSide = 5;
        static int width, height;
        bool isShiftPressed = false;
        public static int PWidth
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        public static int PHeight
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
        // Инициализируем WriteableBitmap
        WriteableBitmap wb;
        bool drawingBrokenLine = false;

        //Структуры для хранения кооординат
        System.Drawing.Point prev, position, tempBrokenLine, startBrokenLine;

        // Создаем два стека. Один хранит состояние битмапа до отмены действия(undoStack), другой
        // хранит состояние битмапа после отмены(redoStack)
        Undo newUndo = new Undo();
        Redo newRedo = new Redo();

        // Переменные, которые являются промежуточными. В них записывается клон битмапа, а потом они записываются в свои стеки.

        ShapeEnum currentShape;
        ThicknessS currentStrategy = new DefaultStrategy();

        System.Windows.Shapes.Line exampleVectorLine;
        System.Windows.Shapes.Rectangle fMoveBox = new System.Windows.Shapes.Rectangle();
        System.Windows.Shapes.Rectangle lMoveBox = new System.Windows.Shapes.Rectangle();

        public MainWindow()
        {
            InitializeComponent();
            width = (int)PaintGrid.Width;
            height = (int)PaintGrid.Height;
            //newImage = new NewImage(width, height);
            //PaintField.Source = NewImage.Instance;
            newCanvas.Width = width;
            newCanvas.Height = height;
            //newUndo.PutInUndoStack(NewImage.GetInstanceCopy());
            BrushToggleBtn.IsChecked = true;
            ColorPrimary.IsChecked = true;
            newCanvas.Children.Add(fMoveBox);
            newCanvas.Children.Add(lMoveBox);

            if ((bool)ColorPrimary.IsChecked)
            {
                ColorPrimaryBorder.Background = Brushes.White;
                ColorPrimaryBorder.BorderBrush = Brushes.LightBlue;
            }
            else
            {
                ColorPrimaryBorder.Background = Brushes.Transparent;
                ColorPrimaryBorder.BorderBrush = Brushes.Transparent;
            }

            if ((bool)ColorSecondary.IsChecked)
            {
                ColorSecondaryBorder.Background = Brushes.White;
                ColorSecondaryBorder.BorderBrush = Brushes.LightBlue;
            }
            else
            {
                ColorSecondaryBorder.Background = Brushes.Transparent;
                ColorSecondaryBorder.BorderBrush = Brushes.Transparent;
            }
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
            newCanvas.Width = width;
            newCanvas.Height = height;
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

        private void EndOfBrokenLine(object sender, MouseButtonEventArgs e)
        {
            drawingBrokenLine = false;
            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            {
                Shape createdShape = new LineCreator().CreateShape(tempBrokenLine, startBrokenLine);
                createdShape.ds = new DrawByLine();
                createdShape.fs = new NoFillStrategy();
                SurfaceStrategy.thicknessStrategy = currentStrategy;
                createdShape.Draw();
            }
        }

        private void PaintField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //запоминаем координаты в момент нажатия ЛКМLineShpe

            prev.X = (int)(e.GetPosition(PaintField).X);
            prev.Y = (int)(e.GetPosition(PaintField).Y);

            if ((bool)Filling.IsChecked)
            {

                tools.Filling filling = new Filling();
                filling.PixelFill(prev.X, prev.Y);
            }

            if ((bool)Shapes.IsChecked)
            {
                PickedColor(ColorPrimaryRect);
            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            {
                Shape createdShape = new LineCreator().CreateShape(tempBrokenLine, position);
                createdShape.ds = new DrawByLine();
                createdShape.fs = new NoFillStrategy();
                SurfaceStrategy.thicknessStrategy = currentStrategy;
                createdShape.Draw();
                PaintField.Source = NewImage.Instance;
                NewImage.Instance = NewImage.GetInstanceCopy();
            }

            if ((bool)BrushToggleBtn.IsChecked)
            {
                PickedColor(ColorPrimaryRect);
            }
        }
        private void PaintField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            newUndo.PutInUndoStack(NewImage.GetInstanceCopy());
            NewImage.Instance = (WriteableBitmap)PaintField.Source;

            position.X = (int)(e.GetPosition(PaintField).X);
            position.Y = (int)(e.GetPosition(PaintField).Y);
            tempBrokenLine = position;
        }
        private void PaintField_MouseMove(object sender, MouseEventArgs e)
        {
            position.X = (int)e.GetPosition(PaintField).X;
            position.Y = (int)e.GetPosition(PaintField).Y;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if ((bool)Shapes.IsChecked)
                {
                    ShapeListSelection();
                    ShapeCreator currentCreator = null;

                    switch (currentShape)
                    {
                        case ShapeEnum.Circle:
                            currentCreator = new CircleCreator(isShiftPressed);
                            break;
                        case ShapeEnum.Line:
                            currentCreator = new LineCreator();
                            break;
                        case ShapeEnum.Rect:
                            currentCreator = new RectCreator(isShiftPressed);
                            break;
                        case ShapeEnum.Triangle:
                            currentCreator = new TriangleCreator(isShiftPressed);
                            break;
                        case ShapeEnum.Polygone:
                            currentCreator = new PolygonCreator(polygonNumberOfSide);
                            break;
                        case ShapeEnum.Dot:
                            currentCreator = new DotCreator();
                            break;
                        default:
                            currentCreator = new LineCreator();
                            break;
                    }

                    Shape createdShape = currentCreator.CreateShape(prev, position);
                    createdShape.ds = new DrawByLine();
                    createdShape.fs = new NoFillStrategy();
                    SurfaceStrategy.thicknessStrategy = currentStrategy;
                    createdShape.Draw();
                    PaintField.Source = NewImage.Instance;           //две строчки для динамической отрисовки
                    NewImage.Instance = NewImage.GetInstanceCopy();  //две строчки для динамической отрисовки
                }

                if ((bool)BrushToggleBtn.IsChecked)
                {
                    Brush newBrush = new Brush();
                    newBrush.DrawingBrush(prev, position, currentStrategy);
                    prev = position;
                }
                if ((bool)EraserToggleBtn.IsChecked)
                {
                    PaintColor.ColorData = new byte[] { 255, 255, 255, 255 };
                    Brush newBrush = new Brush();
                    newBrush.DrawingBrush(prev, position, currentStrategy);
                    prev = position;
                }

            }

            if ((bool)Shapes.IsChecked && ShapeList.SelectedItem == BrokenLineShape)
            {
                if (drawingBrokenLine && e.LeftButton == MouseButtonState.Pressed)
                {
                    Shape createdShape = new LineCreator().CreateShape(tempBrokenLine, position);
                    createdShape.ds = new DrawByLine();
                    createdShape.fs = new NoFillStrategy();
                    SurfaceStrategy.thicknessStrategy = currentStrategy;
                    createdShape.Draw();
                    PaintField.Source = NewImage.Instance;
                    NewImage.Instance = NewImage.GetInstanceCopy();
                }

                if (drawingBrokenLine == false)
                {
                    startBrokenLine = tempBrokenLine = position;
                }

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    drawingBrokenLine = true;
                }

            }
        }

        private void NumberOfSideInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberOfSide_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (Int32.TryParse(textBox.Text, out int number))
            {
                polygonNumberOfSide = Convert.ToInt32(textBox.Text.ToString());
            }

            if (polygonNumberOfSide < 5)
                polygonNumberOfSide = 5;
            else if (polygonNumberOfSide > 20)
                polygonNumberOfSide = 20;
        }

        private void ShapeList_SelectionChanged(object sender, SelectionChangedEventArgs e)//фигуры
        {
            ShapeListSelection();
        }

        private void ShapeListSelection()
        {
            if ((bool)Shapes.IsChecked)
            {
                switch (ShapeList.SelectedIndex)
                {
                    case 0:
                        currentShape = ShapeEnum.Line;
                        break;
                    case 1:
                        currentShape = ShapeEnum.Rect;
                        break;
                    case 2:
                        currentShape = ShapeEnum.Circle;
                        break;
                    case 3:
                        currentShape = ShapeEnum.Triangle;
                        break;
                    case 4:
                        currentShape = ShapeEnum.Polygone;
                        break;
                }
            }
        }
        private void AdditionalPanelToggler()
        {
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
                NumbersSide.Visibility = Visibility.Visible;
            }

            else
            {
                ShapeList.Visibility = Visibility.Collapsed;
                NumbersSide.Visibility = Visibility.Collapsed;
            }
            AdditionalPanelToggler();
            if ((bool)BrushToggleBtn.IsChecked)
                currentShape = ShapeEnum.Dot;
        }
        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            numberOfSize = Convert.ToInt32(SizeSlider.Value);

            switch (numberOfSize)
            {
                case 1:
                    currentStrategy = new DefaultStrategy();
                    break;
                case 2:
                    currentStrategy = new MediumStrategy();
                    break;
                case 3:
                    currentStrategy = new BigStrategy();
                    break;
                case 4:
                    currentStrategy = new VeryBigStrategy();
                    break;
                default:
                    currentStrategy = new DefaultStrategy();
                    break;
            }
        }
        #region Цвета
        private void ColorBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PickedColor();
        }


        private void PickedColor(System.Windows.Shapes.Rectangle rectColor)
        {
            // Метод для выбора цвета кисти в комбобоксе
            // Смотрим текстовое значение цвета в поле Fill у выбранного цвета в комбобоксе 
            // и переделываем его в RGB, а затем передаем в метод SetColor для изменения цвета
            //ComboBoxItem currentSelectedComboBoxItem = (ComboBoxItem)ColorInput.SelectedItem;
            //Grid currentSelectedComboBoxItemGrid = (Grid)currentSelectedComboBoxItem.Content;
            //System.Windows.Shapes.Rectangle colorRectangle = (System.Windows.Shapes.Rectangle)currentSelectedComboBoxItemGrid.Children[0];

            //System.Windows.Media.Color clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorRectangle.Fill.ToString());
            System.Windows.Media.Color clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(rectColor.Fill.ToString());
            // Создали новый цвет и добавили в него значение. 

            PaintColor.ColorData = new byte[] { clr.B, clr.G, clr.R, 255 };
        }
        #endregion
        #region Обраотчики кнопки LeftShift
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
        #endregion
        #region СТИРАНИЕ ПИКСЕЛЕЙ
        private void CleaningField(object sender, RoutedEventArgs e)
        {

            newImage.PaintBitmap((int)PaintGrid.Width, (int)PaintGrid.Height, 255, 255, 255, 255);
        }
        private void PaintField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (newImage != null)
            {
                newImage.PaintBitmap((int)PaintGrid.Width, (int)PaintGrid.Height, 255, 255, 255, 255);
                NewImage.Instance = NewImage.Instance;
            }
        }

        private void ExitContextBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ShapeColorTypeChange(object sender, RoutedEventArgs e)
        {
            RadioButton chosenShapeColorType = (RadioButton)e.Source;

            if ((bool)ColorPrimary.IsChecked)
            {
                ColorPrimaryBorder.Background = Brushes.White;
                ColorPrimaryBorder.BorderBrush = Brushes.LightBlue;
            }
            else
            {
                ColorPrimaryBorder.Background = Brushes.Transparent;
                ColorPrimaryBorder.BorderBrush = Brushes.Transparent;
            }

            if ((bool)ColorSecondary.IsChecked)
            {
                ColorSecondaryBorder.Background = Brushes.White;
                ColorSecondaryBorder.BorderBrush = Brushes.LightBlue;
            }
            else
            {
                ColorSecondaryBorder.Background = Brushes.Transparent;
                ColorSecondaryBorder.BorderBrush = Brushes.Transparent;
            }
        }

        private void SelectColor(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType() != typeof(System.Windows.Shapes.Rectangle))
            {
                return;
            }
            else
            {
                System.Windows.Shapes.Rectangle rect = (System.Windows.Shapes.Rectangle)e.Source;
                if ((bool)ColorPrimary.IsChecked)
                {
                    ColorPrimaryRect.Fill = rect.Fill;
                    PickedColor(rect);
                }
                else ColorSecondaryRect.Fill = rect.Fill;
            }
        }

        #endregion

        #region Импорт\экспорт
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

        System.Drawing.Point startVectorPoint;
        bool vectorShapeChosen = false; 

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (vectorShapeChosen && e.LeftButton == MouseButtonState.Pressed)
            {
                exampleVectorLine.X1 += (int)e.GetPosition(newCanvas).X - startVectorPoint.X;
                exampleVectorLine.X2 += (int)e.GetPosition(newCanvas).X - startVectorPoint.X;
                exampleVectorLine.Y1 += (int)e.GetPosition(newCanvas).Y - startVectorPoint.Y;
                exampleVectorLine.Y2 += (int)e.GetPosition(newCanvas).Y - startVectorPoint.Y;
                startVectorPoint.X = (int)e.GetPosition(newCanvas).X;
                startVectorPoint.Y = (int)e.GetPosition(newCanvas).Y;
                Canvas.SetLeft(fMoveBox, exampleVectorLine.X1 - fMoveBox.Width / 2);
                Canvas.SetTop(fMoveBox, exampleVectorLine.Y1 - fMoveBox.Width / 2);
                Canvas.SetLeft(lMoveBox, exampleVectorLine.X2 - lMoveBox.Width / 2);
                Canvas.SetTop(lMoveBox, exampleVectorLine.Y2 - lMoveBox.Width / 2);
            }
        }

        private void VectorLineMouseDown(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Shapes.Line startVectorPoint = (System.Windows.Shapes.Line)newCanvas.Children[0];
            //startVectorPoint.Stroke = Brushes.Blue;
            System.Windows.Shapes.Line test = (System.Windows.Shapes.Line)sender;
            startVectorPoint.X = (int)e.GetPosition(newCanvas).X;
            startVectorPoint.Y = (int)e.GetPosition(newCanvas).Y;
            //Trace.WriteLine($"X: {(int)e.GetPosition(newCanvas).X}, Y: {(int)e.GetPosition(newCanvas).Y}");
        }

        private void VectorLineMouseUp(object sender, MouseButtonEventArgs e)
        {
            exampleVectorLine = (System.Windows.Shapes.Line)sender;
            fMoveBox.Fill = Brushes.Transparent;
            fMoveBox.Stroke = Brushes.Red;
            fMoveBox.StrokeThickness = 1;
            fMoveBox.Width = 7;
            fMoveBox.Height = 7;
            Canvas.SetLeft(fMoveBox, exampleVectorLine.X1 - fMoveBox.Width / 2);
            Canvas.SetTop(fMoveBox, exampleVectorLine.Y1 - fMoveBox.Width / 2);
            lMoveBox.Fill = Brushes.Transparent;
            lMoveBox.Stroke = Brushes.Red;
            lMoveBox.StrokeThickness = 1;
            lMoveBox.Width = 7;
            lMoveBox.Height = 7;
            Canvas.SetLeft(lMoveBox, exampleVectorLine.X2 - lMoveBox.Width / 2);
            Canvas.SetTop(lMoveBox, exampleVectorLine.Y2 - lMoveBox.Width / 2);
            exampleVectorLine.Cursor = Cursors.SizeAll;
            vectorShapeChosen = true;
            fMoveBox.Visibility = Visibility.Visible;
            lMoveBox.Visibility = Visibility.Visible;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (vectorShapeChosen && !(e.OriginalSource is System.Windows.Shapes.Line))
            {
                vectorShapeChosen = false;
                fMoveBox.Visibility = Visibility.Hidden;
                lMoveBox.Visibility = Visibility.Hidden;
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
        #region Методы Undo, Redo    

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            newRedo.RedoMethod();
            NewImage.Instance = newRedo.ImageFromRedoStack;
            PaintField.Source = NewImage.Instance;

        }
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            newUndo.UndoMethod();
            NewImage.Instance = newUndo.ImageFromUndoStack;
            PaintField.Source = NewImage.Instance;
        }
        #endregion
    }
}
