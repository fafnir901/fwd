using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWD.BusinessObjects.Domain;
using FWD.CommonIterfaces;
using WebRock.Utils.UtilsEntities;

namespace FWD.Services
{
	public class CommentService
	{
		private ICommonRepository<Comment> _repository;

		private readonly static Lazy<CommentService> _instance = new Lazy<CommentService>(() => new CommentService());
		private CommentService() { }

		public static CommentService Instance(ICommonRepository<Comment> repository)
		{
			if (repository == null)
			{
				throw new Exception("Can not create CommentService");
			}
			_instance.Value._repository = repository;
			return _instance.Value;
		}

		public IEnumerable<Comment> GetByGroup(string groupName)
		{
			return this._repository.GetBySqlPredicate("SELECT TOP 20 * FROM Comments WHERE GroupName = @p0 ORDER BY AddedDate", groupName);
		}

		public void SaveAll(IEnumerable<Comment> comments)
		{
			this._repository.SaveMany(comments);
		}

		public void DeleteComment(Comment comment)
		{
			this._repository.Delete(comment,true);
		}

		public void Save(Comment comment)
		{
			this._repository.Save(comment, true);
		}
	}
}
