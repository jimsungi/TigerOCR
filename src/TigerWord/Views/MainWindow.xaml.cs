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
//using Tesseract;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using Point = OpenCvSharp.Point;
using Microsoft.Win32;
//using com.tigerword.ocr;
using MahApps.Metro.Controls;
using Prism.Ioc;
using TigerWord.Core.Services;
using Unity;

namespace TigerWord.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow // System.Windows.Window
    {
        public static MetroWindow current;
        public MainWindow()
        {
            current = this;
            InitializeComponent();
            
        }

        private void btnThemen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGithub_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGitHubLeft_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Topmost = false;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Topmost = true;
        }
    }
}
