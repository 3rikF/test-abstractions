
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

using Xunit;
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
	[InlineData(null,		 "<null>")]
	[InlineData("",			"<empty>")]
	[InlineData("  ",		"[  ]")]
	[InlineData("foobar",	"[foobar]")]
	[InlineData(123,		"[123]")]
	public void Test_B(object? input, string expectedOutput)
	{
		//--- ARRANGE ---------------------------------------------------------
		// (none)

		//--- ACT -------------------------------------------------------------
		string actualOutput = B(input);

		TestConsole.WriteLine($"Expected: {expectedOutput}");
		TestConsole.WriteLine($"Actual:   {actualOutput}");

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(expectedOutput, actualOutput);
	}

	[Fact]
	public void Test_CreateTestFileCleanUp()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string TEST_FILE1 = "Test_CreateTestFileCleanUp-File1.txt";
		const string TEST_FILE2 = "Test_CreateTestFileCleanUp-File2.txt";

		Assert.False(File.Exists(TEST_FILE1), $"Precondition failed: Test file [{TEST_FILE1}] already exists.");
		Assert.False(File.Exists(TEST_FILE2), $"Precondition failed: Test file [{TEST_FILE2}] already exists.");

		//--- ACT -------------------------------------------------------------
		using (CreateTestFileCleanUp(TEST_FILE1, TEST_FILE2))
		{
			File.WriteAllText(TEST_FILE1, "This is a test file.");
			File.WriteAllText(TEST_FILE2, "This is another test file.");
			Assert.True(File.Exists(TEST_FILE1), $"Test file [{TEST_FILE1}] was not created.");
			Assert.True(File.Exists(TEST_FILE2), $"Test file [{TEST_FILE2}] was not created.");
		}

		//--- ASSERT ----------------------------------------------------------
		Assert.False(File.Exists(TEST_FILE1), $"Test file [{TEST_FILE1}]' was not deleted.");
		Assert.False(File.Exists(TEST_FILE2), $"Test file [{TEST_FILE2}]' was not deleted.");
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
		_ = Assert.IsType<ILogger>(logger, false);
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
		_ = Assert.IsType<ILogger<TestBaseTests>>(logger, false);
		_ = Assert.IsType<TestLogger>(logger, false);

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
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [param=foobar]";
		const string TEST_PARAM			= "foobar";

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
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [param1=foobar], [param2=69]";
		const string TEST_PARAM1		= "foobar";
		const int TEST_PARAM2			= 69;

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
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [no parameters]";

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
		const string EXPECTED_MESSAGE	= "This method should not have been executed. [param=foobar]";
		const string TEST_PARAM			= "foobar";

		//--- ACT -------------------------------------------------------------
		FailException ex = Assert.Throws<FailException>(
			() => FailTest<string, int>(TEST_PARAM));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(EXPECTED_MESSAGE, ex.Message);
	}

	#endregion Test FailTest Functions
}
