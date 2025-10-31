
using System.Reflection;

using Xunit;
using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tools;

//-----------------------------------------------------------------------------------------------------------------------------------------
public static class CompareHelper
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Helper Methods

	private static void AssertProperties<T>(T expected, T actual, Action<string, string, object?, object?> assert)
	{
		//--- tests are pointless if the objects are the same instance --------
		Assert.False(ReferenceEquals(expected, actual));

		//--- compare all properties via reflection -----------------------
		PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (PropertyInfo prop in properties)
		{
			object? expectedValue	= prop.GetValue(expected);
			object? actualValue		= prop.GetValue(actual);

			assert(string.Empty, prop.Name, expectedValue, actualValue);

			if (expectedValue is null || actualValue is null)
				continue;

			//--- if its an IEnumerable, compare the elements ------------------
			else if (expectedValue is IEnumerable<object?> expectedEnumeration && actualValue is IEnumerable<object?> actualEnumeration)
			{
				IEnumerator<object?> expectedBlah	= expectedEnumeration.GetEnumerator();
				IEnumerator<object?> actualBlubb	= actualEnumeration.GetEnumerator();

				//--- no recursion, just one level arrays ---
				while (expectedBlah.MoveNext() && actualBlubb.MoveNext())
					assert("\t", $"{prop.Name}", expectedBlah.Current, actualBlubb.Current);
			}

			//--- if its an array, compare the elements -----------------------
			else if (prop.PropertyType.IsArray && expectedValue is Array expectedArray && actualValue is Array actualArray)
			{
				//--- nu recursion, just one level arrays ---
				for (int i = 0; i < expectedArray.Length; i++)
					assert("\t", $"{prop.Name}[{i}]", expectedArray.GetValue(i), actualArray.GetValue(i));
			}
		}
	}

	public static void AssertEqual<T>(T expected, T actual, ITestOutputHelper testOutput)
	{
		AssertProperties(
			expected
			, actual
			, (indention, propertyName, a, b) =>
			{
				testOutput.WriteLine($"{indention}[{propertyName}]");
				Assert.Equal(a, b);
				testOutput.WriteLine($"{indention}[👍 '{a}' == '{b}']");
				testOutput.WriteLine(string.Empty);
			});
	}

	public static void AssertCompletelyUnequal<T>(T expected, T actual, ITestOutputHelper testOutput)
	{
		AssertProperties(
			expected
			, actual
			, (indention, propertyName, a, b) =>
			{
				testOutput.WriteLine($"{indention}[{propertyName}]");
				Assert.NotEqual(a, b);
				testOutput.WriteLine($"{indention}[👍 '{a}' != '{b}']");
				testOutput.WriteLine(string.Empty);
			});
	}

	#endregion Helper Methods
}
