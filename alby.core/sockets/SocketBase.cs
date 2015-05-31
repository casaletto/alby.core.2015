//
// SocketBase.cs
//

using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.IO ;
using System.Text ;
using System.Text.RegularExpressions ;
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Diagnostics ;	
using System.Reflection ;
using System.Threading ;
using System.Net ;
using SNS=System.Net.Sockets ;

using alby.core.threadpool ;

namespace alby.core.sockets 
{
	public class SocketBase
	{		
		//
		//
		//
		protected SNS.Socket _socket = null ;
		
		//
		//
		//
		public SNS.Socket RawSocket
		{
			get
			{
				return _socket ;
			}
			set
			{
				_socket = value ;
			}
		}
		
		//
		//
		//
		protected byte[] _receiveBuffer = new byte[ SocketConstants.SOCKET_MAX_TRANSFER_BYTES ] ;
		
		//
		//
		//
		public byte[] ReceiveBuffer
		{
			get
			{
				return _receiveBuffer ;
			}
		}
		
		//
		//
		//
		protected long _statsBytesSent     = 0 ;
		protected long _statsBytesReceived = 0 ;
		
		//
		//
		//
		public void ResetStats()
		{
			_statsBytesSent     = 0 ;
			_statsBytesReceived = 0 ;
		}
		
		//
		//
		//
		public long BytesSent
		{
			get
			{
				return _statsBytesSent ;
			}
		}
		
		//
		//
		//
		public long BytesReceived
		{
			get
			{
				return _statsBytesReceived ;
			}
		}
		
		//
		//
		//
		public SocketBase() 
		{
		}
		
		//
		//
		//
		public string ReceiveString()
		{
			int bytes = this.ReceiveBytes() ;
			if ( bytes == 0 ) return "" ;
			
			return Encoding.UTF8.GetString( _receiveBuffer, 0, bytes ) ;
		}
				
		//
		//  
		//
		public int ReceiveBytes()
		{
			Array.Clear( _receiveBuffer, 0, _receiveBuffer.Length ) ;
			
			if ( _socket == null )
				throw new SocketException( "Socket is null." ) ;
			
			_socket.Poll( -1, SNS.SelectMode.SelectRead ) ; // wait until we can read
			
			// read the 4 byte header that tells us how many bytes we will get
			byte[] readHeader = new byte[4]; 
			_socket.Receive( readHeader, 0, 4, SNS.SocketFlags.None ) ;			
			
			int bytesToRead = BitConverter.ToInt32( readHeader, 0 ) ;
			//Helper.WriteLine( "+++bytesToRead==" + bytesToRead ) ;
		
			// read the bytes in a loop
			int readBlockSize = (int) _socket.GetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.ReceiveBuffer ) ;		
			//Helper.WriteLine( "+++readBlockSize==" + readBlockSize ) ;
			
			int bytesReadSoFar = 0 ;
			while ( true )
			{
				int bytesLeftToRead = bytesToRead - bytesReadSoFar ;
				if ( bytesLeftToRead <= 0 ) break ; // finished receiving
				
				_socket.Poll( -1, SNS.SelectMode.SelectRead ) ; // wait until we can read
				
				// do the read
				int bytesToReadThisTime = Math.Min( readBlockSize, bytesLeftToRead ) ;
				
				if ( bytesToReadThisTime + bytesReadSoFar > SocketConstants.SOCKET_MAX_TRANSFER_BYTES )
					throw new SocketException( "You are trying to read " + bytesToRead + " bytes. Dont read more than " + SocketConstants.SOCKET_MAX_TRANSFER_BYTES + " bytes." ) ;
				
				int bytesReadThisTime = _socket.Receive( _receiveBuffer, bytesReadSoFar, bytesToReadThisTime, SNS.SocketFlags.None ) ;			
				//Helper.WriteLine( "+++bytesReadThisTime==" + bytesReadThisTime ) ;
			
				bytesReadSoFar += bytesReadThisTime ;				
			}
					
			_statsBytesReceived += bytesReadSoFar ; // update stats			
										
			//Helper.WriteLine( "+++finished reading"  ) ;							
			return bytesReadSoFar ;									
		}
		
		//
		//
		//
		public void Send( byte[] buffer, int bytesToSend ) 
		{
			if ( bytesToSend > SocketConstants.SOCKET_MAX_TRANSFER_BYTES )
				throw new SocketException( "You are trying to send " + bytesToSend + " bytes. Dont send more than " + SocketConstants.SOCKET_MAX_TRANSFER_BYTES + " bytes." ) ;
				
			if ( _socket == null )
				throw new SocketException( "Socket is null." ) ;
								
			_socket.Poll( -1, SNS.SelectMode.SelectWrite ) ; // wait until we can write
				
				
			// send the 4 byte header
			
			//Helper.WriteLine( "---bytesToSend==" + bytesToSend ) ;
			byte[] sendHeader = BitConverter.GetBytes( bytesToSend ) ; // first send the number of bytes that we will be sending
			_socket.Send( sendHeader, 4, 0 ) ;

			// send the bytes in a loop
			int sendBlockSize = (int) _socket.GetSocketOption( SNS.SocketOptionLevel.Socket, SNS.SocketOptionName.SendBuffer ) ;		
			//Helper.WriteLine( "---SendBlockSize==" + sendBlockSize ) ;
			
			int bytesSentSoFar = 0 ;
			while ( true )
			{
				int bytesLeftToSend = bytesToSend - bytesSentSoFar ;
				if ( bytesLeftToSend <= 0 ) break ; // finished sending
				
				_socket.Poll( -1, SNS.SelectMode.SelectWrite ) ; // wait until we can write
				
				// do the write
				int bytesToSendThisTime = Math.Min( sendBlockSize, bytesLeftToSend ) ;
				int bytesSentThisTime   = _socket.Send( buffer, bytesSentSoFar, bytesToSendThisTime, SNS.SocketFlags.None ) ;			
				//Helper.WriteLine( "---bytesSentThisTime==" + bytesSentThisTime ) ;
			
				bytesSentSoFar += bytesSentThisTime ;				
			}
					
			_statsBytesSent += bytesSentSoFar ; // update stats		
			//Helper.WriteLine( "_statsBytesSent: " + _statsBytesSent  ) ;
			
			//Helper.WriteLine( "---finished sending"  ) ;
		}
				
		//
		//
		//
		public void Send( string str ) 
		{
			byte[] buffer = Encoding.UTF8.GetBytes( str ) ; // UTF8
			
			this.Send( buffer ) ;
		}		 
		
		//
		//
		//
		public void Send( byte[] buffer ) 
		{
			this.Send( buffer, buffer.Length ) ;
		}
								
		//
		//
		//		
		protected string _separator = "___" + new string( '\x269B', 10 ) + "___" ; // the nuclear symbol, 10 of them
				
		public string DictionarySeparator
		{
			get
			{
				return _separator ;
			}
			set
			{
				_separator = value ;
			}
		}
						
		//
		// 
		//
		public void Send( Dictionary<string,string> dic ) 
		{
			string str = this.SerialiseDictionary( dic, _separator ) ;
			this.Send( str ) ;
		}		 
		
		//
		// 
		//
		public Dictionary<string,string> ReceiveDictionary()
		{
			string str = this.ReceiveString() ;
			return this.DeserialiseDictionary( str, _separator ) ;
		}
		
		//
		// add key value pairs
		//
		protected string SerialiseDictionary( Dictionary<string,string> dic, string separator )
		{
			StringBuilder bob = new StringBuilder() ;
			
			foreach( string key in dic.Keys )
			{
				string value = dic[ key ] ;
			
				bob.Append( key       ) ;
				bob.Append( separator ) ;
				
				bob.Append( value     ) ;
				bob.Append( separator ) ;
			}
			
			//Helper.WriteLine( "SerialiseDictionary\n" + bob.ToString() ) ;
			return bob.ToString() ;
		} 
		
		//
		//
		//
		protected Dictionary<string,string> DeserialiseDictionary( string str, string separator )
		{
			Dictionary<string,string> dic = new Dictionary<string,string>() ;
			
			string key   = null ;
			string value = null ;
			
			string[] items = Regex.Split( str, separator ) ;
			
			foreach ( string item in items ) 
			{
				if ( key == null ) 
					key = item ;
				else
				{
					value = item ;
					dic.Add( key, value ) ;
					
					key   = null ;
					value = null ;
				}
			}
			
			return dic ;
		} 
		
	} // end class		
}
