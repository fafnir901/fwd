using System.Linq.Expressions;

namespace WebRock.Utils.ExpressionTrees.Models
{
	public class LastLevelNodeExpressionTreeModel
	{
		public Expression LeftExpression { get; set; }

		public Expression RightExpression { get; set; }

		public ExpressionWithMethodModel ExpressionWithMethodModel { get; set; }
	}
}
