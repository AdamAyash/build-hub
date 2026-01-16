using BuildHub.CommandBuilder.CommandBuilders;

namespace TestProject.CommandBuilders
{
	[TestClass]
	public class TfsCommandBuilderTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TfsCommandBuilder_GenerateCommand_Defaults()
		{
			var builder = new TfsCommandBuilder()
				.SetWorkspacePath("C:\\workspace");

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

			StringAssert.StartsWith(command, "tf get \"C:\\workspace\"");
			StringAssert.Contains(command, "/version:T");
			StringAssert.Contains(command, "/recursive");
			StringAssert.Contains(command, "/force");

			TestContext.WriteLine(result.ToString());
		}

		[TestMethod]
		public void TfsCommandBuilder_GenerateCommand_WithChangesetAndOptions()
		{
			var builder = new TfsCommandBuilder()
				.SetWorkspacePath("C:\\workspace")
				.SetChangeset("1234")
				.SetRecursive(false)
				.SetForce(false);

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

			StringAssert.Contains(command, "/version:C1234");
			Assert.DoesNotContain("/recursive", command);
			Assert.DoesNotContain("/force", command);

			TestContext.WriteLine(result.ToString());
		}

		[TestMethod]
		public void TfsCommandBuilder_MissingWorkspace_Throws()
		{
			var builder = new TfsCommandBuilder();
			Assert.Throws<InvalidOperationException>(() => builder.GenerateCommand());
		}
	}
}
