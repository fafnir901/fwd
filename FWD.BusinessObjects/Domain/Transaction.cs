using System;
using System.Collections.Generic;

namespace FWD.BusinessObjects.Domain
{
	public class Transaction
	{
		public int Id { get; set; }
		public DateTime ActionDateTime { get; set; }
		public string ActionType { get; set; }
		public string EntityType { get; set; }
		public string Description { get; set; }
		public int EntityId { get; set; }
		public string Parameters { get; set; }
		public User User { get; set; }

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
	}

	internal enum ActionType
	{
		Default = 0,
		Adding = 1,
		Updating = 2,
		Deleting = 3,
		Read = 4,
		Import = 5,
		Export = 6,
		SaveToXml = 7
	}

	internal enum EntityType
	{
		Default = 0,
		Article = 1,
		Group = 2,
		Image = 3,
		Comment = 4,
		User = 5
	}

	internal static class TranEnumHelper
	{
		public static string GetEntityTypeName(this EntityType type)
		{
			var entity = string.Empty;
			switch (type)
			{
				case EntityType.Article:
					entity = "Статья";
					break;
				case EntityType.Comment:
					entity = "Комментарий";
					break;
				case EntityType.Group:
					entity = "Группа";
					break;
				case EntityType.Image:
					entity = "Изображение";
					break;
				case EntityType.User:
					entity = "Пользователь";
					break;
				default:
					entity = "Неизвестная сущность";
					break;
			}
			return entity;
		}

		public static string GetActionTypeName(this ActionType type)
		{
			var action = string.Empty;
			switch (type)
			{
				case ActionType.Adding:
					action = "добавлена";
					break;
				case ActionType.Deleting:
					action = "удалена";
					break;
				case ActionType.Updating:
					action = "обновлена";
					break;
				default:
					action = "неопределенное действие";
					break;
			}
			return action;
		}
	}
}
