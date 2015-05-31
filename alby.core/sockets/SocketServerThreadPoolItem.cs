//
// SocketServerThreadPoolItem.cs
//

using System ;
using System.IO ;
using System.Data;
using System.Text ;	
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Collections ;
using System.Collections.Generic ;
using System.Reflection ;
using System.Threading ;
using System.Net ;
using SNS=System.Net.Sockets ;

using alby.core.threadpool;

namespace alby.core.sockets
{
	public abstract class SocketServerThreadPoolItem : MyThreadPoolItemBase
	{
		//
		//
		//
		protected SocketBase _socketBase = new SocketBase() ;		
		
		public SocketBase Socket
		{
			get
			{
				return _socketBase ;
			}
		}
		
		//
		//
		//
		public SocketServerThreadPoolItem() : base()
		{
		}		
						
		//
		//
		//
		public void SetRawSocket( SNS.Socket rawSocket ) 
		{
			_socketBase.RawSocket = rawSocket ;
		}
						
		//
		//
		//
		public override void Run() 
		{						
			//Console.WriteLine( "SocketServerThreadPoolItem.Run ################# #{0} starting", this.ID ) ;
			DateTime t1 = System.DateTime.Now ;
			
			try
			{
				_socketBase.RawSocket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.Linger,        new SNS.LingerOption(false, 0) ) ;
				_socketBase.RawSocket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.ReceiveBuffer, SocketConstants.SOCKET_MAX_TRANSFER_BYTES  ) ;
				_socketBase.RawSocket.SetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.SendBuffer,    SocketConstants.SOCKET_MAX_TRANSFER_BYTES  ) ;
				_socketBase.RawSocket.SetSocketOption( SNS.SocketOptionLevel.Tcp,    SNS.SocketOptionName.NoDelay,       1 ) ;
				
				// do stuff
				this.RunSocket() ; 	
			}
			catch( Exception ex )
			{
				Console.WriteLine( "SocketServerThreadPoolItem.Run ################# #{0} EXCEPTION.\n\t\t\t{1}", this.ID, ex ) ;
			}
			finally
			{
				try
				{
					_socketBase.RawSocket.Shutdown( SNS.SocketShutdown.Both ) ;
				}
				catch( Exception ) {}

				try
				{
					_socketBase.RawSocket.Close() ;						
				}
				catch( Exception ) {}
				
				TimeSpan ts = System.DateTime.Now.Subtract( t1 ) ;
				//Console.WriteLine( "SocketServerThreadPoolItem.Run ################# #{0} stopping, which ran for {1} ms.", this.ID, ts.TotalMilliseconds ) ;
			}		
		}	
		
		//
		//
		//
		public virtual void RunSocket() 
		{
			throw new NotImplementedException() ;
		}	
		
	} // end class

} // end namespace
