using BuildHub.CommandBuilder.CommandBuilders;
using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Enums;

namespace TestProject.CommandBuilders
{
	[TestClass]
	public class MSBuildCommandBuilderTests
	{
		public TestContext TestContext { get; set; }

		private BuildVersion SampleVersion => new BuildVersion
		{
			Major = 1,
			Minor = 2,
			LastBuild = 3
		};

		[TestMethod]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64")]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86")]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU")]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU")]
		public void GenerateCommand_VariousConfigurations_GeneratesCorrectCommand(
			BuildType buildType,
			BuildFlavor flavor,
			string platform)
		{
			var builder = new MSBuildCommandBuilder()
				.SetBuildType(buildType)
				.SetBuildFlavor(flavor)
				.SetPlatform(platform)
				.SetTarget("My Project")
				.SetVersion(SampleVersion)
				.SetExtraParams(new Dictionary<string, string> { { "param1", "/p:Custom=true" } });

			var result = builder.GenerateCommand();

			Assert.IsTrue(result.Success);
			Assert.HasCount(1, result.Steps);

			var step = result.Steps.First();

			Assert.IsNotNull(step);
			Assert.HasCount(1, step.Commands);

			var executeCommand = step.Commands.First();

			Assert.IsNotNull(executeCommand);
			Assert.IsFalse(string.IsNullOrEmpty(executeCommand.Arguments));
			Assert.IsFalse(string.IsNullOrWhiteSpace(executeCommand.Arguments));

			var command = executeCommand.Arguments;

			StringAssert.Contains(command, "\"My Project\"");
			StringAssert.Contains(command, $"/p:Configuration=\"{flavor}\"");
			StringAssert.Contains(command, $"/p:Platform=\"{platform}\"");
			StringAssert.Contains(command, "/p:VersionAssembly=1.2.3");
			StringAssert.Contains(command, $"/t:{buildType.ToString().ToLower()}");
			StringAssert.Contains(command, "-restore");
			StringAssert.Contains(command, "/p:Custom=true");

			TestContext.WriteLine(result.ToString());
		}

		[TestMethod]
		[DataRow(null, "x64", BuildFlavor.Release, "1.2.3")]
		[DataRow("Proj", null, BuildFlavor.Release, "1.2.3")]
		[DataRow("Proj", "x64", BuildFlavor.Release, null)]
		[DataRow(null, null, null, null)]
		public void GenerateCommand_InvalidMandatoryFields_Throws(
			string target, string platform, BuildFlavor flavor, string version)
		{
			var builder = new MSBuildCommandBuilder()
				.SetTarget(target)
				.SetPlatform(platform)
				.SetBuildFlavor(flavor);

			if (version == null)
				Assert.Throws<FormatException>(() => builder.SetVersion(version));
			else
				Assert.Throws<InvalidOperationException>(() => builder.SetVersion(version).GenerateCommand());
		}

		[TestMethod]
		public void FluentApi_ReturnsConcreteBuilderType()
		{
			var builder = new MSBuildCommandBuilder()
				.SetPlatform("x64");

			Assert.IsInstanceOfType(builder, typeof(MSBuildCommandBuilder));
		}
	}
}
