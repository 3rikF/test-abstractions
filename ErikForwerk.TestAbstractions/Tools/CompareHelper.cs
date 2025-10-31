
using System.Reflection;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tools;

//-----------------------------------------------------------------------------------------------------------------------------------------
public static class CompareHelper
{
	//-----------------------------------------------------------------------------------------------------------------
	private delegate void PropertyAssertDelegate(string indention, string propertyName, object? expectedValue, object? actualValue);

	//-----------------------------------------------------------------------------------------------------------------
	#region Helper Methods

	private static void AssertProperties<T>(T expected, T actual, PropertyAssertDelegate assertLogic)
	{
		//--- tests are pointless if the objects are the same instance --------
		Assert.False(ReferenceEquals(expected, actual));

		//--- compare all properties via reflection -----------------------
		PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (PropertyInfo prop in properties)
		{
			object? expectedValue	= prop.GetValue(expected);
			object? actualValue		= prop.GetValue(actual);

			//--- if its an IEnumerable, compare the elements ------------------
			if (expectedValue is IEnumerable<object?> expectedEnumeration && actualValue is IEnumerable<object?> actualEnumeration)
				assertLogic(string.Empty, prop.Name, expectedEnumeration, actualEnumeration);

			//--- if its an array, compare the elements -----------------------
			else if (prop.PropertyType.IsArray && expectedValue is Array expectedArray && actualValue is Array actualArray)
				assertLogic(string.Empty, prop.Name, expectedArray, actualArray);

			else
				assertLogic(string.Empty, prop.Name, expectedValue, actualValue);
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
