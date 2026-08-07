
using ErikForwerk.TestAbstractions.v3.Models;

using Microsoft.Extensions.Logging;

using Xunit.Sdk;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.v3.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestBaseTests(ITestOutputHelper toh) : TestBase(toh)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Test Helper Methods

	[Theory]
	[InlineData(null, "<null>")]
	[InlineData("", "<empty>")]
	[InlineData("  ", "<whitespace>")]
	[InlineData("\t", "<whitespace>")]
	[InlineData("\r", "<whitespace>")]
	[InlineData("\n", "<whitespace>")]
	[InlineData("\r\n", "<whitespace>")]
	[InlineData("foobar", "[foobar]")]
	[InlineData(123, "[123]")]
	[InlineData("Line1\rLine2\nLine3\r\nLine4\tTabbed", "[Line1\\rLine2\\nLine3\\r\\nLine4\\tTabbed]")]
	public void Test_B(object? input, string expectedOutput)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestConsole.WriteLine($"Expected output {expectedOutput}");

		//--- ACT -------------------------------------------------------------
		string actualOutput = B(input);
		TestConsole.WriteLine($"Actual output   {actualOutput}");

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expectedOutput, actualOutput);
	}

	[Fact]
	public void Test_B_AlreadyBracketed()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string INPUT				= "[AlreadyBracketed]";
		const string EXPECTED_OUTPUT	= "[AlreadyBracketed]";
		TestConsole.WriteLine($"Expected output {EXPECTED_OUTPUT}");

		//--- ACT -------------------------------------------------------------
		string actualOutput = B(INPUT);
		TestConsole.WriteLine($"Actual output   {actualOutput}");

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_OUTPUT, actualOutput);
	}

	[Theory]
	[InlineData(null,						"<null>")]
	[InlineData(typeof(TestBaseTests),		"[TestBaseTests]")]
	[InlineData(typeof(int),				"[Int32]")]
	[InlineData(typeof(long),				"[Int64]")]
	[InlineData(typeof(byte),				"[Byte]")]
	[InlineData(typeof(float),				"[Float]")]
	[InlineData(typeof(double),				"[Double]")]
	[InlineData(typeof(string),				"[String]")]
	[InlineData(typeof(int[]),				"[Int32[]]")]
	[InlineData(typeof(IEnumerable<int>),	"[IEnumerable<Int32>]")]
	[InlineData(typeof(Action<float, int, string>),		"[Action<Float, Int32, String>]")]
	[InlineData(typeof(Func<float, int, string>),		"[Func<Float, Int32, String>]")]
	[InlineData(typeof(ValueTuple<float, int, string>),	"[ValueTuple<Float, Int32, String>]")]
	public void Test_B_TypeShorthand(Type? testType, string expectedOutput)
	{
		//--- ACT -------------------------------------------------------------
		string actualOutput = B(testType);

		TestConsole.WriteLine($"Expected output       {expectedOutput}");
		TestConsole.WriteLine($"Actual output         {actualOutput}");

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expectedOutput, actualOutput);
	}

	[Fact]
	public void Test_CreateTestFileCleanUp()
	{
		//--- ACT -------------------------------------------------------------
		using IDisposable sut = CreateTestFileCleanUp();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(sut);
		_ = Assert.IsAssignableFrom<TestFileCleanUp>(sut);
	}

	#endregion Test Helper Methods

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Logger Methods

	[Fact]
	public void Test_GetTestLogger()
	{
		//--- ACT -------------------------------------------------------------
		TestLogger logger = GetTestLogger();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(logger);
		_ = Assert.IsAssignableFrom<ILogger>(logger);
		Assert.Empty(logger.LogMessages);
		Assert.True(logger.IsEnabled(LogLevel.Information));
	}

	[Fact]
	public void Test_GetTestLogger_Generic()
	{
		//--- ACT -------------------------------------------------------------
		TestLoggerGeneric<TestBaseTests> logger = GetTestLogger<TestBaseTests>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(logger);
		_ = Assert.IsAssignableFrom<ILogger<TestBaseTests>>(logger);
		_ = Assert.IsAssignableFrom<TestLogger>(logger);

		Assert.Empty(logger.LogMessages);
		Assert.Empty(logger.LogMessages);
		Assert.True(logger.IsEnabled(LogLevel.Information));
	}

	#endregion Test Logger Methods

	//-----------------------------------------------------------------------------------------------------------------
	#region Test FailTest Actions

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (but never actually call) <see cref="TestBase.FailTest"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_Parameterless()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE = "This method should not have been executed.";

		//--- ACT -------------------------------------------------------------
		XunitException ex = Assert.Throws<XunitException>(FailTest);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest{T1}"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (bet never actually call) <see cref="TestBase.FailTest{T1}"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_OneParam()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [param=foobar]";
		const string TEST_PARAM			= "foobar";

		//--- ACT -------------------------------------------------------------
		XunitException ex = Assert.Throws<XunitException>(
			() => FailTest(TEST_PARAM));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest{T1, T2}"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (bet never actually call) <see cref="TestBase.FailTest{T1, T2}"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_TwoParams()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [param1=Foo], [param2=Bar]";
		const string TEST_PARAM_1		= "Foo";
		const string TEST_PARAM_2		= "Bar";

		//--- ACT -------------------------------------------------------------
		XunitException ex = Assert.Throws<XunitException>(
			() => FailTest(TEST_PARAM_1, TEST_PARAM_2));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	#endregion Test FailTest Actions

	//-----------------------------------------------------------------------------------------------------------------
	#region Test FailTest Functions

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest{TReturn}"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (but never actually call) <see cref="TestBase.FailTest{TReturn}"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_Function_Parameterless()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE = "This method should not have been executed. [no parameters]";

		//--- ACT -------------------------------------------------------------
		XunitException ex = Assert.Throws<XunitException>(FailTest<string>);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest{T1, TReturn}"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (but never actually call) <see cref="TestBase.FailTest{T1, TReturn}"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_Function_OneParam()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [param=TestValue]";
		const string TEST_PARAM			= "TestValue";

		//--- ACT -------------------------------------------------------------
		XunitException ex = Assert.Throws<XunitException>(
			() => FailTest<string, string>(TEST_PARAM));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	#endregion Test FailTest Functions
}
