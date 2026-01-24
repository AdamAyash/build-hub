namespace BuildHub.Common.Application
{
	using Logger;

	/// <summary>
	/// An static class holding utility methods for application management.
	/// </summary>
	public static class Application
	{
		/// <summary>
		/// Exits the application gracefully.
		/// </summary>
		public static void Exit()
		{
			Logger.LogInformation("Build-Hub has been shutdown.");
			Environment.Exit(0);
		}

		/// <summary>
		/// Exits the application with an error message.
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="message"></param>
		/// <param name="propertyValues"></param>
		public static void ExitWithError(Exception? exception, string message, params object[] propertyValues)
		{
			Logger.LogError(exception, message, propertyValues);
			Logger.LogError("Build-Hub has been shutdown due to an error.");
			Environment.Exit(0);
		}

		/// <summary>
		/// Exits the application with an error message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="propertyValues"></param>
		public static void ExitWithError(string message, params object[] propertyValues)
		{
			Logger.LogError(message, propertyValues);
			Logger.LogError("Build-Hub has been shutdown due to an error.");
			Environment.Exit(0);
		}

		/// <summary>
		/// Gets the unique identifier for the currently executing thread.
		/// </summary>
		/// <remarks>The managed thread ID is unique within the application domain and remains constant for the
		/// lifetime of the thread.</remarks>
		/// <returns>An integer representing the managed thread ID of the currently executing thread.</returns>
		public static int GetCurrentThreadId() => Thread.CurrentThread.ManagedThreadId;
	}
}
