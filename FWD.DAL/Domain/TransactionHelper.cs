using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.DAL.Entities;
using Article = FWD.DAL.Entities.Article;
using ArticleGroup = FWD.DAL.Entities.ArticleGroup;
using Comment = FWD.DAL.Entities.Comment;
using User = FWD.DAL.Entities.User;

namespace FWD.DAL.Domain
{
	internal static class TransactionHelper
	{
		public static void AddTransaction(DbContext context, ActionType type, object entity)
		{
			var log = GetTLog(context, type, entity);
			if (context != null)
				context.Set<TransactionLog>().Add(log);
		}

		private static TransactionLog GetTLog(DbContext context, ActionType type, object entity)
		{
			var builder = new StringBuilder();
			var entityType = GetEntityType(entity);
			AppendDescription(context, builder, entityType, type, entity);
			return new TransactionLog
			{
				ActionDateTime = DateTime.Now,
				ActionType = type,
				EntityType = entityType,
				Description = builder.ToString()
			};
		}

		private static EntityType GetEntityType(object entity)
		{
			if (entity is Article || entity is BusinessObjects.Domain.Article)
			{
				return EntityType.Article;
			}
			if (entity is Comment)
			{
				return EntityType.Comment;
			}
			if (entity is User)
			{
				return EntityType.User;
			}
			if (entity is ArticleGroup)
			{
				return EntityType.Group;
			}
			if (entity is EmbdedImage || entity is Image)
			{
				return EntityType.Image;
			}
			return EntityType.Default;
		}

		private static void AppendDescription(DbContext context, StringBuilder builder, EntityType type, ActionType aType, object entity)
		{

			if (context != null && entity != null)
			{
				AppendDescriptionUsingContext(context, builder, type, aType, entity);
			}

			if (context == null && entity != null)
			{
				AppendDescriptionWithOutContext(builder, type, aType, entity);
			}

			if (aType == ActionType.Export || aType == ActionType.Import)
			{
				builder.Append(aType.GetActionTypeName());
			}
		}

		private static void AppendDescriptionUsingContext(DbContext context, StringBuilder builder, EntityType type, ActionType aType, object entity)
		{
			string additionalInfo = string.Empty;
			string identity = string.Empty;
			var enrty = context.Entry(entity);

			var prop =
				enrty.Entity.GetType()
					.GetProperties()
					.FirstOrDefault(c => c.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() != null);
			var name = enrty.Entity.GetType().GetProperties().FirstOrDefault(c => c.Name.Contains("Name"));

			identity = CreateIdentityString(entity, prop, identity, name);

			if (aType == ActionType.Updating)
			{
				additionalInfo = string.Format("Были изменены следующие поля: {0}",
					string.Join(",", enrty.CurrentValues.PropertyNames));
			}

			if (aType != ActionType.Import || aType != ActionType.Export)
			{
				builder.Append(string.Format("Сущность \"{0}\" {1} была {2}.{3}", type.GetEntityTypeName(), identity,
					aType.GetActionTypeName(), additionalInfo));
			}
		}

		private static void AppendDescriptionWithOutContext(StringBuilder builder, EntityType type, ActionType aType, object entity)
		{
			string identity = string.Empty;

			var nameProp = entity.GetType().GetProperties().FirstOrDefault(c => c.Name.ToUpper().Contains("NAME"));
			var idProp = entity.GetType().GetProperties().FirstOrDefault(c => c.Name.ToUpper().Contains("ID"));

			identity = CreateIdentityString(entity, idProp, identity, nameProp);

			builder.Append(string.Format("Сущность \"{0}\" {1} была {2}", type.GetEntityTypeName(), identity,
					aType.GetActionTypeName()));
		}

		private static string CreateIdentityString(object entity, PropertyInfo idProp, string identity, PropertyInfo nameProp)
		{
			if (idProp != null)
			{
				identity = idProp.GetValue(entity, null).ToString();
			}

			identity = identity != "0" ? string.Format("с идентификатором {0}", identity) : "";

			if (nameProp != null)
			{
				identity = string.Format("{0} и названием \"{1}\"", identity, nameProp.GetValue(entity, null));
			}
			return identity;
		}
	}
}
