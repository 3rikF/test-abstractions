
using ErikForwerk.TestAbstractions.Models;
using ErikForwerk.TestAbstractions.Tools;

using Xunit;
using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestStringExtensionsTests(ITestOutputHelper toh) : TestBase(toh)
{
	[Theory]
	[InlineData(null,										NULL_STRING)]
	[InlineData("",											EMPTY_STRING)]
	[InlineData("	",										"\\t")]
	[InlineData("Hello, World!",							"Hello, World!")]
	[InlineData("Line1\rLine2\nLine3\r\nLine4\tTabbed",		"Line1\\rLine2\\nLine3\\r\\nLine4\\tTabbed")]
	public void EscapeTestOutput_ShouldReturnExpectedResults(string? input, string expected)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestConsole.WriteLine($"Expected Output:    [{expected}]");

		//--- ACT -------------------------------------------------------------
		string result = input.EscapeTestOutput();
		TestConsole.WriteLine($"Actual Output:      [{result}]");

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expected, result);
	}
}
