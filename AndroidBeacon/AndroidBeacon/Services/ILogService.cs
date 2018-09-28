using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidBeacon.Services
{
    using System.Threading.Tasks;

    public interface ILogService
    {
        byte [] GetZipLogAsByteArray ();

        void DeleteZipLog ();

        void WriteToLog (string fileName, string message);

        List<string> GetLogFiles ();

        string GetLogContent (string fileName);

        Task<string> GetLogContentAsync (string fileName);

        void DeleteLogFile (string fileName);
    }
}
