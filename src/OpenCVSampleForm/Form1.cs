using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Extensions;

using System.Threading;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace OpenCVSampleForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //do_image();
            ////do_video();
            //do_colorspace();
            //do_mirror();
            //do_pyramid();
            //do_resize();
            //do_cut();
            //do_bidirect();
            //do_image_calc_flamingo();
            //do_image_calc_elephant();
            //do_image_blur();
            //do_image_edge();
            //do_image_contour();
            //do_image_poly();
            //image_ct_1();
            //image_ct_2();
            //do_image_rotation();
            //do_affine();
            //do_image_perspect();
            do_image_mopology();

            //video_out();

        }

        void video_out()
        {
            VideoCapture video = new VideoCapture("a.mp4");
            Mat frame = new Mat();

            while (video.PosFrames != video.FrameCount)
            {
                video.Read(frame);
                Cv2.ImShow("frame", frame);
                Cv2.WaitKey(33);
            }

            frame.Dispose();
            video.Release();
            Cv2.DestroyAllWindows();
        }

        void do_image_mopology()
        {
            Mat src = new Mat("nape.jpg");
            Mat dilate = new Mat();
            Mat erode = new Mat();
            Mat dst = new Mat();

            Mat element = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(5, 5));
            Cv2.Dilate(src, dilate, element, new Point(2, 2), 3);
            Cv2.Erode(src, erode, element, new Point(-1, -1), 3);

            Cv2.HConcat(new Mat[] { dilate, erode }, dst);
            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }
        void do_affine()
        {
            Mat src = new Mat("snow.jpg");
            Mat dst = new Mat();

            List<Point2f> src_pts = new List<Point2f>()
            {
                new Point2f(0.0f, 0.0f),
                new Point2f(0.0f, src.Height),
                new Point2f(src.Width, src.Height)
            };

            List<Point2f> dst_pts = new List<Point2f>()
            {
               new Point2f(300.0f, 300.0f),
               new Point2f(300.0f, src.Height),
               new Point2f(src.Width - 400.0f, src.Height - 200.0f)
            };

            Mat matrix = Cv2.GetAffineTransform(src_pts, dst_pts);
            Cv2.WarpAffine(src, dst, matrix, new Size(src.Width, src.Height));

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }

        void do_image_perspect()
        {
            Mat src = new Mat("plate.jpg");
            Mat dst = new Mat();

            List<Point2f> src_pts = new List<Point2f>()
            {
                new Point2f(0.0f, 0.0f),
                new Point2f(0.0f, src.Height),
                new Point2f(src.Width, src.Height),
                new Point2f(src.Width, 0.0f)
            };

            List<Point2f> dst_pts = new List<Point2f>()
            {
               new Point2f(300.0f, 100.0f),
               new Point2f(300.0f, src.Height),
               new Point2f(src.Width - 400.0f, src.Height - 200.0f),
               new Point2f(src.Width - 200.0f, 200.0f)
            };

            Mat matrix = Cv2.GetPerspectiveTransform(src_pts, dst_pts);
            Cv2.WarpPerspective(src, dst, matrix, new Size(src.Width, src.Height));

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }
        void do_image_rotation()
        {
            Mat src = new Mat("wine.jpg");
            Mat dst = new Mat();

            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), 45.0, 1.0);
            Cv2.WarpAffine(src, dst, matrix, new Size(src.Width, src.Height));

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }

        void image_ct_1()
        {
            Mat src = new Mat("hex.jpg");
            Mat yellow = new Mat();
            Mat dst = src.Clone();

            Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.InRange(src, new Scalar(0, 127, 127), new Scalar(100, 255, 255), yellow);
            Cv2.FindContours(yellow, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            foreach (Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                double area = Cv2.ContourArea(p, true);

                if (length < 100 && area < 1000 && p.Length < 5) continue;

                Rect boundingRect = Cv2.BoundingRect(p);
                RotatedRect rotatedRect = Cv2.MinAreaRect(p);
                RotatedRect ellipse = Cv2.FitEllipse(p);

                Point2f center;
                float radius;
                Cv2.MinEnclosingCircle(p, out center, out radius);

                Cv2.Rectangle(dst, boundingRect, Scalar.Red, 2);
                Cv2.Ellipse(dst, rotatedRect, Scalar.Blue, 2);
                Cv2.Ellipse(dst, ellipse, Scalar.Green, 2);
                Cv2.Circle(dst, (int)center.X, (int)center.Y, (int)radius, Scalar.Yellow, 2);
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }
        void image_ct_2()
        {
            Mat src = new Mat("hex.jpg");
            Mat yellow = new Mat();
            Mat dst = src.Clone();

            Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.InRange(src, new Scalar(0, 127, 127), new Scalar(100, 255, 255), yellow);
            Cv2.FindContours(yellow, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            foreach (Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                double area = Cv2.ContourArea(p, true);

                if (length < 100 && area < 1000 && p.Length < 5) continue;

                bool convex = Cv2.IsContourConvex(p);
                Point[] hull = Cv2.ConvexHull(p, true);
                Moments moments = Cv2.Moments(p, false);

                //Cv2.FillConvexPoly(dst, hull, Scalar.White);
                //Cv2.Polylines(dst, new Point[][] { hull }, true, Scalar.White, 1);
                Cv2.DrawContours(dst, new Point[][] { hull }, -1, Scalar.White, 1);
                Cv2.Circle(dst, (int)(moments.M10 / moments.M00), (int)(moments.M01 / moments.M00), 5, Scalar.Black, -1);
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }

        void do_image_poly()
        {
            Mat src = new Mat("hex.jpg");
            Mat yellow = new Mat();
            Mat dst = src.Clone();

            Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.InRange(src, new Scalar(0, 127, 127), new Scalar(100, 255, 255), yellow);
            Cv2.FindContours(yellow, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            List<Point[]> new_contours = new List<Point[]>();
            foreach (Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                if (length < 100) continue;

                new_contours.Add(Cv2.ApproxPolyDP(p, length * 0.02, true));
            }

            Cv2.DrawContours(dst, new_contours, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, null, 1);
            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }
        void do_image_contour()
        {
            Mat src = new Mat("hex.jpg");
            Mat yellow = new Mat();
            Mat dst = src.Clone();

            Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.InRange(src, new Scalar(0, 127, 127), new Scalar(100, 255, 255), yellow);
            Cv2.FindContours(yellow, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            List<Point[]> new_contours = new List<Point[]>();
            foreach (Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                if (length > 100)
                {
                    new_contours.Add(p);
                }
            }

            Cv2.DrawContours(dst, new_contours, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, null, 1);
            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }
        void do_image_edge()
        {
            Mat src = new Mat("wintry.jpg");
            Mat blur = new Mat();

            Mat sobel = new Mat();
            Mat scharr = new Mat();
            Mat laplacian = new Mat();
            Mat canny = new Mat();

            Cv2.GaussianBlur(src, blur, new Size(3, 3), 1, 0, BorderTypes.Default);

            Cv2.Sobel(blur, sobel, MatType.CV_32F, 1, 0,  3,  1, 0, BorderTypes.Default);
            sobel.ConvertTo(sobel, MatType.CV_8UC1);

            Cv2.Scharr(blur, scharr, MatType.CV_32F, 1, 0, scale: 1, delta: 0,borderType: BorderTypes.Default);
            scharr.ConvertTo(scharr, MatType.CV_8UC1);

            Cv2.Laplacian(blur, laplacian, MatType.CV_32F, ksize: 3, scale: 1, delta: 0, borderType: BorderTypes.Default);
            laplacian.ConvertTo(laplacian, MatType.CV_8UC1);

            Cv2.Canny(blur, canny, 100, 200, 3, true);

            Cv2.ImShow("sobel", sobel);
            Cv2.ImShow("scharr", scharr);
            Cv2.ImShow("laplacian", laplacian);
            Cv2.ImShow("canny", canny);
            Cv2.WaitKey(0);
        }
        void do_image_blur()
        {
            Mat src = new Mat("sparkler.jpg");
            Mat blur = new Mat();
            Mat box_filter = new Mat();
            Mat median_blur = new Mat();
            Mat gaussian_blur = new Mat();
            Mat bilateral_filter = new Mat();

            Cv2.Blur(src, blur, new Size(9, 9), new Point(-1, -1), BorderTypes.Default);
            Cv2.BoxFilter(src, box_filter, MatType.CV_8UC3, new Size(9, 9), new Point(-1, -1), true, BorderTypes.Default);
            Cv2.MedianBlur(src, median_blur, 9);
            Cv2.GaussianBlur(src, gaussian_blur, new Size(9, 9), 1, 1, BorderTypes.Default);
            Cv2.BilateralFilter(src, bilateral_filter, 9, 3, 3, BorderTypes.Default);

            Cv2.ImShow("blur", blur);
            Cv2.ImShow("box_filter", box_filter);
            Cv2.ImShow("median_blur", median_blur);
            Cv2.ImShow("gaussian_blur", gaussian_blur);
            Cv2.ImShow("bilateral_filter", bilateral_filter);
            Cv2.WaitKey(0);
        }

        void do_image_calc_elephant()
        {
            Mat src1 = new Mat("elephant.jpg", ImreadModes.ReducedColor2);
            Mat src2 = src1.Flip(FlipMode.Y);

            Mat and = new Mat();
            Mat or = new Mat();
            Mat xor = new Mat();
            Mat not = new Mat();
            Mat compare = new Mat();

            Cv2.BitwiseAnd(src1, src2, and);
            Cv2.BitwiseOr(src1, src2, or);
            Cv2.BitwiseXor(src1, src2, xor);
            Cv2.BitwiseNot(src1, not);
            Cv2.Compare(src1, src2, compare, CmpType.EQ);

            Cv2.ImShow("and", and);
            Cv2.ImShow("or", or);
            Cv2.ImShow("xor", xor);
            Cv2.ImShow("not", not);
            Cv2.ImShow("compare", compare);
            Cv2.WaitKey(0);
        }

        void do_image_calc_flamingo()
        {
            Mat src = new Mat("flamingo.jpg", ImreadModes.ReducedColor2);
            Mat val = new Mat(src.Size(), MatType.CV_8UC3, new Scalar(0, 0, 30));

            Mat add = new Mat();
            Mat sub = new Mat();
            Mat mul = new Mat();
            Mat div = new Mat();
            Mat max = new Mat();
            Mat min = new Mat();
            Mat abs = new Mat();
            Mat absdiff = new Mat();

            Cv2.Add(src, val, add);
            Cv2.Subtract(src, val, sub);
            Cv2.Multiply(src, val, mul);
            Cv2.Divide(src, val, div);
            Cv2.Max(src, mul, max);
            Cv2.Min(src, mul, min);
            abs = Cv2.Abs(mul);
            Cv2.Absdiff(src, mul, absdiff);

            Cv2.ImShow("add", add);
            Cv2.ImShow("sub", sub);
            Cv2.ImShow("mul", mul);
            Cv2.ImShow("div", div);
            Cv2.ImShow("max", max);
            Cv2.ImShow("min", min);
            Cv2.ImShow("abs", abs);
            Cv2.ImShow("absdiff", absdiff);
            Cv2.WaitKey(0);
        }

        void do_bidirect()
        {
            Mat src = new Mat("rose.png");
            Mat gray = new Mat();
            Mat binary = new Mat();

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, 150, 255, ThresholdTypes.Binary);

            Cv2.ImShow("src", src);
            Cv2.ImShow("dst", binary);
            Cv2.WaitKey(0);
        }

        void do_cut()
        {
            Mat src = new Mat("test.png");

            int width = src.Width;
            int height = src.Height;
            Mat dst = src.SubMat(new Rect(width / 2, height / 2, width / 2, height / 2));

            Cv2.ImShow("src", src);
            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }

        void do_resize()
        {
            Mat src = new Mat("test.png");
            Mat dst = new Mat();

            Cv2.Resize(src, dst, new OpenCvSharp.Size(500, 250));

            Cv2.ImShow("src", src);
            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
        }

        void do_pyramid()
        {
            Mat src = new Mat("test3.png");
            Mat pyrUp = new Mat();
            Mat pyrDown = new Mat();

            Cv2.PyrUp(src, pyrUp);
            Cv2.PyrDown(src, pyrDown);

            Cv2.ImShow("pyrUp", pyrUp);
            Cv2.ImShow("pyrDown", pyrDown);
            Cv2.WaitKey(0);
        }

        void do_mirror()
        {
            Mat src = Cv2.ImRead("test.png");
            Mat dst = new Mat(src.Size(), MatType.CV_8UC3);

            Cv2.Flip(src, dst, FlipMode.Y);

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        void do_colorspace()
        {
            Mat src = Cv2.ImRead("test2.png");
            Mat dst = new Mat(src.Size(), MatType.CV_8UC1);

            Cv2.CvtColor(src, dst, ColorConversionCodes.BGR2GRAY);

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        void do_video()
        {

            VideoCapture video = new VideoCapture("a.mp4");
            Mat frame = new Mat();

            while (video.PosFrames != video.FrameCount)
            {
                video.Read(frame);
                Cv2.ImShow("frame", frame);
                Cv2.WaitKey(33);
            }

            frame.Dispose();
            video.Release();
            Cv2.DestroyAllWindows();

        }

        void do_image()
        {
            Mat image = Cv2.ImRead("test.png", ImreadModes.Grayscale);
            Cv2.ImShow("image", image);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        VideoCapture capture;
        Mat frame;
        Bitmap image;
        private Thread camera;
        int isCameraRunning = 0;
        void test1()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.Start();
        }

        private void CaptureCameraCallback()
        {
            frame = new Mat();
            capture = new VideoCapture();
            capture.Open(0);

            while (isCameraRunning == 1)
            {
                capture.Read(frame);
                if (!frame.Empty())
                {
                    image = BitmapConverter.ToBitmap(frame);
                    pictureBox1.Image = image;
                }
                image = null;
            }

        }
    }
}
