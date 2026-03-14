
using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestOutputCollector : ITestOutputHelper
{
	public void WriteLine(string message)
		=> Output.Add(message);

	public void WriteLine(string format, params object[] args)
		=> Output.Add(string.Format(format, args));

	public List<string> Output
		{ get; } = [];
}
