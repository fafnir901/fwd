using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRock.Utils.Annotations.Attributes
{
	public class InlineableAttribute : Attribute
	{
		private readonly string _inlineMethodName;

		public string InlineMethodName
		{
			get
			{
				return this._inlineMethodName;
			}
		}

		public InlineableAttribute(string inlineMethodName = null)
		{
			this._inlineMethodName = inlineMethodName;
		}
	}
}
