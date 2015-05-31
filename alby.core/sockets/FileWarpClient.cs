//
// FileWarpClient.cs
//

using System ;
using System.IO ;
using System.Text ;
using System.Collections ;
using System.Collections.Generic ;
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
	public class FileWarpClient
	{
		protected FileWarpHelper _fwh = new FileWarpHelper() ;
	
		protected SNS.AddressFamily _addressFamily  ;

		public SNS.AddressFamily AddressFamily
		{
			get
			{
				return _addressFamily ;
			}
		}

		protected string _server = "" ;
		
		public string Server
		{
			get
			{
				return _server ;
			}
		}
		
		protected int _port = 0  ;
		
		public int Port
		{
			get
			{
				return _port ;
			}
		}
		
		public FileWarpClient( SNS.AddressFamily addressFamily, string server, int port )
		{
            _addressFamily = addressFamily ;
			_server = server ;
			_port   = port ;
		}
	
		//
		// push file up to the server
		//
		public void UploadFile( string localFile )
		{
			using ( SocketClient client = new SocketClient() )
			{
				// get file size to send over
				FileInfo fi = new FileInfo( localFile ) ;
				if ( fi.Length == 0 )
					throw new SocketException( "Cant send 0 byte file " + localFile ) ;   
				
				// connect to server
				client.Connect( _addressFamily, _server, _port ) ;
				
				// call 0 : header call
				Dictionary<string,string> request = new Dictionary<string,string>()    ;
				request[ "Action"             ] = "Upload" ;
				request[ "ClientFile"         ] = Path.GetFileName( localFile ) ;
				request[ "ClientFileBytes"    ] = fi.Length.ToString() ; // long
				request[ "ClientFileChecksum" ] = _fwh.CalcChecksum( localFile ).ToString() ; 
				
				//Helper.WriteLine( "client checksum: " + request[ "ClientFileChecksum" ] ) ;						
				
				Dictionary<string,string> response = client.Call( request ) ; // ove the aether we go...				
				string rc = response[ "ResponseStatus" ] ;
				if ( rc != "OK" ) 
					throw new SocketException( "Error from FileWarp server: " + response[ "ResponseError" ] ) ;   
										
				// alloc a blob for file bytes
				int blobSize = SocketConstants.SOCKET_MAX_TRANSFER_BYTES ;
				byte[] blob = new byte[ blobSize ] ;
				
				// read and send the file - blob at a time 
				using ( FileStream stream = File.OpenRead( localFile ) )
				{
					while ( true )
					{
						int bytesRead = stream.Read( blob, 0, blobSize ) ; 
						bool isFinished = ( bytesRead == 0 ) ;
						
						// call 1a: see if the client has more bytes to send or server shutdown
						request = new Dictionary<string,string>() ;
						request[ "ClientIsFinished" ] = isFinished.ToString() ;						
						response = client.Call( request ) ; 
						rc = response[ "ResponseStatus" ] ;
						if ( rc != "OK" ) 
							throw new SocketException( "Error from FileWarp server: " + response[ "ResponseError" ] ) ;   
					
						if ( isFinished ) break ; // client finished

						// call 1b: send byte blob, see if any error or server shutdown	
						response = client.Call_SendBytesReceiveDictionary( blob, bytesRead ) ;										
						rc = response[ "ResponseStatus" ] ;
						if ( rc != "OK" ) 
							throw new SocketException( "Error from FileWarp server: " + response[ "ResponseError" ] ) ;   						
					}
				}
				
				// call 2: ok - finised sending bytes [server can commit the file], ask server to give us status				
				request = new Dictionary<string,string>()    ;
				request[ "Action" ] = "UploadCommit" ;
				response = client.Call( request ) ; 			
				rc = response[ "ResponseStatus" ] ;
				if ( rc != "OK" ) 
					throw new SocketException( "Error from FileWarp server: " + response[ "ResponseError" ] ) ;   
			
				//Helper.WriteLine( "Uploaded file ok [{0}]", localFile ) ;							
			}		
		}
	
		//
		// download a file !
		//
		public void DownloadFile( string remoteFile, string localFile )
		{
			using ( SocketClient client = new SocketClient() )
			{
				string partFile = localFile + ".part" ;
				
				// create 0 byte .part file and delete it to see if we have write permission - throws exception if we dont
				if ( File.Exists( partFile ) )
					File.Delete( partFile ) ;
					
				using ( FileStream fs = File.OpenWrite( partFile ) ) 
				{}
				File.Delete( partFile ) ;
			
				// connect to server
				client.Connect( _addressFamily, _server, _port ) ;
				
				// call 0 : header call - get file size from server & if file ok to read
				Dictionary<string,string> request = new Dictionary<string,string>()    ;
				request[ "Action"          ] = "Download" ;
				request[ "ServerFile"      ] = remoteFile ;
				
				Dictionary<string,string> response = client.Call( request ) ; // ove the aether we go...				
				string rc = response[ "ResponseStatus" ] ;
				if ( rc != "OK" ) 
					throw new SocketException( "Error from FileWarp server: " + response[ "ResponseError" ] ) ;   
				
				long remoteFileSize     = long.Parse( response[ "ServerFileBytes"    ] ) ; // long
				int  remoteFileChecksum = int.Parse ( response[ "ServerFileChecksum" ] ) ; // firstByte * 256 * 256 + midByte * 256 + lastByte
				//Helper.WriteLine( "remoteFileSize     [{0}]", remoteFileSize ) ;
				//Helper.WriteLine( "remoteFileChecksum [{0}]", remoteFileChecksum ) ;
				
				// read socket and save the file - blob at a time 
				while ( true )
				{
					// call 1a: see if the server has more bytes to send or any errors, or shutdown
 					request = new Dictionary<string,string>()    ;
					response = client.Call( request ) ; 
					rc = response[ "ResponseStatus" ] ;
					if ( rc != "OK" ) 
						throw new SocketException( "Error from FileWarp server: " + response[ "ResponseError" ] ) ;   
					
					bool isFinished = bool.Parse( response[ "ServerIsFinished" ] ) ;
					//Helper.WriteLine( "ServerIsFinished: [{0}]", isFinished ) ;
					if ( isFinished ) break ;
					
					// call 1b: read a blob of the file
					int bytesRead = client.Call_SendDictionaryReceiveBytes( request ) ; 
					//Helper.WriteLine( "bytes read [{0}]", bytesRead ) ;
					
					// write the bytesRead to the end of the .part file
					using ( FileStream stream = File.OpenWrite( partFile ) )
					{
						stream.Seek( 0, SeekOrigin.End ) ;
						stream.Write( client.ReceiveBuffer, 0, bytesRead ) ;
					}
				}
												
				// sanity check
				FileInfo fi = new FileInfo( partFile ) ;
				//Helper.WriteLine( "local file size [{0}] - [{1}]", fi.Length, partFile ) ;
				
				if ( remoteFileSize != fi.Length )
					throw new SocketException( "Mismatched local and remote file sizes - local size = " + fi.Length 
												+ ", remote size = " + remoteFileSize ) ;   				
			
				// calculate checksum
				int checksum = _fwh.CalcChecksum( partFile ) ;
				//Helper.WriteLine( "local file checksum [{0}] - [{1}]", checksum, partFile ) ;
				
				if ( remoteFileChecksum != checksum )
					throw new SocketException( "Mismatched local and remote file checksums - local checksum = " + checksum 
												+ ", remote checksum = " + remoteFileChecksum ) ;   				
				
				// all good - commit - move .part to real file
				File.Delete( localFile ) ;
				File.Move( partFile, localFile ) ;
				
				//Helper.WriteLine( "local file ok [{0}]", localFile ) ;				
			}		
		}	
		
	} // end class		

    public class FileWarpClient4 : FileWarpClient 
    {
		public FileWarpClient4( string server, int port ) 
            : base( SNS.AddressFamily.InterNetwork, server, port ) 
        {
        }
    }

    public class FileWarpClient6 : FileWarpClient 
    {
		public FileWarpClient6( string server, int port ) 
            : base( SNS.AddressFamily.InterNetworkV6, server, port ) 
        {
        }
    }

}
