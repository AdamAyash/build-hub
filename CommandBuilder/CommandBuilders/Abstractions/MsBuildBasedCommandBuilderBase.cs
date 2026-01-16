using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Enums;

namespace BuildHub.CommandBuilder.CommandBuilders.Abstractions;

/// <summary>
/// Base class for build-related command builders, providing shared
/// build configuration and validation logic.
/// </summary>
/// <remarks>
/// This class continues the use of self-referencing generics to ensure
/// fluent methods return the concrete builder type in derived build
/// command builders.
/// </remarks>
public abstract class MsBuildBasedCommandBuilderBase<TBuilder, TContext>
	: CommandBuilderBase<TBuilder, TContext>
	where TBuilder : MsBuildBasedCommandBuilderBase<TBuilder, TContext>
	where TContext : MsBuildBasedContext
{
	public MsBuildBasedCommandBuilderBase(TContext context)
		: base(context)
	{ }

	protected override void ValidateCore()
	{
		if (string.IsNullOrEmpty(Context.Platform))
		{ throw new InvalidOperationException("Build platform cannot be empty."); }

		if (string.IsNullOrEmpty(Context.Target))
		{ throw new InvalidOperationException("Build target cannot be empty."); }
	}

	/// <summary>
	/// Produces the combined configuration and platform string used by IncrediBuild.
	/// </summary>
	/// <returns>A configuration string in the form "Flavor|Platform".</returns>
	protected string GetConfigurationPlatform()
	{ return $"{Context.Flavor.ToString()}|{Context.Platform}"; }

	/// <summary>
	/// Specifies the build operation type.
	/// </summary>
	public TBuilder SetBuildType(BuildType buildType)
	{ Context.BuildType = buildType; return Self; }

	/// <summary>
	/// Specifies the build flavor used for the build.
	/// </summary>
	public TBuilder SetBuildFlavor(BuildFlavor flavor)
	{ Context.Flavor = flavor; return Self; }

	/// <summary>
	/// Specifies the target platform for the build.
	/// </summary>
	public TBuilder SetPlatform(string platform)
	{ Context.Platform = platform; return Self; }

	/// <summary>
	/// Specifies the build target executed by IncrediBuild.
	/// </summary>
	public TBuilder SetTarget(string target)
	{ Context.Target = target; return Self; }
}
