using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using WebRock.Utils.Monad;

namespace Text.Analizer.Termins
{
	public class Sign : BaseTermin
	{
		private int? _index;
		private static readonly Dictionary<SignEnum, char> _signDictionary = new Dictionary<SignEnum, char>
		{
			{SignEnum.Point,'.'},
			{SignEnum.Comma,','},
			{SignEnum.Excamation,'!'},
			{SignEnum.Question,'?'},
			{SignEnum.Colon,':'},
			{SignEnum.Semicolon,';'},
			{SignEnum.Quotes,'"'},
			{SignEnum.Dash,'-'}
		};

		public Sign(int? index, string sign, IEntityContainer textAnalyzerManager, BaseTermin current, bool isHtml = false)
			: base(textAnalyzerManager, current)
		{
			textAnalyzerManager.Container.SetOriginalString(sign, isHtml);//OriginalString = sign;
			textAnalyzerManager.Container.Parent = current;
			textAnalyzerManager.Container.Current = this;
			CurrentSign = SignDictionary.First(c => c.Value.Equals(sign.ToCharArray()[0])).Key;
			_index = index;
		}

		public static Dictionary<SignEnum, char> SignDictionary
		{
			get
			{
				return _signDictionary;
			}
		}

		public override int Index
		{
			get { return _index ?? 0; }
		}

		public SignEnum CurrentSign { get; set; }

		public BaseTermin GetBeforeSign()
		{
			return EntityManager.MaybeAs<ISignAnalizerManager>().Bind(c => c.GetBeforeSign()).GetOrDefault(null);
		}

		public BaseTermin GetAfterSign()
		{
			return EntityManager.MaybeAs<ISignAnalizerManager>().Bind(c => c.GetAfterSign()).GetOrDefault(null);
		}
	}

	public enum SignEnum
	{
		/// <summary>
		/// .
		/// </summary>
		Point,
		/// <summary>
		/// ,
		/// </summary>
		Comma,
		/// <summary>
		/// !
		/// </summary>
		Excamation,
		/// <summary>
		/// ?
		/// </summary>
		Question,
		/// <summary>
		/// :
		/// </summary>
		Colon,
		/// <summary>
		/// ;
		/// </summary>
		Semicolon,
		/// <summary>
		/// "
		/// </summary>
		Quotes,
		/// <summary>
		/// -
		/// </summary>
		Dash,
		NotASign = 0
	}
}
