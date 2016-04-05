using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Internals
{
	public class ExpressionAssembler
	{
		private readonly IMapper _mapper;
		private readonly IMemberHelper _memberHelper;
		private readonly InternalExpressionHelper _helper;

		public ExpressionAssembler(IMapper mapper = null, IMemberHelper memberHelper = null)
		{
			_mapper = mapper;
			_helper = new InternalExpressionHelper(_mapper);
			_memberHelper = memberHelper ?? new MemberExpressionHelper();
		}

		public BinaryExpression AssemblyExpression(List<Expression> listConvertedExpressions, List<ExpressionType> concatedNodeTypeList)
		{
			Expression parameter = null;
			for (int index = 0; index < concatedNodeTypeList.Count; ++index)
			{
				int count = index * 2;
				if (count > concatedNodeTypeList.Count)
					count = concatedNodeTypeList.Count - 1;
				var list = (listConvertedExpressions).Skip(count).Take(2).ToList();
				var expression = list.Count <= 1
					? _helper.DefineTypeOfOperator(concatedNodeTypeList[index], parameter, new[] { list[0] }, null)
					: _helper.DefineTypeOfOperator(concatedNodeTypeList[index], list[0], new[] { list[1] }, null);
				parameter = expression;
			}
			return parameter.MaybeAs<BinaryExpression>().GetOrDefault(null);
		}


		public Expression AssemblyMultiplyBinaryExpression<TTo>(IEnumerable<LastLevelNodeExpressionTreeModel> models, ParameterExpression parameter)
		{

			var model = models.FirstOrDefault(x => x.ExpressionWithMethodModel.CurrentMethodInfo != null && x.ExpressionWithMethodModel.CurrentMethodInfo.GetParameters().Any(c => c.Name == "predicate"));

			var constantList = models
				.Select(lastLevelNodeExpressionTreeModel =>
					lastLevelNodeExpressionTreeModel.RightExpression != null
					? Expression.Constant(lastLevelNodeExpressionTreeModel.RightExpression.MaybeAs<ConstantExpression>().GetOrDefault(null).Value, lastLevelNodeExpressionTreeModel.RightExpression.Type)
					: null)
				.Cast<Expression>()
				.ToList();

			
			MemberExpression memberExpression;
			if (model.LeftExpression != null)
			{
				var memberName = _memberHelper.GetMemberName(model.LeftExpression);
				var property = typeof(TTo).GetProperty(memberName);
				memberExpression = _helper.DefineMemberExpression(parameter, property, memberName);
			}
			else
			{
				var member = typeof(TTo).GetMember(typeof(TTo).FullName)[0];
				memberExpression = Expression.MakeMemberAccess(parameter, member);
			}
			return _helper.DefineTypeOfOperator(model.ExpressionWithMethodModel.ValueOfExpressionType, memberExpression, constantList, model.ExpressionWithMethodModel.CurrentMethodInfo);
		}
		public Expression AssemblySimpleBinaryExpression<TTo>(LastLevelNodeExpressionTreeModel model, ParameterExpression parameter)
		{
			var constantExpression = model.RightExpression != null
				? Expression.Constant(model.RightExpression.MaybeAs<ConstantExpression>().GetOrDefault(null).Value, model.RightExpression.Type)
				: null;

			MemberExpression memberExpression;
			if (model.LeftExpression != null)
			{
				var memberName = _memberHelper.GetMemberName(model.LeftExpression);
				var property = typeof(TTo).GetProperty(memberName);
				memberExpression = _helper.DefineMemberExpression(parameter, property, memberName);
			}
			else
			{
				var member = typeof(TTo).GetMember(typeof(TTo).FullName)[0];
				memberExpression = Expression.MakeMemberAccess(parameter, member);
			}
			return _helper.DefineTypeOfOperator(model.ExpressionWithMethodModel.ValueOfExpressionType, memberExpression, new[] { constantExpression }, model.ExpressionWithMethodModel.CurrentMethodInfo);
		}

		public Expression AssemblySimpleUnaryExpression<TTo>(LastLevelNodeExpressionTreeModel model, ParameterExpression parameter, IEnumerable<Expression> consts)
		{
			var memberName = _memberHelper.GetMemberName(model.LeftExpression);
			var property = typeof(TTo).GetProperty(memberName);
			var memberExpression = _helper.DefineMemberExpression(parameter, property, memberName);
			return _helper.DefineTypeOfOperator(model.ExpressionWithMethodModel.ValueOfExpressionType, memberExpression, consts, model.ExpressionWithMethodModel.CurrentMethodInfo);
		}

		public Expression AssemblySimpleCallExpression<TTo>(LastLevelNodeExpressionTreeModel model, ParameterExpression parameter, IEnumerable<Expression> consts)
		{
			var memberName = _memberHelper.GetMemberName(model.LeftExpression);
			var property = typeof(TTo).GetProperty(memberName);
			var memberExpression = _helper.DefineMemberExpression(parameter, property, memberName);
			return _helper.DefineTypeOfOperator(model.ExpressionWithMethodModel.ValueOfExpressionType, memberExpression, consts, model.ExpressionWithMethodModel.CurrentMethodInfo);
		}
	}
}
