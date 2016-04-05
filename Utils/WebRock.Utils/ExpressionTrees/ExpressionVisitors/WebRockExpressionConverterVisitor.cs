using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebRock.Utils.ExpressionTrees.Models;
using WebRock.Utils.Monad;

namespace WebRock.Utils.ExpressionTrees.ExpressionVisitors
{
	public class WebRockExpressionConverterVisitor : ExpressionVisitor
	{
		private readonly List<Expression> _leftVisitor = new List<Expression>();
		private readonly List<Expression> _rightVisitor = new List<Expression>();
		private readonly List<ExpressionWithMethodModel> _lastLevelNodeTypes = new List<ExpressionWithMethodModel>();
		private readonly List<ExpressionWithMethodModel> _concatNodeTypes = new List<ExpressionWithMethodModel>();

		public List<Expression> Lefts
		{
			get
			{
				return _leftVisitor;
			}
		}

		public List<Expression> Rights
		{
			get
			{
				return _rightVisitor;
			}
		}

		public List<ExpressionWithMethodModel> LastLevelNodeTypes
		{
			get
			{
				return _lastLevelNodeTypes;
			}
		}

		public List<ExpressionWithMethodModel> ConcatNodeTypes
		{
			get
			{
				return _concatNodeTypes;
			}
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (!_leftVisitor
				.Any(c => c.MaybeAs<MemberExpression>()
				.Bind((x => x.Member.MemberType == node.Member.MemberType && x.Member.Name == node.Member.Name))
				.GetOrDefault(false)))
			{
				_leftVisitor.Add(node);
			}
			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			_rightVisitor.Add(node);
			return base.VisitConstant(node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (IsLastLevelOperator(node.NodeType))
			{
				_lastLevelNodeTypes.Add(new ExpressionWithMethodModel()
				{
					CurrentMethodInfo = node.Method,
					ValueOfExpressionType = node.NodeType,
					ExpressionType = typeof (MethodCallExpression)
				});
			}
			if (IsConcatOperator(node.NodeType))
			{
				_concatNodeTypes.Add(new ExpressionWithMethodModel()
				{
					CurrentMethodInfo = node.Method,
					ValueOfExpressionType = node.NodeType
				});
			}
			return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (IsLastLevelOperator(node.NodeType))
			{
				_lastLevelNodeTypes.Add(new ExpressionWithMethodModel()
				{
					CurrentMethodInfo = node.Method,
					ValueOfExpressionType = node.NodeType,
					ExpressionType = typeof (BinaryExpression)
				});
			}
			if (IsConcatOperator(node.NodeType))
			{
				_concatNodeTypes.Add(new ExpressionWithMethodModel()
				{
					CurrentMethodInfo = node.Method,
					ValueOfExpressionType = node.NodeType
				});
			}
			return base.VisitBinary(node);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (IsLastLevelOperator(node.NodeType))
			{
				_lastLevelNodeTypes.Add(new ExpressionWithMethodModel()
				{
					CurrentMethodInfo = node.Method,
					ValueOfExpressionType = node.NodeType,
					ExpressionType = typeof (UnaryExpression)
				});
			}
			if (IsConcatOperator(node.NodeType))
			{
				_concatNodeTypes.Add(new ExpressionWithMethodModel()
				{
					CurrentMethodInfo = node.Method,
					ValueOfExpressionType = node.NodeType
				});
			}
			return base.VisitUnary(node);
		}

		private bool IsLastLevelOperator(ExpressionType type)
		{
			return type != ExpressionType.AndAlso
				&& type != ExpressionType.Or 
				&& type != ExpressionType.And
				&& type != ExpressionType.OrElse;
		}

		private bool IsConcatOperator(ExpressionType type)
		{
			return type != ExpressionType.Equal 
				&& type != ExpressionType.LessThan 
				&& (type != ExpressionType.GreaterThan && type != ExpressionType.GreaterThanOrEqual) 
				&& (type != ExpressionType.LessThanOrEqual && type != ExpressionType.Call && type != ExpressionType.Not) 
				&& type != ExpressionType.NotEqual;
		}
	}

}
