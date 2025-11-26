namespace BuildHub.Common.Application
{
	using Logger;

	public static class Application
	{
		public static void Exit()
		{
			Logger.LogInformation("Build-Hub has been shutdown.");
			Environment.Exit(0);
		}

		public static void ExitWithError(Exception? exception, string message, params object[] propertyValues)
		{
			Logger.LogError(exception, message, propertyValues);
			Logger.LogError("Build-Hub has been shutdown due to an error.");
			Environment.Exit(0);
		}

		public static void ExitWithError(string message, params object[] propertyValues)
		{
			Logger.LogError(message, propertyValues);
			Logger.LogError("Build-Hub has been shutdown due to an error.");
			Environment.Exit(0);
		}
	}
}
