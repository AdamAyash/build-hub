using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Execution;

namespace UnitTests.CommandBuilders.Models
{
	[TestClass]
	public sealed class ExecutionPlanTests
	{
		private static ExecutionPlan Create(
			Guid? id = null,
			string name = "Plan",
			BuildVersion? version = null,
			List<ExecutionStep>? steps = null)
		{
			var p = new ExecutionPlan
			{
				Id = id ?? Guid.NewGuid(),
				Name = name,
				Steps = steps ?? []
			};

			if (version is not null)
			{
				p.Version = version;
			}

			return p;
		}

		[TestMethod]
		public void Defaults_AreExpected_AndCollectionsAreNonNull()
		{
			// Ensures we execute property initializer lines:
			// Name = string.Empty, Version = new(...), Steps = Array.Empty(...)
			var p = new ExecutionPlan();

			Assert.AreEqual(Guid.Empty, p.Id);
			Assert.AreEqual(string.Empty, p.Name);

			Assert.IsNotNull(p.Version, "Version should be initialized by default.");
			Assert.IsNotNull(p.Steps);
			Assert.IsEmpty(p.Steps);
		}

		[TestMethod]
		[DataRow("")]
		[DataRow("Build Plan")]
		[DataRow("Release Pipeline - Windows")]
		public void AssignedValues_AreReadable(string name)
		{
			var id = Guid.NewGuid();

			var p = new ExecutionPlan
			{
				Id = id,
				Name = name
			};

			Assert.AreEqual(id, p.Id);
			Assert.AreEqual(name, p.Name);
		}

		[TestMethod]
		public void Steps_CanBeAssigned_AndPreserved()
		{
			var step1 = new ExecutionStep { Id = Guid.NewGuid(), Order = 1, Name = "Fetch", StepType = "tfs" };
			var step2 = new ExecutionStep { Id = Guid.NewGuid(), Order = 2, Name = "Build", StepType = "msbuild" };

			var plan = Create(
				name: "MyPlan",
				steps: new List<ExecutionStep> { step1, step2 });

			Assert.AreEqual("MyPlan", plan.Name);
			Assert.IsNotNull(plan.Steps);
			Assert.HasCount(2, plan.Steps);
			Assert.AreSame(step1, plan.Steps[0]);
			Assert.AreSame(step2, plan.Steps[1]);
		}

		[TestMethod]
		public void Version_IsInitialized_ByDefault_AndIsStableAcrossReads()
		{
			var plan = new ExecutionPlan();

			var v1 = plan.Version;
			var v2 = plan.Version;

			Assert.IsNotNull(v1);
			Assert.AreSame(v1, v2, "Version should refer to the same instance unless replaced.");
		}

		[TestMethod]
		public void CreateHelper_AllowsCustomIdNameAndEmptySteps()
		{
			var id = Guid.NewGuid();

			var plan = Create(id: id, name: "Custom", steps: null);

			Assert.AreEqual(id, plan.Id);
			Assert.AreEqual("Custom", plan.Name);
			Assert.IsNotNull(plan.Version);
			Assert.IsNotNull(plan.Steps);
			Assert.IsEmpty(plan.Steps);
		}
	}
}
