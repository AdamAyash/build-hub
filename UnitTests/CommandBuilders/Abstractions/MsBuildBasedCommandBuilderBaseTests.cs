using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models.Contexts;
using BuildHub.CommandBuilder.Models.Enums;
using BuildHub.CommandBuilder.Models.Execution;

namespace UnitTests.CommandBuilders.Abstractions
{
	[TestClass]
	public sealed class MsBuildBasedCommandBuilderBaseTests
	{
		// Concrete context for testing (since MsBuildBasedContext is abstract).
		private sealed class TestMsBuildContext : MsBuildBasedContext
		{
			// No extra members needed.
		}

		private sealed class TestBuilder : MsBuildBasedCommandBuilderBase<TestBuilder, TestMsBuildContext>
		{
			public bool DerivedValidated { get; private set; }

			public TestBuilder(TestMsBuildContext ctx) : base(ctx)
			{
				SetName("TestMsBuildBased");
			}

			protected override void ValidateDerived()
			{
				DerivedValidated = true;
			}

			protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
				=> Array.Empty<ExecutionStep>();

			public string ExposeConfigurationPlatform() => GetConfigurationPlatform();
		}

		[TestMethod]
		public void ValidateCore_WhenPlatformIsEmpty_Throws()
		{
			var ctx = new TestMsBuildContext
			{
				Platform = "",
				Target = "a.sln",
				Flavor = BuildFlavor.Debug,
				BuildType = BuildType.Build
			};

			var b = new TestBuilder(ctx);

			var ex = Assert.Throws<InvalidOperationException>(() => b.ValidateAll());
			StringAssert.Contains(ex.Message, "Build platform cannot be empty");
		}

		[TestMethod]
		public void ValidateCore_WhenTargetIsEmpty_Throws()
		{
			var ctx = new TestMsBuildContext
			{
				Platform = "x64",
				Target = "",
				Flavor = BuildFlavor.Release,
				BuildType = BuildType.Build
			};

			var b = new TestBuilder(ctx);

			var ex = Assert.Throws<InvalidOperationException>(() => b.ValidateAll());
			StringAssert.Contains(ex.Message, "Build target cannot be empty");
		}

		[TestMethod]
		public void ValidateAll_WhenCoreValid_CallsValidateDerived()
		{
			var ctx = new TestMsBuildContext
			{
				Platform = "AnyCPU",
				Target = "a.sln",
				Flavor = BuildFlavor.Release,
				BuildType = BuildType.Build
			};

			var b = new TestBuilder(ctx);

			b.ValidateAll();

			Assert.IsTrue(b.DerivedValidated, "Expected ValidateDerived() to be invoked when core validation passes.");
		}

		[TestMethod]
		public void GetConfigurationPlatform_FormatsFlavorPipePlatform()
		{
			var ctx = new TestMsBuildContext
			{
				Platform = "Win32",
				Target = "a.sln",
				Flavor = BuildFlavor.Release,
				BuildType = BuildType.Build
			};

			var b = new TestBuilder(ctx);

			Assert.AreEqual("Release|Win32", b.ExposeConfigurationPlatform());
		}

		[TestMethod]
		public void FluentSetters_UpdateContext_AndReturnSameBuilderInstance()
		{
			var ctx = new TestMsBuildContext
			{
				Platform = "Win32",
				Target = "old.sln",
				Flavor = BuildFlavor.Debug,
				BuildType = BuildType.Build
			};

			var b = new TestBuilder(ctx);

			var returned =
				b.SetPlatform("x64")
				 .SetTarget("new.sln")
				 .SetBuildFlavor(BuildFlavor.Release)
				 .SetBuildType(BuildType.Rebuild);

			Assert.AreSame(b, returned, "Fluent setters should return the same builder instance.");

			Assert.AreEqual("x64", ctx.Platform);
			Assert.AreEqual("new.sln", ctx.Target);
			Assert.AreEqual(BuildFlavor.Release, ctx.Flavor);
			Assert.AreEqual(BuildType.Rebuild, ctx.BuildType);
		}
	}
}
