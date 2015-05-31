#define TRACE
#define DEBUG

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
using System.Net.Sockets ;
using System.Text.RegularExpressions ;

namespace alby.core.web
{
	public class MyHttpGet
	{
		protected int _timeoutMilliseconds = 1000 * 60 ; // default 60 second timeout

		public int TimeoutMilliseconds
		{
			get
			{
				return _timeoutMilliseconds ;
			}
			set
			{
				_timeoutMilliseconds = value ;
			}
		}

		public bool HasInternetConnection()
		{
			// try to dns google
			// if i cannot, then (1) i am not connected to the internet, or (2) the world has ended.

			try
			{
				Dns.GetHostEntry( @"www.google.com" ) ;
				return true ;
			}
			catch( SocketException )
			{
				return false ; 
			}
		}

		protected void Log( string msg, params object[] args )
		{
			string str = string.Format( msg, args ) ;
			Trace.WriteLine( str ) ;
		}

		public bool IsOK( HttpStatusCode code )
		{
			return code == HttpStatusCode.OK ;
		}

		public bool IsError( HttpStatusCode code )
		{
			return ! IsOK( code ) ;
		}

		protected HttpWebResponse GetResponse( string url, string method, out HttpStatusCode statusCode ) 
		{
			statusCode = HttpStatusCode.Unused ;
			HttpWebResponse response = null ;

			HttpWebRequest request = WebRequest.Create( url ) as HttpWebRequest ;
			request.Method = method ; // "GET" or "HEAD" ;
			request.Timeout	= this.TimeoutMilliseconds ;
			request.AllowAutoRedirect = true ;
			request.MaximumAutomaticRedirections = 3 ;
			request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5" ;
			request.Referer	= "http://www.google.com" ;

			try
			{
				response = request.GetResponse() as HttpWebResponse ; 
			}
			catch ( WebException wex )
			{
				response = wex.Response as HttpWebResponse ;
			} 

			if ( response == null ) return null ;

			statusCode = response.StatusCode ; 

			Log( "Response StatusCode [{0}]", response.StatusCode )  ;
			Log( "Response CharacterSet [{0}]", response.CharacterSet ) ;
			Log( "Response ContentLength [{0}]", response.ContentLength ) ;
			foreach( string key in response.Headers.Keys ) 
				Log( "Response header [{0}]=[{1}]", key, response.Headers[ key ] ) ;

			return response ;			
		}

		public byte[] DownloadFile( string url, out HttpStatusCode statusCode ) 
		{
			byte[] buffer = new byte[ 10000000 ] ; // 10 meg buffer
			byte[] bytes = null ;

			Log( "Downloading, url = [{0}]", url ) ;
			HttpWebResponse response = GetResponse( url, "GET", out statusCode ) ;
			if ( response == null ) return null ;

			Stream responseStream = response.GetResponseStream() ;
			using ( MemoryStream ms = new MemoryStream() )
			{
				while ( true )
				{
					Array.Clear( buffer, 0, buffer.Length ) ;
					int bytesRead = responseStream.Read( buffer, 0, buffer.Length ) ;
					if ( bytesRead == 0 ) break ;

					ms.Write( buffer, 0, bytesRead ) ;
				}

				ms.Flush() ;
				bytes = ms.ToArray() ;
				ms.Close() ;
			}
			response.Close() ;

			Log( "Downloaded, read [{0}] bytes total", bytes.Length ) ;
			return bytes ;
		}

		public void DownloadFile( string url, string filename, out HttpStatusCode statusCode )
		{
			byte[] bytes = DownloadFile( url, out statusCode ) ;
			if ( bytes == null ) return ;

			Log( "Downloaded, writing [{0}] bytes to [{1}]", bytes.Length, filename ) ;
			File.Delete( filename ) ;

			using ( FileStream fs = File.Create( filename ) )
			{
				fs.Write( bytes, 0, bytes.Length ) ;
				fs.Flush() ;
				fs.Close() ;
			}
		}

		public string GetCharsetFromMetaTag( string html ) 
		{
			const string regex = @"<meta.*charset[ \t]*=([A-Za-z0-9_\-]+)[ \t]*""" ;

			Match match = Regex.Match( html, regex, RegexOptions.IgnoreCase ) ;
			if ( match.Success )
				if ( match.Groups.Count >= 2 )
					return match.Groups[1].Value.Trim() ;

			return "" ;
		}

		public Encoding GetEncoding( string htmlfile ) 
		{
			Encoding defaultEncoding = Encoding.UTF8 ;

			string html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			string charset = GetCharsetFromMetaTag( html ) ;

			if ( charset.Length == 0 ) 
				 return defaultEncoding ;

			try
			{
				return Encoding.GetEncoding( charset ) ;
			} 
			catch( ApplicationException ) 
			{}

			return defaultEncoding ;
		}

		public string ConvertFileToString( string filename ) 
		{
			return ConvertFileToString( filename,null ) ;
		}

		public string ConvertFileToString( string filename, Encoding encoding ) 
		{
			if ( encoding == null )
				 encoding = GetEncoding( filename ) ;

			Log( "Encoding [{0}]", encoding.WebName ) ;

			string str = File.ReadAllText( filename, encoding ) ;
			Log( "Characters [{0}]", str.Length ) ;

			return str ;
		}

		public string DownloadTextFile( string url, out HttpStatusCode statusCode ) 
		{
			return DownloadTextFile( url, null, out statusCode ) ;
		}

		public string DownloadTextFile( string url, Encoding encoding, out HttpStatusCode statusCode ) 
		{
			// get a temp file
			string tempFilename = Path.GetTempFileName() ;
			File.Delete( tempFilename ) ;
			tempFilename += ".alby.core.MyHttpGet.tmp" ;

			try
			{
				DownloadFile( url, tempFilename, out statusCode ) ;
				return ConvertFileToString( tempFilename, encoding ) ;
			}
			catch( Exception )
			{
				throw ;
			}
			finally 
			{
				File.Delete( tempFilename ) ;
			}
		}


	} // end class
}