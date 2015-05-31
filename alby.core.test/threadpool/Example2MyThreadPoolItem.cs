//
// Example2ThreadPoolItem.cs
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

using alby.core.threadpool;

namespace alby.core.threadpool.test
{
	public class Example2ThreadPoolItem : MyThreadPoolItemBase
	{
		//
		//
		//
		public const int FILE_READ_BUFFER_SIZE = 4000000 ; // the size of each read chunk - 4 meg
		
		//
		//
		//
		protected string _sourceMachinePath        = "" ;
		protected string _destinationMachinePath   = "" ;
		protected long   _destinationLength        = -1 ;
		
		//
		//
		//
		public Example2ThreadPoolItem() : base()
		{
		}		
		
		//
		//
		//
		public string SourceMachinePath
		{
			get
			{
				return _sourceMachinePath ;
			}
			set
			{
				_sourceMachinePath = value ;
			}
		}
		
		//
		//
		//
		public string DestinationMachinePath
		{
			get
			{
				return _destinationMachinePath ;
			}
			set
			{
				_destinationMachinePath = value ;
			}
		}
		
		//
		//
		//
		protected void AppendBytesToFile( string filePath, byte[] buffer, int bytes ) 
		{
			using ( FileStream streamOut = new FileStream( filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read ) )
			{
				streamOut.Position = streamOut.Length ; //  goto EOF
				streamOut.Write( buffer, 0, bytes )   ; // write the bytes
			}
		}
		
		//
		//
		//
		public override void Run() 
		{						
			Helper.MtWriteLine( "\tExample2ThreadPoolItem.Run ################# #{0} starting \n\t\t[{1}] --> \n\t\t[{2}]", 
								this.ID, _sourceMachinePath, _destinationMachinePath ) ;
			DateTime t1 = System.DateTime.Now ;
			
			try
			{
				// check file nulls
				if ( _sourceMachinePath == null )
					throw new Exception( string.Format( "Source file [{0}] is null.", _sourceMachinePath ) ) ;				
				
				if ( _destinationMachinePath == null )
					throw new Exception( string.Format( "Destination file [{0}] is null.", _destinationMachinePath ) ) ;				
				
				// normalise filenames
				_sourceMachinePath       = _sourceMachinePath.Trim() ;
				_destinationMachinePath  = _destinationMachinePath.Trim() ;		
				string partialFile       = _destinationMachinePath + ".part" ;
					
				// check file blanks
				if ( _sourceMachinePath.Length == 0 )
					throw new Exception( string.Format( "Source file name [{0}] is zero length.", _sourceMachinePath ) ) ;				
				
				if ( _destinationMachinePath.Length == 0 )
					throw new Exception( string.Format( "Destination file name [{0}] is zero length.", _destinationMachinePath ) ) ;				
				
				// check source file existance
				if ( ! File.Exists( _sourceMachinePath ) )
					throw new Exception( string.Format( "Source file [{0}] does not exist.", _sourceMachinePath ) ) ;				
				
				// check zero size source file
				FileInfo fiSource = new FileInfo( _sourceMachinePath ) ;
				if ( fiSource.Length == 0  )
					throw new Exception( string.Format( "Source file [{0}] has no bytes!", _sourceMachinePath ) ) ;				
								
				// delete dest partial file
				File.Delete( partialFile ) ;
				
				// read chunks from source file and send to partial file - 4 meg at a time
				using ( FileStream streamIn = new FileStream( _sourceMachinePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				{
					byte[] buffer = new byte[ FILE_READ_BUFFER_SIZE ] ; 
					
					while( true )
					{
						int bytesRead = streamIn.Read( buffer, 0, buffer.Length ) ;
						if ( bytesRead == 0 ) break ;
						
						AppendBytesToFile( partialFile, buffer, bytesRead ) ; // write bytes to file
					}
				}
				
				// sanity check - compare file sizes
				FileInfo fiDestination = new FileInfo( partialFile ) ;
				if ( fiSource.Length != fiDestination.Length ) 
					throw new Exception( string.Format( "Source file [{0}] size [{1}] does not match destination file size [{2}].", 
						_sourceMachinePath, fiSource.Length, fiDestination.Length ) ) ;				
				
				// delete destination file
				File.Delete( _destinationMachinePath ) ;
				
				// all being well up to here - move partial file to real file name
				File.Move( partialFile, _destinationMachinePath ) ;
				_destinationLength = fiDestination.Length ;				
			}
			catch( Exception ex )
			{
				Helper.MtWriteLine( "\tExample2ThreadPoolItem.Run ################# #{0} EXCEPTION.\n\t\t\t{1}", this.ID, ex ) ;
			}
			finally
			{
				TimeSpan ts = System.DateTime.Now.Subtract( t1 ) ;
				Helper.MtWriteLine( "\tExample2ThreadPoolItem.Run ################# #{0} stopping, which ran for {1} ms \n\t\t--> [{2}] [{3} bytes]", 
									this.ID, ts.TotalMilliseconds, _destinationMachinePath, _destinationLength ) ;
			}
			
		}	
		
	} // end class

} // end namespace
