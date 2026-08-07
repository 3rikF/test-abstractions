
using ErikForwerk.TestAbstractions.v3.Models;

using Microsoft.Extensions.Logging;

using Xunit;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.v3.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestLoggerFactoryTests
{
	[Fact]
	public void CreateLogger()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger expectedLogger	= new();
		TestLoggerFactory sut		= new (expectedLogger);

		//--- ACT -------------------------------------------------------------
		ILogger logger = sut.CreateLogger("Test logger");

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(logger);
		Assert.Same(expectedLogger, logger);
	}

	[Fact]
	public void AddProvider_ThrowsNotImplementedException()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLoggerFactory sut = new (new TestLogger());

		//--- ACT -------------------------------------------------------------
		NotImplementedException ex = Assert.Throws<NotImplementedException>(
			() => sut.AddProvider(null!));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("The method or operation is not implemented.", ex.Message);
	}

	[Fact]
	public void Dispose_ThrowsNotImplementedException()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLoggerFactory sut = new (new TestLogger());

		//--- ACT -------------------------------------------------------------
		NotImplementedException ex = Assert.Throws<NotImplementedException>(sut.Dispose);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("The method or operation is not implemented.", ex.Message);
	}
}
