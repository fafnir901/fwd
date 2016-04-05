using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FWD.BusinessObjects.Absrtact;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class PlanDto : IDto
	{
		public string Type
		{
			get { return "plan"; }
		}


		public PlanDto()
		{
			
		}


		public PlanDto(IPlan plan)
		{
			this.PlanId = plan.Id;
			this.PlanName = plan.Name;
			this.AddedDate = plan.AddedDate;
			this.Description = plan.Description;
			this.IsDone = plan.IsDone;
		}


		public int PlanId { get; set; }

		public string PlanName { get; set; }

		public DateTime AddedDate { get; set; }

		public DateTime? ChangedDate { get; set; }

		public string Description { get; set; }

		public bool IsDone { get; set; }
	}
}