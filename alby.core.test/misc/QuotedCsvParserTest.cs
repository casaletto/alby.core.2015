using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;
using ac = alby.core ;

namespace alby.core.misc.test 
{
	[TestFixture]
	public class QuotedCsvParserTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		protected void Dump( List< string[] > list )
		{
			int i = 0 ;
			foreach( string[] sa in list )
			{
				i++ ;
				Console.WriteLine( "line #{0}", i ) ;
				this.Dump( sa ) ;
			}
		}

		protected void Dump( string[] sa )
		{
			if ( sa == null )
				Console.WriteLine( "\tNULL array" ) ;			
			else
			for ( int i = 0 ; i < sa.Length ; i++ )
				Console.WriteLine( "\tfield #{0} [{1}]", i, sa[i] ) ;
		}

		[Test]
		public void StringTest_Simple1()
		{
			string str = "   alpha, bravo   , charlie,delta  " ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "   alpha" , sa[0] ) ;
			Assert.AreEqual( " bravo   ", sa[1] ) ;
			Assert.AreEqual( " charlie" , sa[2] ) ;
			Assert.AreEqual( "delta  "  , sa[3] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
		}

		[Test]
		public void StringTest_Simple2()
		{
			string str = " ,   alpha, bravo   ,,, charlie,delta  ," ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 8, sa.Length ) ;
			Assert.AreEqual( " ",		  sa[0] ) ;
			Assert.AreEqual( "   alpha",  sa[1] ) ;
			Assert.AreEqual( " bravo   ", sa[2] ) ;
			Assert.AreEqual( "",		  sa[3] ) ;
			Assert.AreEqual( "",		  sa[4] ) ;
			Assert.AreEqual( " charlie" , sa[5] ) ;
			Assert.AreEqual( "delta  "  , sa[6] ) ;
			Assert.AreEqual( "",		  sa[7] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 8, sa.Length ) ;
			Assert.AreEqual( "",	     sa[0] ) ;
			Assert.AreEqual( "alpha",    sa[1] ) ;
			Assert.AreEqual( "bravo",    sa[2] ) ;
			Assert.AreEqual( "",		 sa[3] ) ;
			Assert.AreEqual( "",		 sa[4] ) ;
			Assert.AreEqual( "charlie" , sa[5] ) ;
			Assert.AreEqual( "delta"   , sa[6] ) ;
			Assert.AreEqual( "",		 sa[7] ) ;
		}

		[Test]
		public void StringTest_Simple3()
		{
			string str = " |   alpha| bravo   ,,, charlie|delta  " ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			csv.Delimiter = ac.SimpleDelimitedValuesParser.FATBAR ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( " ",					 sa[0] ) ;
			Assert.AreEqual( "   alpha",			 sa[1] ) ;
			Assert.AreEqual( " bravo   ,,, charlie", sa[2] ) ;
			Assert.AreEqual( "delta  ",				 sa[3] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "",	                sa[0] ) ;
			Assert.AreEqual( "alpha",               sa[1] ) ;
			Assert.AreEqual( "bravo   ,,, charlie", sa[2] ) ;
			Assert.AreEqual( "delta",				sa[3] ) ;
		}

		[Test]
		public void StringTest_Quoted0()
		{
			string str = "  \" alpha, bravo \"  , charlie,delta , \" wow  \" " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( " alpha, bravo " , sa[0] ) ;
			Assert.AreEqual( " charlie"       , sa[1] ) ;
			Assert.AreEqual( "delta "         , sa[2] ) ;
			Assert.AreEqual( " wow  "         , sa[3] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "alpha, bravo" , sa[0] ) ;
			Assert.AreEqual( "charlie"      , sa[1] ) ;
			Assert.AreEqual( "delta"        , sa[2] ) ;
			Assert.AreEqual( "wow"          , sa[3] ) ;
		}

		[Test]
		public void StringTest_Quote1()
		{
			string str = @"""alpha"",""bravo"",""charlie"",""delta"",""echo""," ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 6, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
			Assert.AreEqual( ""        , sa[5] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 6, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
			Assert.AreEqual( ""        , sa[5] ) ;
		}

		[Test]
		public void StringTest_Quote2()
		{
			string str = @"""alpha"",""bravo"",""charlie"",""delta"",""echo"",   " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 6, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
			Assert.AreEqual( "   "     , sa[5] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 6, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
			Assert.AreEqual( ""        , sa[5] ) ;
		}

		[Test]
		public void StringTest_Quote3()
		{
			string str = @"""alpha"",""bravo"",""charlie"",""delta"",""echo""" ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 5, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 5, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
		}

		[Test]
		public void StringTest_Quote4()
		{
			string str = @"""alpha"",""bravo"",""charlie"",""delta"",""echo""  " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 5, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 5, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
		}

		[Test]
		public void StringTest_Quote5()
		{
			string str = @"   ""alpha"",""bravo"",""charlie"",""delta"",""echo""  " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 5, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 5, sa.Length ) ;
			Assert.AreEqual( "alpha"   , sa[0] ) ;
			Assert.AreEqual( "bravo"   , sa[1] ) ;
			Assert.AreEqual( "charlie" , sa[2] ) ;
			Assert.AreEqual( "delta"   , sa[3] ) ;
			Assert.AreEqual( "echo"    , sa[4] ) ;
		}

		[Test]
		public void StringTest_Quote6()
		{
			string str = @"""""" ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;
		}

		[Test]
		public void StringTest_Quote7()
		{
			string str = @"   """"    " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;
		}

		[Test]
		public void StringTest_Quote8()
		{
			string str = @"""     """ ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;
		}

		[Test]
		public void StringTest_Quote9()
		{
			string str = @"   ""     ""  " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;
			Assert.IsNull( sa ) ;
		}

		[Test]
		public void StringTest_Quote10()
		{
			string str = @"   ""  a,b   ""  " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ; 
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 1, sa.Length ) ;
			Assert.AreEqual( "  a,b   ", sa[0] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 1, sa.Length ) ;
			Assert.AreEqual( "a,b", sa[0] ) ;
		}

		[Test]
		public void StringTest_Quote11()
		{
			string str = @"   ""  a,b   ""    ,   " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "  a,b   ", sa[0] ) ;
			Assert.AreEqual( "   ",      sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "a,b",  sa[0] ) ;
			Assert.AreEqual( "",     sa[1] ) ;
		}

		[Test]
		public void StringTest_Quote12()
		{
			string str = @"   ""  a,b   ""    ," ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "  a,b   ", sa[0] ) ;
			Assert.AreEqual( "",         sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "a,b",  sa[0] ) ;
			Assert.AreEqual( "",     sa[1] ) ;
		}

		[Test]
		public void StringTest_Quote13()
		{
			string str = @"   ""  a,b   ""," ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "  a,b   ", sa[0] ) ;
			Assert.AreEqual( "",         sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "a,b",  sa[0] ) ;
			Assert.AreEqual( "",     sa[1] ) ;
		}

		[Test]
		public void StringTest_Quote14()
		{
			string str = @"   ""  a,b   ""  ,  "" c,d   ""    " ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "  a,b   ", sa[0] ) ;
			Assert.AreEqual( " c,d   " , sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "a,b",  sa[0] ) ;
			Assert.AreEqual( "c,d",  sa[1] ) ;
		}

		[Test]
		public void StringTest_Quote15()
		{
			string str = @"""a,b"",""c,d""" ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "a,b" , sa[0] ) ;
			Assert.AreEqual( "c,d" , sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 2, sa.Length ) ;
			Assert.AreEqual( "a,b",  sa[0] ) ;
			Assert.AreEqual( "c,d",  sa[1] ) ;
		}

		[Test]
		public void StringTest_Quote16()
		{
			string str = @"""a,b"",""c,d"",""""" ;
			Console.WriteLine( "[{0}]", str ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 3, sa.Length ) ;
			Assert.AreEqual( "a,b" , sa[0] ) ;
			Assert.AreEqual( "c,d" , sa[1] ) ;
			Assert.AreEqual( ""    , sa[2] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 3, sa.Length ) ;
			Assert.AreEqual( "a,b",  sa[0] ) ;
			Assert.AreEqual( "c,d",  sa[1] ) ;
			Assert.AreEqual( ""    , sa[2] ) ;
		}

		[Test]
		public void FileTest_Unicode_1()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.unicode.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			csv.Encoding = Encoding.Unicode ;
			List< string[] > list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { " unicode"," alpha "," bravo "," charlie "," delta      " }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek "," bravo "," charlie "," delta"," ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a "," b  "," c " }, list[2] ) ;
			Assert.AreEqual( new string[] { "  a  " }, list[3] ) ;
			Assert.AreEqual( new string[] { "a ", "   " }, list[4] ) ;

			// trim
			csv.Trim = true ;
			list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { "unicode","alpha","bravo","charlie","delta" }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek","bravo","charlie","delta","ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a","b","c" }, list[2] ) ;
			Assert.AreEqual( new string[] { "a" }, list[3] ) ;
			Assert.AreEqual( new string[] { "a", "" }, list[4] ) ;
		}

		[Test]
		public void FileTest_Unicode_2()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.unicode.quoted.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			csv.Encoding = Encoding.Unicode ;
			List< string[] > list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { " unicode"," alpha "," bravo "," charlie "," delta      " }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek "," bravo "," charlie"," delta"," ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a "," b  "," c " }, list[2] ) ;
			Assert.AreEqual( new string[] { "   a  " }, list[3] ) ;
			Assert.AreEqual( new string[] { "a ", "   " }, list[4] ) ;

			// trim
			csv.Trim = true ;
			list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { "unicode","alpha","bravo","charlie","delta" }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek","bravo","charlie","delta","ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a","b","c" }, list[2] ) ;
			Assert.AreEqual( new string[] { "a" }, list[3] ) ;
			Assert.AreEqual( new string[] { "a", "" }, list[4] ) ;
		}

		[Test]
		public void FileTest_Utf8_1()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.utf8.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			csv.Encoding = Encoding.UTF8 ;
			List< string[] > list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { " unicode"," alpha "," bravo "," charlie "," delta      " }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek "," bravo "," charlie "," delta"," ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a "," b  "," c " }, list[2] ) ;
			Assert.AreEqual( new string[] { "  a  " }, list[3] ) ;
			Assert.AreEqual( new string[] { "a ", "   " }, list[4] ) ;

			// trim
			csv.Trim = true ;
			list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { "unicode","alpha","bravo","charlie","delta" }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek","bravo","charlie","delta","ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a","b","c" }, list[2] ) ;
			Assert.AreEqual( new string[] { "a" }, list[3] ) ;
			Assert.AreEqual( new string[] { "a", "" }, list[4] ) ;
		}

		[Test]
		public void FileTest_Utf8_2()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.utf8.quoted.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			csv.Encoding = Encoding.UTF8 ;
			List< string[] > list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { " unicode"," alpha "," bravo "," charlie "," delta      " }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek "," bravo "," charlie "," delta"," ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a "," b  "," c " }, list[2] ) ;
			Assert.AreEqual( new string[] { "  a  " }, list[3] ) ;
			Assert.AreEqual( new string[] { "a ", "   " }, list[4] ) ;

			// trim
			csv.Trim = true ;
			list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { "unicode","alpha","bravo","charlie","delta" }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek","bravo","charlie","delta","ϖϖΘρρ" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a","b","c" }, list[2] ) ;
			Assert.AreEqual( new string[] { "a" }, list[3] ) ;
			Assert.AreEqual( new string[] { "a", "" }, list[4] ) ;
		}

		[Test]
		public void FileTest_Ascii_1()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.ascii.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			List< string[] > list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { " unicode"," alpha "," bravo "," charlie "," delta      " }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek "," bravo "," charlie "," delta"," ??T??" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a "," b  "," c " }, list[2] ) ;
			Assert.AreEqual( new string[] { "  a  " }, list[3] ) ;
			Assert.AreEqual( new string[] { "a ", "   " }, list[4] ) ;

			// trim
			csv.Trim = true ;
			list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { "unicode","alpha","bravo","charlie","delta" }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek","bravo","charlie","delta","??T??" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a","b","c" }, list[2] ) ;
			Assert.AreEqual( new string[] { "a" }, list[3] ) ;
			Assert.AreEqual( new string[] { "a", "" }, list[4] ) ;
		}

		[Test]
		public void FileTest_Ascii_2()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.ascii.quoted.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.QuotedCsvParser csv = new ac.QuotedCsvParser() ;
			List< string[] > list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { " unicode"," alpha "," bravo "," charlie "," delta      " }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek "," bravo "," charlie "," delta","  ??T??" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a ","b  ","c " }, list[2] ) ;
			Assert.AreEqual( new string[] { "  a  " }, list[3] ) ;
			Assert.AreEqual( new string[] { "a ", "   " }, list[4] ) ;

			// trim
			csv.Trim = true ;
			list = csv.Parse( filename ) ;
			
			this.Dump( list ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( 5, list.Count ) ;
			Assert.AreEqual( new string[] { "unicode","alpha","bravo","charlie","delta" }, list[0] ) ;
			Assert.AreEqual( new string[] { "greek","bravo","charlie","delta","??T??" }, list[1] ) ;
			Assert.AreEqual( new string[] { "a","b","c" }, list[2] ) ;
			Assert.AreEqual( new string[] { "a" }, list[3] ) ;
			Assert.AreEqual( new string[] { "a", "" }, list[4] ) ;
		}

	} // end class
}