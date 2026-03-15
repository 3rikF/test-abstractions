
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using ErikForwerk.TestAbstractions.Models;
using ErikForwerk.TestAbstractions.Tools;

using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tests;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class AutoPropertiesTest(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Nested Types

	private enum ETestEnum
	{
		Unset,
		One,
		Two,
		Three,
		Four,
		Five,
	}

	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private class TestClassFlat
	{
		public ulong ULongProperty { get; set; }
		public long LongProperty { get; set; }

		public uint UIntProperty { get; set; }
		public int IntProperty { get; set; }

		public ushort UShortProperty { get; set; }
		public short ShortProperty { get; set; }

		public byte UByteProperty { get; set; }
		public sbyte ByteProperty { get; set; }

		public float FloatProperty { get; set; }
		public double DoubleProperty { get; set; }
		public decimal DecimalProperty { get; set; }

		public char CharProperty { get; set; }

		public string? StringProperty1 { get; set; }
		public string StringProperty2 { get; set; } = string.Empty;

		public bool BoolProperty { get; set; }
		public ETestEnum EnumProperty { get; set; }

		public string[]? StringArray1 { get; set; }
		public string[] StringArray2 { get; set; } = [];

		public int[]? IntArray1 { get; set; }
		public int[] IntArray2 { get; set; } = [];

		public char[] CharArray { get; set; } = [];

		public List<int>? IntList1 { get; set; }
		public List<int> IntList2 { get; set; } = [];

		public IEnumerable<int>? IntEnumerable1 { get; set; }
		public IEnumerable<int> IntEnumerable2 { get; set; } = [];

		public ICollection<int>? IntCollection1 { get; set; }
		public ICollection<int> IntCollection2 { get; set; } = [];

		public DateTime? DateTime1 { get; set; }
		public DateTime DateTime2 { get; set; }

		public DateTime? DateTimeOffset1 { get; set; }
		public DateTimeOffset DateTimeOffset2 { get; set; }

		public Uri? UriProperty { get; set; }
		public Guid? GuidProperty { get; set; }
		public DateOnly? DateOnlyProperty { get; set; }
		public TimeOnly? TimeOnlyProperty { get; set; }
	}

	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed class TestClassTree : TestClassFlat
	{
		public TestClassFlat? ChildObject { get; set; }

		public object? Object1 { get; set; }
		public object Object2 { get; set; } = new object();

		public object[]? ObjectArray1 { get; set; }
		public object[] ObjectArray2 { get; set; } = [];

		public List<object>? ObjectList1 { get; set; }
		public List<object> ObjectList2 { get; set; } = [];

	}

	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed class TestClassWithUnsupportedTypes
	{
		public CancellationToken? CancellationTokenProperty { get; set; }
	}

	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed record TestClassWithoutDefaultConstructor(string Name)
	{ }

	#endregion Nested Types

	//-----------------------------------------------------------------------------------------------------------------
	#region GenerateClassInstance

	[Fact]
	public void GenerateClassInstance_ViaGeneric()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut		= new();

		//--- ACT -------------------------------------------------------------
		TestClassFlat result	= sut.GenerateClassInstance<TestClassFlat>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated flat object");
	}

	[Fact]
	public void GenerateClassInstance_ViaTypeParameter()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut		= new();

		//--- ACT -------------------------------------------------------------
		TestClassFlat result	= sut.GenerateClassInstance<TestClassFlat>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated flat object");
	}

	private static void AssertNotDefaultValues(TestClassFlat result)
	{
		//--- numeric types ---
		Assert.NotEqual(default, result.ULongProperty);
		Assert.NotEqual(default, result.LongProperty);
		Assert.NotEqual(default, result.UIntProperty);
		Assert.NotEqual(default, result.IntProperty);
		Assert.NotEqual(default, result.UShortProperty);
		Assert.NotEqual(default, result.ShortProperty);
		Assert.NotEqual(default, result.UByteProperty);
		Assert.NotEqual(default, result.ByteProperty);
		Assert.NotEqual(default, result.FloatProperty);
		Assert.NotEqual(default, result.DoubleProperty);
		Assert.NotEqual(default, result.DecimalProperty);

		//--- char ---
		Assert.NotEqual(default, result.CharProperty);

		//--- strings ---
		Assert.NotNull(result.StringProperty1);
		Assert.NotEqual(string.Empty, result.StringProperty2);

		//--- bool is intentionally skipped (only 2 values, 50% chance of matching default) ---

		//--- enum (generator excludes default value) ---
		Assert.NotEqual(default, result.EnumProperty);

		//--- arrays ---
		Assert.NotNull(result.StringArray1);
		Assert.NotEmpty(result.StringArray2);
		Assert.NotNull(result.IntArray1);
		Assert.NotEmpty(result.IntArray2);
		Assert.NotEmpty(result.CharArray);

		//--- lists ---
		Assert.NotNull(result.IntList1);
		Assert.NotEmpty(result.IntList2);

		//--- enumerables ---
		Assert.NotNull(result.IntEnumerable1);
		Assert.NotEmpty(result.IntEnumerable2);

		//--- collections ---
		Assert.NotNull(result.IntCollection1);
		Assert.NotEmpty(result.IntCollection2);

		//--- date/time types ---
		_ = Assert.NotNull(result.DateTime1);
		Assert.NotEqual(default, result.DateTime2);

		_ = Assert.NotNull(result.DateTimeOffset1);
		Assert.NotEqual(default, result.DateTimeOffset2);
	}

	[Fact]
	public void GenerateClassInstance_ViaGeneric_DoesNotReturnDefaultValues()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut		= new();

		//--- ACT -------------------------------------------------------------
		TestClassFlat result	= sut.GenerateClassInstance<TestClassFlat>();

		//--- ASSERT ----------------------------------------------------------
		AssertNotDefaultValues(result);

		TestConsole.WriteLine($"[✔️ PASSED] All properties have non-default values");
	}

	[Fact]
	public void GenerateClassInstance_ViaTypeParameter_DoesNotReturnDefaultValues()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut		= new();

		//--- ACT -------------------------------------------------------------
		TestClassFlat result	= (TestClassFlat)sut.GenerateClassInstance(typeof(TestClassFlat));

		//--- ASSERT ----------------------------------------------------------
		AssertNotDefaultValues(result);

		TestConsole.WriteLine($"[✔️ PASSED] All properties have non-default values");
	}

	[Fact]
	public void GetTestInstance_ObjectTree_Simple()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut		= new(new Random());

		//--- ACT -------------------------------------------------------------
		TestClassTree result	= sut.GenerateClassInstance<TestClassTree>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated object-tree");
	}

	[Fact]
	public void GenerateClassInstance_Repeatability_DifferentInstances()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uutA = new();
		AutoProperties uutB = new();

		//--- ACT -------------------------------------------------------------
		TestClassFlat resultA = uutA.GenerateClassInstance<TestClassFlat>();
		TestClassFlat resultB = uutB.GenerateClassInstance<TestClassFlat>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(resultA);
		Assert.NotNull(resultB);

		//--- compare all properties via reflection ---------------------------
		CompareHelper.AssertEqual(resultA, resultB, TestConsole);
	}

	[Fact]
	public void GenerateClassInstance_Repeatability_SameInstance()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut = new();

		//--- ACT -------------------------------------------------------------
		TestClassFlat resultA = sut.GenerateClassInstance<TestClassFlat>();
		sut.ResetRandom();

		TestClassFlat resultB = sut.GenerateClassInstance<TestClassFlat>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(resultA);
		Assert.NotNull(resultB);

		//--- compare all properties via reflection ---------------------------
		CompareHelper.AssertEqual(resultA, resultB, TestConsole);
	}

	//--- Validation Tests ---

	[Fact]
	public void GenerateClassInstance_NotSupportedType_ThrowsException()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE_PART	= "Type [CancellationToken] is not supported.";
		AutoProperties sut					= new();
	
		//--- ACT -------------------------------------------------------------
		NotSupportedException ex = Assert.Throws<NotSupportedException>(
			sut.GenerateClassInstance<TestClassWithUnsupportedTypes>);
	
		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
		Assert.Contains(EXPECTED_MESSAGE_PART, ex.Message);
	
		TestConsole.WriteLine($"[✔️ PASSED] Correctly threw {nameof(NotSupportedException)} with message containing [{EXPECTED_MESSAGE_PART}]");
	}

	[Theory]
	[InlineData(null, "Value cannot be null.")]
	[InlineData(typeof(DateTime), "Type [DateTime] must be a class with a parameterless constructor.")] //--- not a class ---
	[InlineData(typeof(TestClassWithoutDefaultConstructor), "Type [TestClassWithoutDefaultConstructor] must be a class with a parameterless constructor.")] //--- does not have default constructor ---
	public void GenerateClassInstance_WithInvalidType_ThrowsException(Type? invalidType, string expectedMessagePart)
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut = new();

		//--- ACT -------------------------------------------------------------
		ArgumentException ex = Assert.ThrowsAny<ArgumentException>(
			() => sut.GenerateClassInstance(invalidType!));

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
		Assert.Contains(expectedMessagePart, ex.Message);

		TestConsole.WriteLine($"[✔️ PASSED] Correctly threw {ex.GetType().Name} with expected message");
	}

	#endregion GenerateClassInstance

	//-----------------------------------------------------------------------------------------------------------------
	#region SetProperties

	[Fact]
	public void SetProperties_NullTarget_ThrowsException()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE_PART	= "Value cannot be null.";
		AutoProperties sut					= new();

		//--- ACT -------------------------------------------------------------
		ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
			() => sut.SetProperties<TestClassFlat>(null!));

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
		Assert.Contains(EXPECTED_MESSAGE_PART, ex.Message);
	}

	[Fact]
	public void SetProperties_ExceptProperties_DoesNotSetProperties()
	{
		//--- ARRANGE ---------------------------------------------------------
		DateTime TEST_DATETIME		= new(1234, 12, 12, 12, 12, 12);
		TestClassFlat testObjectA	= new() { DateTime1 = TEST_DATETIME };
		TestClassFlat testObjectB	= new() { DateTime1 = TEST_DATETIME };
		AutoProperties sut			= new();

		//--- ACT -------------------------------------------------------------
		sut.ResetRandom();
		sut.SetProperties(testObjectA);										//--- value should have changed ---

		sut.ResetRandom();
		sut.SetProperties(testObjectB, nameof(TestClassFlat.DateTime1));	//--- value should NOT have changed ---

		//--- Assert ----------------------------------------------------------
		Assert.NotEqual(TEST_DATETIME, testObjectA.DateTime1);
		Assert.Equal(TEST_DATETIME, testObjectB.DateTime1);
	}

	#endregion SetProperties

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Methods

	/// <summary>
	/// Verifies that all (supported) properties are covered by generating random values.
	/// </summary>
	/// <remarks>
	/// This test ensures that the <see cref="AutoProperties"/> class can generate instances of <see cref="TestClassFlat"/>
	/// with distinct property values when different random seeds are used.
	/// It first generates two instances with the same seed to confirm repeatability, then modifies the second instance
	/// with a different seed to ensure all properties differ.
	///
	/// This is a little bit tricky due to the bool-type case, as this tests uses two seeds, that have to generate different bool values.
	/// </remarks>
	[Fact]
	public void ResetRandom_ResetsAllProperties()
	{
		//--- ARRANGE ---------------------------------------------------------
		Random rand			= new(123);
		AutoProperties sut	= new(rand);

		TestClassFlat resultA = sut.GenerateClassInstance<TestClassFlat>();

		//--- currently exactly the same as the first one as the test [Repeatability_SameInstance] shows ---
		sut.ResetRandom();
		TestClassFlat resultB = sut.GenerateClassInstance<TestClassFlat>();

		//--- ACT -------------------------------------------------------------
		//--- TEST: reset the random seed to a different value to get different values for all properties ---
		sut.ResetRandom(43);
		//--- now explicitly set all properties to different values ----------
		sut.SetProperties(resultB);

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(resultA);
		Assert.NotNull(resultB);

		//--- compare all properties via reflection ---------------------------
		CompareHelper.AssertCompletelyUnequal(resultA, resultB, TestConsole);
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(0, 1)]
	[InlineData(1, 2)]
	[InlineData(3, 3)]
	public void GenerateArray_WithValidValues_GeneratesArray(int minLength, int maxLength)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestConsole.WriteLine($"Testing with minLength=[{minLength}], maxLength=[{maxLength}]");
		AutoProperties sut = new();

		int[]? result = null;

		//--- ACT -------------------------------------------------------------
		Exception ex = Record.Exception(
			() => result = sut.GenerateArray<int>(minLength, maxLength));

		//--- ASSERT ----------------------------------------------------------
		Assert.Null(ex);
		Assert.NotNull(result);

		Assert.InRange(result.Length, minLength, maxLength);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated array with length in range [{minLength}, {maxLength}]");
	}

	[Theory]
	[InlineData(0, -1,	"maxLength ('-1') must be a non-negative value.")]
	[InlineData(-1, 0,	"minLength ('-1') must be a non-negative value.")]
	[InlineData(1, -1,	"maxLength ('-1') must be a non-negative value.")]
	[InlineData(-1, -1,	"minLength ('-1') must be a non-negative value.")]
	[InlineData(1, 0,	"maxLength-minLength ('-1') must be a non-negative value.")]
	[InlineData(10, 5,	"maxLength-minLength ('-5') must be a non-negative value.")]
	public void GenerateArray_VMinMaxLength(int minLength, int maxLength, string expectedMessagePart)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestConsole.WriteLine($"Testing with minLength=[{minLength}], maxLength=[{maxLength}]");
		AutoProperties sut = new();

		int[]? result = null;

		//--- ACT -------------------------------------------------------------
		Exception ex = Record.Exception(
			() => result = sut.GenerateArray<int>(minLength, maxLength));

		TestConsole.WriteLine($"[{ex.GetType().Name}] => [{ex.Message}]");

		//--- ASSERT ----------------------------------------------------------
		Assert.Null(result);
		ArgumentOutOfRangeException aoorex = Assert.IsType<ArgumentOutOfRangeException>(ex);
		Assert.Contains(expectedMessagePart, aoorex.Message);

		TestConsole.WriteLine($"[✔️ PASSED] Correctly threw {nameof(ArgumentOutOfRangeException)}]");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(1000)]
	public void GenerateArray_SingleLength(int length)
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut = new();

		//--- ACT -------------------------------------------------------------
		int[] result = sut.GenerateArray<int>(length);

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		Assert.Equal(result.Length, length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated array with length [{length}]");
	}

	[Fact]
	public void GenerateArray_DifferentTypes()
	{
		//--- ARRANGE ---------------------------------------------------------
		const int NUM_ELEMENTS	= 3;
		AutoProperties sut		= new(new Random());

		//--- ACT -------------------------------------------------------------
		int[] intArray				= sut.GenerateArray<int>(NUM_ELEMENTS);
		string[] strings			= sut.GenerateArray<string>(NUM_ELEMENTS);
		TestClassFlat[] objectsA	= sut.GenerateArray<TestClassFlat>(NUM_ELEMENTS);
		TestClassTree[] objectsB	= sut.GenerateArray<TestClassTree>(NUM_ELEMENTS);
		object[] objectsC			= sut.GenerateArray<object>(NUM_ELEMENTS);

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(intArray);
		Assert.Equal(NUM_ELEMENTS, intArray.Length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated [{intArray.GetType().GetElementType()!.Name}]-Array");

		Assert.NotNull(strings);
		Assert.Equal(NUM_ELEMENTS, strings.Length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated [{strings.GetType().GetElementType()!.Name}]-Array");

		Assert.NotNull(objectsA);
		Assert.Equal(NUM_ELEMENTS, objectsA.Length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated [{objectsA.GetType().GetElementType()!.Name}]-Array");

		Assert.NotNull(objectsB);
		Assert.Equal(NUM_ELEMENTS, objectsB.Length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated [{objectsB.GetType().GetElementType()!.Name}]-Array");

		Assert.NotNull(objectsC);
		Assert.Equal(NUM_ELEMENTS, objectsC.Length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated [{objectsC.GetType().GetElementType()!.Name}]-Array");
	}

	[Fact]
	public void GenerateArray_NullType_ThrowsException()
	{
		//--- ARRANGE ---------------------------------------------------------
		const string EXPECTED_MESSAGE_PART	= "Value cannot be null.";
		AutoProperties sut					= new();

		//--- ACT -------------------------------------------------------------
		ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
			() => sut.GenerateArray(null!, 10));

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(ex);
		Assert.Contains(EXPECTED_MESSAGE_PART, ex.Message);

		TestConsole.WriteLine($"[✔️ PASSED] Correctly threw  [{ex.GetType().Name}] for null type");
	}

	[Fact]
	public void GetRandomEnum()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut	= new();

		//--- ACT -------------------------------------------------------------
		ETestEnum value		= sut.GetRandomEnum<ETestEnum>();

		//--- ASSERT ----------------------------------------------------------
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated random enum-value [{value}]");
	}

	[Fact]
	public void GetRandomEnumExcept()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties sut	= new();

		//--- ACT -------------------------------------------------------------
		ETestEnum[] except	= Enum.GetValues<ETestEnum>().Except([ETestEnum.Three]).ToArray();   // except all but one value
		ETestEnum value		= sut.GetRandomEnum(except);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(ETestEnum.Three, value);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully randomly chosen to single allowed enum-value [{value}]");
	}

	[Fact]
	public void GetDefaultEnumValue_ReturnsDefault()
	{
		//--- ARRANGE ---------------------------------------------------------
		MethodInfo? method = typeof(AutoProperties)
			.GetMethod("GetDefaultEnumValue", BindingFlags.NonPublic | BindingFlags.Static);

		Assert.NotNull(method);

		//--- ACT -------------------------------------------------------------
		object? result = method.Invoke(null, [typeof(ETestEnum)]);

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		_ = Assert.IsType<ETestEnum>(result);
		Assert.Equal(ETestEnum.Unset, result);

		TestConsole.WriteLine($"[✔️ PASSED] GetDefaultEnumValue returned default enum value [{result}]");
	}

	#endregion Test Methods
}
