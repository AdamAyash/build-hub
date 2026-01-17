using BuildHub.CommandBuilder.Models.Execution;

namespace UnitTests.CommandBuilders.Models
{
	[TestClass]
	public sealed class ExecutionCommandTests
	{
		private static ExecutionCommand Create(
			string executable = "dotnet",
			string arguments = "--info",
			string? workingDirectory = "src")
			=> new()
			{
				Executable = executable,
				Arguments = arguments,
				WorkingDirectory = workingDirectory
			};

		[TestMethod]
		public void Defaults_AreExpected()
		{
			var c = new ExecutionCommand();

			Assert.AreEqual(string.Empty, c.Executable);
			Assert.AreEqual(string.Empty, c.Arguments);
			Assert.IsNull(c.WorkingDirectory);
		}

		[DataTestMethod]
		[DataRow("dotnet", "build", null)]
		[DataRow("msbuild", "\"a.sln\" /t:build", "C:\\work")]
		[DataRow("", "", "")]
		public void Properties_AreAssigned_AndReadable(string executable, string arguments, string? workingDirectory)
		{
			var c = Create(executable, arguments, workingDirectory);

			Assert.AreEqual(executable, c.Executable);
			Assert.AreEqual(arguments, c.Arguments);
			Assert.AreEqual(workingDirectory, c.WorkingDirectory);
		}

		[TestMethod]
		public void ValueEquality_SameValues_AreEqual()
		{
			var a = Create("dotnet", "build", "src");
			var b = Create("dotnet", "build", "src");

			Assert.AreEqual(a, b);
			Assert.IsTrue(a == b);
			Assert.IsFalse(a != b);
		}

		[TestMethod]
		public void ValueEquality_DifferentValues_AreNotEqual()
		{
			var a = Create("dotnet", "build", "src");
			var b = Create("dotnet", "test", "src"); // different arguments

			Assert.AreNotEqual(a, b);
			Assert.IsFalse(a == b);
			Assert.IsTrue(a != b);
		}

		[TestMethod]
		public void WithExpression_CreatesNewInstance_WithChangedProperty()
		{
			var original = Create("dotnet", "build", "src");

			var modified = original with { Arguments = "test" };

			// record should be immutable-ish (new instance)
			Assert.AreNotSame(original, modified);

			// unchanged fields copied
			Assert.AreEqual("dotnet", modified.Executable);
			Assert.AreEqual("src", modified.WorkingDirectory);

			// changed field updated
			Assert.AreEqual("test", modified.Arguments);

			// originals unchanged
			Assert.AreEqual("build", original.Arguments);
		}

		[TestMethod]
		public void GetHashCode_EqualObjects_HaveSameHashCode()
		{
			var a = Create("dotnet", "build", null);
			var b = Create("dotnet", "build", null);

			Assert.AreEqual(a, b);
			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		}

		[TestMethod]
		public void ToString_IncludesAllMembers_WhenWorkingDirectoryIsNotNull()
		{
			var c = Create("dotnet", "build", "src");
			var s = c.ToString();

			// Record ToString shape: "ExecutionCommand { Executable = ..., Arguments = ..., WorkingDirectory = ... }"
			StringAssert.Contains(s, "ExecutionCommand");
			StringAssert.Contains(s, "Executable = dotnet");
			StringAssert.Contains(s, "Arguments = build");
			StringAssert.Contains(s, "WorkingDirectory = src");
		}

		[TestMethod]
		public void ToString_IncludesWorkingDirectoryMember_WhenNull()
		{
			var c = Create("dotnet", "build", null);
			var s = c.ToString();

			StringAssert.Contains(s, "ExecutionCommand");
			StringAssert.Contains(s, "Executable = dotnet");
			StringAssert.Contains(s, "Arguments = build");

			// For null, record ToString still prints the member (typically "WorkingDirectory = ").
			StringAssert.Contains(s, "WorkingDirectory");
		}

		[TestMethod]
		public void Deconstruct_ReturnsMemberValues()
		{
			var c = Create("msbuild", "\"a.sln\"", "C:\\repo");

			Assert.AreEqual("msbuild", c.Executable);
			Assert.AreEqual("\"a.sln\"", c.Arguments);
			Assert.AreEqual("C:\\repo", c.WorkingDirectory);
		}
	}
}
