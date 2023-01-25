using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using com.tigerword.ocr;
using System.Diagnostics;
using Point = OpenCvSharp.Point;
using System.Security.Cryptography.X509Certificates;
using Tesseract;
using Rectangle = System.Drawing.Rectangle;

namespace com.tigerword.tigeracter
{
    public class OcrService
    {
        private static OcrService Instance = new OcrService();
        static void Go()
        {
            if (Instance == null)
                Instance = new OcrService();
        }
        public static BitmapImage ToBitmapImage(Mat mat)
        {
            Go();
            {
                return Instance._ToBitmapImage(mat);
            }
        }

        private BitmapImage _ToBitmapImage(Mat mat)
        {
            Go();
            {
                Mat cv8 = mat.Clone();
                mat.ConvertTo(cv8, MatType.CV_8U);
                Mat ret = cv8.Clone();

                Bitmap b = BitmapConverter.ToBitmap(mat);
                BitmapImage bi = _Bitmap2BitmapImage(b);
                return bi;
            }
        }

        public static BitmapImage Bitmap2BitmapImage(Bitmap inputBitmap)
        {
            Go();
            {
                return Instance._Bitmap2BitmapImage(inputBitmap);
            }
        }
        private BitmapImage _Bitmap2BitmapImage(Bitmap inputBitmap)
        {
            using (var memory = new MemoryStream())
            {
                inputBitmap.Save((Stream)memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        public static BitmapImage GetBitmapFromFileName(string filename)
        {
            Go();
            {
                return Instance._GetBitmapFromFileName(filename);
            }
        }

        public static void ocr_image(string filename)
        {
            Go();
            {
                Instance._ocr_image(filename);
            }
        }

        public DrawRect drawDelegation = null;

        public delegate void DrawRect(Rectangle rect);

        public static void SetDrawDelegation(DrawRect _drawer)
        {
            Go();
            {
                Instance.drawDelegation = _drawer;
            }
        }

        public void _ocr_image(string filename)
        {
            var testImagePath = filename;

            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "kor+eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                            Console.WriteLine("Text (GetText): \r\n{0}", text);
                            Console.WriteLine("Text (iterator):");
                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();

                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                do
                                                {
                                                    
                                                    Debug.WriteLine(iter.GetText(PageIteratorLevel.Symbol));
                                                    Tesseract.Rect rect
                                                        = new Tesseract.Rect();
                                                    
                                                    if(iter.TryGetBoundingBox(PageIteratorLevel.Symbol,out rect )==true)
                                                    {
                                                        if(drawDelegation!=null)
                                                        {
                                                            Rectangle re = new Rectangle();
                                                            re.X = rect.X1;
                                                            re.Width = rect.Width;
                                                            re.Y = rect.Y1;
                                                            re.Height = rect.Height;

                                                            drawDelegation(re);
                                                        }
                                                    }
                                                } while (iter.Next(PageIteratorLevel.Symbol, PageIteratorLevel.Symbol));
                                                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                                {
                                                    Console.WriteLine("<BLOCK>");
                                                }
                                                Console.Write(iter.GetText(PageIteratorLevel.Word));
                                                Console.Write(" ");

                                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                                {
                                                    Console.WriteLine();
                                                }
                                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                            {
                                                Console.WriteLine();
                                            }
                                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                } while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }
        }
        private BitmapImage _GetBitmapFromFileName(string filename)
        {
            if (File.Exists(filename))
            {
                BitmapImage _Rail1DisplayedImage = new BitmapImage();
                _Rail1DisplayedImage.BeginInit();
                _Rail1DisplayedImage.CacheOption = BitmapCacheOption.OnLoad;
                _Rail1DisplayedImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                _Rail1DisplayedImage.UriSource = new Uri(filename);
                _Rail1DisplayedImage.DecodePixelWidth = 200;
                _Rail1DisplayedImage.EndInit();
                return _Rail1DisplayedImage;
            }
            return null;
        }

        private Mat HarrisCorner(Mat src)
        {
            Mat corner = src.Clone();
            src.CopyTo(corner);
            //Cv2.ImShow("Corner", corner);
            Mat cv8 = corner.Clone();

            corner.ConvertTo(cv8, MatType.CV_8U);
            //Cv2.ImShow("cv8", cv8);

            Mat ret = cv8.Clone();
            //src.Clone();
            //Mat mask = src.Clone();
            //src.CopyTo(corner);

            Cv2.CornerHarris(cv8, ret, 2, 3, 0.05, BorderTypes.Default);
            return ret;
            //Cv.GoodFeaturesToTrack(gray, eigImg, tempImg, out corners, ref cornerCount, 0.01, 5);

            //Cv.FindCornerSubPix(gray, corners, cornerCount, new CvSize(3, 3), new CvSize(-1, -1), new CvTermCriteria(20, 0.03));

            //for (int i = 0; i < cornerCount; i++)
            //{
            //    Cv.Circle(corner, corners[i], 3, CvColor.Black, 2);
            //}
        }

        public static Mat Corner(Mat src)
        {
            Go();
            {
                return Instance._Corner(src);
            }
        }
        private Mat _Corner(Mat src)
        {
            Mat corner = src.Clone();
            Mat mask = src.Clone();
            src.CopyTo(corner);

            Point2f[] corners;
            int maxCorner = 1000;
            corners = Cv2.GoodFeaturesToTrack(src, maxCorner, 0.01, 10, null, 3, false, 0.04);
            bool found = Cv2.Find4QuadCornerSubpix(src, corners, new OpenCvSharp.Size(3, 3));
            if (found)
            {
                for (int i = 0; i < corners.Length; i++)
                {
                    Point c = new Point();
                    c.X = (int)corners[i].X;
                    c.Y = (int)corners[i].Y;
                    Cv2.Circle(corner, c, 5, new Scalar(0, 255, 0));
                }
            }
            return corner;
            //Cv.GoodFeaturesToTrack(gray, eigImg, tempImg, out corners, ref cornerCount, 0.01, 5);

            //Cv.FindCornerSubPix(gray, corners, cornerCount, new CvSize(3, 3), new CvSize(-1, -1), new CvTermCriteria(20, 0.03));

            //for (int i = 0; i < cornerCount; i++)
            //{
            //    Cv.Circle(corner, corners[i], 3, CvColor.Black, 2);
            //}
        }

        public static Mat GrayScale(Mat src)
        {
            Go();
            {
                return Instance._GrayScale(src);
            }
        }
        private Mat _GrayScale(Mat src)
        {
            Mat dst = src.Clone();
            Cv2.CvtColor(src, dst, ColorConversionCodes.BGR2GRAY);
            //Cv2.Threshold(dst, dst, 50, 255, ThresholdTypes.Binary);
            return dst;

        }
        private Mat BinaryThreshold(Mat src)
        {
            Go();
            {
                return Instance._BinaryThreshold(src);
            }
        }

        private Mat _BinaryThreshold(Mat src)
        {
            Mat dst = src.Clone();
            Cv2.Threshold(src, dst, 100, 255, ThresholdTypes.Binary);
            return dst;

        }

        public static Mat CannyEdge(Mat src)
        {
            Go();
            {
                return Instance._CannyEdge(src);
            }
        }
        private Mat _CannyEdge(Mat src)
        {
            Mat ret = src.Clone();
            Cv2.Canny(src, ret, 0, 100);
            return ret;
        }

        public static Mat SobelEdge(Mat src)
        {
            Go();
            {
                return Instance._SobelEdge(src);
            }
        }
        private Mat _SobelEdge(Mat src)
        {
            Mat ret = src.Clone();
            Cv2.Sobel(src, ret, MatType.CV_8U, 1, 0, 3, 1, 0, BorderTypes.Default);
            return ret;
        }
        public static Mat LaplaceEdge(Mat src)
        {
            Go();
            {
                return Instance._LaplaceEdge(src);
            }
        }
        private Mat _LaplaceEdge(Mat src)
        {
            Mat ret = src.Clone();
            Cv2.Laplacian(src, ret, MatType.CV_8U, ksize: 3, scale: 1, delta: 0, borderType: BorderTypes.Default);
            return ret;
        }

        public static OpenCvSharp.Size calcMaxWidthHeight(Point?[] points)
        {
            Go();
            {
                return Instance._calcMaxWidthHeight(points);
            }
        }
        private OpenCvSharp.Size _calcMaxWidthHeight(Point?[] points)
        {
            return calcMaxWidthHeight(points[0].Value, points[3].Value, points[1].Value, points[2].Value);
        }

        public static OpenCvSharp.Size calcMaxWidthHeight(Point tl, Point tr, Point br, Point bl)
        {
            Go();
            {
                return Instance._calcMaxWidthHeight(tl,tr,br,bl);
            }
        }
        private OpenCvSharp.Size _calcMaxWidthHeight(Point tl, Point tr, Point br, Point bl)
        {
            // Calculate width
            var widthA = Math.Sqrt((tl.X - tr.X) * (tl.X - tr.X) + (tl.Y - tr.Y) * (tl.Y - tr.Y));
            var widthB = Math.Sqrt((bl.X - br.X) * (bl.X - br.X) + (bl.Y - br.Y) * (bl.Y - br.Y));
            var maxWidth = Math.Max(widthA, widthB);
            // Calculate height
            var heightA = Math.Sqrt((tl.X - bl.X) * (tl.X - bl.X) + (tl.Y - bl.Y) * (tl.Y - bl.Y));
            var heightB = Math.Sqrt((tr.X - br.X) * (tr.X - br.X) + (tr.Y - br.Y) * (tr.Y - br.Y));
            var maxHeight = Math.Max(heightA, heightB);
            return new OpenCvSharp.Size(maxWidth, maxHeight);
        }

        public static int CompareByX(Point? x, Point? y)
        {
            Go();
            {
                return Instance._CompareByX(x,y);
            }
        }
        private int _CompareByX(Point? x, Point? y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                {
                    if (x.Value.X == y.Value.X)
                        return 0;
                    else if (x.Value.X > y.Value.X)
                        return 1;
                    else if (x.Value.X < y.Value.X)
                        return -1;
                }
            }
            return 0;
        }

        //private void FindRectangles(Image<Gray, Byte> blackAndWhiteImage)
        //{
        //    m_FoundRectangles.Clear();

        //    using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
        //    {
        //        for (Contour<Point> contours = blackAndWhiteImage.FindContours(
        //            Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
        //            Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST,
        //            storage);
        //            contours != null;
        //            contours = contours.HNext)
        //        {
        //            Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);
        //            Debug.WriteLine(currentContour.Area);

        //            if (currentContour.Area > 250) //only consider contours with area greater than 250
        //            {
        //                if (currentContour.Total == 4) //The contour has 4 vertices.
        //                {
        //                    if (IsRectangle(currentContour))
        //                    {
        //                        m_FoundRectangles.Add(currentContour.GetMinAreaRect());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        public void test(string filename)
        {
            var testImagePath = filename;

            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "kor+eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                            Console.WriteLine("Text (GetText): \r\n{0}", text);
                            Console.WriteLine("Text (iterator):");
                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();

                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                                {
                                                    Console.WriteLine("<BLOCK>");
                                                }

                                                Console.Write(iter.GetText(PageIteratorLevel.Word));
                                                Console.Write(" ");

                                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                                {
                                                    Console.WriteLine();
                                                }
                                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                            {
                                                Console.WriteLine();
                                            }
                                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                } while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }
        }



        private void make_scan_image(Mat src, int width, OpenCvSharp.Size ksize, int min_threshold = 75, int max_threshold = 200)
        {
            Mat tmp = null;
            OpenCvSharp.Size newsize = new OpenCvSharp.Size();
            double ratio = src.Width / width;
            int new_height = (int)(src.Height / ratio);

            newsize.Width = width;
            newsize.Height = new_height;
            tmp = src.Clone();
            Cv2.Resize(src, tmp, newsize);

            //Cv2.ImShow("Resize",tmp);
            Mat gray = tmp.Clone();
            Mat blurred = tmp.Clone();
            Mat edged = tmp.Clone();

            Cv2.CvtColor(tmp, gray, ColorConversionCodes.BGR2GRAY);
            //Cv2.ImShow("gray", gray);

            //OpenCvSharp.Size newsize = new OpenCvSharp.Size();
            Cv2.GaussianBlur(gray, blurred, ksize, 0);
            //Cv2.ImShow("blurred", blurred);

            Cv2.Canny(blurred, edged, min_threshold, max_threshold);
            //Cv2.ImShow("edged", edged);


        }
        private void ocr(Mat src)
        {
            //Cv2.ImShow("소스", src);

            OpenCvSharp.Size newsize = new OpenCvSharp.Size(5, 5);
            make_scan_image(src, 400, newsize, 127, 255);

        }

        private void ShowImage(System.Windows.Controls.Image imgctrl, Mat mat)
        {
            Mat cv8 = mat.Clone();
            mat.ConvertTo(cv8, MatType.CV_8U);
            Mat ret = cv8.Clone();

            Bitmap b = BitmapConverter.ToBitmap(mat);
            BitmapImage bi = _Bitmap2BitmapImage(b);
            imgctrl.Source = bi;
        }
        private void FindCorner(Mat src)
        {
            //Mat dst = null;
            //// 이진화 수행
            //dst = GrayScale(src);
            //ShowImage(step1, dst);
            //MessageBox.Show("binary");

            //// 캐니 에지
            //dst = CannyEdge(src);
            //SetSource(imgProcess, dst);
            //MessageBox.Show("CannyEdge");


            //dst = SobelEdge(src);
            //SetSource(imgProcess, dst);
            //MessageBox.Show("SobelEdge");

            //dst = LaplaceEdge(src);
            //SetSource(imgProcess, dst);
            //MessageBox.Show("LaplaceEdge");

            // 코너 검출

            //dst = Corner(dst);
            //SetSource(imgProcess, dst);

            //dst = HarrisCorner(dst);
            //ShowImage(step2, dst);
        }
        public static  Point?[] GetNullable(Point[] src)
        {
            Go();
            {
                return Instance._GetNullable(src);
            }
        }
        private Point?[] _GetNullable(Point[] src)
        {
            if (src == null)
                return null;
            int len = src.Length;
            Point?[] tar = new Point?[len];
            for (int i = 0; i < len; i++)
            {
                tar[i] = src[i];
            }
            return tar;
        }
    }
}
