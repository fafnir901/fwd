using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Interfaces;
using WebRock.Utils.Mappers;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.Internals
{
	internal class ExpressionDisassembler
	{
		private readonly InternalExpressionHelper _helper;
		private readonly VisitHelper _visitHelper;

		public ExpressionDisassembler(IMapper mapper = null)
		{
			_helper = new InternalExpressionHelper(mapper);
			_visitHelper = new VisitHelper();
		}

		public IEnumerable<LastLevelNodeExpressionTreeModel> DisassembleBinaryExpression(BinaryExpression binBody, List<ExpressionType> concatedList)
		{
			var leftList = _visitHelper.VisitLefts(binBody.Left);
			var rightList = _visitHelper.VisitRights(binBody.Right);
			var lastLevelNodeTypes = _visitHelper.VisitForConcatedBodyType(binBody, concatedList);
			var list1 = _helper.PutSimpleExpressionInList(binBody, leftList, rightList);

			var source1 = leftList.Concat(list1.Except(leftList));
			if (source1.Count() <= lastLevelNodeTypes.Count)
			{
				var source2 = source1
					.Where(c => c.LeftExpression == null && c.RightExpression
					.MaybeAs<ConstantExpression>()
					.Bind(x => x.Value)
					.GetOrDefault(null) == null);

				var source3 = source1.Select(c => new ExpressionWithMethodModel()
				{
					ExpressionType = c.ExpressionWithMethodModel.ExpressionType,
					ValueOfExpressionType = c.ExpressionWithMethodModel.ValueOfExpressionType,
					CurrentMethodInfo = c.ExpressionWithMethodModel.CurrentMethodInfo
				})
				.Where(c => c.ValueOfExpressionType != ExpressionType.Throw)
				.SelectMany(c => lastLevelNodeTypes
					.Where(x => c.CurrentMethodInfo != x.CurrentMethodInfo 
						&& c.ValueOfExpressionType != x.ValueOfExpressionType 
						&& c.ExpressionType != x.ExpressionType));

				if (source3.Count() == source2.Count())
				{
					var list2 = new List<LastLevelNodeExpressionTreeModel>();
					foreach (var expressionTreeModel in source2)
					{
						list2.Add(new LastLevelNodeExpressionTreeModel()
						{
							ExpressionWithMethodModel = new ExpressionWithMethodModel()
							{
								ExpressionType = source3.First().ExpressionType,
								CurrentMethodInfo = source3.First().CurrentMethodInfo,
								ValueOfExpressionType = source3.First().ValueOfExpressionType
							},
							LeftExpression = source1.FirstOrDefault((c => c.RightExpression.MaybeAs<ConstantExpression>().Bind(x => x.Value).GetOrDefault(null) == null)).LeftExpression,
							RightExpression = expressionTreeModel.RightExpression
						});
						source3.ToList().Remove(source3.First());
					}

					var list3 = source1.ToList();
					foreach (var expressionTreeModel in source2)
						list3.Remove(expressionTreeModel);
					list3.AddRange(list2);
					source1 = list3;
				}
			}
			return source1;
		}

		public IEnumerable<LastLevelNodeExpressionTreeModel> DisassembleCallExpression(MethodCallExpression callBody, List<ExpressionType> concatedList)
		{
			var list = _visitHelper.VisitLefts(callBody.Object);
			foreach (LastLevelNodeExpressionTreeModel expressionTreeModel in list)
			{
				var expressionWithMethodModel = new ExpressionWithMethodModel()
				{
					ValueOfExpressionType = ExpressionType.Call,
					CurrentMethodInfo = callBody.Method
				};
				expressionTreeModel.ExpressionWithMethodModel = expressionWithMethodModel;
			}
			return list;
		}

		public IEnumerable<LastLevelNodeExpressionTreeModel> DisassembleUnaryExpression(UnaryExpression unaryBody, List<ExpressionType> concatedList)
		{
			List<LastLevelNodeExpressionTreeModel> list = _visitHelper.VisitLefts(unaryBody.Operand);
			foreach (LastLevelNodeExpressionTreeModel expressionTreeModel in list)
			{
				var expressionWithMethodModel = new ExpressionWithMethodModel()
				{
					ValueOfExpressionType = unaryBody.NodeType,
					CurrentMethodInfo = unaryBody.Method
				};
				expressionTreeModel.ExpressionWithMethodModel = expressionWithMethodModel;
			}
			return list;
		}
	}
}
