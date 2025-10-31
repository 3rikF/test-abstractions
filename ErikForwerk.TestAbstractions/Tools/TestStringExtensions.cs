
using ErikForwerk.TestAbstractions.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tools;

//-----------------------------------------------------------------------------------------------------------------------------------------
public static class TestStringExtensions
{
	public static string EscapeTestOutput(this string? text)
	{
		if (text is null)
			return TestBase.NULL_STRING;

		else if (text.Length == 0)
			return TestBase.EMPTY_STRING;

		else
			//--- https://www.compart.com/en/unicode/block/U+2400 ---
			return text
				.Replace("\r\n", "\\r\\n")	//⏎ ␍␊
				.Replace("\r", "\\r")		//← ␍
				.Replace("\n", "\\n")		//↓ ␊
				.Replace("\t", "\\t");
	}
}
