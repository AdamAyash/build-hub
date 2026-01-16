using BuildHub.CommandBuilder.Models;

namespace TestProject.CommandBuilders.General
{
	[TestClass]
	public class BuildVersionTests
	{
		[TestMethod]
		public void Constructor_ShouldInitializeWithDefaultValues()
		{
			// Act
			var version = new BuildVersion();

			// Assert
			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(1, version.Minor);
			Assert.AreEqual(1, version.LastBuild);
		}

		[TestMethod]
		public void IncrementVersion_ShouldIncreaseLastBuildByOne()
		{
			// Arrange
			var version = new BuildVersion();

			// Act
			version.IncrementVersion();

			// Assert
			Assert.AreEqual(2, version.LastBuild);
		}

		[TestMethod]
		public void IncrementVersion_CalledMultipleTimes_ShouldAccumulateCorrectly()
		{
			// Arrange
			var version = new BuildVersion();

			// Act
			version.IncrementVersion();
			version.IncrementVersion();
			version.IncrementVersion();

			// Assert
			Assert.AreEqual(4, version.LastBuild);
		}

		[TestMethod]
		public void GetStringVersion_ShouldReturnCorrectFormat()
		{
			// Arrange
			var version = new BuildVersion
			{
				Major = 2,
				Minor = 5,
				LastBuild = 9
			};

			// Act
			var result = version.GetStringVersion();

			// Assert
			Assert.AreEqual("2.5.9", result);
		}

		[TestMethod]
		public void GetStringVersion_DefaultValues_ShouldReturn111()
		{
			// Arrange
			var version = new BuildVersion();

			// Act
			var result = version.GetStringVersion();

			// Assert
			Assert.AreEqual("1.1.1", result);
		}

		[TestMethod]
		public void SetFromString_ValidVersion_UpdatesProperties()
		{
			var version = new BuildVersion();
			string input = "2.5.10";

			version.SetFromString(input);

			Assert.AreEqual(2, version.Major);
			Assert.AreEqual(5, version.Minor);
			Assert.AreEqual(10, version.LastBuild);
		}

		[TestMethod]
		[DataRow("1.2")]
		[DataRow("1.2.3.4")]
		[DataRow("a.b.c")]
		[DataRow("1..2")]
		[DataRow("")]
		[DataRow(null)]
		public void SetFromString_InvalidFormat_ThrowsFormatException(string input)
		{
			var version = new BuildVersion();
			Assert.Throws<FormatException>(() => version.SetFromString(input));
		}

		[TestMethod]
		public void SetFromString_LeadingZeros_ParsesCorrectly()
		{
			var version = new BuildVersion();
			string input = "01.02.003";

			version.SetFromString(input);

			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(2, version.Minor);
			Assert.AreEqual(3, version.LastBuild);
		}

		[TestMethod]
		public void SetFromString_ReturnsSelf()
		{
			var version = new BuildVersion();
			string input = "1.2.3";

			var result = version.SetFromString(input);

			Assert.AreSame(version, result);
		}
	}
}
