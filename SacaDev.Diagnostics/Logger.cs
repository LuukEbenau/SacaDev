using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SacaDev.Util.String.Url;
namespace SacaDev.Diagnostics
{
    public class Logger : ILogger
	{
		public string LogFilePath { get; }
		private readonly SemaphoreSlim _threadManager;
        public Logger(string logFilePath)
        {
			LogFilePath = logFilePath;
			_threadManager = new SemaphoreSlim(1);
			try {
				if(!File.Exists(logFilePath))
					Directory.CreateDirectory(logFilePath.GetDirectory());
			}
			catch(Exception ex) {
				throw new AggregateException($"The creating of the logfile directory failed with the following exception: \"{ex.Message}\"", ex);
			}
		}
		//private EventLog EventLogger;
		/// <summary>
		/// Write a loggingmessage to the given eventlog
		/// </summary>
		/// <param name="message"></param>
		public void Write(string message) => Write(message, LogType.INFO);
		public void Write(string message, LogType type)
        {
			var writeTask = Task.Run(() => {
				_threadManager.Wait(); //first in, first out, wait until previous logger request finished writing

				string line = $"{DateTime.UtcNow} |{type.ToString()}| {message}";
				try {
					File.AppendAllLines(LogFilePath, new string[] { line }, Encoding.UTF8);
				}
				catch(Exception ex) {
					try {
						Console.WriteLine("Logger service failed with the following exception: {" + ex.Message + "}");
					}
					catch(Exception) { }
				}
				
				_threadManager.Release();
				WrittenToLog?.Invoke(this, new LogMessageEventArgs(message, type));
			});

			//if (alsoWriteToWindowsLog) {
			//    EventLogEntryType t = EventLogEntryType.Information;
			//    switch (type)
			//    {
			//        case LogType.INFO:
			//            t = EventLogEntryType.Information;
			//            break;
			//        case LogType.ERROR:
			//            t = EventLogEntryType.Error;
			//            break;
			//        case LogType.DEBUG:
			//            t = EventLogEntryType.Information;
			//            break;
			//        case LogType.WARNING:
			//            t = EventLogEntryType.Warning;
			//            break;
			//    }
			//    WriteToEventLogEntry(message,t);
			//}
		}
        //public void WriteToEventLogEntry(string message, EventLogEntryType type = EventLogEntryType.Information) {
        //    if (EventLogger == null) InitializeEventLogger();
        //    EventLogger.WriteEntry(message, type);
        //}

        //private void InitializeEventLogger()
        //{
        //    if (!EventLog.SourceExists(Constants.SERVICEAPPNAME))
        //    {
        //        EventLog.CreateEventSource(
        //           Constants.SERVICEAPPNAME, Constants.WINDOWSLOGNAME);
        //    }
        //    EventLogger = new EventLog
        //    {
        //        Source = Constants.SERVICEAPPNAME,
        //        Log = Constants.WINDOWSLOGNAME
        //    };
        //}

        public event EventHandler<LogMessageEventArgs> WrittenToLog;
    }

    public class LogMessageEventArgs : EventArgs
    {
        public string message;
        public LogType type;
        public LogMessageEventArgs(string message, LogType type)
        {
            this.message = message;
            this.type = type;
        }
    }
    public enum LogType {
        INFO=0,
        ERROR,
        DEBUG,
        WARNING
    }
}
