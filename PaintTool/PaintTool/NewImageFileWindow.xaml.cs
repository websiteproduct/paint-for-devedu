using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PaintTool
{
    /// <summary>
    /// Interaction logic for NewImageFileWindow.xaml
    /// </summary>
    public partial class NewImageFileWindow : Window
    {
        private MainWindow parentWindow;
        public NewImageFileWindow()
        {
            InitializeComponent();
            parentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        }

        private void CreateBitmapOrCanvas(object sender, RoutedEventArgs e)
        {
            if ((bool)VectorBtn.IsChecked)
            {
                parentWindow.newCanvas.Visibility = Visibility.Visible;
                parentWindow.SetGridSize(Convert.ToInt32(ImageWidth.Text), Convert.ToInt32(ImageHeight.Text));
                parentWindow.PaintField.Visibility = Visibility.Collapsed;
                //Line test = new Line();
                //test.Fill = Brushes.Black;
                //test.StrokeThickness = 2;
                //test.Stroke = Brushes.Black;
                //test.X1 = 10;
                //test.X2 = 100;
                //test.Y1 = 5;
                //test.Y2 = 5;
                //parentWindow.newCanvas.Children.Add(test);
                this.Close();
            }
            if ((bool)RasterBtn.IsChecked)
            {
                parentWindow.PaintField.Visibility = Visibility.Visible;
                parentWindow.SetGridSize(Convert.ToInt32(ImageWidth.Text), Convert.ToInt32(ImageHeight.Text));
                NewImage.Instance = new WriteableBitmap((int)parentWindow.PaintGrid.Width, (int)parentWindow.PaintGrid.Height, 96, 96, PixelFormats.Bgra32, null);
                MainWindow.PWidth = Convert.ToInt32(ImageWidth.Text);
                MainWindow.PHeight = Convert.ToInt32(ImageHeight.Text);

                parentWindow.PaintField.Source = NewImage.Instance;
                parentWindow.newCanvas.Visibility = Visibility.Collapsed;

                this.Close();
            }
        }

        private void Btn_Checked_Changed(object sender, RoutedEventArgs e)
        {
            ToggleButton currentToggleBtn = (ToggleButton)sender;
            currentToggleBtn.Background = (bool)currentToggleBtn.IsChecked ? Brushes.LightGray : Brushes.Transparent;
        }
    }
}
