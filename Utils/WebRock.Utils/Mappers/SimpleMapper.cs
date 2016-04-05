using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebRock.Utils.Exceptions;
using WebRock.Utils.ExpressionTrees.Internals;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Monad;

namespace WebRock.Utils.Mappers
{
	/// <summary>
	/// Простой маппер
	/// </summary>
	/// <typeparam name="TFrom">Тип объекта из которого будет маппиться</typeparam>
	/// <typeparam name="TTo">Тип объекта на который будет маппиться</typeparam>
	public class SimpleMapper<TFrom, TTo> : BaseMapper<TFrom, TTo>, IMapper<TFrom, TTo>
	{
		/// <summary>
		/// Приниамет имплементацию IMemberHelper
		/// </summary>
		/// <param name="helper"></param>
		public SimpleMapper(IMemberHelper helper = null)
		{
			MemberHelper = helper ?? new MemberExpressionHelper();
		}

		/// <summary>
		/// Добавляет маппинг в словарик
		/// </summary>
		/// <param name="from">Expression из которого маппиться</param>
		/// <param name="to">Expression на который маппиться</param>
		/// <example>mapper.AddMapping(c=>c.User,c=>c.CurrentUser)</example>
		public void AddMapping(Expression<Func<TFrom, object>> from, Expression<Func<TTo, object>> to)
		{
			if (IsStandardType(from.Body.Type) && from.Body.Type != to.Body.Type)
			{
				string fromMemberName = MemberHelper.GetMemberName(from.Body);
				string toMemberName = MemberHelper.GetMemberName(to.Body);
				Type fromMemberType = MemberHelper.GetMemberType(from.Body);
				Type toMemberType = MemberHelper.GetMemberType(to.Body);
				string message = string.Format("Type \"{0}\" of field \"{1}\" is not equal type \"{2}\" of field \"{3}\"",
					fromMemberType, fromMemberName, toMemberType, toMemberName);
				throw new TypeOfMappingFieldIsNotEqualException(message);
			}
			if (!_dictionary.ContainsKey(from))
				_dictionary.Add(from, to);
		}

		/// <summary>
		/// Удаляет маппинг из словарика
		/// </summary>
		/// <param name="from">Expression из которого маппиться</param>
		public void RemoveMapping(Expression<Func<TFrom, object>> from)
		{
			if (_dictionary.ContainsKey(from))
			{
				_dictionary.Remove(from);
			}
		}
		/// <summary>
		/// </summary>
		/// <returns>Возвращает коллецию свойств из словаря в виде строк</returns>
		public IEnumerable<string> GetMappedProperties()
		{
			return _dictionary.Select(func => MemberHelper.GetMemberName(func.Key.Body));
		}

		/// <summary>
		/// </summary>
		/// <param name="propertyName">Название свойства</param>
		/// <returns>Возвращает коллецию свойств из словаря в виде PropetyInfo</returns>
		public IEnumerable<PropertyInfo> GetPropertyFromMapping(string propertyName)
		{
			var properties = new List<PropertyInfo>();
			var propPair = _dictionary.FirstOrDefault(c => MemberHelper.GetMemberName(c.Key.Body) == propertyName);
			var memberName = MemberHelper.GetMemberName(propPair.Value.Body);
			var type = propPair.Value.Body.MaybeAs<MemberExpression>().Bind(c => c.Expression.Type).GetOrDefault(null)
				??
				propPair.Value.Body.MaybeAs<MethodCallExpression>().Bind(c => c.Arguments.Last()
				.MaybeAs<LambdaExpression>()
				.Bind(p => p.Body)
				.GetOrDefault(null)
				.MaybeAs<MemberExpression>()
				.Bind(p => p.Expression.Type))
				.GetOrDefault(null);

			MemberHelper.GetProperty<TTo>(type, memberName, properties);

			if (!properties.Any())
				throw new PropertyNotFoundInMappingException(string.Format("Property \"{0}\" not found in mapping or mapping not specified", propertyName));
			return properties;
		}
		/// <summary>
		/// Имплементация IMemberHelper
		/// </summary>
		public IMemberHelper MemberHelper { get; private set; }
	}
}
