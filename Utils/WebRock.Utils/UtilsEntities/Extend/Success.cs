using System;
using WebRock.Utils.Monad;

namespace WebRock.Utils.UtilsEntities.Extend
{
	public sealed class Success<T> : Try<T>
	{
		private readonly T _value;

		public T Value
		{
			get
			{
				return this._value;
			}
		}

		public bool IsSuccess
		{
			get
			{
				return true;
			}
		}

		public Success(T value)
		{
			this._value = value;
		}

		public T GetOrElse(Func<T> @default)
		{
			return this._value;
		}

		public Try<T> OrElse(Func<Try<T>> @default)
		{
			return (Try<T>)this;
		}

		public Maybe<T> ToMaybe()
		{
			return MaybeExtension.ToMaybe<T>(this._value);
		}

		public Try<U> Select<U>(Func<T, U> selector)
		{
			try
			{
				return (Try<U>)new Success<U>(selector(this._value));
			}
			catch (Exception ex)
			{
				return (Try<U>)new Failure<U>(ex);
			}
		}

		public Try<T> Where(Func<T, bool> filter)
		{
			if (filter(this._value))
				return (Try<T>)this;
			return (Try<T>)new Failure<T>((Exception)new ArgumentOutOfRangeException(StringExtensions.Fmt("Predicate does not hold for {0}", new object[1]
      {
        (object) this._value
      }), (Exception)null));
		}

		public Try<U> SelectMany<U>(Func<T, Try<U>> selector)
		{
			return selector(this._value);
		}

		public void Switch(Action<T> sucess, Action<Exception> exception)
		{
			sucess(this._value);
		}

		public Try<T> Recover(Func<Exception, T> recover)
		{
			return (Try<T>)this;
		}

		public Try<T> Recover(Func<Exception, Try<T>> recover)
		{
			return (Try<T>)this;
		}
	}
}
