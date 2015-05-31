using System ;
using System.IO ;
using System.Collections ;
using System.Collections.Generic ;
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

using NUnit.Framework ;

namespace alby.core.web.test 
{
	[TestFixture, Explicit]
	public class TestMyHttpGetFromInternet
	{
		[Test]
		public void TestInternetConnection()
		{
			MyHttpGet http = new MyHttpGet() ;
			bool ok = http.HasInternetConnection() ;
			Console.WriteLine( ok ) ;
			Assert.IsTrue( ok ) ;
		}

		[Test]
		public void InternetTest404Big5()
		{
			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;

			string url = "http://www.edu.tw/EDU_WEB/EDU_MGT/MANDR/EDU6300001/bbs/1-4-2/1-4-2.html" ;
			string html = http.DownloadTextFile( url, out statusCode ) ; 
			html = Regex.Replace( html, @"\s", "" ) ;

			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			Assert.AreEqual( HttpStatusCode.NotFound, statusCode ) ; 
			Assert.IsTrue( html.Contains( "charset=big5" ) ) ;
			Assert.IsTrue( html.Contains( "404" ) ) ;
			Assert.IsTrue( html.Contains( "找不到檔案或目錄" ) ) ;
			Assert.IsTrue( http.IsError( statusCode ) ) ;
		}

		[Test, Explicit]
		public void InternetDownload13MegImage() 
		{
			string url = @"http://imgsrc.hubblesite.org/hu/db/images/hs-2004-28-b-full_jpg.jpg" ; // 13.8 MB
			string filename = @"c:\temp\hst2004.jpg" ;

			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;
			http.DownloadFile( url, filename, out statusCode ) ;

			Assert.IsTrue( File.Exists( filename ) ) ;
			FileInfo fi = new FileInfo( filename ) ;

			long expectedLength = 14537420 ; // 13.8 MB (14,537,420 bytes)
			Assert.AreEqual( expectedLength, fi.Length ) ;
		}

		[Test]
		public void InternetDownloadTextFilesAsBinary()
		{
			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;

			string url ;
			string filename ;

			//Downloaded, writing [15635] bytes to [c:\temp\chinasea.binary.html]

			url = "http://www.mimosapudica.org" ;//ChinaSea/index_c.html" ; // big 5
			filename = @"c:\temp\chinasea.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://www.smh.com.au]
			//Response CharacterSet [UTF-8]
			//Response header [Content-Type]=[text/html; charset=UTF-8]
			//Downloaded binary, read [197130] bytes
			//Downloaded binary, writing [197130] bytes to [c:\temp\smh.binary.html]

			url = "http://www.smh.com.au" ;
			filename = @"c:\temp\smh.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://cn.yahoo.com]
			//Response CharacterSet [GBK]
			//Response header [Content-Type]=[text/html; charset=GBK]
			//Downloaded binary, read [186647] bytes
			//Downloaded binary, writing [186647] bytes to [c:\temp\cnnyahoo.binary.html]

			url = "http://cn.yahoo.com" ;
			filename = @"c:\temp\cnnyahoo.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://www.bbc.co.uk]
			//Response CharacterSet [ISO-8859-1]
			//Response header [Content-Type]=[text/html]
			//Downloaded binary, read [84407] bytes
			//Downloaded binary, writing [84407] bytes to [c:\temp\bbc.binary.html]

			url = "http://www.bbc.co.uk" ;
			filename = @"c:\temp\bbc.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://www.microsoft.com]
			//Response CharacterSet [utf-8]
			//Response ContentLength [92704]
			//Response header [Content-Type]=[text/html; charset=utf-8]
			//Downloaded binary, read [92704] bytes
			//Downloaded binary, writing [92704] bytes to [c:\temp\ms.binary.html]

			url = "http://www.microsoft.com" ;
			filename = @"c:\temp\ms.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://www.medjugorje.hr/hr]
			//Response CharacterSet [utf-8]
			//Response ContentLength [18812]
			//Response header [Content-Length]=[18812]
			//Response header [Content-Type]=[text/html; charset=utf-8]
			//Downloaded binary, read [18812] bytes
			//Downloaded binary, writing [18812] bytes to [c:\temp\medj.binary.html]

			url = "http://www.medjugorje.hr/hr" ;
			filename = @"c:\temp\medj.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://www.jpostlite.co.il]
			//Response CharacterSet [ISO-8859-1]
			//Response header [Content-Type]=[text/html]
			//Downloaded binary, read [7339] bytes
			//Downloaded binary, writing [7339] bytes to [c:\temp\jerupost.binary.html]

			url = "http://www.jpostlite.co.il" ;
			filename = @"c:\temp\jerupost.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;

			//Downloading binary, url = [http://welcome.hp.com/country/jp/ja/cs/home.html]
			//Response CharacterSet [ISO-8859-1]
			//Response ContentLength [101683]
			//Response header [Content-Length]=[101683]
			//Response header [Content-Type]=[text/html]
			//Downloaded binary, read [101683] bytes
			//Downloaded binary, writing [101683] bytes to [c:\temp\hpjap.binary.html]

			url = "http://welcome.hp.com/country/jp/ja/cs/home.html" ;
			filename = @"c:\temp\hpjap.binary.html" ;
			http.DownloadFile( url, filename, out statusCode ) ;
			Assert.IsTrue( http.IsOK( statusCode ) ) ;
		}

		[Test]
		public void InternetDownloadTextFileAndChangeEncoding()
		{
			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;

			//Downloaded, writing [11771] bytes to [C:\Users\albert\AppData\Local\Temp\tmp71F7.tmp]

			// this one has no encoding but is actually big 5
			string url = "http://cdp.sinica.edu.tw/" ; 

			string html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;

			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 500 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			html = http.DownloadTextFile( url, Encoding.GetEncoding( "big5" ), out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;

			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 500 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;
		}

		[Test]
		public void InternetDownloadTextFiles1()
		{
			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;

			string html ;
			string url ;

			var theRegex = @"[a-zA-Z0-9\<\>/="";:.\(\)\[\]\$\-!,_{}'\#\+\&\*\?\|\\\%]";

			url = "http://cn.yahoo.com" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			//html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://www.bbc.co.uk" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			//html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://www.microsoft.com" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			//html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://www.medjugorje.hr/hr" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			//html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 200 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://www.jpostlite.co.il" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://club.japantimes.co.jp/jt" ; // japanese
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://welcome.hp.com/country/jp/ja/cs/home.html" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;
		}

		[Test]
		public void InternetDownloadTextFiles2()
		{
			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;

			var theRegex = @"[a-zA-Z0-9\<\>/="";:.\(\)\[\]\$\-!,_{}'\#\+\&\*\?\|\\\%]" ;

			string html ;
			string url ;

			url = "http://www.cnn.co.jp/" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, theRegex, "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://arabic.cnn.com/" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

			url = "http://www8.hp.com/th/en/hp-news/article_detail.html?compURI=tcm:120-479498-16";
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;
			
			url = "http://te.wikipedia.org/wiki/%E0%B0%87%E0%B0%9C%E0%B1%8D%E0%B0%B0%E0%B0%BE%E0%B0%AF%E0%B0%BF%E0%B0%B2%E0%B1%8D" ;
			url = Uri.UnescapeDataString( url ) ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Trace.WriteLine(html);
			Console.WriteLine("################################################################################################################");

			url = "http://www.google.cn" ;
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html ) ; //.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;
						
			url = "http://www.thehinditimes.com/BusinessNews.aspx?Region=Canada";
			html = http.DownloadTextFile( url, out statusCode ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace(html, theRegex, "");
			Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
			Console.WriteLine( url ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 100 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;
		}

	} // end class
}
