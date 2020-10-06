using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Logging
{
    public interface ILogger
    {
        void Info(string message);

        void Error(string message);
    }
}
