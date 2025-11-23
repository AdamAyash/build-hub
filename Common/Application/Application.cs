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

		public static void ExitWithError()
		{
			Environment.Exit(0);
			Logger.LogError("Build-Hub has been shutdown due to an error.");
		}
	}
}
