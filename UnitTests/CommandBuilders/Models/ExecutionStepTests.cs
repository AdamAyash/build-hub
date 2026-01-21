using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Execution;

namespace UnitTests.CommandBuilders.Models
{
	[TestClass]
	public sealed class ExecutionStepTests
	{
		private static ExecutionStep Create(
			Guid? id = null,
			int order = 1,
			string name = "Step",
			string stepType = "msbuild",
			IReadOnlyList<ExecutionCommand>? commands = null,
			IReadOnlyList<Guid>? dependsOn = null,
			object? metadata = null,
			RetryPolicy? retryPolicy = null,
			TimeoutPolicy? timeoutPolicy = null,
			IReadOnlyDictionary<string, string>? env = null)
		{
			return new ExecutionStep
			{
				Id = id ?? Guid.NewGuid(),
				Order = order,
				Name = name,
				StepType = stepType,
				Commands = commands ?? Array.Empty<ExecutionCommand>(),
				DependsOn = dependsOn ?? Array.Empty<Guid>(),
				Metadata = metadata,
				RetryPolicy = retryPolicy,
				TimeoutPolicy = timeoutPolicy,
				EnvironmentVariables = env
			};
		}

		[TestMethod]
		public void Defaults_AreExpected_AndCollectionsAreNonNull()
		{
			// This test ensures we execute the property initializer lines:
			// Name = string.Empty, StepType = string.Empty, Commands = Array.Empty, DependsOn = Array.Empty.
			var s = new ExecutionStep();

			Assert.AreEqual(Guid.Empty, s.Id);
			Assert.AreEqual(0, s.Order);
			Assert.AreEqual(string.Empty, s.Name);
			Assert.AreEqual(string.Empty, s.StepType);

			Assert.IsNotNull(s.Commands);
			Assert.IsEmpty(s.Commands);

			Assert.IsNotNull(s.DependsOn);
			Assert.IsEmpty(s.DependsOn);

			Assert.IsNull(s.Metadata);
			Assert.IsNull(s.RetryPolicy);
			Assert.IsNull(s.TimeoutPolicy);
			Assert.IsNull(s.EnvironmentVariables);
		}

		[TestMethod]
		[DataRow(0, "", "")]
		[DataRow(10, "Compile", "msbuild")]
		[DataRow(-1, "Fetch", "git")]
		public void AssignedValues_AreReadable(int order, string name, string stepType)
		{
			var id = Guid.NewGuid();

			var s = new ExecutionStep
			{
				Id = id,
				Order = order,
				Name = name,
				StepType = stepType
			};

			Assert.AreEqual(id, s.Id);
			Assert.AreEqual(order, s.Order);
			Assert.AreEqual(name, s.Name);
			Assert.AreEqual(stepType, s.StepType);
		}

		[TestMethod]
		public void CreateHelper_SupportsCommandsAndDependencies()
		{
			var cmd1 = new ExecutionCommand { Executable = "dotnet", Arguments = "build", WorkingDirectory = "src" };
			var cmd2 = new ExecutionCommand { Executable = "dotnet", Arguments = "test", WorkingDirectory = "src" };

			var dep1 = Guid.NewGuid();
			var dep2 = Guid.NewGuid();

			var step = Create(
				order: 5,
				name: "Build & Test",
				stepType: "dotnet",
				commands: new[] { cmd1, cmd2 },
				dependsOn: new[] { dep1, dep2 });

			Assert.AreEqual(5, step.Order);
			Assert.AreEqual("Build & Test", step.Name);
			Assert.AreEqual("dotnet", step.StepType);

			Assert.HasCount(2, step.Commands);
			Assert.AreEqual("dotnet", step.Commands[0].Executable);
			Assert.AreEqual("test", step.Commands[1].Arguments);

			CollectionAssert.AreEquivalent(new[] { dep1, dep2 }, step.DependsOn.ToArray());
		}

		[TestMethod]
		public void OptionalProperties_CanBeSet_ToCoverNullableLines()
		{
			var retry = new RetryPolicy { MaxAttempts = 3, Delay = TimeSpan.FromSeconds(1) };
			var timeout = new TimeoutPolicy { Timeout = TimeSpan.FromSeconds(30), KillProcessTree = true };
			var env = new Dictionary<string, string>
			{
				["CI"] = "true",
				["CONFIG"] = "Release"
			};

			var metadata = new { Key = "Value" };

			var step = Create(
				metadata: metadata,
				retryPolicy: retry,
				timeoutPolicy: timeout,
				env: env);

			Assert.IsNotNull(step.Metadata);
			Assert.AreEqual("Value", step.Metadata!.GetType().GetProperty("Key")!.GetValue(step.Metadata));

			Assert.IsNotNull(step.RetryPolicy);
			Assert.AreEqual(3, step.RetryPolicy!.MaxAttempts);

			Assert.IsNotNull(step.TimeoutPolicy);
			Assert.AreEqual(TimeSpan.FromSeconds(30), step.TimeoutPolicy!.Timeout);
			Assert.IsTrue(step.TimeoutPolicy!.KillProcessTree);

			Assert.IsNotNull(step.EnvironmentVariables);
			Assert.AreEqual("true", step.EnvironmentVariables!["CI"]);
			Assert.AreEqual("Release", step.EnvironmentVariables["CONFIG"]);
		}

		[TestMethod]
		public void Commands_And_DependsOn_CanBeLeftAsDefault_EmptyArrays()
		{
			var step = Create(commands: null, dependsOn: null);

			Assert.IsNotNull(step.Commands);
			Assert.IsEmpty(step.Commands);

			Assert.IsNotNull(step.DependsOn);
			Assert.IsEmpty(step.DependsOn);
		}
	}

}
