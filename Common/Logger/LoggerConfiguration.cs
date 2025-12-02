using Serilog;
using Serilog.Events;
using BuildHub.Common.ConfigurationManager.Base;

namespace BuildHub.Common.Logger
{
	/// <summary>
	/// Represents the configuration settings for a logging system, including log levels, output destinations, and
	/// formatting options.
	/// </summary>
	/// <remarks>This class provides properties to configure various aspects of logging, such as the minimum log
	/// level,  file-based logging settings, and integration with external logging services. It is typically used to 
	/// define the logging behavior for an application.</remarks>
	internal class LoggerConfiguration : IConfigurationModel
	{
		public LogEventLevel MinimumLogEventLevel { get; set; }
		public string LogFileDirectory { get; set; }
		public RollingInterval RollingInterval { get; set; }
		public string SeqServerUrl { get; set; }
		public bool LogToConsoleEnabled { get; set; }
	}
}
