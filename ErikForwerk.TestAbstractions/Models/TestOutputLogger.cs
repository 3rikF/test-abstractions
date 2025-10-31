
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestOutputLogger(ITestOutputHelper testOutputHelper) : ILogger
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Nested Type

	private class IndentionScope : IDisposable
	{
		private readonly TestOutputLogger _logger;

		public IndentionScope(TestOutputLogger logger)
		{
			_logger = logger;
			_logger._indentation += "  ";
		}

		public void Dispose()
			=> _logger._indentation = _logger._indentation[..^2];
	}

	#endregion Nested Type

	//-----------------------------------------------------------------------------------------------------------------
	#region Fields

	private string _indentation = string.Empty;
	private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

	#endregion Fields

	//-----------------------------------------------------------------------------------------------------------------
	#region Methods

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> new IndentionScope(this);

	public bool IsEnabled(LogLevel logLevel)
		=> true;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		string message = formatter(state, exception);
		string[] lines = message
			.Replace("\r\n", "\n")
			.Split('\n');

		foreach (string line in lines)
			_testOutputHelper.WriteLine($"{_indentation}{line}");
	}

	#endregion Methods
}
