using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        private void CreateBitmap(object sender, RoutedEventArgs e)
        {
            parentWindow.SetGridSize(Convert.ToInt32(ImageWidth.Text), Convert.ToInt32(ImageHeight.Text));
            parentWindow.Paint(255, 255, 255, 255);
            this.Close();
        }
    }
}
