using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class SQLiteLoggerManager : BaseLoggerManager
    {
        private readonly LogType logType;

        public SQLiteLoggerManager(LogType logType) : base(logType)
        {
            this.logType = logType;
        }
        protected override void CreateLogger()
        {
            if (this.logType==LogType.SQLITE)
            {
                
            }
        }
        public override void LogDebug(string message)
        {
            throw new NotImplementedException();
        }

        public override void LogError(Exception exception, string message)
        {
            throw new NotImplementedException();
        }

        public override void LogInfo(string message)
        {
            throw new NotImplementedException();
        }

        public override void LogWarn(string message)
        {
            throw new NotImplementedException();
        }
    }
}
