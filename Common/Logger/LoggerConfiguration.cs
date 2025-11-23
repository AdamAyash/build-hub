using BuildHub.Common.ConfigurationManager.Base;
using Serilog;
using Serilog.Events;

namespace BuildHub.Common.Logger
{
	internal class LoggerConfiguration : IConfigurationModel
	{
		public LogEventLevel MinimumLogEventLevel { get; set; }
		public string LogFileDirectory { get; set; }
		public RollingInterval RollingInterval { get; set; }
		public string SeqServerUrl { get; set; }
	}
}
