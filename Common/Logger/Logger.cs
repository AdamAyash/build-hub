namespace BuildHub.Common.Logger
{
	using Serilog;
	using BuildHub.Common.ConfigurationManager;
	using SerilogConfiguration = Serilog.LoggerConfiguration;

	/// <summary>
	/// Provides a singleton instance of a logger for application-wide logging operations.
	/// </summary>
	/// <remarks>Use the <see cref="Initialize"/> method to obtain the shared logger instance. This class
	/// ensures that only one instance of the logger exists throughout the application's lifetime. The constructor is
	/// private to prevent direct instantiation.</remarks>
	public class Logger
	{
		private static Logger? _loggerInstance = null;

		private ConfigurationManager? _configurationManager = null;

		private Logger()
		{
			this.InitializeLogger();
		}

		~Logger()
		{
			Log.CloseAndFlush();
		}

		/// <summary>
		/// Initializes the application logger with default configuration settings.
		/// </summary>
		/// <remarks>This method sets up the logger to output log messages to the console and configures the minimum
		/// log level to Debug. Call this method before logging any messages to ensure that the logger is properly
		/// configured.</remarks>
		private void InitializeLogger()
		{
			_configurationManager = ConfigurationManager.GetConfigurationManager();

			try
			{
				var loggerConfiguration = _configurationManager.GetConfigurationModel<LoggerConfiguration>("LoggerConfiguration");
				if (loggerConfiguration is null)
					throw new InvalidOperationException();

				var serilogConfiguration = new SerilogConfiguration()
					.WriteTo.File(loggerConfiguration.LogFileDirectory, rollingInterval: loggerConfiguration.RollingInterval)
					.MinimumLevel.Is(loggerConfiguration.MinimumLogEventLevel)
					.WriteTo.Seq(loggerConfiguration.SeqServerUrl);

				Log.Logger = serilogConfiguration.CreateLogger();
			}
			catch(Exception)
			{
				Environment.Exit(0);
			}
		}

		/// <summary>
		/// Initializes the logger instance if it has not already been created.
		/// </summary>
		/// <remarks>Call this method before attempting to use logging functionality to ensure that the logger is
		/// available. Subsequent calls have no effect if the logger has already been initialized.</remarks>
		public static void Initialize()
		{
			if (_loggerInstance == null)
				_loggerInstance = new Logger();
		}

		/// <summary>
		/// Writes a debug log message with optional property values for structured logging.
		/// </summary>
		/// <remarks>Use this method to record general information about application execution, such as status updates
		/// or routine events. The message template supports structured logging, allowing property values to be captured for
		/// later analysis. This method does not throw exceptions for null or empty messages, but such messages may result in
		/// incomplete log entries.</remarks>
		/// <param name="message">The message template to log. May include placeholders for property values, which will be replaced by corresponding
		/// elements from <paramref name="propertyValues"/>.</param>
		/// <param name="propertyValues">An array of property values to be formatted into the message template. Each value is substituted into the
		/// corresponding placeholder in <paramref name="message"/>.</param>
		public static void LogDebug(string message, params object[] propertyValues)
		{
			Log.Debug(message, propertyValues);
		}

		/// <summary>
		/// Writes an informational log message with optional property values for structured logging.
		/// </summary>
		/// <remarks>Use this method to record general information about application execution, such as status updates
		/// or routine events. The message template supports structured logging, allowing property values to be captured for
		/// later analysis. This method does not throw exceptions for null or empty messages, but such messages may result in
		/// incomplete log entries.</remarks>
		/// <param name="message">The message template to log. May include placeholders for property values, which will be replaced by corresponding
		/// elements from <paramref name="propertyValues"/>.</param>
		/// <param name="propertyValues">An array of property values to be formatted into the message template. Each value is substituted into the
		/// corresponding placeholder in <paramref name="message"/>.</param>
		public static void LogInformation(string message, params object[] propertyValues)
		{
			Log.Information(message, propertyValues);
		}

		/// <summary>
		/// Writes a warning log message with optional property values for structured logging.
		/// </summary>
		/// <remarks>Use this method to record general information about application execution, such as status updates
		/// or routine events. The message template supports structured logging, allowing property values to be captured for
		/// later analysis. This method does not throw exceptions for null or empty messages, but such messages may result in
		/// incomplete log entries.</remarks>
		/// <param name="message">The message template to log. May include placeholders for property values, which will be replaced by corresponding
		/// elements from <paramref name="propertyValues"/>.</param>
		/// <param name="propertyValues">An array of property values to be formatted into the message template. Each value is substituted into the
		/// corresponding placeholder in <paramref name="message"/>.</param>
		public static void LogWarning(string message, params object[] propertyValues)
		{
			Log.Warning(message, propertyValues);
		}

		/// <summary>
		/// Writes an error log message with optional property values for structured logging.
		/// </summary>
		/// <remarks>Use this method to record general information about application execution, such as status updates
		/// or routine events. The message template supports structured logging, allowing property values to be captured for
		/// later analysis. This method does not throw exceptions for null or empty messages, but such messages may result in
		/// incomplete log entries.</remarks>
		/// <param name="message">The message template to log. May include placeholders for property values, which will be replaced by corresponding
		/// elements from <paramref name="propertyValues"/>.</param>
		/// <param name="propertyValues">An array of property values to be formatted into the message template. Each value is substituted into the
		/// corresponding placeholder in <paramref name="message"/>.</param>
		public static void LogError(string message, params object[] propertyValues)
		{
			Log.Error(message, propertyValues);
		}

		/// <summary>
		/// Logs an error message and associated exception details to the application's error logging system.
		/// </summary>
		/// <remarks>This method is typically used to record unexpected errors or exceptions for diagnostic purposes.
		/// If <paramref name="exception"/> is null, only the message and property values are logged.</remarks>
		/// <param name="exception">The exception to log. Can be null if no exception is associated with the error.</param>
		/// <param name="message">The error message to log. This should describe the error or context.</param>
		/// <param name="propertyValues">Optional property values to format into the message. These are used for structured logging or message formatting.</param>
		public static void LogError(Exception? exception, string message, params object[] propertyValues)
		{
			Log.Error(exception, message, propertyValues);
		}

		/// <summary>
		/// Writes a fatal log message with optional property values for structured logging.
		/// </summary>
		/// <remarks>Use this method to record general information about application execution, such as status updates
		/// or routine events. The message template supports structured logging, allowing property values to be captured for
		/// later analysis. This method does not throw exceptions for null or empty messages, but such messages may result in
		/// incomplete log entries.</remarks>
		/// <param name="message">The message template to log. May include placeholders for property values, which will be replaced by corresponding
		/// elements from <paramref name="propertyValues"/>.</param>
		/// <param name="propertyValues">An array of property values to be formatted into the message template. Each value is substituted into the
		/// corresponding placeholder in <paramref name="message"/>.</param>
		public static void LogFatal(string message, params object[] propertyValues)
		{
			Log.Fatal(message, propertyValues);
		}

		/// <summary>
		/// Writes a fatal log entry with the specified exception, message, and property values. Use this method to record
		/// critical errors that cause the application to terminate or require immediate attention.
		/// </summary>
		/// <remarks>Fatal log entries indicate unrecoverable errors. These logs are typically used for diagnostics
		/// and should be reserved for situations where the application cannot continue.</remarks>
		/// <param name="exception">The exception associated with the fatal event, or <see langword="null"/> if no exception is available.</param>
		/// <param name="message">The message template describing the fatal event. May include placeholders for property values.</param>
		/// <param name="propertyValues">An array of objects to be formatted into the message template as property values.</param>
		public static void LogFatal(Exception? exception, string message, params object[] propertyValues)
		{
			Log.Fatal(exception, message, propertyValues);
		}
	}
}
