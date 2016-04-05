using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FWD.UI.Web.Models.Entities.Dtos;
using FWD.UI.Web.Models.Feature;
using FWD.UI.Web.Models.Helper;

namespace FWD.UI.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class TransactionController : Controller
	{
		[Route("transactions")]
		[Feature(Features.Tranc, "Просмотр журнала транзакций отключен")]
		public ActionResult GetAllTrans(string skip, string take, string order)
		{
			try
			{
				int pSkip, pTake;
				int.TryParse(skip, out pSkip);
				int.TryParse(take, out pTake);
				pTake = pTake > 0 ? pTake : 999;
				var helper = new IocHelper();

				var res = helper.TransactionService.GetAllTransactions(c => c.ActionDateTime, pSkip, pTake);
				var totalCount = helper.TransactionService.TotalCount;
				var plan = new { Trans = res.Select(c => new TransactionWithoutUserDto(c)), TotalCount = totalCount };
				return Json(plan, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

		[Route("transactions/search")]
		[Feature(Features.Tranc, "Просмотр журнала транзакций отключен")]
		public ActionResult GetTransactionBySearchParameter(string parameter, string containsValue, string skip, string take, string order)
		{
			try
			{
				if (string.IsNullOrEmpty(parameter))
					throw new Exception("Параметр поиска не задан");

				int pSkip, pTake;
				int.TryParse(skip, out pSkip);
				int.TryParse(take, out pTake);
				pTake = pTake > 0 ? pTake : 999;
				var helper = new IocHelper();

				var res = helper.TransactionService.GetTransactionBySearchParams(pSkip, pTake, parameter, containsValue);
				var transactions = new { Trans = res.Item2.Select(c=> new TransactionWithoutUserDto(c)), TotalCount = res.Item1 };
				return Json(transactions, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				throw new HttpException(500, e.Message);
			}
		}

	}
}