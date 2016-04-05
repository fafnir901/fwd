using System.Collections.Generic;
using System.Linq;
using Text.Analizer.Strategies;
using Text.Analizer.Termins;
using WebRock.Utils.Monad;

namespace Text.Analizer.Implementation
{
	public class SimpleSignManager : BaseAnalizerManager, ISignAnalizerManager
	{
		private Maybe<Sentense> Parent
		{
			get
			{
				return Container.Parent.MaybeAs<Sentense>();
			}
		}

		private Maybe<Sign> Current
		{
			get
			{
				return Container.Current.MaybeAs<Sign>();
			}
		}

		public SimpleSignManager(IAnalizerContainer container, string language = null)
			: base(container, language)
		{

		}

		public BaseTermin GetBeforeSign()
		{
			int index;
			var sequense = DefineSeqaunceAndIndex(out index);
			return index != -1 ? sequense.If(c=>index > 0).Bind(c=>c[--index]).GetOrDefault(null) : null;
		}

		public IEnumerable<BaseTermin> GetBeforeSignBySign(SignEnum sign)
		{
			var sequense = Parent.Bind(c => c.Sequence.ToList());
			var res = sequense.Bind(c => c.Select(x => x.MaybeAs<Sign>().GetOrDefault(null))).GetOrDefault(null);
			var resSign = res.Where(c => c.CurrentSign == sign);
			return resSign.Select(c => c.GetBeforeSign());
		}

		private Maybe<List<BaseTermin>> DefineSeqaunceAndIndex(out int index)
		{
			var sequense = Parent.Bind(c => c.Sequence.ToList());
			index = sequense
				.Bind(c => c.FindIndex(x => x.Index == Current.Bind(z => z.Index).GetOrDefault(-1)))
				.GetOrDefault(-1);
			return sequense;
		}

		public BaseTermin GetAfterSign()
		{
			int index;
			var sequense = DefineSeqaunceAndIndex(out index);
			return index != -1 ? sequense.If(c => index < sequense.Bind(x=>x.Count).GetOrDefault(0)).Bind(c => c[++index]).GetOrDefault(null) : null;
		}
	}
}
