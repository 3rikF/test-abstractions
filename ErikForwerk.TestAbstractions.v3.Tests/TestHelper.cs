
//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.v3.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestOutputCollector : ITestOutputHelper
{
	public void WriteLine(string message)
		=> Output.Add(message);

	public void WriteLine(string format, params object[] args)
		=> Output.Add(string.Format(format, args));

	public void Write(string message)
		=> WriteLine(message);

	public void Write(string format, params object[] args)
		=> Write(string.Format(format, args));

	public List<string> Output
		{ get; } = [];

	string ITestOutputHelper.Output
		=> string.Join(Environment.NewLine, Output);
}
