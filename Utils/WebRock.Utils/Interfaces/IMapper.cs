using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace WebRock.Utils.Interfaces
{
	public interface IMapper
	{
		IEnumerable<PropertyInfo> GetPropertyFromMapping(string propName);
		IMemberHelper MemberHelper { get; }

	}

	public interface IMapper<TFrom, TTo>:IMapper
	{
		void AddMapping(Expression<Func<TFrom, object>> from, Expression<Func<TTo, object>> to);
		void RemoveMapping(Expression<Func<TFrom, object>> from);
	}
}
