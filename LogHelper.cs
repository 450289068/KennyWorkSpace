using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Seismic.Data.CommonLib
{

    public class LogHelper
    {
        private static ILog log;

        public static string LogFilePath { get; set; }

        private static void ChangeLog4netLogFileName(log4net.ILog iLog, string fileName)
        {
            log4net.Core.LogImpl logImpl = iLog as log4net.Core.LogImpl;
            if (logImpl != null)
            {
                log4net.Appender.AppenderCollection ac = ((log4net.Repository.Hierarchy.Logger)logImpl.Logger).Appenders;
                for (int i = 0; i < ac.Count; i++)
                {
                    log4net.Appender.RollingFileAppender rfa = ac[i] as log4net.Appender.RollingFileAppender;
                    if (rfa != null)
                    {
                        rfa.File = fileName;
                        if (!System.IO.File.Exists(fileName))
                        {
                            System.IO.File.Create(fileName);
                        }

                        rfa.Writer = new System.IO.StreamWriter(rfa.File, rfa.AppendToFile, rfa.Encoding);
                    }
                }
            }
        }

        private static ILog GetInstance()
        {
            if (log == null)
            {
                log = log4net.LogManager.GetLogger("LogHelper");

                log4net.Repository.Hierarchy.Hierarchy h = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;

                if (h != null)
                {
                    log4net.Appender.RollingFileAppender appender = (log4net.Appender.RollingFileAppender)h.GetLogger("LogHelper", h.LoggerFactory).GetAppender("RollingFileAppender");

                    if (appender != null)
                    {
                        string filePath = string.Format(System.Configuration.ConfigurationManager.AppSettings["LogFile"],DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".log");
                        appender.File = filePath;
                        appender.ActivateOptions();
                        LogFilePath = appender.File;
                    }
                }
            }

            return log;
        }

        private LogHelper()
        {

        }

        public static void Debug(string exMessage)
        {
            Debug(exMessage, null);
        }

        public static void Debug(string exMessage, Exception exception)
        {
            GetInstance().Debug(exMessage, exception);
        }

        #region IsErrorEnabled
        public static void Error(string errorMessage)
        {
            Error(errorMessage, null);
        }

        public static void Error(string errorMessage, Exception exception)
        {
            GetInstance().Error(errorMessage, exception);
        }
        #endregion

        #region IsInfoEnabled
        public static void Info(string infoMessage)
        {
            Info(infoMessage, null);
        }

        public static void Info(string infoMessage, Exception exception)
        {
            GetInstance().Info(infoMessage, exception);
        }
        #endregion

        #region IsWarnEnabled
        public static void Warn(string warnMessage)
        {
            Warn(warnMessage, null);
        }

        public static void Warn(string warnMessage, Exception exception)
        {
            GetInstance().Warn(warnMessage, exception);
        }
        #endregion

        public static string GetBakLogFile(string bakfilepath)
        {
            if (string.IsNullOrEmpty(bakfilepath))
            {
                bakfilepath = Path.Combine(
                    Path.GetDirectoryName(LogFilePath),
                    string.Format("TempLogFile_{0}",DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".log"));
            }

            if (!string.IsNullOrEmpty(LogFilePath) && File.Exists(LogFilePath))
            {
                if (File.Exists(bakfilepath))
                    File.Delete(bakfilepath);
                File.Copy(LogFilePath, bakfilepath);
            }
            else
            {
                File.WriteAllText("The log file does not exist.", bakfilepath);
            }

            return bakfilepath;
        }

        public static void ShutDown()
        {
            log = null;
            log4net.LogManager.Shutdown();
        }

    }
}
