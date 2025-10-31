
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public class TestOutputLoggerTests
{
	[Fact]
	public void Log_ShouldWriteMessageToOutput()
	{
		//--- ARRANGE ---------------------------------------------------------
		Mock<ITestOutputHelper> testOutputHelperMock = new();
		TestOutputLogger logger = new(testOutputHelperMock.Object);

		//--- ACT -------------------------------------------------------------
		logger.Log(
			LogLevel.Information
			, new EventId(1)
			, "Test message"
			, null
			, (state, exception) => state.ToString());

		//--- ASSERT ----------------------------------------------------------
		testOutputHelperMock.Verify(
			x => x.WriteLine("Test message")
			, Times.Exactly(1));
	}

	[Fact]
	public void Log_ShouldWriteMessageToOutputWithIndentation()
	{
		//--- ARRANGE ---------------------------------------------------------
		Mock<ITestOutputHelper> testOutputHelperMock = new();
		TestOutputLogger logger = new(testOutputHelperMock.Object);

		//--- ACT -------------------------------------------------------------
		using (logger.BeginScope("Scope"))
		{
			logger.Log(
				LogLevel.Information
				, new EventId(1)
				, "Test message"
				, null
				, (state, exception) => state.ToString());
		}

		//--- ASSERT ----------------------------------------------------------
		testOutputHelperMock
			.Verify(x => x.WriteLine("  Test message")
			, Times.Exactly(1));
	}

	[Theory]
	[InlineData(LogLevel.Trace, true)]
	[InlineData(LogLevel.Debug, true)]
	[InlineData(LogLevel.Information, true)]
	[InlineData(LogLevel.Warning, true)]
	[InlineData(LogLevel.Error, true)]
	[InlineData(LogLevel.Critical, true)]
	public void IsEnabled_ShouldReturnCorrectValue(LogLevel logLevel, bool expected)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestOutputLogger logger = new(Mock.Of<ITestOutputHelper>());

		//--- ACT -------------------------------------------------------------
		bool result = logger.IsEnabled(logLevel);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expected, result);
	}
}
