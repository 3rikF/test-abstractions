
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestLoggerTests
{
	[Fact]
	public void LogMessages_WhenNoMessagesLogged_ReturnsEmptyCollection()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger sut = new ();

		//--- ACT -------------------------------------------------------------
		IEnumerable<string> messages = sut.LogMessages;

		//--- ASSERT ----------------------------------------------------------
		Assert.Empty(messages);
	}

	[Fact]
	public void LogMessages_WhenMessagesLogged_ReturnsCollectionOfMessages()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger sut = new ();

		//--- ACT -------------------------------------------------------------
		sut.Log(LogLevel.Information, new EventId(1), "Test message", null, (s, e) => s.ToString());
		sut.Log(LogLevel.Warning, new EventId(2), "Another test message", null, (s, e) => s.ToString());

		//--- ASSERT ----------------------------------------------------------
		Assert.Collection(sut.LogMessages,
			item => Assert.Equal("Test message", item),
			item => Assert.Equal("Another test message", item));
	}

	[Fact]
	public void IsInScope()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger sut = new ();

		//--- ACT -------------------------------------------------------------
		using (sut.BeginScope("Test scope"))
		{
			//--- ASSERT ------------------------------------------------------
			Assert.True(sut.IsInScope);
		}

		//--- ASSERT ----------------------------------------------------------
		Assert.False(sut.IsInScope);
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
		TestLogger sut = new ();

		//--- ACT -------------------------------------------------------------
		bool isEnabled = sut.IsEnabled(logLevel);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expected, isEnabled);
	}
}

