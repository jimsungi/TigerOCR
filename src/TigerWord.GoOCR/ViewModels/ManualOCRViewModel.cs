using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using OpenCvSharp;
using Point = OpenCvSharp.Point;


using TigerWord.Core.Services;
using Ocr = com.tigerword.tigeracter.OcrService;
using TigerWord.Core.ViewModels;

using System.Linq;
using PDFiumSharp;
using TigerWord.GoOCR.Biz;
using System.Windows.Data;


#pragma warning disable CS8604 // 가능한 null 참조 인수입니다.
namespace TigerWord.GoOCR.ViewModels
{

    /// <summary>
    /// 
    /// </summary>
    public class OcrLanguage : BindableBase
    {
        private string _language;
        /// <summary>
        /// 
        /// </summary>
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                SetProperty(ref _language, value);
            }
        }

        private string _code;
        /// <summary>
        /// 
        /// </summary>
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                SetProperty(ref _code, value);
            }
        }

        private string _description;
        /// <summary>
        /// 
        /// </summary>
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
    }

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
    /// <summary>
    /// 
    /// </summary>
    public class ManualOCRViewModel : BindableBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_UserSettingService"></param>
        /// <param name="ea"></param>
        public ManualOCRViewModel(IUserSettingService _UserSettingService, IEventAggregator ea)
        {
            CreateStepGo();
            Languages = CreateLanguages();
            SelectedLanguage = "kor+eng";
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
                GoStep("init");

            }
            else
            {
                GoStep("");
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
        #region Supported Language

        List<OcrLanguage> CreateLanguages()
        {
            List<OcrLanguage> ret = new List<OcrLanguage>();
            ret.Add(new OcrLanguage()
            {
                Code = "kor+eng",
                Description = "한영(기본)",
                Language = "한글"
            });
            ret.Add(new OcrLanguage()
            {
                Code = "eng",
                Description = "영어",
                Language = "English"
            });
            ret.Add(new OcrLanguage()
            {
                Code = "kor+eng",
                Description = "한글",
                Language = "한글"
            });

            return ret;
        }

        #endregion
        #region Step Control
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
        #endregion

        #region binding property
        private List<OcrLanguage> _languages;
        /// <summary>
        /// 
        /// </summary>
        public List<OcrLanguage> Languages
        {
            get
            {
                return _languages;
            }
            set
            {
                SetProperty(ref _languages, value);
            }
        }

        private string _selectedLanguage;
        /// <summary>
        /// 
        /// </summary>
        public string SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }
            set
            {
                SetProperty(ref _selectedLanguage, value);
            }
        }

        private bool _canStep = true;
        /// <summary>
        /// 
        /// </summary>
        public bool CanStep
        {
            get
            {
                return _canStep;
            }
            set
            {
                SetProperty(ref _canStep, value);
            }
        }


        private string _stepResult;
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public string? ImagePath
        {
            get => _ImagePath;
            set => SetProperty(ref _ImagePath, value);
        }

        private int? _CurrentTab = 0;
        /// <summary>
        /// 
        /// </summary>
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



        private string _step0 = "source";
        /// <summary>
        /// 
        /// </summary>
        public string Step0
        {
            get
            {
                return _step0;
            }
            set
            {
                SetProperty(ref _step0, value);
            }
        }


        private string _step1 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step1
        {
            get
            {
                return _step1;
            }
            set
            {
                SetProperty(ref _step1, value);
            }
        }


        private string _step2 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step2
        {
            get
            {
                return _step2;
            }
            set
            {
                SetProperty(ref _step2, value);
            }
        }


        private string _step3 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step3
        {
            get
            {
                return _step3;
            }
            set
            {
                SetProperty(ref _step3, value);
            }
        }

        private string _step4 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step4
        {
            get
            {
                return _step4;
            }
            set
            {
                SetProperty(ref _step4, value);
            }
        }


        private string _step5 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step5
        {
            get
            {
                return _step5;
            }
            set
            {
                SetProperty(ref _step5, value);
            }
        }


        private string _step6 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step6
        {
            get
            {
                return _step6;
            }
            set
            {
                SetProperty(ref _step6, value);
            }
        }

        private string _step7 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step7
        {
            get
            {
                return _step7;
            }
            set
            {
                SetProperty(ref _step7, value);
            }
        }


        private string _step8 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step8
        {
            get
            {
                return _step8;
            }
            set
            {
                SetProperty(ref _step8, value);
            }
        }

        private string _step9 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step9
        {
            get
            {
                return _step9;
            }
            set
            {
                SetProperty(ref _step9, value);
            }
        }


        private string _step10 = "-";
        /// <summary>
        /// 
        /// </summary>
        public string Step10
        {
            get
            {
                return _step10;
            }
            set
            {
                SetProperty(ref _step10, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepPDF
        {
            get => _StepPDF;
            set => SetProperty(ref _StepPDF, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap0
        {
            get => _StepBitmap0;
            set => SetProperty(ref _StepBitmap0, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap1
        {
            get => _StepBitmap1;
            set => SetProperty(ref _StepBitmap1, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap2
        {
            get => _StepBitmap2;
            set => SetProperty(ref _StepBitmap2, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap3
        {
            get => _StepBitmap3;
            set => SetProperty(ref _StepBitmap3, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap4
        {
            get => _StepBitmap4;
            set => SetProperty(ref _StepBitmap4, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap5
        {
            get => _StepBitmap5;
            set => SetProperty(ref _StepBitmap5, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap6
        {
            get => _StepBitmap6;
            set => SetProperty(ref _StepBitmap6, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap7
        {
            get => _StepBitmap7;
            set => SetProperty(ref _StepBitmap7, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public BitmapImage? StepBitmap8
        {
            get => _StepBitmap8;
            set => SetProperty(ref _StepBitmap8, value);
        }
        /// <summary>
        /// 
        /// </summary>
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



        private void SetSource(int index, Mat mat)
        {
            BitmapImage bi = Ocr.ToBitmapImage(mat);
            SetSource(index, bi);
        }

        private void SetSource(int index, Bitmap mat)
        {
            BitmapImage bi = OcrUtil.ToBitmapImage(mat);
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
        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand<string> SelectImageDeleageCmd =>
            _SelectImageCmd ?? (_SelectImageCmd = new DelegateCommand<string>(SelectImageFunc));
        /// <summary>
        /// 
        /// </summary>
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
                    GoStep("init");
                }
            }
        }
        void StepByStepFunc(string imgFile)
        {
            GoStep(stepIndex);
        }
        #endregion
        #region Step By Step

        string stepIndex = "";

        void GoStep(string indexstring)
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
                            Mat res = OcrUtil.do_resize_mat(400, stepGo.In as Mat);
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
                            Mat res = OcrUtil.do_gray_mat(stepGo.In as Mat);
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
                            Mat res = OcrUtil.do_threadhold_mat(stepGo.In as Mat);
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
                            Point[]? contour = OcrUtil.do_contour(stepGo.In as Mat);
                            Mat drContour = OcrUtil.draw_contour(Steps["resize"].Out as Mat, contour);

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
                                Mat markedSource = OcrUtil.draw_contour(Steps["resize"].In as Mat, org_Contour);
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
                            Mat transform = OcrUtil.do_transform_mat(stepGo.In as Mat, org_Contour);

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

                                Mat res_transform = OcrUtil.do_transform_mat(Steps["resize"].Out as Mat, res_Contour, 1);
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
                            Mat res = OcrUtil.do_ocr_mat(stepGo.In as Mat, SelectedLanguage);
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
                            Mat res = OcrUtil.do_ocr_mat_txt(stepGo.In as Mat, pdf_file_name, SelectedLanguage);
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
                            Mat res = OcrUtil.do_threadhold_mat(stepGo.In as Mat);
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


        #endregion
    }


}
