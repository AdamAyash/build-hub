namespace BuildHub.CommandBuilder.Models.Execution.Enums;

/// <summary>
/// Supported Visual Studio versions.
/// </summary>
public enum VisualStudioVersions
{
	VS2008 = 0,
	VS2010 = 1,
	VS2017 = 2,
	VS2019 = 3,
	VS2022 = 4,
	VS2026 = 5,
}

/// <summary>
/// Provides Visual Studio–related helper methods.
/// </summary>
public static class VisualStudioInfo
{
	/// <summary>
	/// Returns the compiler code name (vc toolset) for the specified Visual Studio version.
	/// </summary>
	/// <param name="tool">The Visual Studio version.</param>
	/// <returns>
	/// A Visual C++ toolset code name (e.g. "vc15", "vc16").
	/// Defaults to "vc15" if the value is null or unknown.
	/// </returns>
	public static string GetCodeName(VisualStudioVersions? tool)
	{
		switch (tool)
		{
			case VisualStudioVersions.VS2008:
				return "vc9";
			case VisualStudioVersions.VS2010:
				return "vc10";
			case VisualStudioVersions.VS2017:
				return "vc15";
			case VisualStudioVersions.VS2019:
				return "vc16";
			case VisualStudioVersions.VS2022:
				return "vc17";
			default:
				return "vc15";
		}
	}

	/// <summary>
	/// Returns the <see cref="VisualStudioVersions"/> value for the specified compiler code name.
	/// </summary>
	/// <param name="codeName">
	/// The compiler code name (e.g. "vc9", "vc10", "vc15", "vc16", "vc17").
	/// </param>
	/// <returns>
	/// The corresponding <see cref="VisualStudioVersions"/> value.
	/// </returns>
	/// <exception cref="FormatException">
	/// Thrown when the code name is null, empty, or not recognized.
	/// </exception>
	public static VisualStudioVersions GetVersionFromCodeName(string codeName)
	{
		if (string.IsNullOrEmpty(codeName))
			throw new FormatException("Code name cannot be null or empty.");

		switch (codeName)
		{
			case "vc9":
				return VisualStudioVersions.VS2008;
			case "vc10":
				return VisualStudioVersions.VS2010;
			case "vc15":
				return VisualStudioVersions.VS2017;
			case "vc16":
				return VisualStudioVersions.VS2019;
			case "vc17":
				return VisualStudioVersions.VS2022;
			default:
				throw new FormatException($"Unknown Visual Studio code name: '{codeName}'.");
		}
	}
}
