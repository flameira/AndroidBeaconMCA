using AndroidBeacon.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogService))]

namespace AndroidBeacon.Droid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     Log service.
    /// </summary>
    public class LogService
    {
        private readonly string diagnosticFileName = "DiagnosticData.zip";
        private readonly object logLock = new object();
        private readonly string logsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public LogService()
        {
            DeleteOldLogFiles();
        }

        public void DeleteLogFile(string fileName)
        {
            try
            {
                var filename = Path.Combine(logsDirectory, fileName);

                if (File.Exists(filename))
                    File.Delete(filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteOldLogFiles()
        {
            lock (logLock)
            {
                try
                {
                    var dir = new DirectoryInfo(logsDirectory);
                    var logFiles = dir.GetFileSystemInfos().Where(f => f.Extension.Equals(".txt")); // get log files
                    foreach (var file in logFiles.Where(f => f.CreationTime < DateTime.Now.AddMonths(-1)))
                        file.Delete();
                }
                catch
                {}
            }
        }

        public void DeleteZipLog()
        {
            DeleteLogFile(diagnosticFileName);
        }

        public string GetLogContent(string fileName)
        {
            lock (logLock)
            {
                try
                {
                    var filename = Path.Combine(logsDirectory, fileName);
                    var content = string.Empty;

                    using (var streamReader = new StreamReader(filename))
                    {
                        content = streamReader.ReadToEnd();
                    }

                    return content;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public async Task<string> GetLogContentAsync(string fileName)
        {
            try
            {
                var filename = Path.Combine(logsDirectory, fileName);
                var content = string.Empty;

                using (var streamReader = File.OpenText(filename))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                return content;
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<string> GetLogFiles()
        {
            try
            {
                var dir = new DirectoryInfo(logsDirectory);
                var files = new List<string>();

                foreach (var item in dir.GetFileSystemInfos().Where(f => f.Extension.Equals(".txt")))
                    files.Add(item.Name);

                return files;
            }
            catch
            {
                return new List<string>();
            }
        }

        public byte[] GetZipLogAsByteArray()
        {
            byte[] data = null;

            try
            {
                var filename = Path.Combine(logsDirectory, diagnosticFileName);
                if (File.Exists(filename))
                    data = File.ReadAllBytes(filename);
            }
            catch
            {}

            return data;
        }

        public void WriteToLog(string fileName, string message)
        {
            lock (logLock)
            {
                try
                {
                    var filename = Path.Combine(logsDirectory,
                        fileName + "_" + DateTime.Today.ToString("yyyyMMdd") + ".txt");

                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        var logLine = DateTime.Now + "," + message;
                        streamWriter.WriteLine(logLine);
                    }
                }
                catch
                {}
            }
        }

        public async Task WriteToLogAsync(string fileName, string message)
        {
            try
            {
                var filename = Path.Combine(logsDirectory,
                    fileName + "_" + DateTime.Today.ToString("yyyyMMdd") + ".txt");

                using (var streamWriter = new StreamWriter(filename, true))
                {
                    var logLine = DateTime.Now + "," + message;
                    await streamWriter.WriteLineAsync(logLine);
                }
            }
            catch
            {}
        }
    }
}