using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.Tool
{
    public class LogHelper
    {
        public static void WriteLog(Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("ZH");
            log.Error("Error", ex);
        }
        public static void WriteLog(string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("ZH");
            log.Error(msg);
        }
        public static string getErrorMsgShort(Exception ex)
        {
            return $"出现应用程序未处理的异常：{ex.Message}\r\n";
        }
        public static string getErrorMsg(Exception ex)
        {
            string str = "";
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";

            if (ex != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                ex.GetType().Name, ex.Message, ex.StackTrace);
            }
            return str;
        }

    }
}
