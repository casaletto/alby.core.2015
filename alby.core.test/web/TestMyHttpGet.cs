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
	[TestFixture]
	public class TestMyHttpGet
	{
		[Test]
		public void TestInternetConnection()
		{
			MyHttpGet http = new MyHttpGet() ;
			bool ok = http.HasInternetConnection() ;
			Console.WriteLine( ok ) ;
		}

		[Test]
		public void Test404Localhost()
		{
			MyHttpGet http = new MyHttpGet() ;
			HttpStatusCode statusCode ;

			string url = "http://localhost/wow.html" ;
			string html = http.DownloadTextFile( url, out statusCode ) ; 
			html = Regex.Replace( html, @"\s", "" ) ;
	
			Console.WriteLine( url ) ; 
			Console.WriteLine( html ) ;

			Assert.AreEqual( HttpStatusCode.NotFound, statusCode ) ; 
			Assert.IsTrue( html.Contains( "404" ) ) ;
		}

		[Test, Explicit]
		public void DumpAllEncodings()
		{
			EncodingInfo[] arr = Encoding.GetEncodings() ;
			foreach( EncodingInfo ei in arr )
			{
				Console.WriteLine( "{0}\t\t{1}\t\t\t{2}\t\t{3}",
					ei.Name, 
					ei.DisplayName,
					ei.CodePage,
					ei.GetEncoding() ) ;
			}		
		}

		[Test]
		public void TestCharsetEncoding()
		{
			MyHttpGet http = new MyHttpGet() ;

			string htmlfile ;
			Encoding encoding ;

			// UTF-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\sinica.binary.html"; // actually big5
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;

			// UTF-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\bbc.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;

			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\hpjap.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;
			
			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\jerupost.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;
			
			// euc-jp
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\jptimes.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "euc-jp", encoding.WebName ) ;
			
			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\ms.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;

			// gb2312
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\cnnyahoo.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "gb2312", encoding.WebName ) ;

			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\medj.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;
		
			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\smh.binary.html";
			encoding = http.GetEncoding( htmlfile ) ;
			Console.WriteLine( "[{0}] [{1}]", encoding.WebName, htmlfile ) ;
			Assert.AreEqual( "utf-8", encoding.WebName ) ;
		}

		[Test]
		public void TestHtmlMetaCharsetRegex()
		{
			MyHttpGet http = new MyHttpGet() ;

			string htmlfile ;
			string html ;
			string charset ;

			// blank
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\sinica.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "", charset ) ;

			// UTF-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\bbc.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "UTF-8", charset ) ;

			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\hpjap.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "utf-8", charset ) ;
			
			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\jerupost.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "utf-8", charset ) ;
			
			// euc-jp
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\jptimes.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "euc-jp", charset ) ;
			
			// utf-8
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\ms.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "utf-8", charset ) ;

			// gb2312
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\cnnyahoo.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "gb2312", charset ) ;

			// blank
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\medj.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "", charset ) ;
		
			// blank
            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\smh.binary.html";
			html = File.ReadAllText( htmlfile, Encoding.ASCII ) ;
			charset = http.GetCharsetFromMetaTag( html ) ;
			Console.WriteLine( "[{0}] [{1}]", charset, htmlfile ) ;
			Assert.AreEqual( "", charset ) ;
		}

		[Test]
		public void TestBinaryToTextDecoding()
		{

			MyHttpGet http = new MyHttpGet() ;

			string htmlfile ;
			string html ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\sinica.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\sinica.binary.html";
			Encoding big5 = Encoding.GetEncoding( "big5" ) ;
			html = http.ConvertFileToString( htmlfile, big5 ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 500 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\bbc.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\hpjap.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\jerupost.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\jptimes.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\ms.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\cnnyahoo.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			html = Regex.Replace( html, @"[a-zA-Z0-9\<\>/="";:.]", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\medj.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;

            htmlfile = @"c:\albyStuff\development\alby.core.2015\alby.core.test\web\htmlPages\smh.binary.html";
			html = http.ConvertFileToString( htmlfile ) ;
			html = Regex.Replace( html, @"\s", "" ) ;
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( htmlfile ) ; 
			Console.WriteLine( "----------------------------------------------------------------------------------------------------------------" ) ;
			Console.WriteLine( html.Substring( 0, 1000 ) ) ;
			Console.WriteLine( "################################################################################################################" ) ;
		}

	} // end class
}
