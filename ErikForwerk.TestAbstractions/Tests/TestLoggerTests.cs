
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

using Xunit;

namespace ErikForwerk.TestAbstractions.Tests;

public sealed class TestLoggerTests
{
	[Fact]
	public void LogMessages_WhenNoMessagesLogged_ReturnsEmptyCollection()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger logger = new ();

		//--- ACT -------------------------------------------------------------
		IEnumerable<string> messages = logger.LogMessages;

		//--- ASSERT ----------------------------------------------------------
		Assert.Empty(messages);
	}

	[Fact]
	public void LogMessages_WhenMessagesLogged_ReturnsCollectionOfMessages()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger logger = new ();

		//--- ACT -------------------------------------------------------------
		logger.Log(LogLevel.Information, new EventId(1), "Test message", null, (s, e) => s.ToString());
		logger.Log(LogLevel.Warning, new EventId(2), "Another test message", null, (s, e) => s.ToString());

		//--- ASSERT ----------------------------------------------------------
		Assert.Collection(logger.LogMessages,
			item => Assert.Equal("Test message", item),
			item => Assert.Equal("Another test message", item));
	}

	[Fact]
	public void IsInScope()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger logger = new ();

		//--- ACT -------------------------------------------------------------
		using (logger.BeginScope("Test scope"))
		{
			//--- ASSERT ------------------------------------------------------
			Assert.True(logger.IsInScope);
		}

		//--- ASSERT ----------------------------------------------------------
		Assert.False(logger.IsInScope);
	}

	[Theory]
	[InlineData(LogLevel.Trace, true)]
	[InlineData(LogLevel.Debug, true)]
	[InlineData(LogLevel.Information, true)]
	[InlineData(LogLevel.Warning, true)]
	[InlineData(LogLevel.Error, true)]
	[InlineData(LogLevel.Critical, true)]
	[InlineData(LogLevel.None, true)]
	public void IsEnabled(LogLevel logLevel, bool expected)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger logger = new ();

		//--- ACT -------------------------------------------------------------
		bool isEnabled = logger.IsEnabled(logLevel);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expected, isEnabled);
	}
}

