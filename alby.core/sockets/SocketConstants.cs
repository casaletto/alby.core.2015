//
// SocketBase.cs
//

using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.IO ;
using System.Text ;
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Diagnostics ;	
using System.Reflection ;
using System.Threading ;
using System.Net ;
using SNS=System.Net.Sockets ;

using alby.core.threadpool;

namespace alby.core.sockets
{
	public class SocketConstants
	{		
		//
		//
		//
		public const int SOCKET_BACKLOG             = 200 ; 	//500     ; // # of sockets
		public const int SOCKET_MAX_TRANSFER_BYTES  = 1000000 ; // buffer size
		
		//
		//
		//
		public SocketConstants() 
		{
		}
		
	}		
}
