using System.Text.RegularExpressions;

namespace BuildHub.CommandBuilder.Models;

public class BuildVersion
{
	/// <summary> Gets or sets the major version number. </summary>
	public int Major { get; set; }

	/// <summary> Gets or sets the minor version number. </summary>
	public int Minor { get; set; }

	/// <summary> Gets or sets the last build (revision) number. </summary>
	public int LastBuild { get; set; }

	public BuildVersion()
	{
		Major = 1;
		Minor = 1;
		LastBuild = 1;
	}

	/// <summary>
	/// Increments the <see cref="LastBuild"/> value by one.
	/// </summary>
	public void IncrementVersion()
	{
		LastBuild++;
	}

	/// <summary>
	/// Returns the version formatted as a string in the form:
	/// Major.Minor.LastBuild (e.g. 1.2.3).
	/// </summary>
	/// <returns>
	/// A string representation of the version.
	/// </returns>
	public string GetStringVersion()
	{
		return $"{Major}.{Minor}.{LastBuild}";
	}

	/// <summary>
	/// Sets <see cref="Major"/>, <see cref="Minor"/>, and <see cref="LastBuild"/> from a "Major.Minor.LastBuild" string.
	/// </summary>
	/// <param name="version">Version string, e.g. "1.2.3".</param>
	/// <returns>The current <see cref="BuildVersion"/> instance.</returns>
	public BuildVersion SetFromString(string version)
	{
		if (version == null || !Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"))
			throw new FormatException($"Invalid version string: '{version}'. Expected format is 'Major.Minor.LastBuild'.");

		var parts = version.Split('.');

		Major = int.Parse(parts[0]);
		Minor = int.Parse(parts[1]);
		LastBuild = int.Parse(parts[2]);

		return this;
	}
}
