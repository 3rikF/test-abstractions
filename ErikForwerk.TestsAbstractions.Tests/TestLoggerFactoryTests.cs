
using ErikForwerk.TestAbstractions.Models;

using Microsoft.Extensions.Logging;

namespace ErikForwerk.TestAbstractions.Tests;

public sealed class TestLoggerFactoryTests
{
	[Fact]
	public void CreateLogger()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLogger expectedLogger	= new();
		TestLoggerFactory factory	= new (expectedLogger);

		//--- ACT -------------------------------------------------------------
		ILogger logger = factory.CreateLogger("Test logger");

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(logger);
		Assert.Same(expectedLogger, logger);
	}

	[Fact]
	public void AddProvider_ThrowsNotImplementedException()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLoggerFactory factory = new (new TestLogger());

		//--- ACT -------------------------------------------------------------
		NotImplementedException ex = Assert.Throws<NotImplementedException>(
			() => factory.AddProvider(null!));

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("The method or operation is not implemented.", ex.Message);
	}

	[Fact]
	public void Dispose_ThrowsNotImplementedException()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestLoggerFactory factory = new (new TestLogger());

		//--- ACT -------------------------------------------------------------
		NotImplementedException ex = Assert.Throws<NotImplementedException>(factory.Dispose);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal("The method or operation is not implemented.", ex.Message);
	}
}

