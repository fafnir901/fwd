using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebRock.Utils.Exceptions;
using WebRock.Utils.ExpressionTrees.Internals;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Monad;

namespace WebRock.Utils.Mappers
{
	public class XmlMapper<TTo>
	{
		protected readonly Dictionary<string, Expression<Func<TTo, object>>> _dictionary = new Dictionary<string, Expression<Func<TTo, object>>>();
		public XmlMapper(IMemberHelper helper = null)
		{
			MemberHelper = helper ?? new MemberExpressionHelper();
		}

		public void AddMapping(string @from, Expression<Func<TTo, object>> to)
		{
			if (!_dictionary.ContainsKey(from))
				_dictionary.Add(from, to);
		}

		public void RemoveMapping(string @from)
		{
			if (_dictionary.ContainsKey(from))
			{
				_dictionary.Remove(from);
			}
		}

		public IEnumerable<PropertyInfo> GetPropertyFromMapping(string propName)
		{
			var properties = new List<PropertyInfo>();
			var propPair = _dictionary.FirstOrDefault(c => c.Key == propName);
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
				throw new PropertyNotFoundInMappingException(string.Format("Property \"{0}\" not found in mapping or mapping not specified", propName));
			return properties;
		}

		public IMemberHelper MemberHelper { get; private set; }
	}
}
