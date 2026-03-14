
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;
using Xunit.Sdk;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

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
	public void Test_CreateTestFileCleanUp()
	{
		//--- ARRANGE ---------------------------------------------------------
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
		//--- ARRANGE ---------------------------------------------------------
		// (none)

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
		//--- ARRANGE ---------------------------------------------------------
		// (none)

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
		FailException ex = Assert.Throws<FailException>(FailTest);

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
		const string EXPECTED_MESSAGE = "This method should not have been executed. [param=foobar]";
		const string TEST_PARAM = "foobar";

		//--- ACT -------------------------------------------------------------
		FailException ex = Assert.Throws<FailException>(
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
		const string EXPECTED_MESSAGE = "This method should not have been executed. [param1=foobar], [param2=69]";
		const string TEST_PARAM1 = "foobar";
		const int TEST_PARAM2 = 69;

		//--- ACT -------------------------------------------------------------
		FailException ex = Assert.Throws<FailException>(
			() => FailTest(TEST_PARAM1, TEST_PARAM2));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	#endregion Test FailTest Actions

	//-----------------------------------------------------------------------------------------------------------------
	#region Test FailTest Functions

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest{T1}"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (bet never actually call) <see cref="TestBase.FailTest{T1}"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_WithReturnOnly()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE = "This method should not have been executed. [no parameters]";

		//--- ACT -------------------------------------------------------------
		FailException ex = Assert.Throws<FailException>(
			() => FailTest<int>());

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	/// <summary>
	/// Ensures that <see cref="TestBase.FailTest{T1}"/> fails as expected.
	/// Also covers this code-path for all other test methods, who use (bet never actually call) <see cref="TestBase.FailTest{T1}"/>.
	/// </summary>
	[Fact]
	public void Test_FailTest_WithReturn_OneParam()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE = "This method should not have been executed. [param=foobar]";
		const string TEST_PARAM = "foobar";

		//--- ACT -------------------------------------------------------------
		FailException ex = Assert.Throws<FailException>(
			() => FailTest<string, int>(TEST_PARAM));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	#endregion Test FailTest Functions
}
