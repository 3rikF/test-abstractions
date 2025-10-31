
using Microsoft.Extensions.Logging;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestLoggerFactory : ILoggerFactory
{
	public ILogger TestLogger { get; }

	public TestLoggerFactory(ILogger testLogger)
		=> TestLogger = testLogger;

	public ILogger CreateLogger(string categoryName)
		=> TestLogger;

	public void AddProvider(ILoggerProvider provider)
		=> throw new NotImplementedException();

	public void Dispose()
		=> throw new NotImplementedException();
}
