using ErikForwerk.TestAbstractions.Models;
using ErikForwerk.TestAbstractions.Tools;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ErikForwerk.TestAbstractions.Tests;

using FDTC = CompareHelperTests.FirstDummyTestClass;

public sealed class CompareHelperTests(ITestOutputHelper toh) : TestBase(toh)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Nested Types
	internal record FirstDummyTestClass
	{
		public FirstDummyTestClass()
		{ }

		public FirstDummyTestClass(string name)
			=> Name = name;

		public string? Name { get; set; }
	}

	internal sealed class SecondDummyTestClass
	{
		public IEnumerable<int>? Numbers { get; set; }
	}

	internal sealed class ThirdDummyTestClass
	{
		public IEnumerable<object>? Objects { get; set; }
	}

	#endregion Nested Types

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Data

	public static TheoryData<bool, IEnumerable<int>?, IEnumerable<int>?> IntegerEnumerableTestData()
	{
		return new()
		{
			{ true,		null,						null },
			{ true,		Array.Empty<int>(),			Array.Empty<int>() },
			{ true,		Enumerable.Empty<int>(),	Enumerable.Empty<int>() },
			{ true,		new List<int>(),			new List<int>() },

			{ true,		new int[] { 1, 2, 3 },		new int[] { 1, 2, 3 } },
			{ false,	new int[] { 1, 2, 3 },		new int[] { 4, 5, 6 } },
			{ false,	new int[] { 1, 2, 3 },		new int[] { 1, 2, 3, 4} },
			{ false,	new int[] { 1, 2, 3, 4},	new int[] { 1, 2, 3} },
			{ false,	new int[] { 1, 2, 3 },		null },
			{ false,	null,						new int[] { 1, 2, 3 } },

			{ true,		new List<int>([1, 2, 3]),	new List<int>([1, 2, 3]) },
			{ false,	new List<int>([1, 2, 3]),	new List<int>([1, 2, 3, 4]) },
			{ false,	new List<int>([1, 2, 3]),	null },
			{ false,	null,						new List<int>([1, 2, 3])},
		};
	}

	public static TheoryData<bool, IEnumerable<object?>?, IEnumerable<object?>?> ObjectEnumerableTestData()
	{
		return new()
		{
			{ true,		null,												null },
			{ true,		Array.Empty<FDTC>(),								Array.Empty<FDTC>() },
			{ true,		Enumerable.Empty<FDTC>(),							Enumerable.Empty<FDTC>() },
			{ true,		new List<FDTC>(),									new List<FDTC>() },

			{ true,		new FDTC[] { new("A"), new("B")},					new FDTC[] { new("A"), new("B") } },
			{ false,	new FDTC[] { new("A"), new("B")},					new FDTC[] {new("E"), new("F")} },
			{ false,	new FDTC[] { new("A"), new("B")},					new FDTC[] {new("A"), new("B"), new("C")} },
			{ false,	new FDTC[] { new("A"), new("B"), new("C")},			new FDTC[] {new("A"), new("B")} },

			{ false,	new FDTC[] { new("A"), new("B")},					null },
			{ false,	null,												new FDTC[] {new("A"), new("B")} },

			{ true,		new List<FDTC>([new FDTC("A"), new FDTC("B")]),		new List<FDTC>([new FDTC("A"), new FDTC("B")]) },
			{ false,	new List<FDTC>([new FDTC("A"), new FDTC("B")]),		null },
			{ false,	new List<FDTC>([new FDTC("A"), new FDTC("B")]),		new List<FDTC>([new FDTC("A"), new FDTC("B"), new FDTC("C")]) },
			{ false,	null,												new List<FDTC>([ new FDTC("A"), new FDTC("B")])},
		};
	}

	#endregion Test Data

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
	[MemberData(nameof(IntegerEnumerableTestData))]
	public void CompareTestHelper_EnumerableIntegerProperties(bool expectedEquality, IEnumerable<int>? testDataA, IEnumerable<int>? testDataB)
	{
		//--- ARRANGE ---------------------------------------------------------
		SecondDummyTestClass obj1 = new() { Numbers = testDataA };
		SecondDummyTestClass obj2 = new() { Numbers = testDataB };

		TestConsole.WriteLine($"Test data A Length: [{testDataA?.Count().ToString() ?? "<null>"}]");
		TestConsole.WriteLine($"Test data B Length: [{testDataB?.Count().ToString() ?? "<null>"}]");
		TestConsole.WriteLine($"Expected Equality:  [{expectedEquality}]");
		TestConsole.WriteLine("");

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

	[Theory]
	[MemberData(nameof(ObjectEnumerableTestData))]
	public void CompareTestHelper_EnumerableObjectProperties(bool expectedEquality, IEnumerable<object>? testDataA, IEnumerable<object>? testDataB)
	{
		//--- ARRANGE ---------------------------------------------------------
		ThirdDummyTestClass obj1 = new() { Objects = testDataA };
		ThirdDummyTestClass obj2 = new() { Objects = testDataB };

		TestConsole.WriteLine($"Test data A Length: [{testDataA?.Count().ToString() ?? "<null>"}]");
		TestConsole.WriteLine($"Test data B Length: [{testDataB?.Count().ToString() ?? "<null>"}]");
		TestConsole.WriteLine($"Expected Equality:  [{expectedEquality}]");
		TestConsole.WriteLine("");

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

