//
// FileWarpHelper.cs
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
	public class FileWarpHelper
	{		
		//
		//
		//
		public FileWarpHelper()
		{
		}
		
		//
		//
		//
		public int CalcChecksum( string file )
		{
			int checksum = 0 ;
			using ( FileStream stream = File.OpenRead( file ) )
			{
				int firstByte = stream.ReadByte() ;
				
				stream.Seek( stream.Length / 2, SeekOrigin.Begin ) ;
				int middleByte = stream.ReadByte() ;
				
				stream.Seek( stream.Length, SeekOrigin.Begin ) ;
				int lastByte = stream.ReadByte() ;
				
				checksum = firstByte * 256 * 256 + middleByte * 256 + lastByte ;
				//Helper.WriteLine( "checksum: [" + file + "] = " + checksum ) ;
			}
		
			return checksum ;
		}
		
	
	} // end class		
}
