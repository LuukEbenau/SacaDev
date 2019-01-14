using System;

namespace SacaDev.Diagnostics
{
	public interface ILogger
	{
		event EventHandler<LogMessageEventArgs> WrittenToLog;

		void Write(string message);
		void Write(string message, LogType type);
	}
}