using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public abstract class BaseLoggerManager : ILoggerManager
    {
        private readonly LogType logType;

        public enum LogType { EXCEL, TXT, SQLITE }
        protected ILogger Log { get; private set; }
        protected BaseLoggerManager(LogType logType)
        {

            this.logType = logType;
            CreateLogger();
        }

        protected virtual void CreateLogger()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            string extension = String.Empty;
            extension = logType == LogType.TXT ? ".txt" : ".xls";

            Log = new LoggerConfiguration().
                MinimumLevel.Debug().
                WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info" + extension, rollingInterval: RollingInterval.Day)).
                WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error" + extension, rollingInterval: RollingInterval.Day)).
                WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug" + extension, rollingInterval: RollingInterval.Day)).
                CreateLogger();
        }
        public abstract void LogDebug(string message);
        public abstract void LogError(Exception exception, string message);
        public abstract void LogInfo(string message);
        public abstract void LogWarn(string message);
    }
}
