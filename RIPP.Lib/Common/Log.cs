using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace RIPP.Lib
{
    public class Log
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
 
        public static void Debug(object message, Exception exception = null)
        {
            if (log.IsDebugEnabled)
                log.Debug(message, exception);
        }

        public static void Error(object message, Exception exception = null)
        {
            if (log.IsErrorEnabled)
                log.Error(message, exception);
        }
        public static void Fatal(object message, Exception exception = null)
        {
            if (log.IsFatalEnabled)
                log.Fatal(message, exception);
        }
        public static void Info(object message, Exception exception = null)
        {
            if (log.IsInfoEnabled)
                log.Info(message, exception);
        }
        public static void Warn(object message, Exception exception = null)
        {
            if (log.IsWarnEnabled)
                log.Warn(message, exception);
        }
    }
}