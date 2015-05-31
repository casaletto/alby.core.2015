using System ;
using System.Runtime.Serialization ;

using alby.core.threadpool;

namespace alby.core.sockets
{
	//
	//
	//
	public class SocketException: ApplicationException
	{
		public SocketException() : base()
		{
		}
		
		public SocketException( string message ) : base( message ) 
		{
		}

		public SocketException( string message, Exception innerException  ) : base( message, innerException ) 
		{
		}

		public SocketException( SerializationInfo info, StreamingContext context ) : base( info, context ) 
		{
		}
	}
}
