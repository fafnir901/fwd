using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FWD.BusinessObjects.Domain;
using WebRock.Utils.Monad;

namespace FWD.BusinessObjects.Xml
{
	public abstract class XmlBase<T,TU> where T:class where TU:class 
	{
		public Stream Serialize()
		{
			try
			{
				var stream = new MemoryStream();
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(stream, this);
				return stream;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		public byte[] SerializeToByteArray()
		{
			using (var stream = Serialize())
			{
				return stream.MaybeAs<MemoryStream>().Bind(c => c.ToArray()).GetOrDefault(Enumerable.Empty<byte>().ToArray());
			}

		}

		public string SerializeToString()
		{
			var array = SerializeToByteArray();
			return System.Text.Encoding.UTF8.GetString(array);
		}

		public static TU Deserialize(TextReader reader)
		{
			using (reader)
			{
				var serializer = new XmlSerializer(typeof(TU));
				return serializer.Deserialize(reader).MaybeAs<TU>().GetOrDefault(null);
			}
		}

		public static List<TU> DeserializeEnumerable(XmlReader reader)
		{
			using (reader)
			{
				var serializer = new XmlSerializer(typeof(List<TU>));
				return serializer.Deserialize(reader).MaybeAs<List<TU>>().GetOrDefault(null);
			}
		}

		public static Stream Serialize(Type t,object obj)
		{
			try
			{
				var stream = new MemoryStream();
				var serializer = new XmlSerializer(t);
				serializer.Serialize(stream, obj);
				return stream;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			
		}
	}
}
