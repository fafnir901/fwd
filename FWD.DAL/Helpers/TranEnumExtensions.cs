using System;
using System.Collections.Generic;
using FWD.DAL.Entities;
using FWD.DAL.Entities.Enums;

namespace FWD.DAL.Helpers
{
	public static class TranEnumExtensions
	{
		public static string GetEntityTypeName(this EntityType type)
		{
			var entity = string.Empty;
			switch (type)
			{
				case EntityType.Article:
					entity = "������";
					break;
				case EntityType.Comment:
					entity = "�����������";
					break;
				case EntityType.Group:
					entity = "������";
					break;
				case EntityType.Image:
					entity = "�����������";
					break;
				case EntityType.User:
					entity = "������������";
					break;
				case EntityType.Plan:
					entity = "����";
					break;
				case EntityType.Tag:
					entity = "���";
					break;
				default:
					entity = "����������� ��������";
					break;
			}
			return entity;
		}

		public static KeyValuePair<Type, EntityType> GetEnityType(this object entity)
		{
			if (entity is Article)
			{
				return new KeyValuePair<Type, EntityType>(typeof(Article),EntityType.Article);
			}
			if (entity is ArticleGroup)
			{
				return new KeyValuePair<Type, EntityType>(typeof(ArticleGroup), EntityType.Group);
			}
			if (entity is EmbdedImage)
			{
				return new KeyValuePair<Type, EntityType>(typeof(EmbdedImage), EntityType.Image);
			}
			if (entity is Comment)
			{
				return new KeyValuePair<Type, EntityType>(typeof(Comment), EntityType.Comment);
			}
			if (entity is User)
			{
				return new KeyValuePair<Type, EntityType>(typeof(User), EntityType.User);
			}
			if (entity is Plan)
			{
				return new KeyValuePair<Type, EntityType>(typeof(Plan), EntityType.Plan);
			}

			if (entity is Tag)
			{
				return new KeyValuePair<Type, EntityType>(typeof(Tag), EntityType.Tag);
			}
			return new KeyValuePair<Type, EntityType>(null, EntityType.Default);
		}

		public static string GetActionTypeName(this ActionType type)
		{
			var action = string.Empty;
			switch (type)
			{
				case ActionType.Adding:
					action = "���������";
					break;
				case ActionType.Deleting:
					action = "�������";
					break;
				case ActionType.Updating:
					action = "���������";
					break;
				case ActionType.Read:
					action = "���������";
					break;
				case ActionType.Export:
					action = "��� ���������� �������";
					break;
				case ActionType.Import:
					action = "��� ���������� ������";
					break;
				case ActionType.SaveToXml:
					action = "��������� � XML";
					break;
				default:
					action = "�������������� ��������";
					break;
			}
			return action;
		}
	}
}