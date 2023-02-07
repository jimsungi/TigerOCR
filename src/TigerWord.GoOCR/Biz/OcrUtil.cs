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
using TigerWord.GoOCR.Biz;


namespace TigerWord.GoOCR.Biz
{
    public class OcrUtil
    {
        public static Mat draw_contour(Mat src, Point[]? contour, int b = 255, int g = 0, int r = 0, int thick = 10)
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

        public static Point[]? do_contour(Mat do_src)
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

        public static Mat do_transform_mat(Mat mid_resize, Point[] _do_rect, double resize_ratio = 1)
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
            var dh = maxSize.Height;// dw * maxSize.Height / maxSize.Width;

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

        public static Mat do_resize_mat(int width, Mat do_src)
        {
            double resize_width = width;
            OpenCvSharp.Size newsize = new OpenCvSharp.Size();
            double resize_ratio = (double)do_src.Width / (double)resize_width;
            int new_height = (int)(do_src.Height / resize_ratio);

            newsize.Width = (int)resize_width;
            newsize.Height = new_height;
            Mat tmp = do_src.Clone();
            Cv2.Resize(do_src, tmp, newsize);
            return tmp;
        }


        public static Mat do_gray_mat(Mat do_src)
        {
            Mat gray = do_src.Clone();
            Cv2.CvtColor(do_src, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        public static Mat do_threadhold_mat(Mat gray)
        {
            // 이진화
            Mat binSrc = gray.Clone();
            Cv2.Threshold(gray, binSrc, 0.0, 255.0, ThresholdTypes.Otsu);
            return binSrc;
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
        public static Mat do_ocr_mat(Mat crop_src, string language)
        {
            Mat to_ocr_image = crop_src.Clone();

            Ocr.SetImageTarget(to_ocr_image);
            Ocr.SetDrawDelegation(DrawRect);
            Ocr.ocr_image(to_ocr_image,language);
            return to_ocr_image;
        }
        public static Mat do_ocr_mat_txt(Mat crop_src, string out_pdf_filename,string language)
        {
            Mat to_ocr_txt_image = crop_src.Clone();

#if ITEXTSHARP
            DrawTextObject param = new DrawTextObject();
            param.fontpath = "./resources/fonts/H2GTRM.ttf";
            param.fontUnicode =
    PdfFontFactory.CreateFont(param.fontpath, PdfEncodings.IDENTITY_H);

            PdfWriter writer = new PdfWriter(out_pdf_filename);

            // Must have write permissions to the path folder
            iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer);
            Document document = new Document(pdf);
            pdf.AddNewPage();
            var page = pdf.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page);
            param.PdfWidth = page.GetPageSizeWithRotation().GetWidth();
            param.PdfHeight = page.GetPageSizeWithRotation().GetHeight();

            Bitmap bmp = BitmapConverter.ToBitmap(to_ocr_txt_image);
            bmp.Save("tmp.xxx.png", System.Drawing.Imaging.ImageFormat.Png);
            Image img = new Image(ImageDataFactory
               .Create("tmp.xxx.png"));
            float img_width = img.GetImageWidth();
            float img_height = img.GetImageHeight();
            float ratio = 1;
            if (img_width > param.PdfWidth)
            {
                ratio = param.PdfWidth / img_width;
                float tmp_height = img_height * ratio;
                if (tmp_height > param.PdfHeight)
                {
                    ratio = param.PdfHeight / img_height;
                    img_width *= ratio;
                    img_height = param.PdfHeight;
                    param.ImageLeft = (param.PdfWidth - img_width) / 2;
                    param.ImageTop = 0;
                }
                else
                {
                    img_width = param.PdfWidth;
                    img_height *= ratio;
                    param.ImageTop = (param.PdfHeight - img_height) / 2;
                    param.ImageLeft = 0;
                }
            }
            else if (img_height > param.PdfHeight)
            {
                ratio = img_height / param.PdfHeight;
                float tmp_width = img_width * ratio;
                if (tmp_width > param.PdfWidth)
                {
                    ratio = param.PdfWidth / img_width;
                    img_height *= ratio;
                    img_width = param.PdfWidth;
                    param.ImageTop = (param.PdfHeight - img_height) / 2;
                    param.ImageLeft = 0;
                }
                else
                {
                    img_height = param.PdfHeight;
                    img_width *= ratio;
                    param.ImageLeft = (param.PdfWidth - img_width) / 2;
                    param.ImageTop = 0;
                }
            }
            param.ImageRatio = ratio;
            param.document = document;
            param.canvas = canvas;
            img.SetWidth(img_width)
            .SetHeight(img_height)
            .SetTextAlignment(TextAlignment.CENTER).SetFixedPosition(param.ImageLeft, param.ImageTop);
            document.Add(img);

            //writer.getDirect
            Ocr.SetResultTarget(param);
            Ocr.SetTextDelegation(DrawText);
            Ocr.ocr_image_txt(to_ocr_txt_image,language);

            // Close document
            document.Close();

#endif
            return to_ocr_txt_image;
        }

        public static void DrawRect(object ocr_src, System.Drawing.Rectangle rect)
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

        public static void DrawText(object ocr_src, System.Drawing.RectangleF rect, string text)
        {
            DrawTextObject param = ocr_src as DrawTextObject;
            if (param != null && !string.IsNullOrWhiteSpace(text))
            {
                rect.X *= param.ImageRatio;
                rect.Y *= param.ImageRatio;
                rect.Width *= param.ImageRatio;
                rect.Height *= param.ImageRatio;
                //text = "공";
                Document document = param.document;
                PdfCanvas canvas = param.canvas;
                rect.Y = param.PdfHeight - rect.Y - param.ImageTop;
                rect.X = rect.X + param.ImageLeft;

                canvas.SaveState();
                canvas.SetFillColor(iText.Kernel.Colors.ColorConstants.YELLOW);
                canvas.SetExtGState(new iText.Kernel.Pdf.Extgstate.PdfExtGState().SetFillOpacity(1f));
                canvas.Rectangle(rect.X, rect.Y - rect.Height, rect.Width, rect.Height);
                canvas.Fill();
                canvas.RestoreState();

                document.SetFontSize(8);
                document.SetFont(param.fontUnicode).
                    ShowTextAligned(text, rect.X, rect.Y, TextAlignment.LEFT,
                    VerticalAlignment.TOP, 0);
            }
        }
#endif
    }

    public class DrawTextObject
    {
        public string fontpath = "./resources/fonts/H2GTRM.ttf";
        //PdfFont fontUnicode =
        //    PdfFontFactory.CreateFont(hcrBatang, PdfEncodings.IDENTITY_H, true);
        public PdfFont fontUnicode = null;
        public float PdfWidth = 0;
        public float PdfHeight = 0;
        public float ImageLeft = 0;
        public float ImageTop = 0;
        public float ImageRatio = 1;
        public Document document = null;
        public PdfCanvas canvas = null;
    }

}
