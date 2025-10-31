using System.Diagnostics.CodeAnalysis;

using ErikForwerk.TestAbstractions.Models;
using ErikForwerk.TestAbstractions.Tools;

using Xunit;
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
		public ulong ULongProperty{ get; set; }
		public long LongProperty { get; set; }

		public uint UIntProperty{ get; set; }
		public int IntProperty { get; set; }

		public ushort UShortProperty{ get; set; }
		public short ShortProperty { get; set; }

		public byte UByteProperty{ get; set; }
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
	}


	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed class TestClassTree : TestClassFlat
	{
		public TestClassFlat? ChildObject { get; set; }

		public object? Object1 {get; set;}
		public object Object2 {get; set;} = new object();

		public object []? ObjectArray1 { get; set; }
		public object[] ObjectArray2 { get; set; } = [];

		public List<object>? ObjectList1 { get; set; }
		public List<object> ObjectList2 { get; set; } = [];

	}


	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed class TestClassWithUnsupportedTypes
	{
		public Guid? GuidProperty { get; set; }
		public CancellationToken? CancellationTokenProperty { get; set; }
		public DateOnly? DateOnlyProperty { get; set; }
		public TimeOnly? TimeOnlyProperty { get; set; }
	}


	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed class TestClassWithUncooperativeTypes
	{
		public Uri? UriProperty { get; set; }
	}


	[ExcludeFromCodeCoverage(Justification = "dummy test-class without logic")]
	private sealed record TestClassWithoutDefaultConstructor(string Name)
	{ }

	#endregion Nested Types

	//-----------------------------------------------------------------------------------------------------------------
	#region GenerateClassInstance

	[Fact]
	public void GenerateClassInstance_PlainClass_Simple()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uut = new (new Random());

		//--- ACT -------------------------------------------------------------
		TestClassFlat result = uut.GenerateClassInstance<TestClassFlat>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated flat object");
	}

	[Fact]
	public void GetTestInstance_ObjectTree_Simple()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uut = new (new Random());

		//--- ACT -------------------------------------------------------------
		TestClassTree result = uut.GenerateClassInstance<TestClassTree>();

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated object-tree");
	}

	[Fact]
	public void GenerateClassInstance_Repeatability_DifferentInstances()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uutA = new ();
		AutoProperties uutB = new ();

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
		AutoProperties uut = new ();

		//--- ACT -------------------------------------------------------------
		TestClassFlat resultA = uut.GenerateClassInstance<TestClassFlat>();
		uut.ResetRandom();

		TestClassFlat resultB = uut.GenerateClassInstance<TestClassFlat>();

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
		AutoProperties uut = new ();

		//--- ACT -------------------------------------------------------------
		NotSupportedException ex = Assert.Throws<NotSupportedException>(
			uut.GenerateClassInstance<TestClassWithUnsupportedTypes>);

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
	}

	[Fact]
	public void GenerateClassInstance_NotSupportedProperties_ThrowsException()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uut = new ();

		//--- ACT -------------------------------------------------------------
		ArgumentException ex = Assert.Throws<ArgumentException>(
			uut.GenerateClassInstance<TestClassWithUncooperativeTypes>);

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
	}

	[Theory]
	[InlineData(null)]
	[InlineData(typeof(DateTime))] //--- not a class ---
	[InlineData(typeof(TestClassWithoutDefaultConstructor))] //--- does not have default constructor ---
	public void GenerateClassInstance_WithInvalidType_ThrowsException(Type? invalidType)
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uut = new ();

		//--- ACT -------------------------------------------------------------
		Exception ex = Record.Exception(
			() => uut.GenerateClassInstance(invalidType!));

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
		_ = Assert.IsType<ArgumentException>(ex, false);
	}

	#endregion GenerateClassInstance

	//-----------------------------------------------------------------------------------------------------------------
	#region SetProperties

	[Fact]
	public void SetProperties_NullTarget_ThrowsException()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties uut = new ();
		//--- ACT -------------------------------------------------------------
		ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
			() => uut.SetProperties<TestClassFlat>(null!));

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(ex);
	}

	[Fact]
	public void SetProperties_ExceptProperties_ThrowsException()
	{
		//--- ARRANGE ---------------------------------------------------------
		DateTime TEST_DATETIME		= new DateTime(1234, 12, 12, 12, 12, 12);
		AutoProperties uut			= new ();
		TestClassFlat testObjectA	= new () { DateTime1 = TEST_DATETIME };
		TestClassFlat testObjectB	= new () { DateTime1 = TEST_DATETIME };

		//--- ACT -------------------------------------------------------------
		uut.ResetRandom();
		uut.SetProperties(testObjectA);										//--- value should have changed ---

		uut.ResetRandom();
		uut.SetProperties(testObjectB, nameof(TestClassFlat.DateTime1));	//--- value should NOT have changed ---

		//--- Assert ----------------------------------------------------------
		Assert.NotEqual(TEST_DATETIME,	testObjectA.DateTime1);
		Assert.Equal(TEST_DATETIME,		testObjectB.DateTime1);
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
		Random rand			= new (123);
		AutoProperties uut	= new (rand);

		TestClassFlat resultA = uut.GenerateClassInstance<TestClassFlat>();

		//--- currently exactly the same as the first one as the test [Repeatability_SameInstance] shows ---
		uut.ResetRandom();
		TestClassFlat resultB = uut.GenerateClassInstance<TestClassFlat>();

		//--- ACT -------------------------------------------------------------
		//--- TEST: reset the random seed to a different value to get different values for all properties ---
		uut.ResetRandom(43);
		//--- now explicitly set all properties to different values ----------
		uut.SetProperties(resultB);

		//--- Assert ----------------------------------------------------------
		Assert.NotNull(resultA);
		Assert.NotNull(resultB);

		//--- compare all properties via reflection ---------------------------
		CompareHelper.AssertCompletelyUnequal(resultA, resultB, TestConsole);
	}

	[Theory]
	[InlineData(0,	-1,	true)]
	[InlineData(-1,	0,	true)]
	[InlineData(1,	-1,	true)]
	[InlineData(-1,	-1,	true)]
	[InlineData(1,	0,	true)]
	[InlineData(10,	5,	true)]

	[InlineData(0,	0,	false)]
	[InlineData(0,	1,	false)]
	[InlineData(1,	2,	false)]
	[InlineData(3,	3,	false)]
	public void GenerateArray_MinMaxLength(int minLength, int maxLength, bool exceptionExpected)
	{
		//--- ARRANGE ---------------------------------------------------------
		TestConsole.WriteLine($"Testing with minLength=[{minLength}], maxLength=[{maxLength}]");
		AutoProperties unitUnderTest = new (new Random());

		int[]? result = null;

		//--- ACT -------------------------------------------------------------
		Exception ex = Record.Exception(
			() => result = unitUnderTest.GenerateArray<int>(minLength, maxLength));

		//--- ASSERT ----------------------------------------------------------
		if (exceptionExpected)
		{
			Assert.NotNull(ex);
			Assert.Null(result)
				;
			_ = Assert.IsType<ArgumentOutOfRangeException>(ex);
			TestConsole.WriteLine($"[✔️ PASSED] Correctly threw ArgumentOutOfRangeException");
		}
		else
		{
			Assert.Null(ex);
			Assert.NotNull(result);

			Assert.InRange(result.Length, minLength, maxLength);
			TestConsole.WriteLine($"[✔️ PASSED] Successfully generated array with length in range [{minLength}, {maxLength}]");
		}
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(1000)]
	public void GenerateArray_SingleLength(int length)
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties unitUnderTest = new (new Random());

		//--- ACT -------------------------------------------------------------
		int[] result = unitUnderTest.GenerateArray<int>(length);

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(result);
		Assert.Equal(result.Length, length);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated array with length [{length}]");
	}

	[Fact]
	public void GenerateArray_DifferentTypes()
	{
		//--- ARRANGE ---------------------------------------------------------
		const int NUM_ELEMENTS = 3;
		AutoProperties unitUnderTest = new (new Random());

		//--- ACT -------------------------------------------------------------
		int[] intArray				= unitUnderTest.GenerateArray<int>(NUM_ELEMENTS);
		string[] strings			= unitUnderTest.GenerateArray<string>(NUM_ELEMENTS);
		TestClassFlat[] objectsA	= unitUnderTest.GenerateArray<TestClassFlat>(NUM_ELEMENTS);
		TestClassTree[] objectsB	= unitUnderTest.GenerateArray<TestClassTree>(NUM_ELEMENTS);
		object[] objectsC			= unitUnderTest.GenerateArray<object>(NUM_ELEMENTS);

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
		AutoProperties unitUnderTest = new (new Random());

		//--- ACT -------------------------------------------------------------
		ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
			() => unitUnderTest.GenerateArray(null!, 10));

		//--- ASSERT ----------------------------------------------------------
		Assert.NotNull(ex);
		TestConsole.WriteLine($"[✔️ PASSED] Correctly threw  [{ex.GetType().Name}] for null type");
	}

	[Fact]
	public void GetRandomEnum()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties unitUnderTest = new (new Random());

		//--- ACT -------------------------------------------------------------
		ETestEnum value = unitUnderTest.GetRandomEnum<ETestEnum>();

		//--- ASSERT ----------------------------------------------------------
		TestConsole.WriteLine($"[✔️ PASSED] Successfully generated random enum-value [{value}]");
	}

	[Fact]
	public void GetRandomEnumExcept()
	{
		//--- ARRANGE ---------------------------------------------------------
		AutoProperties unitUnderTest = new (new Random());

		//--- ACT -------------------------------------------------------------
		ETestEnum[] except	= Enum.GetValues<ETestEnum>().Except([ETestEnum.Three]).ToArray();	// except all but one value
		ETestEnum value		= unitUnderTest.GetRandomEnum(except);

		//--- ASSERT ----------------------------------------------------------
		Assert.Equal(ETestEnum.Three, value);
		TestConsole.WriteLine($"[✔️ PASSED] Successfully randomly chosen to single allowed enum-value [{value}]");
	}

	#endregion Test Methods
}
