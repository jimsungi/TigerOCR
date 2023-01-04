using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerOCR
{
    public class ImUtils
    {
        public static Point[] grab_contours(Point[][] cnts)
        {
            /*
             *     # if the length the contours tuple returned by cv2.findContours
    # is '2' then we are using either OpenCV v2.4, v4-beta, or
    # v4-official
    if len(cnts) == 2:
        cnts = cnts[0]

    # if the length of the contours tuple is '3' then we are using
    # either OpenCV v3, v4-pre, or v4-alpha
    elif len(cnts) == 3:
        cnts = cnts[1]

    # otherwise OpenCV has changed their cv2.findContours return
    # signature yet again and I have no idea WTH is going on
    else:
        raise Exception(("Contours tuple must have length 2 or 3, "
            "otherwise OpenCV changed their cv2.findContours return "
            "signature yet again. Refer to OpenCV's documentation "
            "in that case"))

    # return the actual contours array
    return cnts
             */
            if (cnts == null)
                return null;
            if (cnts.Length == 1)
                return cnts[0];
            else if (cnts.Length == 2)
                return cnts[0];
            else if (cnts.Length == 3)
                return cnts[1];
            else
                throw new Exception(@"Contours tuple must have length 2 or 3,
otherwise OpenCV changed their cv2.findContours return 
signature yet again. Refer to OpenCV's documentation 
in that case");
        }
    }
}
