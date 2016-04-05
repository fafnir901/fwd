using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Mappers
{
	public class BaseMapper<TFrom, TTo>
	{
		protected readonly Dictionary<Expression<Func<TFrom, object>>, Expression<Func<TTo, object>>> _dictionary = new Dictionary<Expression<Func<TFrom, object>>, Expression<Func<TTo, object>>>();

		/// <summary>
		/// Проверка,явлется ли тип стандартным
		/// </summary>
		/// <param name="type">Тип для проверки</param>
		/// <returns>булевское значение</returns>
		protected bool IsStandardType(Type type)
		{
			if (type == typeof(string)
				|| type == typeof(DateTime)
				|| type == typeof(TimeSpan)
				|| type.IsPrimitive)
			{
				return true;
			}
			return false;
		}
	}
}
