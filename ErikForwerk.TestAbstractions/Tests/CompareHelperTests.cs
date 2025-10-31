using ErikForwerk.TestAbstractions.Models;
using ErikForwerk.TestAbstractions.Tools;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ErikForwerk.TestAbstractions.Tests;

public sealed class CompareHelperTests(ITestOutputHelper toh) : TestBase(toh)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Nested Types

	private sealed class FirstDummyTestClass
	{
		public string? Name { get; set; }
	}

	private sealed class SecondDummyTestClass
	{
		public IEnumerable<int>? Numbers { get; set; }
	}

	#endregion Nested Types

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Methods

	[Theory]
	[InlineData(true,	null,	null)]
	[InlineData(true,	"",		"")]
	[InlineData(true,	"Test",	"Test")]

	[InlineData(false,	"Test",	"Different")]
	[InlineData(false,	"Test",	null)]
	[InlineData(false,	null,	"Test")]
	public void CompareTestHelper_SimpleProperties(bool expectedEquality, string? testStringA, string? testStringB)
	{
		//--- ARRANGE ---------------------------------------------------------
		FirstDummyTestClass obj1 = new() { Name = testStringA };
		FirstDummyTestClass obj2 = new() { Name = testStringB };

		//--- ACT & ASSERT ----------------------------------------------------
		Exception exception = Record.Exception(
			() => CompareHelper.AssertEqual(obj1, obj2, TestConsole));

		if (expectedEquality)
		{
			Assert.Null(exception);
			TestConsole.WriteLine("[ ✔️ PASSED] Objects are equal as expected.");
		}
		else
		{
			EqualException ex = Assert.IsType<EqualException>(exception);
			Assert.Contains("Values differ", ex.Message);
			TestConsole.WriteLine($"[ ✔️ PASSED] Expected [{ex.GetType().Name}] was thrown for unequal objects.");
		}
	}

	[Theory]
	[InlineData(true,	null,						null)]
	[InlineData(true,	new int[] {},				new int[] {})]
	[InlineData(true,	new int[] { 1, 2, 3 },		new int[] { 1, 2, 3 })]

	[InlineData(false,	new int[] { 1, 2, 3 },		null)]
	[InlineData(false,	new int[] { 1, 2, 3 },		new int[] { 4, 5, 6 })]
	[InlineData(false,	new int[] { 1, 2, 3 },		new int[] { 1, 2, 3, 4})]
	[InlineData(false,	new int[] { 1, 2, 3, 4},	new int[] { 1, 2, 3})]
	[InlineData(false,	null,						new int[] { 1, 2, 3 })]
	public void CompareTestHelper_EnumerableProperties(bool expectedEquality, int[]? testArrayA, int[]? testArrayB)
	{
		//--- ARRANGE ---------------------------------------------------------
		SecondDummyTestClass obj1 = new() { Numbers = testArrayA };
		SecondDummyTestClass obj2 = new() { Numbers = testArrayB };

		//--- ACT & ASSERT ----------------------------------------------------
		Exception ex = Record.Exception(
			() => CompareHelper.AssertEqual(obj1, obj2, TestConsole));

		if (expectedEquality)
		{
			Assert.Null(ex);
			TestConsole.WriteLine("[ ✔️ PASSED] Objects are equal as expected.");
		}
		else
		{
			_ = Assert.IsType<EqualException>(ex);
			Assert.Contains("Collections differ", ex.Message);
			TestConsole.WriteLine($"[ ✔️ PASSED] Expected [{ex.GetType().Name}] was thrown for unequal objects.");
		}
	}

	#endregion Test Methods
}

