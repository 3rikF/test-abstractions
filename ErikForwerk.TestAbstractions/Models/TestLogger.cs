
using Microsoft.Extensions.Logging;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
public class TestLogger : ILogger
{
	private readonly List<string> _messageLog = [];

	private sealed class ScopeTracker : IDisposable
	{
		private readonly TestLogger _testLogger;

		public ScopeTracker(TestLogger testLogger)
		{
			_testLogger				= testLogger;
			testLogger.IsInScope	= true;
		}

		public void Dispose()
			=> _testLogger.IsInScope = false;
	}

	public bool IsInScope
		{ get; private set; }

	public IEnumerable<string> LogMessages
		=> _messageLog;


	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> new ScopeTracker(this);

	public bool IsEnabled(LogLevel logLevel)
		=> true;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		=> _messageLog.Add(formatter(state, exception));
}

//-----------------------------------------------------------------------------------------------------------------------------------------
public class TestLoggerGeneric<T> : TestLogger, ILogger<T>
{ }