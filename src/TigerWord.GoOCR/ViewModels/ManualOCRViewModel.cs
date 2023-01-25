using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Diagnostics;

using Microsoft.Win32;

using Unity;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using Tesseract;

using OpenCvSharp.Extensions;
using OpenCvSharp;
using Point = OpenCvSharp.Point;


using TigerWord.Core.Services;
using com.tigerword.ocr;
using Ocr = com.tigerword.tigeracter.OcrService;
using TigerWord.Core.ViewModels;
using TigerWord.Core.Biz;
using System.Windows.Controls;
using System.Reflection;

#pragma warning disable CS8604 // 가능한 null 참조 인수입니다.
namespace TigerWord.GoOCR.ViewModels
{
    public class ManualOCRViewModel : BindableBase
    {
        public ManualOCRViewModel(IUserSettingService _UserSettingService,IEventAggregator ea)
        {
            // Menu Event Aggregator
            _ea = ea;
            _ea.GetEvent<MenuSentEvent>().Subscribe(MenuExecuted);
            RunMenuCommand = new DelegateCommand<object>(RunMenu);
            // Service
            UserSettingService = _UserSettingService;
            string lastfile = UserSettingService.GetAppSetting("TigerOCR", "LastFile");
            ImagePath = lastfile;
            SetSource(0, ImagePath);
        }

        #region MenuReceive , MenuSend
        
        private void MenuExecuted(object menu)
        {
            string menu_txt = menu as string;
            string[] menu_path = menu_txt.Split(":");
            if (menu_path != null && menu_path.Length == 2)
            {
                if (menu_path[0] == GoOCR.ModuleID)
                {
                    switch (menu_path[1])
                    {
                        //case "EXIT":
                        //    ExitFunc();
                        //    break;
                        //case "SETTING":
                        //    SettingFunc();
                        //    break;
                        default:
                            break;
                    }
                }
            }
        }

        public DelegateCommand<object> RunMenuCommand { get; set; }

        private void RunMenu(object param)
        {
            _ea.GetEvent<MenuSentEvent>().Publish(param);
        }

        #endregion

        #region binding property
        private string? _ImagePath;
        public string? ImagePath
        {
            get => _ImagePath;
            set => SetProperty(ref _ImagePath, value);
        }

        private int? _CurrentTab=0;
        public int? CurrentTab 
        {
            get => _CurrentTab;
            set => SetProperty(ref _CurrentTab, value);
        }

        private BitmapImage? _StepBitmap0;
        private BitmapImage? _StepBitmap1;
        private BitmapImage? _StepBitmap2;
        private BitmapImage? _StepBitmap3;
        private BitmapImage? _StepBitmap4;
        private BitmapImage? _StepBitmap5;
        private BitmapImage? _StepBitmap6;
        private BitmapImage? _StepBitmap7;
        private BitmapImage? _StepBitmap8;
        private BitmapImage? _StepBitmap9;
        public BitmapImage? StepBitmap0
        {
            get => _StepBitmap0;
            set => SetProperty(ref _StepBitmap0, value);
        }
        public BitmapImage? StepBitmap1
        {
            get => _StepBitmap1;
            set => SetProperty(ref _StepBitmap1, value);
        }
        public BitmapImage? StepBitmap2
        {
            get => _StepBitmap2;
            set => SetProperty(ref _StepBitmap2, value);
        }
        public BitmapImage? StepBitmap3
        {
            get => _StepBitmap3;
            set => SetProperty(ref _StepBitmap3, value);
        }
        public BitmapImage? StepBitmap4
        {
            get => _StepBitmap4;
            set => SetProperty(ref _StepBitmap4, value);
        }
        public BitmapImage? StepBitmap5
        {
            get => _StepBitmap5;
            set => SetProperty(ref _StepBitmap5, value);
        }
        public BitmapImage? StepBitmap6
        {
            get => _StepBitmap6;
            set => SetProperty(ref _StepBitmap6, value);
        }
        public BitmapImage? StepBitmap7
        {
            get => _StepBitmap7;
            set => SetProperty(ref _StepBitmap7, value);
        }
        public BitmapImage? StepBitmap8
        {
            get => _StepBitmap8;
            set => SetProperty(ref _StepBitmap8, value);
        }
        public BitmapImage? StepBitmap9
        {
            get => _StepBitmap9;
            set => SetProperty(ref _StepBitmap9, value);
        }
        #endregion binding property
        #region Services
        private IUserSettingService? _userSettingService;
        private IEventAggregator _ea;
        public IUserSettingService? UserSettingService
        {
            get => _userSettingService;
            set => SetProperty(ref _userSettingService, value);
        }
        #endregion Services


        #region set visible
        private void SetSource(int i,string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    BitmapImage _Rail1DisplayedImage = new BitmapImage();
                    _Rail1DisplayedImage.BeginInit();
                    _Rail1DisplayedImage.CacheOption = BitmapCacheOption.OnLoad;
                    _Rail1DisplayedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    _Rail1DisplayedImage.UriSource = new Uri(filename);
                    _Rail1DisplayedImage.DecodePixelWidth = 200;
                    _Rail1DisplayedImage.EndInit();
                    SetSource(i, _Rail1DisplayedImage);
                    
                }catch
                { 
                }
            }        
        }

        private void SetSource(int index, Mat mat)
        {
            BitmapImage bi = Ocr.ToBitmapImage(mat);
            SetSource(index, bi);
        }

        private void SetSource(int index, BitmapImage bi)
        {
            switch (index)
            {
                case 0:
                    StepBitmap0 = bi;
                    break;
                case 1:
                    StepBitmap1 = bi;
                    break;
                case 2:
                    StepBitmap2 = bi;
                    break;
                case 3:
                    StepBitmap3 = bi;
                    break;
                case 4:
                    StepBitmap4 = bi;
                    break;
                case 5:
                    StepBitmap5 = bi;
                    break;
                case 6:
                    StepBitmap6 = bi;
                    break;
                case 7:
                    StepBitmap7 = bi;
                    break;
                case 8:
                    StepBitmap8 = bi;
                    break;
                case 9:
                    StepBitmap9 = bi;
                    break;
                default:
                    break;
            }
        }
        private void SetSource(System.Windows.Controls.Image imgctrl, string filename)
        {
            BitmapImage bitmap = Ocr.GetBitmapFromFileName(filename);
            if(bitmap !=null)
            {
                imgctrl.SetCurrentValue(System.Windows.Controls.Image.SourceProperty, bitmap);
            }
        }
        #endregion set visible
        #region Delegate Command
        private DelegateCommand<string>? _SelectImageCmd;
        private DelegateCommand<string>? _StepByStepDeleageCmd;
        public DelegateCommand<string> SelectImageDeleageCmd =>
            _SelectImageCmd ?? (_SelectImageCmd = new DelegateCommand<string>(SelectImageFunc));
        public DelegateCommand<string> StepByStepDeleageCmd =>
    _StepByStepDeleageCmd ?? (_StepByStepDeleageCmd = new DelegateCommand<string>(StepByStepFunc));
        //string regstring = @"tigerocr\setting";
        void SelectImageFunc(string imgFile)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                //txtImage.Text = File.ReadAllText(openFileDialog.FileName);
                string filename = openFileDialog.FileName;
                UserSettingService.SetAppSetting("TigerOCR","LastFile",filename);
                ImagePath = filename;
                SetSource(0,filename);
            }
        }
        void StepByStepFunc(string imgFile)
        {
            step = GoStep(step);
        }
        #endregion
        #region Step By Step



        Mat final_src_resize;
        Mat final_src;
        List<StepInfo> do_Transform_info = null;

        Point?[] mid_Contour = null;
        List<StepInfo> do_Contour_info = null;

        Mat mid_threshold;
        List<StepInfo> do_thread_info = null;
        Mat start_src = null;

        Mat mid_resize;
        //List<StepInfo> do_resize_info = null;
        int resize_width = 0;
        double resize_ratio = 0f;

        //int min_thread_hold = 127;
        //int max_thread_hold = 255;
        int step = 0;

        const int STEP_SRC = 0;
        const int STEP_RESIZE = 1;
        const int STEP_THREADHOLD = 2;
        const int STEP_CONTOUR = 3;
        const int STEP_TRANSFORM = 4;
        const int STEP_OCR = 7;
        int GoStep(int _step)
        {
            bool step_success = false;
            switch (_step)
            {
                case STEP_SRC:

                    string filename = ImagePath;
                    StepBitmap1 = null;
                    StepBitmap2 = null;
                    StepBitmap3 = null;
                    StepBitmap4 = null;

                    step_success = this.do_select_file(filename);

                    if (!step_success)
                        return _step;
                    SetSource(0, start_src);
                    CurrentTab  = _step;
                    _step = STEP_RESIZE;
                    break;
                case STEP_RESIZE:
                    step_success = this.do_resize(400);
                    if (step_success)
                    {
                        SetSource(1, mid_resize);
                    }
                    else
                    {
                        SetSource(1, start_src);
                    }
                    CurrentTab = _step;
                    _step = STEP_THREADHOLD;
                    break;
                case STEP_THREADHOLD:
                    step_success = this.do_thread_hold();
                    if (!step_success)
                    {
                        _step = STEP_SRC;
                        return _step;
                    }
                    SetSource(2, mid_threshold);
                    CurrentTab = _step;
                    _step = STEP_CONTOUR;
                    break;
                case STEP_CONTOUR:
                    step_success = this.do_Contour();
                    if (!step_success)
                    {
                        // Ignore 2 step
                        _step = STEP_SRC;
                        return _step;
                    }
                    if (mid_Contour.Length != 4)
                    {
                        _step = STEP_SRC;
                        return _step;
                    }
                    Mat imgRect = start_src.Clone();
                    Point[] org_Contour = new Point[4];
                    for (int i = 0; i < 4; i++)
                    {
                        org_Contour[i].X = (int)(mid_Contour[i].Value.X * resize_ratio);
                        org_Contour[i].Y = (int)(mid_Contour[i].Value.Y * resize_ratio);
                    }
                    // DRAW RECTANGE FOR FINDED CONTOUR
                    start_src.CopyTo(imgRect);
                    Cv2.Line(imgRect, org_Contour[0], org_Contour[1], new Scalar(255, 0, 0), 10);
                    Cv2.Line(imgRect, org_Contour[1], org_Contour[2], new Scalar(255, 0, 0), 10);
                    Cv2.Line(imgRect, org_Contour[2], org_Contour[3], new Scalar(255, 0, 0), 10);
                    Cv2.Line(imgRect, org_Contour[3], org_Contour[0], new Scalar(255, 0, 0), 10);

                    Mat imgRectResize = mid_resize.Clone();
                    mid_resize.CopyTo(imgRectResize);
                    Cv2.Line(imgRectResize, mid_Contour[0].Value, mid_Contour[1].Value, new Scalar(255, 0, 0), 10);
                    Cv2.Line(imgRectResize, mid_Contour[1].Value, mid_Contour[2].Value, new Scalar(255, 0, 0), 10);
                    Cv2.Line(imgRectResize, mid_Contour[2].Value, mid_Contour[3].Value, new Scalar(255, 0, 0), 10);
                    Cv2.Line(imgRectResize, mid_Contour[3].Value, mid_Contour[0].Value, new Scalar(255, 0, 0), 10);

                    SetSource(3, imgRect);
                    SetSource(5, imgRectResize);

                    CurrentTab = _step;
                    _step = STEP_TRANSFORM;

                    break;
                case STEP_TRANSFORM:

                    // DRAW RECTANGE FOR FINDED CONTOUR
                    step_success = this.do_Transform();
                    if (step_success)
                    {
                        SetSource(4, final_src);
                        SetSource(6, final_src_resize);
                    }
                    CurrentTab = _step;
                    _step = STEP_OCR;
                    break;
                case STEP_OCR:
                    
                    step_success = this.do_ocr(ImagePath);
                    if(step_success)
                    {
                        SetSource(7, final_src);
                    }
                    CurrentTab = _step;
                    _step = STEP_SRC;
                    break;
                default:
                    _step = STEP_SRC;
                    break;
            }
            return _step;
        }


        private bool do_select_file(string filename)
        {
            if (File.Exists(filename))
            {
                start_src = new Mat(filename);
                return true;
            }
            return false;
        }

        void DrawRect(Rectangle rect)
        {

        }

        private bool do_ocr(string filename)
        {
            if(File.Exists(filename))
            {
                Ocr.SetDrawDelegation(DrawRect);
                Ocr.ocr_image(filename);
            }
            return false;
        }

        private bool do_Contour(Mat src = null)
        {
            mid_Contour = null;
            do_Contour_info = new List<StepInfo>();
            Mat do_src = src;
            if (src == null)
                do_src = mid_threshold;
            {
                // 윤곽선을 찾는다
                Point[][] contours1 = new Point[1][]; ;
                //Point[] approx = null;
                Point[][] contours0;
                HierarchyIndex[] hierarchy;
                Cv2.FindContours(do_src, out contours0, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

                Point[] big = null;
                double maxarea = 0.0;

                // 가장 면적이 큰 윤곽선을 찾는다
                for (int i = 0; i < contours0.Length; i++)
                {
                    double area = Cv2.ContourArea(contours0[i]);


                    //Point[] tobR = Cv2.ApproxPolyDP(contours0[i], 0.02 * areaArc, true);   
                    //if (tobR != null && tobR.Length == 4)
                    {
                        // 4각형 영역을 찾았다.
                        if (area > maxarea)
                        {
                            big = contours0[i];
                            maxarea = area;
                            //approx = tobR;
                        }
                    }
                }
                if (big == null)
                {
                    do_Contour_info.Add(new StepInfo(InfoType.Error, "CANNOT FIND COUNTOUR"));
                    return false;
                }
                if (maxarea < 400)
                {
                    do_Contour_info.Add(new StepInfo(InfoType.Error, "COUNTOUR TOO SMALL"));
                    return false;
                }
                // 윤곽선을 근사화한다.
                double areaArc = Cv2.ArcLength(big, true);
                Point[] tobR = Cv2.ApproxPolyDP(big, 0.02 * areaArc, true);
                if (tobR.Length != 4)
                {
                    do_Contour_info.Add(new StepInfo(InfoType.Error, "RECTANGLE NOT FOUND"));
                    return false;
                }
                if (!Cv2.IsContourConvex(tobR))
                {
                    do_Contour_info.Add(new StepInfo(InfoType.Error, "NOT A CONVEX"));
                    return false;
                }
                mid_Contour = Ocr.GetNullable(tobR);
            }
            return true;
        }
        private bool do_Transform(Mat src = null, Point?[] rect = null)
        {
            do_Transform_info = new List<StepInfo>();
            Mat do_src = src;
            Point?[] do_rect = rect;
            if (src == null)
                do_src = start_src;

            if (rect == null)
                do_rect = mid_Contour;

            {
                // x 순으로 정렬
                Array.Sort<Point?>(do_rect, Ocr.CompareByX);
                int len = mid_Contour.Length;
                // 시계방향 정렬
                if (mid_Contour[0].Value.Y > mid_Contour[1].Value.Y)
                {
                    var tmp = mid_Contour[0];
                    mid_Contour[0] = mid_Contour[1];
                    mid_Contour[1] = tmp;
                }
                if (mid_Contour[2].Value.Y < mid_Contour[3].Value.Y)
                {
                    var tmp = mid_Contour[2];
                    mid_Contour[2] = mid_Contour[3];
                    mid_Contour[3] = tmp;
                }

                OpenCvSharp.Size maxSize = Ocr.calcMaxWidthHeight(mid_Contour);
                var dw = maxSize.Width;
                var dh = dw * maxSize.Height / maxSize.Width;
                {
                    Point2f[] srcQuard = new Point2f[4];
                    srcQuard[0].X = (int)(mid_Contour[0].Value.X);//좌상
                    srcQuard[0].Y = (int)(mid_Contour[0].Value.Y * 1);
                    srcQuard[1].X = (int)(mid_Contour[1].Value.X * 1);// 좌하
                    srcQuard[1].Y = (int)(mid_Contour[1].Value.Y * 1);
                    srcQuard[2].X = (int)(mid_Contour[2].Value.X * 1);//우하
                    srcQuard[2].Y = (int)(mid_Contour[2].Value.Y * 1);
                    srcQuard[3].X = (int)(mid_Contour[3].Value.X * 1);// 우상
                    srcQuard[3].Y = (int)(mid_Contour[3].Value.Y * 1);

                    Point2f[] dsQuard = new Point2f[4];
                    dsQuard[0].X = 0;
                    dsQuard[0].Y = 0;
                    dsQuard[1].X = 0;
                    dsQuard[1].Y = (int)(dh * 1);
                    dsQuard[2].X = (int)(dw * 1);
                    dsQuard[2].Y = (int)(dh * 1);
                    dsQuard[3].X = (int)(dw * 1);
                    dsQuard[3].Y = 0;

                    {
                        //  투시변환 매트릭스 구하기
                        Mat re = Cv2.GetPerspectiveTransform(srcQuard, dsQuard);
                        Mat dst = mid_resize.Clone();
                        Cv2.WarpPerspective(mid_resize, dst, re, new OpenCvSharp.Size((dw * 1), (dh * 1)));
                        final_src_resize = dst;
                    }
                }

                {
                    Point2f[] srcQuard = new Point2f[4];
                    srcQuard[0].X = (int)(mid_Contour[0].Value.X * resize_ratio);//좌상
                    srcQuard[0].Y = (int)(mid_Contour[0].Value.Y * resize_ratio);
                    srcQuard[1].X = (int)(mid_Contour[1].Value.X * resize_ratio);// 좌하
                    srcQuard[1].Y = (int)(mid_Contour[1].Value.Y * resize_ratio);
                    srcQuard[2].X = (int)(mid_Contour[2].Value.X * resize_ratio);//우하
                    srcQuard[2].Y = (int)(mid_Contour[2].Value.Y * resize_ratio);
                    srcQuard[3].X = (int)(mid_Contour[3].Value.X * resize_ratio);// 우상
                    srcQuard[3].Y = (int)(mid_Contour[3].Value.Y * resize_ratio);

                    Point2f[] dsQuard = new Point2f[4];
                    dsQuard[0].X = 0;
                    dsQuard[0].Y = 0;
                    dsQuard[1].X = 0;
                    dsQuard[1].Y = (int)(dh * resize_ratio);
                    dsQuard[2].X = (int)(dw * resize_ratio);
                    dsQuard[2].Y = (int)(dh * resize_ratio);
                    dsQuard[3].X = (int)(dw * resize_ratio);
                    dsQuard[3].Y = 0;

                    {
                        //  투시변환 매트릭스 구하기
                        Mat re = Cv2.GetPerspectiveTransform(srcQuard, dsQuard);
                        Mat dst = start_src.Clone();
                        Cv2.WarpPerspective(start_src, dst, re, new OpenCvSharp.Size((dw * resize_ratio), (dh * resize_ratio)));
                        final_src = dst;
                    }
                }
            }

            return true;
        }
        private bool do_resize(int width, Mat src = null)
        {
            do_thread_info = new List<StepInfo>();
            Mat do_src = src;
            if (src == null)
                do_src = start_src;

            resize_width = width;
            OpenCvSharp.Size newsize = new OpenCvSharp.Size();
            resize_ratio = (double)do_src.Width / (double)resize_width;
            int new_height = (int)(do_src.Height / resize_ratio);

            newsize.Width = resize_width;
            newsize.Height = new_height;
            Mat tmp = do_src.Clone();
            Cv2.Resize(do_src, tmp, newsize);

            mid_resize = tmp;
            return true;
        }
        private bool do_thread_hold(Mat src = null)
        {
            do_thread_info = new List<StepInfo>();
            Mat do_src = src;
            if (src == null)
                do_src = mid_resize;
            {
                //try
                //{
                // 흑백영상으로 변환
                Mat gray = do_src.Clone();
                Cv2.CvtColor(do_src, gray, ColorConversionCodes.BGR2GRAY);

                // 이진화
                Mat binSrc = gray.Clone();
                Cv2.Threshold(gray, binSrc, 0.0, 255.0, ThresholdTypes.Otsu);
                if (binSrc == null)
                    return false;
                mid_threshold = binSrc;
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}
                return true;
            }
        }
        
        #endregion Step By Step

    }
}
