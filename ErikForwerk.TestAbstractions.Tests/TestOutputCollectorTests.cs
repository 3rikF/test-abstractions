
//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestOutputCollectorTests
{
	[Fact]
	public void Output_WhenNoMessagesWritten_ReturnsEmptyCollection()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestOutputCollector sut	= new ();

		//--- ACT -------------------------------------------------------------
		List<string> output		= sut.Output;

		//--- ASSERT ----------------------------------------------------------
		Assert.Empty(output);
	}

	[Fact]
	public void WriteLine_WithMessage_AddsMessageToOutput()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestOutputCollector sut = new ();

		//--- ACT -------------------------------------------------------------
		sut.WriteLine("Hello, World!");

		//--- ASSERT ----------------------------------------------------------
		string singenContent = Assert.Single(sut.Output);
		Assert.Equal("Hello, World!", singenContent);
	}

	[Fact]
	public void WriteLine_WithMultipleMessages_AddsAllMessagesToOutput()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestOutputCollector sut = new ();

		//--- ACT -------------------------------------------------------------
		sut.WriteLine("First");
		sut.WriteLine("Second");
		sut.WriteLine("Third");

		//--- ASSERT ----------------------------------------------------------
		Assert.Collection(sut.Output,
			item => Assert.Equal("First", item),
			item => Assert.Equal("Second", item),
			item => Assert.Equal("Third", item));
	}

	[Fact]
	public void WriteLine_WithFormatString_AddsFormattedMessageToOutput()
	{
		//--- ARRANGE ---------------------------------------------------------
		TestOutputCollector sut = new ();

		//--- ACT -------------------------------------------------------------
		sut.WriteLine("Hello, {0}!", "World");

		//--- ASSERT ----------------------------------------------------------
		string singleContent = Assert.Single(sut.Output);
		Assert.Equal("Hello, World!", singleContent);
	}
}
