using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Grade
{
    public static class LogUtils
    {
        #region 内部实现
        private static List<string> logList = new List<string>();
        private static Thread thread = new Thread(LogContext);
        private static string logPath = Environment.CurrentDirectory;
        private static bool appAlive = true;
        private static void LogContext()
        {
            try
            {
                while (true)
                {
                    if (logList.Count <= 0)
                    {
                        try
                        {
                            Thread.Sleep(Timeout.Infinite);
                        }
                        catch (ThreadInterruptedException)
                        {

                        }
                    }
                    else
                    {
                        LogContextReal(logList[0]);
                        logList.RemoveAt(0);
                    }
                }
            }
            catch
            {

            }
        }
        private static void LogContextReal(string log)
        {
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                using (StreamWriter sw = new StreamWriter(Path.GetFullPath(logPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"), true))
                {
                    sw.Write(log);
                }
            }
            catch
            {

            }
        }

        private static void Start()
        {
            try
            {
                if (appAlive)
                {
                    if (logList.Count > 0)
                    {
                        thread.Interrupt();
                    }
                }
            }
            catch
            {

            }
        }

        #endregion
        /// <summary>
        /// 启动日志程序
        /// </summary>
        public static void Begin()
        {
            try
            {
                if (appAlive)
                {
                    thread.Start();
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 终止日志程序
        /// </summary>
        public static void End()
        {
            try
            {
                appAlive = false;
                thread.Abort();
            }
            catch
            {

            }
        }
        /// <summary>
        /// 写字符串日志
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message, string source = null)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                str.Append("#BEGIN\r\n");
                str.Append("#TYPE:MESSAGE\r\n");
                if (source != null)
                {
                    str.Append("#SOURCE:");
                    str.Append(source);
                    str.Append("\r\n");
                }
                str.Append("DateTime:");
                str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                str.Append("\r\nMessage:");
                str.Append(message);
                str.Append("\r\n#END\r\n");
                logList.Add(str.ToString());
                Start();
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Write(Exception ex, string message = null, DateTime? dateTime = null, string source = null)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                str.Append("#BEGIN\r\n");
                str.Append("#TYPE:EXCEPTION\r\n");
                if (source != null)
                {
                    str.Append("#SOURCE:");
                    str.Append(source);
                    str.Append("\r\n");
                }
                else
                {
                    str.Append("#SOURCE:EXCEPTION\r\n");
                }
                str.Append("DateTime:");
                if (dateTime == null)
                {
                    str.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    str.Append(dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                str.Append("\r\n");
                if (message != null)
                {
                    str.Append("Message:");
                    str.Append(message);
                    str.Append("\r\n");
                }
                str.Append("ExceptionMessage:");
                str.Append(ex.Message);
                str.Append("\r\n");
                str.Append("ExceptionStackTrace:");
                str.Append(ex.StackTrace);
                str.Append("\r\n#END\r\n");
                if (ex.InnerException != null)
                {
                    str.Append("InnerExceptionMessage:");
                    str.Append(ex.InnerException.Message);
                    str.Append("\r\n");
                    str.Append("InnerExceptionStackTrace:");
                    str.Append(ex.InnerException.StackTrace);
                    str.Append("\r\n#END\r\n");
                }
                logList.Add(str.ToString());
                Start();
            }
            catch
            {
            }
        }
    }
}
