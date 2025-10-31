
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

	protected TestLogger GetTestLogger()
		=> new ();

	protected TestLoggerGeneric<T> GetTestLogger<T>()
		=> new ();

	#endregion Properties

	//-----------------------------------------------------------------------------------------------------------------
	#region Methods

	protected IDisposable CreateTestFileCleanUp(params string[] filePaths)
		=> new TestFileCleanUp(TestConsole, filePaths);

	/// <summary>
	/// 'B' as in 'Bracketed'.
	/// Converts the specified object to a string representation enclosed in square brackets,
	/// handling null and empty values with predefined constants.
	/// </summary>
	/// <remarks>
	/// If the object's string representation already contains square brackets,
	/// they are trimmed before enclosing the result in new brackets.
	/// This method ensures consistent formatting for null and empty values.
	/// </remarks>
	/// <param name="toStringObject">The object to convert to a string. If null, a predefined constant for null strings is returned.</param>
	/// <returns>
	/// A string enclosed in square brackets if the object's string representation is non-empty;
	/// a predefined constant for null or empty strings otherwise.
	/// </returns>
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

	[DoesNotReturn]
	protected static void FailTest()
		=> throw FailException.ForFailure("This method should not have been executed.");

	[DoesNotReturn]
	protected static void FailTest<T1>(T1 p1)
		=> throw FailException.ForFailure($"This method should not have been executed. [param={p1}]");

	[DoesNotReturn]
	protected static void FailTest<T1, T2>(T1 p1, T2 p2)
		=> throw FailException.ForFailure($"This method should not have been executed. [param1={p1}], [param2={p2}]");

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
