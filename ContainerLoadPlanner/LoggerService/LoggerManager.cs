using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class LoggerManager : BaseLoggerManager
    {
        
        public LoggerManager(LogType logType):base(logType)
        {
            CreateLogger();
            
        }
       
       
        public override void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public override void LogError(Exception exception, string message)
        {
            Log.Error(exception.InnerException,exception.InnerException?.Message, LogEventLevel.Error);
            Log.Error(exception, message, LogEventLevel.Error);
        }

        public override void LogInfo(string message)
        {
            Log.Information(message);
        }

        public override void LogWarn(string message)
        {
            Log.Warning(message);
        }

       
    }
}
