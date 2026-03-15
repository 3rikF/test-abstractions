
using ErikForwerk.TestAbstractions.STA.Models;

using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.STA.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class StaTestBaseTests(ITestOutputHelper toh) : StaTestBase(toh)
{
	[Fact]
	public void RunOnSTAThread_Action_RelaysException()
	{
		//--- ACT -------------------------------------------------------------
		Exception ex = Assert.Throws<Exception>(
			() => RunOnSTAThread(() => throw new Exception("Test exception")));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("Test exception", ex.Message);
	}

	[Fact]
	public void RunOnSTAThread_Action_ThrowsNoOwnException()
	{
		//---ARRANGE ----------------------------------------------------------
		bool didInFactExecutedTheDelegateAndNotJustSilentlyDidNothingLikeIgnoringTheActionWithoutSombodyEverNoticingAndThatsWhatThisVariableIsFor = false;

		//--- ACT -------------------------------------------------------------
		RunOnSTAThread(() => { didInFactExecutedTheDelegateAndNotJustSilentlyDidNothingLikeIgnoringTheActionWithoutSombodyEverNoticingAndThatsWhatThisVariableIsFor = true; });

		//--- ASSERT ----------------------------------------------------------
		Assert.True(didInFactExecutedTheDelegateAndNotJustSilentlyDidNothingLikeIgnoringTheActionWithoutSombodyEverNoticingAndThatsWhatThisVariableIsFor);
	}

	[Fact]
	public void RunOnSTAThread_Action_FinallyIsExecuted()
	{
		//--- ARRANGE ---------------------------------------------------------
		bool finallyCalled = false;
		Action dummyAction = () => { /* No-op */ };

		//--- ACT -------------------------------------------------------------
		RunOnSTAThread(
			dummyAction
			, @finally: () => finallyCalled = true );

		//--- ASSERT ----------------------------------------------------------
		Assert.True(finallyCalled);

		TestConsole.WriteLine("[✔ PASSED] Finally was called as expected.");
	}

	[Fact]
	public void RunOnSTAThread_Func_RelaysException()
	{
		//--- ACT -------------------------------------------------------------
		Exception ex = Assert.Throws<Exception>(
			() => RunOnSTAThread<object>(() => throw new Exception("Test exception")));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("Test exception", ex.Message);
	}

	[Fact]
	public void RunOnSTAThread_Func_ReturnsValue()
	{
		//--- ACT -------------------------------------------------------------
		int result = RunOnSTAThread(() => 42);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(42, result);
	}

	[Fact]
	public void RunOnSTAThread_Func_FinallyIsExecuted()
	{
		//--- ARRANGE ---------------------------------------------------------
		bool finallyCalled			= false;
		Func<bool> dummyFunction	= () => true;

		//--- ACT -------------------------------------------------------------
		_ = RunOnSTAThread(
			dummyFunction
			, @finally: p => finallyCalled = p );

		//--- ASSERT ----------------------------------------------------------
		Assert.True(finallyCalled);

		TestConsole.WriteLine("[✔ PASSED] Finally was called as expected.");
	}
}