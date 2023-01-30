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
#if PDFSHARP
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
#endif
#if ITEXTSHARP

using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Action;
using Table = iText.Layout.Element.Table;
using Image = iText.Layout.Element.Image;
using Paragraph = iText.Layout.Element.Paragraph;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.IO.Font;
#endif
//using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Documents;
using ControlzEx.Standard;
using System.Linq;
using iText.StyledXmlParser.Css.Selector;
using PDFiumSharp;


#pragma warning disable CS8604 // 가능한 null 참조 인수입니다.
namespace TigerWord.GoOCR.ViewModels
{
    class StepGo : BindableBase
    {
        public StepGo(int index)
        {
            ImageIndex = index;
        }
        private object _in;
        public object In
        {
            get
            {
                return _in;
            }
            set
            {
                SetProperty(ref _in, value);
            }
        }

        private object _out;
        public object Out
        {
            get
            {
                return _out;
            }
            set
            {
                SetProperty(ref _out, value);
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
            }
        }


        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                SetProperty(ref _result, value);
            }
        }

        private int _imageIndex;
        public int ImageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                SetProperty(ref _imageIndex, value);
            }
        }

    }
    public class ManualOCRViewModel : BindableBase
    {
        public ManualOCRViewModel(IUserSettingService _UserSettingService, IEventAggregator ea)
        {
            CreateStepGo();
            // Menu Event Aggregator
            _ea = ea;
            _ea.GetEvent<MenuSentEvent>().Subscribe(MenuExecuted);
            RunMenuCommand = new DelegateCommand<object>(RunMenu);
            // Service
            UserSettingService = _UserSettingService;
            string lastfile = UserSettingService.GetAppSetting("TigerOCR", "LastFile");
            ImagePath = lastfile;
            if (File.Exists(ImagePath))
            {
                GoStep2("init");

            }
            else
            {
                GoStep2("");
            }
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

        private string _stepResult;
        public string StepResult
        {
            get
            {
                return _stepResult;
            }
            set
            {
                SetProperty(ref _stepResult, value);
            }
        }


        private string _stepDesc;
        public string StepDesc
        {
            get
            {
                return _stepDesc;
            }
            set
            {
                SetProperty(ref _stepDesc, value);
            }
        }

        private string _stepText;
        public string StepText
        {
            get
            {
                return _stepText;
            }
            set
            {
                SetProperty(ref _stepText, value);
            }
        }

        private string _stepGo;
        public string StepGo
        {
            get
            {
                return _stepGo;
            }
            set
            {
                SetProperty(ref _stepGo, value);
            }
        }



        private string? _ImagePath;
        public string? ImagePath
        {
            get => _ImagePath;
            set => SetProperty(ref _ImagePath, value);
        }

        private int? _CurrentTab = 0;
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
        private BitmapImage? _StepPDF;


        Dictionary<string, StepGo> Steps = new Dictionary<string, StepGo>();
        void CreateStepGo()
        {
            Steps.Add("", new StepGo(0)
            {
                Title = "N/A",
                Result = "",
                Description = "Waiting Select File",
            });
            Steps.Add("init", new StepGo(0)
            {
                Title = "init",
                Result = "",
                Description = "Select File to process",
            });
            Steps.Add("resize", new StepGo(0)
            {
                Title = "resize",
                Result = "",
                Description = "Resize Image for speed",
            });
            Steps.Add("gray", new StepGo(0)
            {
                Title = "gray",
                Result = "",
                Description = "Make image gray",
            });
            Steps.Add("threadhold", new StepGo(0)
            {
                Title = "Threadhold",
                Result = "",
                Description = "Make image threadhold",
            });
            Steps.Add("contour", new StepGo(0)
            {
                Title = "Contour",
                Result = "",
                Description = "Find target image",
            });
            Steps.Add("transform", new StepGo(0)
            {
                Title = "Transform",
                Result = "",
                Description = "Make target image",
            });
            Steps.Add("ocr", new StepGo(0)
            {
                Title = "OCR image",
                Result = "",
                Description = "Find character",
            });
            Steps.Add("txt", new StepGo(0)
            {
                Title = "Save as txt",
                Result = "",
                Description = "Save result as text",
            });
            Steps.Add("pdf", new StepGo(0)
            {
                Title = "Save to pdf",
                Result = "",
                Description = "Save result as pdf ",
            });
        }

        public BitmapImage? StepPDF
        {
            get => _StepPDF;
            set => SetProperty(ref _StepPDF, value);
        }
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
        private void SetSource(int i, string filename)
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
                    //_Rail1DisplayedImage.DecodePixelWidth = 200;
                    _Rail1DisplayedImage.EndInit();
                    SetSource(i, _Rail1DisplayedImage);

                }
                catch
                {
                }
            }
        }

            public static BitmapImage ToBitmapImage(Bitmap bitmap)
            {
                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }

            private void SetSource(int index, Mat mat)
        {
            BitmapImage bi = Ocr.ToBitmapImage(mat);
            SetSource(index, bi);
        }

        private void SetSource(int index, Bitmap mat)
        {
            BitmapImage bi = ToBitmapImage(mat);
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
            if (bitmap != null)
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
                UserSettingService.SetAppSetting("TigerOCR", "LastFile", filename);
                ImagePath = filename;
                if (File.Exists(ImagePath))
                {
                    GoStep2("init");
                }
            }
        }
        void StepByStepFunc(string imgFile)
        {
            GoStep2(stepIndex);
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
        string stepIndex = "";

        void GoStep2(string indexstring)
        {
            if (Steps.Keys.Contains(indexstring))
            {
                StepGo stepGo = Steps[indexstring];
                StepText = stepGo.Title;
                StepDesc = stepGo.Description;
                switch (indexstring)
                {
                    case "":
                        {
                            StepGo = "N/A";
                            SetSource(0, ImagePath);
                            stepIndex = "";
                            CurrentTab = 0;
                        }
                        break;
                    case "init":
                        if (File.Exists(ImagePath))
                        {
                            SetSource(0, ImagePath);
                            StepResult = stepGo.Result = "File selected";
                            StepGo = "Resize";
                            stepIndex = "resize";
                            Steps["resize"].In = new Mat(ImagePath);
                            CurrentTab = 0;
                        }
                        else
                        {
                            SetSource(0, ImagePath);
                            StepResult = stepGo.Result = "File not found";
                            StepGo = "N/A";
                            stepIndex = "";
                            CurrentTab = 0;
                        }
                        break;
                    case "resize":
                        {
                            StepGo prev = Steps["init"];
                            Mat res = do_resize_mat(400, stepGo.In as Mat);
                            if (res != null)
                            {
                                StepGo = "Gray";
                                stepIndex = "gray";
                                stepGo.Out = res;
                                StepResult = stepGo.Result = "Resizing image success (width 400)";
                                SetSource(1, stepGo.Out as Mat);
                                Steps["gray"].In = res;
                                CurrentTab = 1;
                            }
                            else
                            {
                                StepGo = "OCR";
                                stepIndex = "ocr";
                                StepResult = stepGo.Result = "Fail resize file (Use original file to ocr)";
                                Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }
                        }
                        break;
                    case "gray":
                        {
                            Mat res = do_gray_mat(stepGo.In as Mat);
                            if (res != null)
                            {
                                StepGo = "Threadhold";
                                stepIndex = "threadhold";
                                stepGo.Out = res;
                                StepResult = stepGo.Result = "Graying image success";
                                SetSource(2, stepGo.Out as Mat);
                                Steps["threadhold"].In = res;
                                CurrentTab = 2;
                            }
                            else
                            {
                                StepGo = "OCR";
                                stepIndex = "ocr";
                                StepResult = stepGo.Result = "Fail gray image (Use original file to ocr)";
                                Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }
                        }
                        break;
                    case "threadhold":
                        {
                            Mat res = do_threadhold_mat(stepGo.In as Mat);
                            if (res != null)
                            {
                                StepGo = "Contour";
                                stepIndex = "contour";
                                stepGo.Out = res;
                                StepResult = stepGo.Result = "Threadholding image success";
                                SetSource(3, stepGo.Out as Mat);
                                Steps["contour"].In = res;
                                CurrentTab = 3;
                            }
                            else
                            {
                                StepGo = "OCR";
                                stepIndex = "ocr";
                                StepResult = stepGo.Result = "Fail threadholding image (Use original file to ocr)";
                                Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }
                        }
                        break;
                    case "contour":
                        {
                            Point[]? contour = do_contour(stepGo.In as Mat);
                            Mat drContour = draw_contour(Steps["resize"].Out as Mat, contour);

                            if (contour != null && drContour != null)
                            {
                                StepGo = "Transform";
                                stepIndex = "transform";
                                stepGo.Out = contour;
                                StepResult = stepGo.Result = "Find contour";

                                // Draw rectangle on resized image
                                // From threadhold image find rectangle to ocr, 
                                // Mark rectangle to resized source, to view rectange accuracy
                                SetSource(4, drContour as Mat);

                                // Draw same to the source image
                                double resize_ratio = (double)(Steps["resize"].In as Mat).Width
                                    / (double)(Steps["resize"].Out as Mat).Width;
                                Point[] org_Contour = new Point[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    org_Contour[i].X = (int)(contour[i].X * resize_ratio);
                                    org_Contour[i].Y = (int)(contour[i].Y * resize_ratio);
                                }
                                Mat markedSource = draw_contour(Steps["resize"].In as Mat, org_Contour);
                                SetSource(5, markedSource);

                                Steps["transform"].In = Steps["resize"].In;
                                CurrentTab = 4;
                            }
                            else
                            {
                                StepGo = "OCR";
                                stepIndex = "ocr";
                                StepResult = stepGo.Result = "Fail to find contour (Use full image to ocr)";
                                Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }

                        }
                        break;
                    case "transform":
                        {
                            double resize_ratio = (double)(Steps["resize"].In as Mat).Width
    / (double)(Steps["resize"].Out as Mat).Width;
                            Point[] res_Contour = Steps["contour"].Out as Point[];
                            Point[] org_Contour = new Point[4];
                            for (int i = 0; i < 4; i++)
                            {
                                org_Contour[i].X = (int)(res_Contour[i].X * resize_ratio);
                                org_Contour[i].Y = (int)(res_Contour[i].Y * resize_ratio);
                            }
                            Mat transform = do_transform_mat(stepGo.In as Mat, org_Contour);

                            if (transform != null)
                            {
                                StepGo = "OCR";
                                stepIndex = "ocr";
                                stepGo.Out = transform;
                                StepResult = stepGo.Result = "Cut ocr range";

                                // Draw rectangle on resized image
                                // From threadhold image find rectangle to ocr, 
                                // Mark rectangle to resized source, to view rectange accuracy
                                SetSource(6, transform);
                                Steps["ocr"].In = transform;

                                Mat res_transform = do_transform_mat(Steps["resize"].Out as Mat, res_Contour, 1);
                                SetSource(7, res_transform);

                                CurrentTab = 6;
                            }
                            else
                            {
                                StepGo = "OCR";
                                stepIndex = "ocr";
                                StepResult = stepGo.Result = "Fail to find contour (Use full image to ocr)";
                                Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }

                        }
                        break;
                    case "ocr":
                        {
                            Mat res = do_ocr_mat(stepGo.In as Mat);
                            if (res != null)
                            {
                                StepGo = "OCR text";
                                stepIndex = "txt";
                                stepGo.Out = res;
                                StepResult = stepGo.Result = "Ocr done. Ocr range is marked";
                                SetSource(8, stepGo.Out as Mat);
                                Steps["txt"].In = stepGo.In;
                                CurrentTab = 8;
                            }
                            else
                            {
                                StepGo = "Init";
                                stepIndex = "init";
                                StepResult = stepGo.Result = "OCR FAIL";
                                //Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }
                        }
                        break;
                    case "txt":
                        {
                            string pdf_file_name = ImagePath + ".t.pdf";
                            Mat res = do_ocr_mat_txt(stepGo.In as Mat, pdf_file_name);
                            if (res != null)
                            {
                                StepGo = "Done";
                                stepIndex = "init";
                                stepGo.Out = res;
                                StepResult = stepGo.Result = "Ocr text done. Ocr text is marked";
                                SetSource(9, stepGo.Out as Mat);
                                CurrentTab = 9;

                                using (var doc = new PDFiumSharp.PdfDocument(pdf_file_name))
                                {
                                    var page = doc.Pages.ElementAt(0);
                                    System.Windows.Media.ImageSource n = page.CreateImageSource((int)page.Width, (int)page.Height);

                                    BitmapImage m = n as BitmapImage;
                                    StepPDF = m;
                                }
                                
                            }
                            else
                            {
                                StepGo = "Init";
                                stepIndex = "init";
                                StepResult = stepGo.Result = "OCR FAIL";
                                //Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }
                        }
                        break;
                    case "pdf":
                        {
                            Mat res = do_threadhold_mat(stepGo.In as Mat);
                            if (res != null)
                            {
                                StepGo = "Contour";
                                stepIndex = "contour";
                                stepGo.Out = res;
                                StepResult = stepGo.Result = "Threadholding image success";
                                SetSource(3, stepGo.Out as Mat);
                                Steps["contour"].In = res;
                                CurrentTab = 3;
                            }
                            else
                            {
                                StepGo = "Init";
                                stepIndex = "init";
                                StepResult = stepGo.Result = "OCR FAIL";
                                //Steps["ocr"].In = stepGo.In;
                                CurrentTab = 0;
                            }
                        }
                        break;
                }
            }
        }

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
                    {
                        {
                            ImagePath = "";
                            StepGo = "N/A";
                            StepText = "Init";
                            StepDesc = "Need to Select File";
                            StepResult = "File Not Found";
                        }
                        return _step;
                    }
                    {
                        StepGo = "Resize";
                        StepText = "Init";
                        StepDesc = "File Selected / (next) File Resize";
                        StepResult = "Ready to start";
                    }
                    SetSource(0, start_src);
                    CurrentTab = _step;
                    _step = STEP_RESIZE;
                    break;
                case STEP_RESIZE:
                    step_success = this.do_resize(400);
                    if (step_success)
                    {
                        {
                            StepGo = "ThreadHold";
                            StepText = "File Resize";
                            StepDesc = "Resize Image";
                            StepResult = "Image Resize succeeded";
                        }
                        SetSource(1, mid_resize);
                    }
                    else
                    {
                        StepGo = "ThreadHold";
                        StepText = "File Resize";
                        StepDesc = "Resize Image";
                        StepResult = "Resize fail (Use source for next step)";
                        SetSource(1, start_src);
                    }
                    CurrentTab = _step;
                    _step = STEP_THREADHOLD;
                    break;
                case STEP_THREADHOLD:
                    step_success = this.do_gray();
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

                    step_success = this.do_ocr(final_src);
                    if (step_success)
                    {
                        //SetSource(7, final_src);
                    }
                    CurrentTab = 8;
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

        void DrawRect(object ocr_src, System.Drawing.Rectangle rect)
        {
            Mat imgRect = ocr_src as Mat;
            OpenCvSharp.Rect orect = new OpenCvSharp.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            if (imgRect != null)
            {
                //Cv2.Line(imgRect, new Point(rect.X, rect.Y), new Scalar(255, 0, 0), 10);
                Cv2.Rectangle(imgRect, orect, new Scalar(255, 0, 0), 1);
            }
        }
#if PDFSHARP
        XFont xfont = null;
        void DrawText(object ocr_src, Rectangle rect, string text)
        {
            XGraphics graph = ocr_src as XGraphics;


            if (xfont == null)
            {
                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
                xfont = new XFont("Times New Roman", 12, XFontStyle.Regular, options);
            }
            XTextFormatter tf = new XTextFormatter(graph);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(text, xfont, XBrushes.Black,
                new XRect(rect.X, rect.Y,rect.Width, rect.Height), XStringFormats.TopLeft);
            }
            
#endif

#if ITEXTSHARP
        string hcrBatang = "./resources/fonts/H2GTRM.ttf";
        //PdfFont fontUnicode =
        //    PdfFontFactory.CreateFont(hcrBatang, PdfEncodings.IDENTITY_H, true);
        PdfFont fontUnicode = null;
        float PdfWidth = 0;
        float PdfHeight = 0;
        float ImageLeft = 0;
        float ImageTop = 0;
        float ImageRatio = 1;

        void DrawText(object ocr_src, System.Drawing.RectangleF rect, string text)
        {
            rect.X *= ImageRatio;
            rect.Y *= ImageRatio;
            rect.Width *= ImageRatio;
            rect.Height *= ImageRatio;
            Tuple<Document, PdfCanvas> tt = ocr_src as Tuple<Document, PdfCanvas>;
            if (tt != null && !string.IsNullOrWhiteSpace(text))
            {
                //text = "공";
                Document document = tt.Item1 as Document;
                PdfCanvas canvas = tt.Item2 as PdfCanvas;
                rect.Y = PdfHeight - rect.Y - ImageTop;
                rect.X = rect.X + ImageLeft;

                canvas.SaveState();
                canvas.SetFillColor(iText.Kernel.Colors.ColorConstants.YELLOW);
                canvas.SetExtGState(new iText.Kernel.Pdf.Extgstate.PdfExtGState().SetFillOpacity(1f));
                canvas.Rectangle(rect.X, rect.Y - rect.Height, rect.Width, rect.Height);
                canvas.Fill();
                canvas.RestoreState();

                document.SetFontSize(8);
                document.SetFont(fontUnicode).
                    ShowTextAligned(text, rect.X, rect.Y, TextAlignment.LEFT,
                    VerticalAlignment.TOP, 0);
            }
        }
#endif
        private bool do_ocr(Mat crop_src)
        {
            //if(File.Exists(filename))
            {
                Mat gray = crop_src.Clone();
                Mat imgRect = gray.Clone();
                Cv2.CvtColor(imgRect, gray, ColorConversionCodes.BGR2GRAY);
                SetSource(7, imgRect);
                Mat to_ocr_image = imgRect.Clone();
                Mat to_ocr_txt_image = imgRect.Clone();

                Ocr.SetImageTarget(to_ocr_image);
                Ocr.SetDrawDelegation(DrawRect);




                Ocr.ocr_image(to_ocr_image);
                SetSource(8, to_ocr_image);
#if PDFSHARP

                PdfDocument doc = new PdfDocument();

                PdfPage page = doc.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);
                double width = page.Width.Point;
                double height = page.Height.Point;
                XImage image = XImage.FromFile(ImagePath);
                double ratio = 72 / image.HorizontalResolution;
                double x = (width - image.PixelWidth * ratio) / 2;
                gfx.DrawImage(image, x, 0);


                Ocr.SetResultTarget(gfx);
                Ocr.SetTextDelegation(DrawText);
                Ocr.ocr_image_txt(imgRect2);
                doc.Save(ImagePath + ".pdf");
#endif
#if ITEXTSHARP

                //PdfFont fontUnicode =
                //    PdfFontFactory.CreateFont(hcrBatang, PdfEncodings.IDENTITY_H, true);
                //if (fontUnicode == null)
                //{
                fontUnicode =
    PdfFontFactory.CreateFont(hcrBatang, PdfEncodings.IDENTITY_H);
                //}
                PdfWriter writer = new PdfWriter(ImagePath + ".t.pdf");

                // Must have write permissions to the path folder
                iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                Document document = new Document(pdf);
                pdf.AddNewPage();
                var page = pdf.GetPage(1);
                PdfCanvas canvas = new PdfCanvas(page);
                PdfWidth = page.GetPageSizeWithRotation().GetWidth();
                PdfHeight = page.GetPageSizeWithRotation().GetHeight();

                Bitmap bmp = BitmapConverter.ToBitmap(to_ocr_txt_image);
                bmp.Save("tmp.xxx.png", System.Drawing.Imaging.ImageFormat.Png);
                Image img = new Image(ImageDataFactory
                   .Create("tmp.xxx.png"));
                float img_width = img.GetImageWidth();
                float img_height = img.GetImageHeight();
                float ratio = 1;
                if (img_width > PdfWidth)
                {
                    ratio = PdfWidth / img_width;
                    float tmp_height = img_height * ratio;
                    if (tmp_height > PdfHeight)
                    {
                        ratio = PdfHeight / img_height;
                        img_width *= ratio;
                        img_height = PdfHeight;
                        ImageLeft = (PdfWidth - img_width) / 2;
                        ImageTop = 0;
                    }
                    else
                    {
                        img_width = PdfWidth;
                        img_height *= ratio;
                        ImageTop = (PdfHeight - img_height) / 2;
                        ImageLeft = 0;
                    }
                }
                else if (img_height > PdfHeight)
                {
                    ratio = img_height / PdfHeight;
                    float tmp_width = img_width * ratio;
                    if (tmp_width > PdfWidth)
                    {
                        ratio = PdfWidth / img_width;
                        img_height *= ratio;
                        img_width = PdfWidth;
                        ImageTop = (PdfHeight - img_height) / 2;
                        ImageLeft = 0;
                    }
                    else
                    {
                        img_height = PdfHeight;
                        img_width *= ratio;
                        ImageLeft = (PdfWidth - img_width) / 2;
                        ImageTop = 0;
                    }
                }
                ImageRatio = ratio;

                img.SetWidth(img_width)
                .SetHeight(img_height)
                .SetTextAlignment(TextAlignment.CENTER).SetFixedPosition(ImageLeft, ImageTop);
                document.Add(img);

                //writer.getDirect
                Ocr.SetResultTarget(new Tuple<Document, PdfCanvas>(document, canvas));
                Ocr.SetTextDelegation(DrawText);
                Ocr.ocr_image_txt(to_ocr_txt_image);

                // Close document
                document.Close();

#endif
                SetSource(9, to_ocr_txt_image);
            }
            return false;
        }


        private Mat do_ocr_mat(Mat crop_src)
        {
            Mat to_ocr_image = crop_src.Clone();

            Ocr.SetImageTarget(to_ocr_image);
            Ocr.SetDrawDelegation(DrawRect);
            Ocr.ocr_image(to_ocr_image);
            return to_ocr_image;
        }
        private Mat do_ocr_mat_txt(Mat crop_src, string out_pdf_filename)
        {
            Mat to_ocr_txt_image = crop_src.Clone();

#if ITEXTSHARP

            fontUnicode =
    PdfFontFactory.CreateFont(hcrBatang, PdfEncodings.IDENTITY_H);
            PdfWriter writer = new PdfWriter(out_pdf_filename);

            // Must have write permissions to the path folder
            iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer);
            Document document = new Document(pdf);
            pdf.AddNewPage();
            var page = pdf.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page);
            PdfWidth = page.GetPageSizeWithRotation().GetWidth();
            PdfHeight = page.GetPageSizeWithRotation().GetHeight();

            Bitmap bmp = BitmapConverter.ToBitmap(to_ocr_txt_image);
            bmp.Save("tmp.xxx.png", System.Drawing.Imaging.ImageFormat.Png);
            Image img = new Image(ImageDataFactory
               .Create("tmp.xxx.png"));
            float img_width = img.GetImageWidth();
            float img_height = img.GetImageHeight();
            float ratio = 1;
            if (img_width > PdfWidth)
            {
                ratio = PdfWidth / img_width;
                float tmp_height = img_height * ratio;
                if (tmp_height > PdfHeight)
                {
                    ratio = PdfHeight / img_height;
                    img_width *= ratio;
                    img_height = PdfHeight;
                    ImageLeft = (PdfWidth - img_width) / 2;
                    ImageTop = 0;
                }
                else
                {
                    img_width = PdfWidth;
                    img_height *= ratio;
                    ImageTop = (PdfHeight - img_height) / 2;
                    ImageLeft = 0;
                }
            }
            else if (img_height > PdfHeight)
            {
                ratio = img_height / PdfHeight;
                float tmp_width = img_width * ratio;
                if (tmp_width > PdfWidth)
                {
                    ratio = PdfWidth / img_width;
                    img_height *= ratio;
                    img_width = PdfWidth;
                    ImageTop = (PdfHeight - img_height) / 2;
                    ImageLeft = 0;
                }
                else
                {
                    img_height = PdfHeight;
                    img_width *= ratio;
                    ImageLeft = (PdfWidth - img_width) / 2;
                    ImageTop = 0;
                }
            }
            ImageRatio = ratio;

            img.SetWidth(img_width)
            .SetHeight(img_height)
            .SetTextAlignment(TextAlignment.CENTER).SetFixedPosition(ImageLeft, ImageTop);
            document.Add(img);

            //writer.getDirect
            Ocr.SetResultTarget(new Tuple<Document, PdfCanvas>(document, canvas));
            Ocr.SetTextDelegation(DrawText);
            Ocr.ocr_image_txt(to_ocr_txt_image);

            // Close document
            document.Close();

#endif
            return to_ocr_txt_image;
        }
        Scalar blue_color = new Scalar(255, 0, 0);
        private Mat draw_contour(Mat src, Point[]? contour, int b = 255, int g = 0, int r = 0, int thick = 10)
        {
            if (src == null || contour == null || contour.Length != 4)
            {
                return null;
            }
            Mat imgRect = src.Clone();
            Cv2.Line(imgRect, contour[0], contour[1], new Scalar(b, g, r), thick);
            Cv2.Line(imgRect, contour[1], contour[2], new Scalar(b, g, r), thick);
            Cv2.Line(imgRect, contour[2], contour[3], new Scalar(b, g, r), thick);
            Cv2.Line(imgRect, contour[3], contour[0], new Scalar(b, g, r), thick);

            return imgRect;
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

        private Point[]? do_contour(Mat do_src)
        {
            //Point[]? mid_Contour = null;
            //{
            // 윤곽선을 찾는다
            //Point[][] contours1 = new Point[1][];
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
                //do_Contour_info.Add(new StepInfo(InfoType.Error, "CANNOT FIND COUNTOUR"));
                return null;
            }
            if (maxarea < 400)
            {
                //do_Contour_info.Add(new StepInfo(InfoType.Error, "COUNTOUR TOO SMALL"));
                return null;
            }
            // 윤곽선을 근사화한다.
            double areaArc = Cv2.ArcLength(big, true);
            Point[] tobR = Cv2.ApproxPolyDP(big, 0.02 * areaArc, true);
            if (tobR.Length != 4)
            {
                //do_Contour_info.Add(new StepInfo(InfoType.Error, "RECTANGLE NOT FOUND"));
                return null;
            }
            if (!Cv2.IsContourConvex(tobR))
            {
                //do_Contour_info.Add(new StepInfo(InfoType.Error, "NOT A CONVEX"));
                return null;
            }
            return tobR;// mid_Contour = Ocr.GetNullable(tobR);
            //}
            //return mid_Contour;
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

        private Mat do_transform_mat(Mat mid_resize, Point[] _do_rect, double resize_ratio = 1)
        {
            if (mid_resize == null || _do_rect == null || _do_rect.Length != 4)
                return null;
            Point?[] do_rect = new Point?[4];
            for (int i = 0; i < 4; i++)
            {
                do_rect[i] = _do_rect[i];
            }
            // x 순으로 정렬
            Array.Sort<Point?>(do_rect, Ocr.CompareByX);
            int len = do_rect.Length;
            // 시계방향 정렬
            if (do_rect[0].Value.Y > do_rect[1].Value.Y)
            {
                var tmp = do_rect[0];
                do_rect[0] = do_rect[1];
                do_rect[1] = tmp;
            }
            if (do_rect[2].Value.Y < do_rect[3].Value.Y)
            {
                var tmp = do_rect[2];
                do_rect[2] = do_rect[3];
                do_rect[3] = tmp;
            }

            OpenCvSharp.Size maxSize = Ocr.calcMaxWidthHeight(do_rect);
            var dw = maxSize.Width;
            var dh = dw * maxSize.Height / maxSize.Width;
            
            Point2f[] srcQuard = new Point2f[4];
            srcQuard[0].X = (int)(do_rect[0].Value.X * resize_ratio);//좌상
            srcQuard[0].Y = (int)(do_rect[0].Value.Y * resize_ratio);
            srcQuard[1].X = (int)(do_rect[1].Value.X * resize_ratio);// 좌하
            srcQuard[1].Y = (int)(do_rect[1].Value.Y * resize_ratio);
            srcQuard[2].X = (int)(do_rect[2].Value.X * resize_ratio);//우하
            srcQuard[2].Y = (int)(do_rect[2].Value.Y * resize_ratio);
            srcQuard[3].X = (int)(do_rect[3].Value.X * resize_ratio);// 우상
            srcQuard[3].Y = (int)(do_rect[3].Value.Y * resize_ratio);

            Point2f[] dsQuard = new Point2f[4];
            dsQuard[0].X = 0;
            dsQuard[0].Y = 0;
            dsQuard[1].X = 0;
            dsQuard[1].Y = (int)(dh * resize_ratio);
            dsQuard[2].X = (int)(dw * resize_ratio);
            dsQuard[2].Y = (int)(dh * resize_ratio);
            dsQuard[3].X = (int)(dw * resize_ratio);
            dsQuard[3].Y = 0;


            //  투시변환 매트릭스 구하기
            Mat re = Cv2.GetPerspectiveTransform(srcQuard, dsQuard);
            Mat dst = mid_resize.Clone();
            Cv2.WarpPerspective(mid_resize, dst, re, new OpenCvSharp.Size((dw * resize_ratio), (dh * resize_ratio)));
            return dst;
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

        private Mat do_resize_mat(int width, Mat do_src)
        {
            resize_width = width;
            OpenCvSharp.Size newsize = new OpenCvSharp.Size();
            resize_ratio = (double)do_src.Width / (double)resize_width;
            int new_height = (int)(do_src.Height / resize_ratio);

            newsize.Width = resize_width;
            newsize.Height = new_height;
            Mat tmp = do_src.Clone();
            Cv2.Resize(do_src, tmp, newsize);
            return tmp;
        }

        private bool do_gray(Mat src = null)
        {
            do_thread_info = new List<StepInfo>();
            Mat do_src = src;
            if (src == null)
                do_src = mid_resize;

            //try
            //{
            // 흑백영상으로 변환
            Mat gray = do_src.Clone();
            Cv2.CvtColor(do_src, gray, ColorConversionCodes.BGR2GRAY);
            if (gray == null)
                return false;
            return true;
        }

        private Mat do_gray_mat(Mat do_src)
        {
            Mat gray = do_src.Clone();
            Cv2.CvtColor(do_src, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        private bool do_threadhold(Mat gray = null)
        {
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

        private Mat do_threadhold_mat(Mat gray)
        {
            // 이진화
            Mat binSrc = gray.Clone();
            Cv2.Threshold(gray, binSrc, 0.0, 255.0, ThresholdTypes.Otsu);
            return binSrc;
        }
        #endregion Step By Step

    }
}
