using System;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;

namespace Text.Analizer.Implementation
{
	public class SimpleAnalizerContainer : IAnalizerContainer
	{
		private string _language;

		private string _originalString;

		private event EventHandler<EventArgs> OriginalStringChanged;

		public string OriginalString
		{
			get { return _originalString; }
		}

		public string Language
		{
			get
			{
				if (OriginalStringChanged == null)
				{
					OriginalStringChanged += (sender, args) =>
					{
						Language = LanguageFactory.DefineLanguage(OriginalString);
					};
				}

				if (string.IsNullOrEmpty(_language))
				{
					_language = LanguageFactory.DefineLanguage(OriginalString);
				}
				return _language;
			}
			set { _language = value; }
		}

		public BaseTermin Current { get; set; }
		public BaseTermin Parent { get; set; }

		public ILanguageRule Rules
		{
			get
			{
				return LanguageFactory.GetRules(Language, OriginalString);
			}
		}

		public void SetOriginalString(string str, bool isHtml = false)
		{
			_originalString = isHtml ? str.StripTagsCharArray() : str;
			if (OriginalStringChanged != null)
			{
				OriginalStringChanged.Invoke(this, new EventArgs());
			}
		}
	}
}
