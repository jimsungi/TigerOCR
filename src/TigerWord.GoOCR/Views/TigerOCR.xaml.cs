using Microsoft.Win32;
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
using Tesseract;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using Point = OpenCvSharp.Point;
using com.tigerword.ocr;
using MahApps.Metro.Controls;
using System.IO;
using System.Diagnostics;
using TigerWord.Core.Services;

namespace TigerWord.GoOCR.Views
{
    /// <summary>
    /// TigerOCR.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TigerOCR : UserControl
    {
        public TigerOCR()
        {
            InitializeComponent();
        }

    }
}
