using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models.Execution;

namespace UnitTests.CommandBuilders.Abstractions
{

	using static BuildHub.Common.Utilities.StringUtilities;

	[TestClass]
	public sealed class CommandBuilderBaseTests
	{
		private sealed class TestContext : ICommandBuilderContext
		{
			public string Value { get; set; } = "ok";
		}

		private sealed class GoodBuilder : CommandBuilderBase<GoodBuilder, TestContext>
		{
			protected override string Name => "GoodBuilder";

			public bool CoreValidated { get; private set; }
			public bool DerivedValidated { get; private set; }
			public bool InternalCalled { get; private set; }

			public GoodBuilder(TestContext ctx) : base(ctx)
			{ }

			protected override void ValidateCore()
			{
				CoreValidated = true;
			}

			protected override void ValidateDerived()
			{
				DerivedValidated = true;
			}

			protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
			{
				InternalCalled = true;
				return new List<ExecutionStep>
			{
				new ExecutionStep
				{
					Id = Guid.NewGuid(),
					Order = 1,
					Name = "Step1",
					StepType = "test",
					Commands = Array.Empty<ExecutionCommand>(),
					DependsOn = Array.Empty<Guid>()
				}
			};
			}
		}

		private sealed class ThrowingBuilder : CommandBuilderBase<ThrowingBuilder, TestContext>
		{
			protected override string Name => "ThrowingBuilder";
			
			public bool DerivedValidated { get; private set; }

			public ThrowingBuilder(TestContext ctx) : base(ctx)
			{ }

			protected override void ValidateDerived()
			{
				DerivedValidated = true;
			}

			protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
			{
				throw new InvalidOperationException("kaboom");
			}
		}

		private sealed class ValidateThrowsBuilder : CommandBuilderBase<ValidateThrowsBuilder, TestContext>
		{
			protected override string Name => "ValidateThrowsBuilder";

			public ValidateThrowsBuilder(TestContext ctx) : base(ctx)
			{ }

			protected override void ValidateDerived()
			{
				throw new InvalidOperationException("bad validate");
			}

			protected override IReadOnlyList<ExecutionStep> GenerateCommandInternal()
				=> Array.Empty<ExecutionStep>();
		}

		[TestMethod]
		public void Ctor_WhenContextNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new GoodBuilder(null!));
		}

		[TestMethod]
		public void AddQuotes_WrapsValueInDoubleQuotes()
		{
			var b = new GoodBuilder(new TestContext());

			var quoted = AddQuotes(@"C:\Path With Space");

			Assert.AreEqual("\"C:\\Path With Space\"", quoted);
		}

		[TestMethod]
		public void ValidateAll_CallsValidateCore_AndValidateDerived()
		{
			var b = new GoodBuilder(new TestContext());

			b.Validate();

			Assert.IsTrue(b.CoreValidated, "ValidateCore should be called.");
			Assert.IsTrue(b.DerivedValidated, "ValidateDerived should be called.");
		}

		[TestMethod]
		public void GenerateCommand_CallsValidateAll_ThenGenerateCommandInternal_OnSuccess()
		{
			var b = new GoodBuilder(new TestContext());

			var result = b.GenerateCommand();

			// ValidateAll was invoked by GenerateCommand
			Assert.IsTrue(b.CoreValidated);
			Assert.IsTrue(b.DerivedValidated);

			// Internal generator was invoked
			Assert.IsTrue(b.InternalCalled);

			// Success result
			Assert.IsTrue(result.Success);
			Assert.AreEqual("GoodBuilder", result.BuilderName);
			Assert.IsNotNull(result.Steps);
			Assert.HasCount(1, result.Steps);
			Assert.AreEqual(string.Empty, result.ErrorMessage);
		}

		[TestMethod]
		public void GenerateCommand_WhenGenerateCommandInternalThrows_ReturnsFailure()
		{
			var b = new ThrowingBuilder(new TestContext());

			var result = b.GenerateCommand();

			Assert.IsFalse(result.Success);
			Assert.AreEqual("ThrowingBuilder", result.BuilderName);
			StringAssert.Contains(result.ErrorMessage, "kaboom");
			Assert.IsNotNull(result.Steps);
			Assert.IsEmpty(result.Steps);
		}

		[TestMethod]
		public void GenerateCommand_WhenValidateAllThrows_PropagatesException()
		{
			// Your base code calls ValidateAll() before try/catch (in your original),
			// and many designs keep it that way (validation errors should throw).
			// If you instead moved ValidateAll inside the try/catch, update this test accordingly.
			var b = new ValidateThrowsBuilder(new TestContext());

			var ex = Assert.Throws<InvalidOperationException>(() => b.GenerateCommand());
			StringAssert.Contains(ex.Message, "bad validate");
		}
	}
}
