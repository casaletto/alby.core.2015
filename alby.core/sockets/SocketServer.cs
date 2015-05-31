//
// SocketServer.cs
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
	public class SocketServer<T> 
		where T : SocketServerThreadPoolItem, new()
	{		
		//
		//
		//
		protected bool _stop = false ;

        //
		//
		//
		public SocketServer() 
		{
 		}

		//
		//
		//
		public void Listen4( int port, int timeoutSec, string stopFile ) 
		{
			this.Listen4( port, timeoutSec, stopFile, new Dictionary<string,string>() ) ;		
		}

        //
		//
		//
		public void Listen4( int port, int timeoutSec, string stopFile, Dictionary<string,string> parameters ) 
		{		
			this.Listen( SNS.AddressFamily.InterNetwork, port, timeoutSec, stopFile, parameters ) ;		
        }		

		//
		//
		//
		public void Listen6( int port, int timeoutSec, string stopFile ) 
		{
			this.Listen6( port, timeoutSec, stopFile, new Dictionary<string,string>() ) ;		
		}

		//
		//
		//
		public void Listen6( int port, int timeoutSec, string stopFile, Dictionary<string,string> parameters ) 
		{		
			this.Listen( SNS.AddressFamily.InterNetworkV6, port, timeoutSec, stopFile, parameters ) ;		
        }		

		//
		//
		//
		public void Listen( SNS.AddressFamily addressFamily, int port, int timeoutSec, string stopFile, Dictionary<string,string> parameters ) 
		{		
			DateTime startTime = System.DateTime.Now ;
			
			SNS.Socket listenSocket = null ;
			
			try
			{								
				// delete stop file 				
				File.Delete( stopFile ) ;
				
				// create the main listening socket
                listenSocket = new SNS.Socket( addressFamily, SNS.SocketType.Stream, SNS.ProtocolType.Tcp ) ;
					
				// set socket options
				listenSocket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.ReuseAddress,  1 ) ;
				listenSocket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.Linger,        new SNS.LingerOption(false, 0) ) ;
				listenSocket.SetSocketOption( SNS.SocketOptionLevel.Tcp,    SNS.SocketOptionName.NoDelay,       1 ) ;

                // bind the listening socket
                if ( addressFamily == SNS.AddressFamily.InterNetworkV6 )
                    listenSocket.Bind( new IPEndPoint( IPAddress.IPv6Any , port ) ) ;
                else
                if ( addressFamily == SNS.AddressFamily.InterNetwork )
                    listenSocket.Bind( new IPEndPoint( IPAddress.Any , port ) ) ;
                else
                    throw new SocketException( "Cant bind server socket. Unknown address family " + addressFamily.ToString() ) ;

                // listen
				listenSocket.Listen( SocketConstants.SOCKET_BACKLOG ) ;
				Console.WriteLine( "SocketServer - listening on port [{0}] family [{1}]", port, addressFamily.ToString() ) ;
	
				// accept client connections - run in a thread pool thread					
				using ( MyThreadPoolManager tpm = new MyThreadPoolManager( 50, 5000 ) ) // max 50 concurrent clients
				{
					while ( true ) 
					{	
						// see if it is time to stop the server
						
						if ( File.Exists( stopFile ) ) // stop listening if stop file exists
							_stop = true ;
						else
						if ( timeoutSec > 0 ) // stop listening if server timed out
						{
							TimeSpan ts = System.DateTime.Now.Subtract( startTime ) ;
							if ( ts.TotalSeconds >= timeoutSec ) 
								_stop = true ; 
						}
					
						if ( _stop ) // time to go
						{
							tpm.Shutdown() ;
							Console.WriteLine("SocketServer - listening on port [{0}] stopping...", port);
							break; 
						}
						
						if ( listenSocket.Poll( 200000, SNS.SelectMode.SelectRead ) ) // ready to accept - 0.2 sec wait max
						{
							SNS.Socket clientSocket = listenSocket.Accept() ; 
							
							T tpi = new T() ; 
							tpi.Parameters = parameters ;
							tpi.SetRawSocket( clientSocket ) ; 
							tpm.Queue( tpi ) ;
                        }
						
					}
					
					//tpm.WaitUntilAllStarted() ; // servers alive until all queued sockets execute
				}
			}
			finally // tidy up
			{
				Console.WriteLine( "SocketServer [{0}] - listening on port [{1}] stopped", addressFamily, port ) ;
				
				try
				{
					listenSocket.Shutdown( SNS.SocketShutdown.Both ) ;
				}
				catch( Exception ) {}
				
				try
				{
					listenSocket.Close () ;
				}
				catch( Exception ) {}				
			}							
		
		} // end method

	} // end class	
}
