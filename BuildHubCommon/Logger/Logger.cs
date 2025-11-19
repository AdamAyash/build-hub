using Serilog;

namespace BuildHubCommon.Logger
{
    public sealed class Logger
    {
        public static Logger? _loggerInstance = null;
        private static Serilog.Core.Logger? _internalLogger = null;

        private Logger()
        {
            Initialize();
        }

        private void Initialize()
        {
            _internalLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        }

        public static Logger GetInstance()
        {
            if(_loggerInstance is null)
                _loggerInstance = new Logger();

            return _loggerInstance;
        }
    }
}
