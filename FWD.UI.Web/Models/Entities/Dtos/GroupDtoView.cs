using System.Collections.Generic;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class GroupDtoView:IDto
	{
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		public List<string> GroupsList { get; set; }

		public GroupDtoView()
		{
			
		}

		public GroupDtoView(ArticleGroup articleGroup)
		{
			this.GroupId = articleGroup.GroupId;
			this.GroupName = articleGroup.GroupName;
			this.GroupsList = articleGroup.Groups;
		}

		public string Type
		{
			get { return "group"; }
		}

	}
}