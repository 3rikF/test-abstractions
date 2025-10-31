
using ErikForwerk.TestAbstractions.Models;

using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class TestFileCleanUpTests(ITestOutputHelper toh) : TestBase(toh)
{
	[Fact]
	public void Dispose_DeletesAllExistingFiles()
	{
		//--- ARRANGE ---------------------------------------------------------
		string tempFile1	= Path.GetTempFileName();
		string tempFile2	= Path.GetTempFileName();
		string[] files		= [ tempFile1, tempFile2 ];

		//--- ACT -------------------------------------------------------------
		using (CreateTestFileCleanUp(files))
		{
			// Dispose will be called automatically
		}

		//--- ASSERT ----------------------------------------------------------
		Assert.False(File.Exists(tempFile1),	"First file should be deleted.");
		Assert.False(File.Exists(tempFile2),	"Second file should be deleted.");
	}

	[Fact]
	public void Dispose_IgnoresNonExistingFiles()
	{
		//--- ARRANGE ---------------------------------------------------------
		string nonExistentFile	= Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		string[] files			= [ nonExistentFile ];

		//--- ACT -------------------------------------------------------------
		using (CreateTestFileCleanUp(files))
		{
			// Dispose will be called automatically
		}
		//--- ASSERT ----------------------------------------------------------
		Assert.False(File.Exists(nonExistentFile),	"Non-existent file should remain non-existent.");
	}

	[Fact]
	public void Dispose_IgnoresEmptyOrNullPaths()
	{
		//--- ARRANGE ---------------------------------------------------------
		string tempFile	= Path.GetTempFileName();
		string[] files	= [tempFile, string.Empty, null!];

		//--- ACT -------------------------------------------------------------
		using (CreateTestFileCleanUp(files))
		{
			// Dispose will be called automatically
		}

		//--- ASSERT ----------------------------------------------------------
		Assert.False(File.Exists(tempFile),	"Temp file should be deleted.");
	}
}
