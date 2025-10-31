
using ErikForwerk.TestAbstractions.Models;

using Xunit;
using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.STA.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
public abstract class StaTestBase : TestBase
{
	protected StaTestBase(ITestOutputHelper output) : base(output)
	{
	}

	//-----------------------------------------------------------------------------------------------------------------
	#region STA Threading Support

	public sealed class STATheoryAttribute : TheoryAttribute
	{ }

	public sealed class STAFactAttribute : FactAttribute
	{ }

	protected static void RunOnSTAThread(Action action)
	{
		Exception? exception = null;

		Thread thread = new(() =>
		{
			try
			{
				action();
			}
			//--- Intentional: capture for cross-thread re-throw ---
			catch (Exception ex)
			{
				exception = ex;
			}
		});

		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		thread.Join();

		if (exception is not null)
			throw exception;
	}

	protected static TReturn? RunOnSTAThread<TReturn>(Func<TReturn> action)
	{
		TReturn? result			= default;
		Exception? exception	= null;

		Thread thread = new(() =>
		{
			try
			{
				result = action();
			}
			//--- Intentional: capture for cross-thread re-throw ---
			catch (Exception ex)
			{
				exception = ex;
			}
		});

		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		thread.Join();

		if (exception is not null)
			throw exception;

		return result;
	}

	#endregion STA Threading Support
}