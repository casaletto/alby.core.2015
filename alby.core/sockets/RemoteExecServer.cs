//
// RemoteExecServer.cs
//

using System ;
using System.IO ;
using System.Collections ;
using System.Collections.Generic ;
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
    public class RemoteExecServer6 : RemoteExecServer 
	{
		public RemoteExecServer6( int port, int timeoutSec, string stopFile )
            : base( SNS.AddressFamily.InterNetworkV6, port, timeoutSec, stopFile ) 
        {
        }

		public override void Listen()
		{			
			_server.Listen6( _port, _timeoutSec, _stopFile ) ; // this is a blocking call
		}
    }

	public class RemoteExecServer4 : RemoteExecServer 
	{
		public RemoteExecServer4( int port, int timeoutSec, string stopFile )
            : base( SNS.AddressFamily.InterNetwork, port, timeoutSec, stopFile ) 
        {
        }

		public override void Listen()
		{			
			_server.Listen4( _port, _timeoutSec, _stopFile ) ; // this is a blocking call
		}
    }

	public abstract class RemoteExecServer
	{
		protected SNS.AddressFamily _addressFamily ;
        protected int    _port        = 0  ;
		protected int    _timeoutSec  = 0  ;
		protected string _stopFile    = "" ;

		protected SocketServer<RemoteExecServerThreadPoolItem> _server = new SocketServer<RemoteExecServerThreadPoolItem>() ;		

		public RemoteExecServer( SNS.AddressFamily addressFamily, int port, int timeoutSec, string stopFile )
		{
            _addressFamily = addressFamily ;
			_port = port ;
			_timeoutSec = timeoutSec ;
			_stopFile = stopFile ;
		}

        abstract public void Listen() ;
	
	} // end class		
}
