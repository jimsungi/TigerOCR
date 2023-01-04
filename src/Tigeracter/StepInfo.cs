using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tigerword.ocr
{
    public enum InfoType
        {
        Info,Error,Warning,Exception
    }
    public class StepInfo
    {
        public InfoType info_type= InfoType.Info;
        private string info_string = string.Empty;
        private Exception info_exception=null;

        public StepInfo(string info, Exception e)
        {
            info_type = InfoType.Exception;
            info_string = info;
            info_exception = e;

        }

        public StepInfo(InfoType _info_type, string info_txt, Exception e = null)
        {
            info_type = _info_type;
            info_string = info_txt;
            if(info_type == InfoType.Exception)
            {
                info_exception = e;
            }
        }
        public override string ToString()
        {
            switch(info_type)
            {
                case InfoType.Exception:
                    if (info_exception != null)
                    {
                        return string.Format("{0}:>{1}\r\n{2}", info_type.ToString(), info_string, info_exception.ToString());
                    }
                    else
                    {
                        return string.Format("{0}:>{1}", info_type.ToString(), info_string);
                    }
                    //break;
                default:
                    return string.Format("{0}:>{1}", info_type.ToString(), info_string);
                    //break;
            }
            //return "";
        }
    }
}
