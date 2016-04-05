using System;
using FWD.BusinessObjects.Domain;

namespace FWD.UI.Web.Models.Entities.Dtos
{
	public class TransactionWithoutUserDto
	{
		public int Id { get; set; }
		public DateTime ActionDateTime { get; set; }
		public string ActionType { get; set; }
		public string EntityType { get; set; }
		public string Description { get; set; }
		public int EntityId { get; set; }
		public string Parameters { get; set; }
		public int EntityTypeInt
		{
			get
			{
				switch (EntityType)
				{
					case "Article":
						return 1;
					case "Group":
						return 2;
					case "Image":
						return 3;
					case "Comment":
						return 4;
					case "User":
						return 5;
					case "Default":
						return 0;
					default:
						return 0;
				}
			}
		}

		public int ActionTypeInt
		{
			get
			{
				switch (ActionType)
				{
					case "Adding":
						return 1;
					case "Updating":
						return 2;
					case "Deleting":
						return 3;
					case "Import":
						return 5;
					case "Export":
						return 6;
					case "SaveToXml":
						return 7;
					case "Read":
						return 4;
					case "Default":
						return 0;
					default:
						return 0;
				}
			}
		}
		public TransactionWithoutUserDto(Transaction trans)
		{
			Id = trans.Id;
			EntityId = trans.EntityId;
			Description = trans.Description;
			ActionDateTime = trans.ActionDateTime;
			EntityType = trans.EntityType;
			Parameters = trans.Parameters;
			ActionType = trans.ActionType;
		}
	}
}