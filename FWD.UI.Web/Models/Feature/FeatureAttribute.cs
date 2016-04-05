using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWD.UI.Web.Models.Feature
{
	[AttributeUsage(AttributeTargets.Method)]
	public class FeatureAttribute : ActionFilterAttribute
	{
		public Features Feature { get; private set; }
		public string ExceptionMessage { get; private set; }

		public FeatureAttribute(Features feature,string exceptionMessage)
		{
			Feature = feature;
			ExceptionMessage = exceptionMessage;
		}

		private void Validate()
		{
			if(!FeatureListModel.Instance.CheckFeatureForEnabled(Feature))
				throw new Exception(ExceptionMessage);
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			Validate();
			base.OnActionExecuting(filterContext);
		}
	}
}