using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace GroupProject.Logging
{
    public class Log4NetLogger : ILogger
    {
        private ILog _log;

        private ILog Log
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger("FileLogger");
                }

                return _log;
            }
        }


        public void Info(string message)
        {
            Log.Info(message);
        }


        public void Error(string message)
        {
            Log.Error(message);
        }
    }
}