using System;
using System.Xml.Serialization;

namespace FWD.BusinessObjects.Domain
{
	[Serializable]
	public class Comment
	{
		[XmlAttribute]
		public Guid CommentId { get; set; }
		[XmlAttribute]
		public string CommentText { get; set; }
		[XmlAttribute]
		public string UserName { get; set; }
		[XmlAttribute]
		public string  GroupName { get; set; }
		[XmlAttribute]
		public DateTime AddedDate { get; set; }

		[XmlAttribute]
		public int UserId { get; set; }
	}
}
