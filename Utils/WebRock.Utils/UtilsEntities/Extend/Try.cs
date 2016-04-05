using System;
using WebRock.Utils.Annotations;
using WebRock.Utils.Monad;

namespace WebRock.Utils.UtilsEntities.Extend
{
	public static class Try
	{
		public static Try<T> Create<T>([InstantHandle] Func<T> process)
		{
			try
			{
				return (Try<T>)new Success<T>(process());
			}
			catch (Exception ex)
			{
				return (Try<T>)new Failure<T>(ex);
			}
		}

		public static Try<T> Success<T>(T value)
		{
			return (Try<T>)new Success<T>(value);
		}
	}

	public interface Try<T>
	{
		T Value { get; }

		bool IsSuccess { get; }

		T GetOrElse(Func<T> @default);

		Try<T> OrElse(Func<Try<T>> @default);

		Maybe<T> ToMaybe();

		Try<U> Select<U>(Func<T, U> selector);

		Try<T> Where(Func<T, bool> filter);

		Try<U> SelectMany<U>(Func<T, Try<U>> selector);

		void Switch(Action<T> sucess, Action<Exception> exception);

		Try<T> Recover(Func<Exception, T> recover);

		Try<T> Recover(Func<Exception, Try<T>> recover);
	}
}
