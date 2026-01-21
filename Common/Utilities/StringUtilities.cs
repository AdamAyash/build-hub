namespace BuildHub.Common.Utilities;

public class StringUtilities
{
    /// <summary>
	/// Wraps a value in double quotes for safe use in command-line arguments.
	/// </summary>
	/// <param name="value">Value to be quoted.</param>
	/// <returns>The quoted value.</returns>
	public static string AddQuotes(string value)
	{
		return $@"""{value}""";
	}
}
