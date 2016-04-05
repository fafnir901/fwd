using System;
using System.Linq.Expressions;
using WebRock.Utils.Mappers;

namespace WebRock.Utils.Interfaces
{
	public interface IExpressionConverter<TTo, TFrom>
	{
		Expression<Func<TTo, bool>> Convert(Expression<Func<TFrom, bool>> predicate);
		IMapper<TTo, TFrom> Mapper { get; }

		IMemberHelper MemberHelper { get; }
	}
}
