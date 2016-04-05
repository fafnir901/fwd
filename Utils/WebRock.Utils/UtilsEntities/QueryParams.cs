using System;

namespace WebRock.Utils.UtilsEntities
{
	/// <summary>
	/// Представление параметра запроса для педжинга
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class QueryParams<T> where T : class
	{
		/// <summary>
		/// Количество элементов для пропуска
		/// </summary>
		public int? Skip { get; set; }
		/// <summary>
		/// Количество элементов для взятия(или учитывание,хз как правильно)
		/// </summary>
		public int? Take { get; set; }
		/// <summary>
		/// Функтор, представляющий собой условия упорядовачения
		/// </summary>
		public Func<T, object> Order { get; set; }

		public QueryParams()
		{
			
		}

		/// <summary>
		/// Конструктор с параметрами(для удобства)
		/// </summary>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <param name="order"></param>
		public QueryParams(int? skip,int? take,Func<T, object> order)
		{
			this.Skip = skip;
			this.Take = take;
			this.Order = order;
		}

		public static QueryParams<T> Validate(QueryParams<T> queryParams, Func<T, object> defaultOrder, int defaultTakeCount = 999)
		{
			if (queryParams == null)
			{
				queryParams = new QueryParams<T>();
			}
			if (!queryParams.Skip.HasValue)
			{
				queryParams.Skip = new int?(0);
			}
			if (!queryParams.Take.HasValue)
			{
				queryParams.Take = new int?(defaultTakeCount);
			}
			if (queryParams.Order == null)
			{
				queryParams.Order = defaultOrder;
			}
			return queryParams;
		}
	}
}
