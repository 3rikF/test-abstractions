
using ErikForwerk.TestAbstractions.Models;

using Xunit;
using Xunit.Abstractions;

//-----------------------------------------------------------------------------------------------------------------------------------------
namespace ErikForwerk.TestAbstractions.STA.Models;

//-----------------------------------------------------------------------------------------------------------------------------------------
public abstract class StaTestBase(ITestOutputHelper output) : TestBase(output)
{
	//-----------------------------------------------------------------------------------------------------------------
	#region STA Threading Support

	public sealed class STATheoryAttribute : TheoryAttribute
	{ }

	public sealed class STAFactAttribute : FactAttribute
	{ }

	/// <summary>
	/// Executes the specified action on a new single-threaded apartment (STA) thread and waits for its completion.
	/// Optionally invokes a finalization action after the main action completes, regardless of success or failure.
	/// </summary>
	/// <remarks>
	/// This method is useful for running code that requires STA thread context, such as certain Windows Forms or COM operations.
	/// Any exception thrown by the action is re-thrown on the calling thread after the STA thread completes.
	/// The calling thread will block until the STA thread completes.
	/// The finalization action, if provided, is always executed after the main action, even if an exception occurred.
	/// </remarks>
	/// <param name="action">
	/// The action to execute on the STA thread.
	/// This delegate is invoked synchronously and must not be <see langword="null">.
	/// </param>
	/// <param name="finally">
	/// An optional action to execute after the main action completes, whether it succeeds or throws an exception.
	/// This delegate may be <see langword="null">.
	/// </param>
	protected static void RunOnSTAThread(Action action, Action? @finally=null)
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
			finally
			{
				@finally?.Invoke();
			}
		});

		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		thread.Join();

		if (exception is not null)
			throw exception;
	}

	/// <summary>
	/// Executes the specified function on a new single-threaded apartment (STA) thread and returns its result.
	/// Optionally invokes a finalization action after the main action completes, regardless of success or failure.
	/// </summary>
	/// <remarks>
	/// This method is useful for running code that requires STA threading, such as certain Windows Forms or COM operations.
	/// Any exception thrown by the executed function is re-thrown on the calling thread.
	/// The calling thread will block until the STA thread completes.
	/// The finalization action, if provided, is always executed after the main action, even if an exception occurred.
	/// </remarks>
	/// <typeparam name="TReturn">The type of the value returned by the function to execute on the STA thread.</typeparam>
	/// <param name="action">
	/// A function that represents the operation to execute on the STA thread.
	/// This function is invoked and its result is returned.
	/// </param>
	/// <param name="finally">
	/// An optional action to invoke after the main function completes, regardless of success or exception.
	/// Receives the result of the main function, which may be null if an exception occurred.
	/// </param>
	/// <returns>
	/// The result returned by the executed function,
	/// or the default value of <typeparamref name="TReturn"/> if an exception occurs.
	/// </returns>
	protected static TReturn? RunOnSTAThread<TReturn>(Func<TReturn> action, Action<TReturn?>? @finally=null)
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
			finally
			{
				@finally?.Invoke(result);
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