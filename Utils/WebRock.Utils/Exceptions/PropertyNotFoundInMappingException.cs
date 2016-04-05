using System;

namespace WebRock.Utils.Exceptions
{
	public class PropertyNotFoundInMappingException:Exception
	{
		public PropertyNotFoundInMappingException()
		{
			
		}

		public PropertyNotFoundInMappingException(string message)
			: base(message)
		{
			
		}
	}
}
