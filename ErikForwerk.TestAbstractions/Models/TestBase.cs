
using System.Diagnostics.CodeAnalysis;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
public abstract class TestBase(ITestOutputHelper output)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Fields

	protected internal const string NULL_STRING = "<null>";
	protected internal const string EMPTY_STRING = "<empty>";

	#endregion Fields

	//-----------------------------------------------------------------------------------------------------------------
	#region Properties

	protected ITestOutputHelper TestConsole
	{ get; } = output;

	protected TestLogger TestLogger
	{ get; } = new TestLogger();

	#endregion Properties

	//-----------------------------------------------------------------------------------------------------------------
	#region Methods

	protected IDisposable CreateTestFileCleanUp(params string[] filePaths)
		=> new TestFileCleanUp(TestConsole, filePaths);

	protected static string B(object? toStringObject)
	{
		if (toStringObject?.ToString() is not string s)
			return NULL_STRING;

		else if (s.Length == 0)
			return EMPTY_STRING;

		else
			return string.Concat('[', s.Trim('[', ']'), ']');
	}

	#endregion Methods

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Helper Actions

	protected static void FailTest()
		=> Assert.Fail("This method should not have been executed.");

	protected static void FailTest<T1>(T1 p1)
		=> Assert.Fail($"This method should not have been executed. [param={p1}]");

	protected static void FailTest<T1, T2>(T1 p1, T2 p2)
		=> Assert.Fail($"This method should not have been executed. [param1={p1}], [param2={p2}]");

	#endregion Test Helper Actions

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Helper Functions

	[DoesNotReturn]
	protected static TReturn FailTest<TReturn>()
		=> throw FailException.ForFailure($"This method should not have been executed. [no parameters]");

	[DoesNotReturn]
	protected static TReturn FailTest<T1, TReturn>(T1 p1)
		=> throw FailException.ForFailure($"This method should not have been executed. [param={p1}]");

	#endregion Test Helper Functions
}
