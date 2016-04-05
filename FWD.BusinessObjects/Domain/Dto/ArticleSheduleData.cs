using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWD.BusinessObjects.Domain.Dto
{
	public class ArticleSheduleData
	{
		public Dictionary<string, IEnumerable<ArticleDto>> ArticleDtos { get; set; }

		public ArticleSheduleData(IEnumerable<ArticleDto> dtos)
		{
			ArticleDtos = new Dictionary<string, IEnumerable<ArticleDto>>();

			var list = dtos.Select(articleDto => new KeyValuePair<MonthAndYear, ArticleDto>(articleDto.AddedDate.GetOnlyMonthAndYear(), articleDto)).ToList();


			var grouped = list.GroupBy(c => c.Key);
			var result = grouped.ToDictionary(c => c.Key, c=>c.Where(x=>x.Key.Equals(c.Key)).Select(x=>x.Value));// c => c.Where(x => x.AddedDate.GetOnlyMonthAndYear().ToString() == c.Key.ToString()));

			var minDate = dtos.Min(c => c.AddedDate);
			var maxDate = dtos.Max(c => c.AddedDate);

			var lst = minDate.GetAllMonthAndYears(maxDate).ToList();
			var current = result.Keys.Union(lst).Distinct().ToList();
			foreach (var item in current)
			{
				if (!result.ContainsKey(item))
					result.Add(item, Enumerable.Empty<ArticleDto>());
			}
			ArticleDtos = result.OrderBy(c => c.Key.Year).ThenBy(c => c.Key.Month).ToDictionary(c => c.Key.ToString(), c => c.Value);
		}
	}

	public class MonthAndYear : IEquatable<MonthAndYear>, IComparable<MonthAndYear>
	{
		public int Month { get; set; }
		public int Year { get; set; }

		public MonthAndYear(DateTime dt)
		{
			Month = dt.Month;
			Year = dt.Year;
		}

		public override bool Equals(object obj)
		{
			if (obj is MonthAndYear == false)
				return false;
			return Equals(obj as MonthAndYear);
		}

		public bool Equals(MonthAndYear other)
		{
			if (other == null)
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return this.Month == other.Month && this.Year == other.Year;
		}

		public int CompareTo(MonthAndYear other)
		{
			if (Year > other.Year || (Year == other.Year && Month > other.Month)) return 1;
			else if (Year < other.Year || (Year == other.Year && Month < other.Month)) return -1;
			else return 0;

		}

		public override int GetHashCode()
		{
			return Year ^ Month * 999;
		}

		bool IEquatable<MonthAndYear>.Equals(MonthAndYear other)
		{
			return this.Equals(other);
		}

		public override string ToString()
		{
			return "{0} {1}".Fmt(Month.GetMonthString(), Year);
		}
	}

	public static class CommonExtensions
	{
		public static MonthAndYear GetOnlyMonthAndYear(this DateTime dt)
		{
			return new MonthAndYear(dt);
		}

		public static string GetMonthString(this int i)
		{
			string month = string.Empty;
			switch (i)
			{
				case 1:
					month = "January";
					break;
				case 2:
					month = "February";
					break;
				case 3:
					month = "March";
					break;
				case 4:
					month = "April";
					break;
				case 5:
					month = "May";
					break;
				case 6:
					month = "June";
					break;
				case 7:
					month = "Jule";
					break;
				case 8:
					month = "August";
					break;
				case 9:
					month = "September";
					break;
				case 10:
					month = "Octember";
					break;
				case 11:
					month = "November";
					break;
				case 12:
					month = "December";
					break;
			}
			return month;
		}
		public static IEnumerable<string> GetAllMonth(this DateTime minDate, DateTime maxDate)
		{
			var deltaYear = maxDate.Year - minDate.Year;
			var firstYear = minDate.CollectMonth();
			var lastYear = maxDate.CollectMonth();
			var bufferYear = new List<string>();
			for (int i = minDate.Year; i < maxDate.Year + 1; i++)
			{
				bufferYear.AddRange(new DateTime(i, 1, 1).CollectMonth());
			}

			return firstYear.Union(bufferYear).Union(lastYear).Distinct();
		}

		public static IEnumerable<MonthAndYear> GetAllMonthAndYears(this DateTime minDate, DateTime maxDate)
		{
			//var deltaYear = maxDate.Year - minDate.Year;
			//var firstYear = minDate.CollectMonthAndYear();
			//var lastYear = maxDate.CollectMonthAndYear();
			var bufferYear = new List<MonthAndYear>();
			for (int i = minDate.Year; i < maxDate.Year + 1; i++)
			{
				bufferYear.AddRange(new DateTime(i, 1, 1).CollectMonthAndYear());
			}

			return bufferYear; //firstYear.Union(bufferYear).Union(lastYear).Distinct();
		}

		private static IEnumerable<string> CollectMonth(this DateTime dt)
		{
			for (var i = 1; i < 13; i++)
			{
				yield return string.Format("{0} {1}", i.GetMonthString(), dt.Year);
			}
		}

		private static IEnumerable<MonthAndYear> CollectMonthAndYear(this DateTime dt)
		{
			for (var i = 1; i < 13; i++)
			{
				yield return new MonthAndYear(new DateTime(dt.Year, i, 1));//string.Format("{0} {1}", i.GetMonthString(), dt.Year);
			}
		}
	}
}
