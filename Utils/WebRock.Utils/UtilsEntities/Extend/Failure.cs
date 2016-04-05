using System;
using WebRock.Utils.Monad;

namespace WebRock.Utils.UtilsEntities.Extend
{
	public sealed class Failure<T> : Try<T>
	{
		public Exception Exception { get; private set; }

		public T Value
		{
			get
			{
				throw this.Exception;
			}
		}

		public bool IsSuccess
		{
			get
			{
				return false;
			}
		}

		public Failure(Exception exception)
		{
			this.Exception = exception;
		}

		public T GetOrElse(Func<T> @default)
		{
			return @default();
		}

		public Try<T> OrElse(Func<Try<T>> @default)
		{
			return @default();
		}

		public Maybe<T> ToMaybe()
		{
			return (Maybe<T>)Maybe.Nothing;
		}

		public Try<U> Select<U>(Func<T, U> selector)
		{
			return (Try<U>)new Failure<U>(this.Exception);
		}

		public Try<T> Where(Func<T, bool> filter)
		{
			return (Try<T>)this;
		}

		public Try<U> SelectMany<U>(Func<T, Try<U>> selector)
		{
			return (Try<U>)new Failure<U>(this.Exception);
		}

		public void Switch(Action<T> sucess, Action<Exception> exception)
		{
			exception(this.Exception);
		}

		public Try<T> Recover(Func<Exception, T> recover)
		{
			return (Try<T>)new Success<T>(recover(this.Exception));
		}

		public Try<T> Recover(Func<Exception, Try<T>> recover)
		{
			return recover(this.Exception);
		}
	}
}
