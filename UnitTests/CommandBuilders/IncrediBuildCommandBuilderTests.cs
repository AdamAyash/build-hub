using BuildHub.CommandBuilder.CommandBuilders;
using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Enums;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace TestProject.CommandBuilders
{
	[TestClass]
	public class IncrediBuildCommandBuilderTests
	{
		public TestContext TestContext { get; set; }

		private BuildVersion SampleVersion => new BuildVersion
		{
			Major = 1,
			Minor = 2,
			LastBuild = 3
		};

		[TestMethod]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2022, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2022, false)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2022, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2022, false)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2019, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2019, false)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2019, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2019, false)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2017, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2017, false)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2017, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2017, false)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2010, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2010, false)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2010, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2010, false)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2008, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2008, false)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2008, false)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2008, false)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2022, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2022, true)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2022, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2022, true)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2019, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2019, true)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2019, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2019, true)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2017, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2017, true)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2017, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2017, true)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2010, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2010, true)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2010, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2010, true)]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2008, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2008, true)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2008, true)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", VisualStudioVersions.VS2008, true)]
		public void GenerateCommand_VariousConfigurations_GeneratesCorrectCommand(
			BuildType buildType,
			BuildFlavor flavor,
			string platform,
			VisualStudioVersions vsVersion,
			bool useMsBuild)
		{
			var builder = new IncrediBuildCommandBuilder()
				.SetBuildType(buildType)
				.SetBuildFlavor(flavor)
				.SetPlatform(platform)
				.SetTarget("Proj")
				.SetOutputFile("C:\\log.txt")
				.SetVisualStudioVersion(vsVersion)
				.SetUseMsBuild(useMsBuild);

			if (useMsBuild)
				builder.SetVersion(SampleVersion);

			// Add extra params for testing MSBuild
			if (useMsBuild)
			{
				builder.SetExtraParams(new Dictionary<string, string> { { "param1", "/p:Custom=true" } });
			}

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

			// Validate target, platform, flavor
			StringAssert.Contains(command, "\"Proj\"");
			StringAssert.Contains(command, $"/Cfg=\"{flavor}|{platform}\"");
			StringAssert.Contains(command, "/Log=\"C:\\log.txt\"");
			StringAssert.Contains(command, $"/VsVersion={VisualStudioInfo.GetCodeName(vsVersion)}");

			// BuildType
			if (buildType == BuildType.Rebuild)
				StringAssert.Contains(command, "/Rebuild");
			else
				Assert.DoesNotContain("/Rebuild", command);

			// MSBuild
			if (useMsBuild)
			{
				StringAssert.Contains(command, "/UseMSBuild");
				StringAssert.Contains(command, "/msbuildargs=");
				StringAssert.Contains(command, "/p:VersionAssembly=1.2.3");
				StringAssert.Contains(command, "/p:Custom=true");
			}
			else
			{
				Assert.DoesNotContain("/UseMSBuild", command);
			}

			TestContext.WriteLine(result.ToString());
		}

		[TestMethod]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", VisualStudioVersions.VS2022, true, "1.2.3")]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", VisualStudioVersions.VS2022, true, "1.2.3")]
		public void GenerateCommand_VariousConfigurations_GeneratesCorrectCommandWithVersionString(
			BuildType buildType,
			BuildFlavor flavor,
			string platform,
			VisualStudioVersions vsVersion,
			bool useMsBuild,
			string version)
		{
			var builder = new IncrediBuildCommandBuilder()
				   .SetBuildType(buildType)
				   .SetBuildFlavor(flavor)
				   .SetPlatform(platform)
				   .SetTarget("Proj")
				   .SetOutputFile("C:\\log.txt")
				   .SetVisualStudioVersion(vsVersion)
				   .SetUseMsBuild(useMsBuild);

			if (useMsBuild)
				builder.SetVersion(version);

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

			// Validate target, platform, flavor
			StringAssert.Contains(command, "\"Proj\"");
			StringAssert.Contains(command, $"/Cfg=\"{flavor}|{platform}\"");
			StringAssert.Contains(command, "/Log=\"C:\\log.txt\"");
			StringAssert.Contains(command, $"/VsVersion={VisualStudioInfo.GetCodeName(vsVersion)}");

			// BuildType
			if (buildType == BuildType.Rebuild)
				StringAssert.Contains(command, "/Rebuild");
			else
				Assert.DoesNotContain("/Rebuild", command);

			// MSBuild
			if (useMsBuild)
			{
				StringAssert.Contains(command, "/UseMSBuild");
				StringAssert.Contains(command, "/msbuildargs=");
				StringAssert.Contains(command, "/p:VersionAssembly=1.2.3");
			}
			else
			{
				Assert.DoesNotContain("/UseMSBuild", command);
			}

			TestContext.WriteLine(result.ToString());
		}

		[TestMethod]
		[DataRow(null, "x64", BuildFlavor.Release, "C:\\log.txt")]
		[DataRow("Proj", null, BuildFlavor.Release, "C:\\log.txt")]
		[DataRow("Proj", "x64", BuildFlavor.Release, null)]
		[DataRow(null, null, null, null)]
		public void GenerateCommand_InvalidMandatoryFields_Throws(
			string target, string platform, BuildFlavor flavor, string outputFile)
		{
			var builder = new IncrediBuildCommandBuilder()
				.SetTarget(target)
				.SetPlatform(platform)
				.SetOutputFile(outputFile)
				.SetUseMsBuild(true)
				.SetBuildFlavor(flavor)
				.SetVersion(SampleVersion);

			Assert.Throws<InvalidOperationException>(() => builder.GenerateCommand());
		}

		[TestMethod]
		public void FluentApi_ReturnsConcreteBuilderType()
		{
			var builder = new IncrediBuildCommandBuilder()
				.SetPlatform("x64")
				.SetOutputFile("out.log");

			Assert.IsInstanceOfType(builder, typeof(IncrediBuildCommandBuilder));
		}
	}
}
