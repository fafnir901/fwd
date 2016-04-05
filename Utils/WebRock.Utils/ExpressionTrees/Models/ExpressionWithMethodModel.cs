using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WebRock.Utils.ExpressionTrees.Models
{
	public class ExpressionWithMethodModel
	{
		public ExpressionType ValueOfExpressionType { get; set; }

		public MethodInfo CurrentMethodInfo { get; set; }

		public Type ExpressionType { get; set; }
	}
}
