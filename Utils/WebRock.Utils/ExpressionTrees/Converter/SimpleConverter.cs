using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WebRock.Utils.ExpressionTrees.ExpressionVisitors;
using WebRock.Utils.ExpressionTrees.Internals;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Converter
{
	public class SimpleConverter<TTo, TFrom> : IExpressionConverter<TTo, TFrom>
	{
		private WebRockExpressionConverterVisitor _visitor = new WebRockExpressionConverterVisitor();

		public IMapper<TTo, TFrom> Mapper { get; private set; }

		public IMemberHelper MemberHelper { get; private set; }

		public SimpleConverter(IMapper<TTo, TFrom> mapper = null, IMemberHelper helper = null)
		{
			this.Mapper = mapper ?? (IMapper<TTo, TFrom>)new SimpleMapper<TTo, TFrom>((IMemberHelper)null);
			this.MemberHelper = helper ?? (IMemberHelper)new MemberExpressionHelper();
		}

		public Expression<Func<TTo, bool>> Convert(Expression<Func<TFrom, bool>> predicate)
		{
			MaybeExtension.GetOrDefault<BinaryExpression>(MaybeExtension.MaybeAs<BinaryExpression>((object)predicate.Body, true), (BinaryExpression)null);
			MaybeExtension.GetOrDefault<UnaryExpression>(MaybeExtension.MaybeAs<UnaryExpression>((object)predicate.Body, true), (UnaryExpression)null);
			MaybeExtension.GetOrDefault<MethodCallExpression>(MaybeExtension.MaybeAs<MethodCallExpression>((object)predicate.Body, true), (MethodCallExpression)null);
			return (Expression<Func<TTo, bool>>)null;
		}

		private Expression DisassemblyBinaryExpression(Expression binBodyExpression)
		{
			if (binBodyExpression == null)
				return (Expression)null;
			this._visitor.Visit(binBodyExpression);
			return (Expression)null;
		}
	}
}
