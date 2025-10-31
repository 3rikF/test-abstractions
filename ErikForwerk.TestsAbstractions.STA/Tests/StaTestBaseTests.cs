using ErikForwerk.TestAbstractions.Models;

using Xunit;
using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

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
	public void RunOnSTAThread_Func_RelaysException()
	{
		//--- ACT -------------------------------------------------------------
		Exception ex = Assert.Throws<Exception>(
			() => RunOnSTAThread<object>(() => throw new Exception("Test exception") ));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("Test exception", ex.Message);
	}
}