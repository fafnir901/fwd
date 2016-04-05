using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using FWD.DAL.Entities;
using FWD.DAL.Entities.Enums;
using WebRock.Utils.Monad;

namespace FWD.DAL.Helpers
{
	public static class TransactionHelper
	{
		public static void AddTransaction(DbContext context, ActionType type, object entity, User user)
		{
			if (user == null)
			{
				throw new Exception("Пользователь не определен");
			}
			var currentData = entity.GetEnityType();
			var description = GetDescription(currentData, type, entity, user);

			int entityId = -1;
			var entityIdProp = entity == null 
				? null 
				: entity.GetType().GetProperties().FirstOrDefault(c => c.Name.ToUpper().Contains("ID"));

			if (entityIdProp != null)
			{
				entityId = entityIdProp.GetValue(entity).MaybeAs<int>().GetOrDefault(-1);
				if (entityId == -1)
				{
					var guid = entityIdProp.GetValue(entity).MaybeAs<Guid>().GetOrDefault(default(Guid));
					entityId = guid == default(Guid) ? -1 : guid.GetHashCode();
				}
			}
			var tLog = new TransactionLog
			{
				ActionDateTime = DateTime.Now,
				ActionType = type,
				EntityType = currentData.Value,
				Description = description,
				EntityId = entityId,
				Parameters = GetJsonDictionary(entity),
				User = user
			};

			context.Set<TransactionLog>().Add(tLog);
		}

		private static string GetJsonDictionary(object entity)
		{
			if (entity != null)
			{
				var builder = new StringBuilder();
				builder.Append("{");
				var props = entity.GetType().GetProperties();
				foreach (var propertyInfo in props)
				{
					var value = propertyInfo.GetValue(entity);
					if (!propertyInfo.Name.ToUpper().Contains("TEXT")
						&& !propertyInfo.Name.ToUpper().Contains("IMAGE")
						&& !propertyInfo.Name.ToUpper().Contains("DATA"))
					{
						var format = "\"{0}\":\"{1}\",";
						builder.Append(format.Fmt(propertyInfo.Name, GetEnumerableStringFromListOrDefault(value ?? "")));
					}
				}
				builder = builder.Remove(builder.ToString().Length - 1, 1);
				builder.Append("}");
				return builder.ToString();
			}
			return string.Empty;
		}

		private static string GetEnumerableStringFromListOrDefault(object value)
		{
			var res = value.MaybeAs<IEnumerable<object>>().GetOrDefault(null);
			var result = string.Empty;
			result = res != null 
				? string.Join(",", res.Select(c => "(" + c.ToString() + ")")) 
				: value.ToString();
			return result;
		}

		private static string GetDescription(KeyValuePair<Type, EntityType> current, ActionType type, object entity, User user)
		{
			var builder = new StringBuilder();
			var firstFormat = string.Format("Сущность \"{0}\"", current.Value.GetEntityTypeName());
			const string idFormat = " с идентификатором \"{0}\"";
			const string nameFormat = " с названием \"{0}\"";
			var actionFormat = string.Format(" была {0}", type.GetActionTypeName());
			var person = string.Format(" пользователем {0} {1}", user.FirstName, user.LastName);

			if (entity != null)
			{
				var idProp = entity.GetType().GetProperties().FirstOrDefault(c => c.Name.ToUpper().Contains("ID"));
				var nameProp = entity.GetType().GetProperties().FirstOrDefault(c => c.Name.ToUpper().Contains("NAME"));

				builder.Append(firstFormat);

				if (idProp != null)
				{
					builder.Append(string.Format(idFormat, idProp.GetValue(entity)));
				}

				if (nameProp != null)
				{
					builder.Append(string.Format(nameFormat, nameProp.GetValue(entity)));
				}

				builder.Append(actionFormat);
				builder.Append(person);
			}

			if (type == ActionType.Export || type == ActionType.Import)
			{
				builder.Append(type.GetActionTypeName());
			}

			return builder.ToString();
		}
	}
}
