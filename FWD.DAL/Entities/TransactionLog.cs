using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FWD.DAL.Entities.Enums;

namespace FWD.DAL.Entities
{
	public class TransactionLog
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public DateTime ActionDateTime { get; set; }
		[Required]
		public ActionType ActionType { get; set; }
		[Required]
		public EntityType EntityType { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public int EntityId { get; set; }
		[Required]
		public virtual User User { get; set; }
		public string Parameters { get; set; }

		public Dictionary<string,string> InitParametersFromJson()
		{
			
			var parameters = new Dictionary<string, string>();
			if (Parameters != null)
			{
				var results = Parameters.Trim("{}\r\n".ToCharArray()).Split(',').WithoutEmptyStrings();
				foreach (var result in results)
				{
					var splitResult = result.Split(':');
					var key = splitResult[0];
					var value = splitResult.ToOneString();

					parameters.Add(key, value);
				}
			}
			return parameters;
		}
	}
}
