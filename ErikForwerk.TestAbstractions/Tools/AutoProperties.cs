
using System.Reflection;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.Tools;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class AutoProperties(Random? rand = null)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region Fields

	private const int DEFAULT_RAND_SEED = 0;

	private Random _rand = rand ?? new Random(DEFAULT_RAND_SEED);

	#endregion Fields

	//-----------------------------------------------------------------------------------------------------------------
	#region Random Helper Methods

	public void ResetRandom(int seed = DEFAULT_RAND_SEED)
		=> _rand = new Random(seed);

	public Guid GenerateGuid()
	{
		byte[] randGuidData = new byte[16];
		_rand.NextBytes(randGuidData);

		return new Guid(randGuidData);
	}

	public string GenerateString()
		=> GenerateGuid().ToString();

	public TEnum GetRandomEnum<TEnum>(params TEnum[] except) where TEnum : Enum
	{
		return (TEnum)GetRandomEnum(typeof(TEnum), except.Cast<object>().ToArray());
	}

	public object GetRandomEnum(Type enumType, params object[] except)
	{
		Enum[] allowedValues = Enum
			.GetValues(enumType)
			.Cast<Enum>()
			.Where(e => !except.Contains(e))
			.ToArray();

		return allowedValues
			.ElementAt(_rand.Next(0, allowedValues.Length));
	}

	#endregion Random Helper Methods

	//-----------------------------------------------------------------------------------------------------------------
	#region Auto Property Methods

	public T GenerateClassInstance<T>()
		where T : new()
	{
		T result = new ();
		SetProperties(result);

		return result;
	}

	public object GenerateClassInstance(Type type)
	{
		//--- check if the type is a class and has a parameterless constructor ---
		if (!type.IsClass || type.GetConstructor(Type.EmptyTypes) is null)
			throw new ArgumentException($"Type [{type.Name}] must be a class with a parameterless constructor.", nameof(type));

		object obj = Activator.CreateInstance(type)!;

		SetProperties(obj);
		return obj;
	}

	public void SetProperties<T>(T target, params string[] exceptProperties)
	{
		PropertyInfo[] properties				= typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		HashSet<string> excludedPropertyNames	= [.. exceptProperties];

		foreach (PropertyInfo prop in properties)
		{
			Type propertyType = prop.PropertyType;

			if (excludedPropertyNames.Contains(prop.Name) || !prop.CanWrite)
				continue;

			//--- check for nullable ------------------------------------------
			if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				propertyType = propertyType.GetGenericArguments()[0];

			//--- check for ye olde array -------------------------------------
			if (propertyType.IsArray)
			{
				Type elementType	= propertyType.GetElementType()!;
				prop.SetValue(target, GenerateArray(elementType, 10));
			}

			//--- check for IEnumerables --------------------------------------
			else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				Type elementType = propertyType.GetGenericArguments()[0];
				prop.SetValue(target, GenerateArray(elementType, 10));
			}

			else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
			{
				Type elementType = propertyType.GetGenericArguments()[0];
				prop.SetValue(target, GenerateArray(elementType, 10));
			}

			else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type elementType = propertyType.GetGenericArguments()[0];

				Type genericListType = typeof(List<>).MakeGenericType(elementType);
				object? genericList = Activator.CreateInstance(genericListType, GenerateArray(elementType, 10));
				prop.SetValue(target, genericList);
			}

			//--- else singe values or class instances ------------------------
			else
				prop.SetValue(target, GenerateSingleValue(propertyType));
		}
	}

	public TElement[] GenerateArray<TElement> (int length)
		=> [.. GenerateArray(typeof(TElement), length, length).Cast<TElement>()];

	public TElement[] GenerateArray<TElement> (int minLength, int maxLength = -1)
		=> [.. GenerateArray(typeof(TElement), minLength, maxLength).Cast<TElement>()];

	public Array GenerateArray(Type elementType, int length)
		=> GenerateArray(elementType, length, length);

	public Array GenerateArray(Type elementType, int minLength, int maxLength)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(minLength);
		ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
		ArgumentOutOfRangeException.ThrowIfNegative(maxLength-minLength);

		int length		= _rand.Next(minLength, maxLength);

		Array result	= Array.CreateInstance(elementType, length);

		for (int i = 0; i < result.Length; i++)
			result.SetValue(GenerateSingleValue(elementType), i);

		return result;
	}

	private object GenerateSingleValue(Type elementType)
		=> elementType switch
	{

		Type t when t == typeof(string)			=> GenerateString(),

		Type t when t == typeof(float)			=> (float)_rand.NextDouble(),
		Type t when t == typeof(double)			=> _rand.NextDouble(),
		Type t when t == typeof(decimal)		=> new decimal(_rand.NextDouble()),

		Type t when t == typeof(ulong)			=> (ulong)_rand.Next(),
		Type t when t == typeof(long)			=> (long)_rand.Next(),
		Type t when t == typeof(uint)			=> (uint)_rand.Next(),
		Type t when t == typeof(int)			=> _rand.Next(int.MinValue, int.MaxValue),
		Type t when t == typeof(ushort)			=> (ushort)_rand.Next(ushort.MaxValue),
		Type t when t == typeof(short)			=> (short)_rand.Next(short.MinValue, short.MaxValue),
		Type t when t == typeof(byte)			=> (byte)_rand.Next(byte.MaxValue),
		Type t when t == typeof(sbyte)			=> (sbyte)_rand.Next(sbyte.MinValue, sbyte.MaxValue),

		Type t when t == typeof(char)			=> (char)_rand.Next(byte.MaxValue),
		Type t when t == typeof(bool)			=> _rand.Next(0, 2) == 1,

		Type t when t == typeof(DateTime)		=> new DateTime(_rand.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks)),
		Type t when t == typeof(DateTimeOffset)	=> new DateTimeOffset(_rand.NextInt64(DateTimeOffset.MinValue.Ticks, DateTimeOffset.MaxValue.Ticks), TimeSpan.FromHours(_rand.Next(-12, 12))),

		//Type t when t == typeof(DateOnly)         => DateOnly.FromDateTime(DateTime.Now),
		//Type t when t == typeof(TimeOnly)         => TimeOnly.FromDateTime(DateTime.Now),
		//Type t when t == typeof(Guid)             => GenerateGuid(),
		//Type t when t == typeof(Uri)              => new Uri($"https://example.com/{GenerateString()}"),
		//Type t when t == typeof(CancellationToken) => new CancellationToken(),
		//Type t when t == typeof(CancellationTokenSource) => new CancellationTokenSource(),
		//Type t when t == typeof(Task)             => Task.CompletedTask,

		{ IsEnum: true }	=> GetRandomEnum(elementType, GetDefaultEnumValue(elementType)),
		{ IsClass: true }	=> GenerateClassInstance(elementType),

		_					=> throw new NotSupportedException($"Type [{elementType.Name}] is not supported.")
	};

	private static object GetDefaultEnumValue(Type enumType)
	{
		//--- get the first value of the enum ---
		Array values = Enum.GetValues(enumType);
		return values.GetValue(0)!;
	}

	#endregion Auto Property Methods
}
