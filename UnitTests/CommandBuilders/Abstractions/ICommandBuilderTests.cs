using BuildHub.CommandBuilder.CommandBuilders.Abstractions;
using BuildHub.CommandBuilder.Models;

namespace UnitTests.CommandBuilders.Abstractions
{
	[TestClass]
	public sealed class ICommandBuilderContractTests
	{
		private sealed class FakeBuilder : ICommandBuilder
		{
			public bool ValidateWasCalled { get; private set; }
			public bool GenerateWasCalled { get; private set; }

			public void ValidateAll() => ValidateWasCalled = true;

			public BuildCommandResult GenerateCommand()
			{
				GenerateWasCalled = true;
				return BuildCommandResult.OnSuccess(string.Empty, TimeSpan.Zero, []);
			}
		}

		[TestMethod]
		public void ICommandBuilder_ValidateAll_IsCallable()
		{
			ICommandBuilder builder = new FakeBuilder();

			builder.ValidateAll();

			Assert.IsTrue(((FakeBuilder)builder).ValidateWasCalled);
		}

		[TestMethod]
		public void ICommandBuilder_GenerateCommand_IsCallable_AndReturnsResult()
		{
			ICommandBuilder builder = new FakeBuilder();

			var result = builder.GenerateCommand();

			Assert.IsTrue(((FakeBuilder)builder).GenerateWasCalled);
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void ICommandBuilder_Allows_ValidateThenGenerate_Flow()
		{
			ICommandBuilder builder = new FakeBuilder();

			builder.ValidateAll();
			var result = builder.GenerateCommand();

			Assert.IsNotNull(result);
		}
	}
}