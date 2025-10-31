
using System.Diagnostics.CodeAnalysis;

using ErikForwerk.TestAbstractions.STA.Models;

using Xunit;
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
			() => RunOnSTAThread(() => throw new Exception("Test exception") ));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("Test exception", ex.Message);
	}

	[Fact]
	public void RunOnSTAThread_Action_ThrowsNoOwnException()
	{
		//---ARRANGE ----------------------------------------------------------
		bool didInFactExecutedTheDelegateAndNotJustSilentlyDoNothingLikeIgnoringTheActionWithoutSombodyEverNoticingAndThatsWhatThisVariableIsFor = false;

		//--- ACT -------------------------------------------------------------
		RunOnSTAThread(() => { didInFactExecutedTheDelegateAndNotJustSilentlyDoNothingLikeIgnoringTheActionWithoutSombodyEverNoticingAndThatsWhatThisVariableIsFor = true; });

		//--- ASSERT ----------------------------------------------------------
		Assert.True(didInFactExecutedTheDelegateAndNotJustSilentlyDoNothingLikeIgnoringTheActionWithoutSombodyEverNoticingAndThatsWhatThisVariableIsFor);
	}

	[Fact]
	public void RunOnSTAThread_Func_RelaysException()
	{
		//--- ACT -------------------------------------------------------------
		Exception ex = Assert.Throws<Exception>(
			() => RunOnSTAThread<object>([DoesNotReturn]() => throw new Exception("Test exception") ));

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
}