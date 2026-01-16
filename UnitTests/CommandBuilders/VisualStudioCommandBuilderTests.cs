using BuildHub.CommandBuilder.CommandBuilders;
using BuildHub.CommandBuilder.Models.Enums;

namespace TestProject.CommandBuilders
{
	[TestClass]
	public class VisualStudioCommandBuilderTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", "C:\\log.txt")]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", "C:\\log.txt")]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", "C:\\log.txt")]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", "C:\\log.txt")]
		[DataRow(BuildType.Build, BuildFlavor.Release, "x64", null)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Release, "x86", null)]
		[DataRow(BuildType.Build, BuildFlavor.Debug, "AnyCPU", null)]
		[DataRow(BuildType.Rebuild, BuildFlavor.Debug, "AnyCPU", null)]
		public void VisualStudioCommandBuilder_GenerateCommand_VariousConfigurations(
			BuildType buildType, BuildFlavor flavor, string platform, string outputFile)
		{
			var builder = new VisualStudioCommandBuilder()
				.SetBuildType(buildType)
				.SetBuildFlavor(flavor)
				.SetPlatform(platform)
				.SetTarget("MyProject")
				.SetOutputFile(outputFile);

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

			StringAssert.Contains(command, $"/{buildType}");
			StringAssert.Contains(command, "\"MyProject\"");
			StringAssert.Contains(command, $"{flavor}|{platform}");

			if (outputFile != null)
				StringAssert.Contains(command, "\"C:\\log.txt\"");

			TestContext.WriteLine(result.ToString());
		}

		[TestMethod]
		[DataRow(null, "x64", BuildFlavor.Release, "C:\\log.txt")]
		[DataRow("Proj", null, BuildFlavor.Release, "C:\\log.txt")]
		[DataRow(null, null, null, null)]
		public void GenerateCommand_InvalidMandatoryFields_Throws(
		   string target, string platform, BuildFlavor flavor, string outputFile)
		{
			var builder = new VisualStudioCommandBuilder()
				.SetTarget(target)
				.SetPlatform(platform)
				.SetOutputFile(outputFile)
				.SetBuildFlavor(flavor);

			Assert.Throws<InvalidOperationException>(() => builder.GenerateCommand());
		}

		[TestMethod]
		public void FluentApi_ReturnsConcreteBuilderType()
		{
			var builder = new VisualStudioCommandBuilder()
				.SetPlatform("x64")
				.SetOutputFile("out.log");

			Assert.IsInstanceOfType(builder, typeof(VisualStudioCommandBuilder));
		}
	}
}
