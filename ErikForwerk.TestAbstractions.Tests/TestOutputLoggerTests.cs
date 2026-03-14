
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestOutputLoggerTests
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

	[Fact]
	public void TestOutputLogger_BeginScope_ShouldAlterIndentationAccordingly()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestOutputCollector testOutputCollector	= new();
		TestOutputLogger sut					= new(testOutputCollector);

		//--- ACT -------------------------------------------------------------
		sut.Log(LogLevel.Information, new EventId(1), "No indent", null, (s, e) => s.ToString());

		using (sut.BeginScope("Scope1"))
		{
			sut.Log(LogLevel.Information, new EventId(2), "One indent", null, (s, e) => s.ToString());

			using (sut.BeginScope("Scope2"))
			{
				sut.Log(LogLevel.Information, new EventId(3), "Two indents", null, (s, e) => s.ToString());
			}

			sut.Log(LogLevel.Information, new EventId(4), "One indent again", null, (s, e) => s.ToString());
		}

		sut.Log(LogLevel.Information, new EventId(5), "No indent again", null, (s, e) => s.ToString());

		//--- ASSERT ----------------------------------------------------------
		Assert.Collection(testOutputCollector.Output,
			line => Assert.Equal("No indent", line),
			line => Assert.Equal("  One indent", line),
			line => Assert.Equal("    Two indents", line),
			line => Assert.Equal("  One indent again", line),
			line => Assert.Equal("No indent again", line));
	}

	[Fact]
	public void TestLogger_BeginScope_DoesNotChangeIndentation()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger sut					= new();

		//--- ACT -------------------------------------------------------------
		sut.Log(LogLevel.Information, new EventId(1), "No indent", null, (s, e) => s.ToString());

		using (sut.BeginScope("Scope1"))
		{
			sut.Log(LogLevel.Information, new EventId(2), "One indent", null, (s, e) => s.ToString());

			using (sut.BeginScope("Scope2"))
			{
				sut.Log(LogLevel.Information, new EventId(3), "Two indents", null, (s, e) => s.ToString());
			}

			sut.Log(LogLevel.Information, new EventId(4), "One indent again", null, (s, e) => s.ToString());
		}

		sut.Log(LogLevel.Information, new EventId(5), "No indent again", null, (s, e) => s.ToString());

		//--- ASSERT ----------------------------------------------------------
		Assert.Collection(sut.LogMessages,
			line => Assert.Equal("No indent", line),
			line => Assert.Equal("One indent", line),
			line => Assert.Equal("Two indents", line),
			line => Assert.Equal("One indent again", line),
			line => Assert.Equal("No indent again", line));
	}

	[Fact]
	public void LogMessages_WhenMessageContainsNewLines_ResultsInMultipleLines()
	{
		//--- ARRANGE ---------------------------------------------------------
		const int ExpectedLineCount = 3;
		TestOutputCollector mockTestConsole = new();
		TestOutputLogger logger = new(mockTestConsole);
		string message = "First line\r\nSecond line\r\nThird line";

		//--- ACT -------------------------------------------------------------
		logger.Log(LogLevel.Information, new EventId(1), message, null, (s, e) => s.ToString());

		//--- ASSERT ----------------------------------------------------------
		//--- "\r\n" will be replaced by "\n" ---
		Assert.Equal(ExpectedLineCount, mockTestConsole.Output.Count);
	}
}
