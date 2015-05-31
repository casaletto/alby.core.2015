//
// FileWarpValidationException.cs 
//

using System ;
using System.Runtime.Serialization ;

using alby.core.threadpool;

namespace alby.core.sockets
{
	//
	//
	//
	public class FileWarpValidationException: ApplicationException
	{
		public FileWarpValidationException() : base()
		{
		}
		
		public FileWarpValidationException( string message ) : base( message ) 
		{
		}

		public FileWarpValidationException( string message, Exception innerException  ) : base( message, innerException ) 
		{
		}

		public FileWarpValidationException( SerializationInfo info, StreamingContext context ) : base( info, context ) 
		{
		}
	}
}
