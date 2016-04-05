using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebRock.Utils.Monad
{
	public static class MaybeExtension
	{
		[DebuggerStepThrough]
		public static Maybe<T> MaybeAs<T>(this object obj, bool nullMeansNothing = true)
		{
			if (!nullMeansNothing && obj == null)
				return Maybe.Just((T)obj);

			return Maybe.Cast<object, T>(obj);
		}
		[DebuggerStepThrough]
		public static T GetOrDefault<T>(this Maybe<T> maybe, T @default)
		{
			return maybe == Maybe.Nothing ? @default : maybe.Value;
		}
		[DebuggerStepThrough]
		public static T GetOrElse<T>(this Maybe<T> maybe, Func<T> @else)
		{
			return maybe == Maybe.Nothing ? @else() : maybe.Value;
		}
		[DebuggerStepThrough]
		public static Maybe<T> OrElse<T>(this Maybe<T> maybe, Func<Maybe<T>> @else)
		{
			return maybe == Maybe.Nothing ? @else() : maybe;
		}
		[DebuggerStepThrough]
		public static Maybe<T> NothingIfNull<T>(this T o)
			where T : class
		{
			return Maybe.ReturnIfNotNull(o);
		}
		[DebuggerStepThrough]
		public static void Do<TFrom>(this Maybe<TFrom> m, Action<TFrom> f, Action @else = null)
		{
			if (m == Maybe.Nothing)
			{
				if (@else != null)
					@else();
			}
			else
				f(m.Value);
		}
		[DebuggerStepThrough]
		public static bool TryGetValue<T>(this Maybe<T> m, out T value)
		{
			if (m != Maybe.Nothing)
			{
				value = m.Value;
				return true;
			}

			value = default(T);
			return false;
		}
		[DebuggerStepThrough]
		public static Maybe<TTo> Bind<TFrom, TTo>(this Maybe<TFrom> m, Func<TFrom, TTo> f)
		{
			return m.Bind(x => Maybe.Return(f(x)));
		}
		[DebuggerStepThrough]
		public static Maybe<TTo> Bind<TFrom, TTo>(this Maybe<TFrom> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.Select(f);
		}
		[DebuggerStepThrough]
		public static Maybe<T> If<T>(this Maybe<T> m, Func<T, bool> condition)
		{
			return m.Bind(x => condition(x) ? x.ToMaybe() : Maybe.Nothing);
		}
		[DebuggerStepThrough]
		public static Maybe<T> Where<T>(this Maybe<T> m, Func<T, bool> condition)
		{
			return m.If(condition);
		}
		[DebuggerStepThrough]
		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
		{
			return FirstOrNothing(items, x => true);
		}
		[DebuggerStepThrough]
		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items)
		{
			return SingleOrNothing(items, x => true);
		}
		[DebuggerStepThrough]
		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items, Func<T, bool> condition)
		{
			using (var enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					if (condition(current))
						return current.ToMaybe();
				}
				return Maybe.Nothing;
			}
		}
		[DebuggerStepThrough]
		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items, Func<T, bool> condition)
		{
			Maybe<T> result = Maybe.Nothing;
			using (var enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					if (condition(current))
					{
						if (result == Maybe.Nothing)
							result = current;
						else
						{
							return Maybe.Nothing;
						}
					}
				}
				return result;
			}
		}
		[DebuggerStepThrough]
		public static IEnumerable<Maybe<TTo>> Bind<TTo, TFrom>(this IEnumerable<Maybe<TFrom>> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.Select(x => x.Select(f));
		}
		[DebuggerStepThrough]
		public static Maybe<TTo> Select<TFrom, TTo>(this Maybe<TFrom> m, Func<TFrom, Maybe<TTo>> f)
		{
			return Maybe.Bind(m, f);
		}
		[DebuggerStepThrough]
		public static TVal GetOrThrow<TVal, TError>(this Maybe<TVal> maybe, Func<TError> error) where TError : Exception
		{
			if (!maybe.HasValue)
			{
				throw error();
			}
			return maybe.Value;
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Choose<T>(this IEnumerable<Maybe<T>> items)
		{
			return items.Choose(x => x);
		}
		[DebuggerStepThrough]
		public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<Maybe<T>> items, Func<T, TResult> f)
		{
			return items.Where(i => i.HasValue).Select(i => f(i.Value));
		}

		[DebuggerStepThrough]
		public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> ma, Func<TA, Maybe<TB>> func, Func<TA, TB, TC> selector)
		{
			return ma.Bind(a => func(a).Bind(b => selector(a, b).ToMaybe()));
		}
		[DebuggerStepThrough]
		public static Maybe<T> ToMaybe<T>(this T value)
		{
			return Maybe.Just(value);
		}

	}
}
