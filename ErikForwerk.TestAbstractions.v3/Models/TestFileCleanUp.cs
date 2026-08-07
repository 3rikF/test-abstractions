using Xunit;

namespace ErikForwerk.TestAbstractions.v3.Models;

public sealed class TestFileCleanUp(ITestOutputHelper Output, params string[] FilePaths) : IDisposable
{
	public void Dispose()
	{
		foreach (string filePath in FilePaths)
		{
			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				File.Delete(filePath);
				Output.WriteLine($"Cleaned up test file: [{filePath}]");
			}
			else
				Output.WriteLine($"[WARNING] Clean-up test file: Invalid path or missing file [{filePath}]");
		}
	}
}
