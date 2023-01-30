using System;
using System.Collections.Generic;
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

namespace TigerWord.GoOCR.Views
{
    /// <summary>
    /// ManualOCR.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualOCR : UserControl
    {
        public ManualOCR()
        {
            InitializeComponent();
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 )
            {
                step7.Width = step7.Width * 1.1;
                step7.Height = step7.Height * 1.1;
            }
            else if(e.Delta < 0)
            {
                step7.Width = step7.Width * 0.9;
                step7.Height = step7.Height * 0.9;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            step7.Width = step7.ActualWidth * 1.1;
            step7.Height = step7.ActualHeight * 1.1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            step7.Width = step7.ActualWidth * 0.9;
            step7.Height = step7.ActualHeight * 0.9;
        }
    }
}
