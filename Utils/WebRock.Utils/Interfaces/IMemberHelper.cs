using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Interfaces
{
	public interface IMemberHelper
	{
		void GetProperty<TTo>(Type type, string memberName, IList<PropertyInfo> properties);
		PropertyInfo GetPropertyInfoFromType(Type nestedType);
		Type GetPropertyType(Type nestedType);
		string GetMemberName(Expression expression);
		Type GetMemberType(Expression expression);

		MemberExpression DefineMemberExpression(ParameterExpression parameter, PropertyInfo property,
			string propertyName, IEnumerable<PropertyInfo> listOfProp);
	}
}
