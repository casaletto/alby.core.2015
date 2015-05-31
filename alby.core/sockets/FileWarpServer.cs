//
// FileWarpServer.cs
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

    public class FileWarpServer6 : FileWarpServer 
	{
		public FileWarpServer6( int port, int timeoutSec, string stopFile, string fileSite )
            : base( SNS.AddressFamily.InterNetworkV6, port, timeoutSec, stopFile, fileSite ) 
        {
        }

		public override void Listen()
		{			
			_server.Listen6( _port, _timeoutSec, _stopFile, GetDictionary() ) ; // this is a blocking call
		}
    }

	public class FileWarpServer4 : FileWarpServer 
	{
		public FileWarpServer4( int port, int timeoutSec, string stopFile, string fileSite )
            : base( SNS.AddressFamily.InterNetwork, port, timeoutSec, stopFile, fileSite ) 
        {
        }

 		public override void Listen()
		{			
			_server.Listen4( _port, _timeoutSec, _stopFile, GetDictionary() ) ; // this is a blocking call
		}
    }

	public abstract class FileWarpServer
	{
        protected SNS.AddressFamily _addressFamily ;
		protected int               _port        = 0  ;
		protected int               _timeoutSec  = 0  ;
		protected string            _stopFile    = "" ;
		protected string            _fileSite    = "" ;
		protected SocketServer<FileWarpServerThreadPoolItem> _server = new SocketServer<FileWarpServerThreadPoolItem>() ;		
		
		public FileWarpServer( SNS.AddressFamily addressFamily, int port, int timeoutSec, string stopFile, string fileSite )
		{
            _addressFamily = addressFamily ;
			_port = port ;
			_timeoutSec = timeoutSec ;
			_stopFile = stopFile ;
			_fileSite = fileSite ;
		}
	        
        abstract public void Listen() ;

        protected Dictionary<string,string> GetDictionary()
        {
			Dictionary<string,string> dic = new Dictionary<string,string>() ;
			dic[ "ServerFileSite" ] = _fileSite ;

            return dic ;
        }

	} // end class		

}
