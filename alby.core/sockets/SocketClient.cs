//
// SocketClient.cs
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
	public class SocketClient : SocketBase, IDisposable
	{
		protected bool _connected = false ;
	
		public SocketClient() 
		{
		}
		
		public void Connect4( string server, int port ) 
		{		
            this.Connect( SNS.AddressFamily.InterNetwork, server, port ) ;
        }

        public void Connect6( string server, int port ) 
		{		
            this.Connect( SNS.AddressFamily.InterNetworkV6, server, port ) ;
        }

		public void Connect( SNS.AddressFamily addressFamily, string server, int port ) 
		{		
            IPAddress ip = GetServerIpAddress( addressFamily, server ) ;     
            IPEndPoint ep = new IPEndPoint( ip, port ) ; 
			
            //Console.WriteLine( "SocketClient - connecting to server [{0}] [{2}:{1}] ", server, port, ip.ToString()  ) ;
        
            _socket = new SNS.Socket( addressFamily, SNS.SocketType.Stream, SNS.ProtocolType.Tcp ) ;
			_socket.Connect( ep ) ;
			
			_connected = true ;
			
			_socket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.Linger,         new SNS.LingerOption(false, 0) ) ;
			_socket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.ReceiveBuffer,  SocketConstants.SOCKET_MAX_TRANSFER_BYTES  ) ;
			_socket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.SendBuffer,     SocketConstants.SOCKET_MAX_TRANSFER_BYTES ) ;
			_socket.SetSocketOption( SNS.SocketOptionLevel.Tcp,    SNS.SocketOptionName.NoDelay,        1 ) ;			
		}
		
        protected IPAddress GetServerIpAddress( SNS.AddressFamily addressFamily, string server ) 
        {
            foreach( IPAddress address in Dns.GetHostAddresses( server ) )
            {
                if ( address.AddressFamily == addressFamily )
                    return address ; 
            }

            string str = string.Format( "Cant find ip address for server [{0}] on protocol [{1}].", server, addressFamily.ToString() ) ;
			throw new SocketException( str ) ;
        }

		//
		//
		//	
		public void Close()
		{
			if ( _connected )
			{
				if ( _socket != null )
				{
					try
					{
						_socket.Shutdown( SNS.SocketShutdown.Both ) ;
					}
					catch( Exception ) {}
					
					try
					{
						_socket.Close ();
					}
					catch( Exception ) {}
				}				
			}
			
			_connected = false ;		
		}
		
		//
		//
		//	
		public void Dispose()
		{
			this.Close() ;
		}
			
		//
		// string request, string response
		//
		public string Call( string request ) 
		{
			if ( ! _connected )
				throw new SocketException( "Socket not connected." ) ;
		
			if ( _socket == null )
				throw new SocketException( "Socket is null." ) ;
		
			this.Send( request ) ;
			
			return this.ReceiveString() ;			
		}
		
		//
		// dictionary request, dictionary response
		//
		public Dictionary<string,string> Call( Dictionary<string,string> request ) 
		{
			if ( ! _connected )
				throw new SocketException( "Socket not connected." ) ;
		
			if ( _socket == null )
				throw new SocketException( "Socket is null." ) ;
		
			this.Send( request ) ;
			
			return this.ReceiveDictionary() ;			
		}
		
		//
		// send a dictionary request, receive byte array response 
		//
		public int Call_SendDictionaryReceiveBytes( Dictionary<string,string> request ) 
		{
			if ( ! _connected )
				throw new SocketException( "Socket not connected." ) ;
		
			if ( _socket == null )
				throw new SocketException( "Socket is null." ) ;
		
			this.Send( request ) ;
			
			return this.ReceiveBytes() ;			
		}		
		
		//
		// send a byte array request, receive a dictionary response
		//
		public Dictionary<string,string> Call_SendBytesReceiveDictionary( byte[] buffer, int bytesToSend )										
		{
			if ( ! _connected )
				throw new SocketException( "Socket not connected." ) ;
		
			if ( _socket == null )
				throw new SocketException( "Socket is null." ) ;
		
			this.Send( buffer, bytesToSend ) ; 
						
			return this.ReceiveDictionary() ;			
		}
		
	}		
}
