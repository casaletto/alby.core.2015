//
// FileWarpServerThreadPoolItem.cs
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
	public class FileWarpServerThreadPoolItem : SocketServerThreadPoolItem
	{		
		//
		//
		//
		protected FileWarpHelper _fwh = new FileWarpHelper() ;
			
		//
		//
		//
		public FileWarpServerThreadPoolItem() : base()
		{	
		}		
				
		//
		// 
		//
		public override void RunSocket()
		{		
			//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} starting", this.ID ) ;
								
			// call 0. get header 
			Dictionary<string,string> request = this.Socket.ReceiveDictionary() ;
			string action =	request[ "Action" ] ;
			
			if ( action == "Upload" )
				DoUpload( request ) ;
			else
				DoDownload( request ) ;					
		}		
		
		//
		// client.DownloadFile()
		//
		protected void DoDownload( Dictionary<string,string> request )
		{
			string downloadFile = "" ;
		
			bool ok = this.DoDownload_Validation( request, out downloadFile ) ;
			if ( ! ok ) return ;
			
			ok = this.DoDownload_SendFile( downloadFile ) ;
			if ( ! ok ) return ;				
		}
		
		//
		// call 0 - header validation
		//
		protected bool DoDownload_Validation( Dictionary<string,string> request, out string downloadFile )
		{	
			downloadFile = "" ;
			
			Dictionary<string,string> response = new Dictionary<string,string>() ;
			response[ "ResponseStatus" ] = "OK" ;
			
			string slash = Path.DirectorySeparatorChar.ToString() ;
			
			try
			{
				// is required key found
				if ( ! this.Parameters.ContainsKey( "ServerFileSite" ) )
					throw new FileWarpValidationException( "Key [ServerFileSite] missing from .Parameters." ) ;
				
				string serverFileSite = this.Parameters[ "ServerFileSite" ] ;
				//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} files are at [{1}]", this.ID, serverFileSite ) ;
				
				// is required key found
				if ( ! request.ContainsKey( "ServerFile" ) )
					throw new FileWarpValidationException( "Key [ServerFile] missing from request." ) ;
			
				downloadFile = serverFileSite + slash + request[ "ServerFile" ] ;
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Downloading to client\n[{1}]", this.ID, downloadFile ) ;
			
				// see if file exists
				if ( ! File.Exists( downloadFile ) )
					throw new FileWarpValidationException( "File [" + downloadFile + "] not found." ) ;
			
				// see if we can read this file
				try
				{
					using ( FileStream fs = File.OpenRead( downloadFile ) ) 
					{}
				}
				catch( Exception ex ) 
				{
					throw new FileWarpValidationException( "File error: [" + downloadFile + "] " + ex.Message ) ;
				}
			
				// ok - file looks good to go
				FileInfo fi = new FileInfo( downloadFile ) ;
				response[ "ServerFileBytes"    ] = fi.Length.ToString()   ;
				response[ "ServerFileChecksum" ] = _fwh.CalcChecksum( downloadFile ).ToString() ;
				this.Socket.Send( response ) ;						
			}
			catch( FileWarpValidationException ex ) // validation error
			{
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Download EXCEPTION\n{1}", this.ID, ex.ToString() ) ;
				
				response[ "ResponseStatus" ] = "Error" ;
				response[ "ResponseError"  ] = ex.Message ;
				this.Socket.Send( response ) ; // send it back to client
				return false ; 
			}	
						
			return true ;
		}
		
		//
		// call 1
		//
		protected bool DoDownload_SendFile( string downloadFile )
		{
			Dictionary<string,string> response = new Dictionary<string,string>() ;
			
			try
			{
				// alloc a blob for file bytes
				int blobSize = SocketConstants.SOCKET_MAX_TRANSFER_BYTES ;
				byte[] blob = new byte[ blobSize ] ;
						
				// read and send the file - blob at a time 
				using ( FileStream stream = File.OpenRead( downloadFile ) )
				{					
					while ( true )
					{	
						// server shutdown check
						this.ShutdownCheck() ; 
					
						// send download status info to client			
						this.Socket.ReceiveDictionary() ;						
						response = new Dictionary<string,string>() ;
						response[ "ResponseStatus" ] = "OK" ;
											
						// get bytes from file, if any left to read
						int bytesRead = stream.Read( blob, 0, blobSize ) ; 
						//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run [{0}] read [{1}] bytes", this.ID, bytesRead ) ;						
						
						response[ "ServerIsFinished" ] = ( bytesRead ==	 0 ).ToString() ;
						this.Socket.Send( response ) ; // send status away
						
						// this download is finished
						if ( bytesRead == 0 ) break ;
							
						// send file bytes to client
						this.Socket.ReceiveDictionary() ;
						this.Socket.Send( blob, bytesRead ) ; 										
					}
				}				
			}
			catch( Exception ex ) 
			{
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Download EXCEPTION\n{1}", this.ID, ex.ToString() ) ;
				
				response[ "ResponseStatus" ] = "Error" ;
				response[ "ResponseError"  ] = ex.Message ;
				this.Socket.Send( response ) ; // send it back to client
				return false ; 
			}		
		
			return true ;
		}
		
		//
		// upload file to server file site
		//
		protected void DoUpload( Dictionary<string,string> request )
		{
			string uploadFile         = "" ;
			long   uploadFileSize     = 0  ;
			int    uploadFileChecksum = 0  ;
		
			bool ok = this.DoUpload_Validation( request, out uploadFile, out uploadFileSize, out uploadFileChecksum ) ;
			if ( ! ok ) return ;
			
			ok = this.DoUpload_ReceiveFile( uploadFile, uploadFileSize, uploadFileChecksum ) ;
			if ( ! ok ) return ;				
		
			ok = this.DoUpload_CommitFile( uploadFile, uploadFileSize, uploadFileChecksum ) ;
			if ( ! ok ) return ;								
		}		
		
		//
		// call 0 - header validation
		//
		protected bool DoUpload_Validation( Dictionary<string,string> request, 
											out string uploadFile,
											out long   uploadFileSize,
											out int    uploadFileChecksum
										  )
		{	
			uploadFile         = "" ;
			uploadFileSize     = 0  ;
			uploadFileChecksum = 0  ;
			
			Dictionary<string,string> response = new Dictionary<string,string>() ;
			response[ "ResponseStatus" ] = "OK" ;
			
			string slash = Path.DirectorySeparatorChar.ToString() ;
			
			try
			{
				// is required key found
				if ( ! this.Parameters.ContainsKey( "ServerFileSite" ) )
					throw new FileWarpValidationException( "Key [ServerFileSite] missing from .Parameters." ) ;
				
				string serverFileSite = this.Parameters[ "ServerFileSite" ] ;
				//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} files are at [{1}]", this.ID, serverFileSite ) ;
				
				// are required keys found
				if ( ! request.ContainsKey( "ClientFile" ) )
					throw new FileWarpValidationException( "Key [ClientFile] missing from request." ) ;
			
				uploadFile = serverFileSite + slash + Path.GetFileName( request[ "ClientFile" ] ) ;
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Uploading from client \n[{1}]", this.ID, uploadFile ) ;
			
				if ( ! request.ContainsKey( "ClientFileBytes" ) )
					throw new FileWarpValidationException( "Key [ClientFileBytes] missing from request." ) ;
			
				uploadFileSize = long.Parse( request[ "ClientFileBytes" ] ) ;
				//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Size [{1}] bytes", this.ID, uploadFileSize ) ;
			
				if ( ! request.ContainsKey( "ClientFileChecksum" ) )
					throw new FileWarpValidationException( "Key [ClientFileChecksum] missing from request." ) ;
			
				uploadFileChecksum = int.Parse( request[ "ClientFileChecksum" ] ) ;
				//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Checksum [{1}] bytes", this.ID, uploadFileChecksum ) ;
						
				// see if we can write this .part file
				string partFile = uploadFile + ".part" ;
				
				try
				{
					if ( File.Exists( partFile ) )
						File.Delete( partFile ) ;
						
					using ( FileStream fs = File.OpenWrite( partFile ) ) 
					{}
					
					File.Delete( partFile ) ;
				}
				catch( Exception ex ) 
				{
					throw new FileWarpValidationException( "File error: [" + uploadFile + "] " + ex.Message ) ;
				}
			
				this.Socket.Send( response ) ;									
			}
			catch( FileWarpValidationException ex ) // validation error
			{
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Upload EXCEPTION\n{1}", this.ID, ex.ToString() ) ;
				
				response[ "ResponseStatus" ] = "Error" ;
				response[ "ResponseError"  ] = ex.Message ;
				this.Socket.Send( response ) ; // send it back to client
				return false ; 
			}	
						
			return true ;
		}
				
		//
		// call 1 - receive file from client
		//
		protected bool DoUpload_ReceiveFile( string uploadFile, long uploadFileSize, int uploadFileChecksum )
		{						
			Dictionary<string,string> response = new Dictionary<string,string>() ;
			response[ "ResponseStatus" ] = "OK" ;
			
			try
			{
				string partFile = uploadFile + ".part" ;
				
				// read from the socket and save the file - blob at a time
			    while ( true ) 
			    {
					// call 1a - client is checking shitdown status or telling server he is finished
					Dictionary<string,string> request  = this.Socket.ReceiveDictionary() ;						
					bool isFinished = bool.Parse( request[ "ClientIsFinished" ] ) ;						
					this.Socket.Send( response ) ; 
					
					if ( isFinished ) // client is finished
						break  ;
					
					this.ShutdownCheck() ; 
			    
			    	// call 1b: other side of Call_SendBytesReceiveDictionary
			    	
					// read a blob from the socket
					int bytesRead = this.Socket.ReceiveBytes() ;			    
					//Helper.WriteLine( "FileWarpServerThreadPoolItem.Run [{0}] read [{1}] bytes", this.ID, bytesRead ) ;						
								    
					// write the bytesRead to the end of the .part file
					using ( FileStream stream = File.OpenWrite( partFile ) )
					{
						stream.Seek( 0, SeekOrigin.End ) ;
						stream.Write( this.Socket.ReceiveBuffer, 0, bytesRead ) ;
					}
								    
					// send ok back
					this.Socket.Send( response ) ; 					    
			    }	 	 				 
			}
			catch( Exception ex ) 
			{
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Upload EXCEPTION\n{1}", this.ID, ex.ToString() ) ;
				
				response[ "ResponseStatus" ] = "Error" ;
				response[ "ResponseError"  ] = ex.Message ;
				this.Socket.Send( response ) ; // send it back to client
				return false ; 
			}		
		
			return true ;
		}
		
		//
		// call 2 - commit file here on server
		//
		protected bool DoUpload_CommitFile( string uploadFile, long uploadFileSize, int uploadFileChecksum )
		{	
			Dictionary<string,string> response = new Dictionary<string,string>() ;
			response[ "ResponseStatus" ] = "OK" ;
			
			try
			{
				string partFile = uploadFile + ".part" ;
			
				this.Socket.ReceiveDictionary() ; //sync client					
			
				// sanity check
				FileInfo fi = new FileInfo( partFile ) ;
				//Helper.WriteLine( "server file size [{0}] - [{1}]", fi.Length, partFile ) ;
				
				if ( uploadFileSize != fi.Length )
					throw new SocketException( "Mismatched client and server file sizes - server size = " + fi.Length 
												+ ", client size = " + uploadFileSize ) ;   				
			
				// calculate checksum
				int checksum = _fwh.CalcChecksum( partFile ) ;
				//Helper.WriteLine( "server file checksum [{0}] - [{1}]", checksum, partFile ) ;
				
				if ( uploadFileChecksum != checksum )
					throw new SocketException( "Mismatched client and server file checksums - server checksum = " + checksum 
												+ ", client checksum = " + uploadFileChecksum ) ;   				
				
				// all good - commit - move .part to real file
				File.Delete( uploadFile ) ;
				File.Move( partFile, uploadFile ) ;			
				
				this.Socket.Send( response ) ; // send it back to client			
			}
			catch( Exception ex ) 
			{
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} Upload EXCEPTION\n{1}", this.ID, ex.ToString() ) ;
				
				response[ "ResponseStatus" ] = "Error" ;
				response[ "ResponseError"  ] = ex.Message ;
				this.Socket.Send( response ) ; // send it back to client
				return false ; 
			}		
			return true ;
		}
		
		//
		//
		//
		protected void ShutdownCheck()
		{
			if ( this.ThreadPoolManager.IsShutdown )
			{
				Helper.MtWriteLine( "FileWarpServerThreadPoolItem.Run ################# #{0} SHUTDOWN notification - bye", this.ID ) ;
				throw new FileWarpValidationException( "FileWarp server received shutdown notification." ) ;
			} 
		}
		
		
	} // end class

} // end namespace
