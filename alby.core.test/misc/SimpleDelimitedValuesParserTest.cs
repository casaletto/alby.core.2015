using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;
using ac = alby.core ;

namespace alby.core.misc.test 
{
	[TestFixture]
	public class SimpleDelimitedValuesParserTest
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
			for ( int i = 0 ; i < sa.Length ; i++ )
				Console.WriteLine( "\tfield #{0} [{1}]", i, sa[i] ) ;
		}
	
		[Test]
		public void StringTest1()
		{
			string str = "   alpha, bravo   , charlie,delta  " ;

			ac.SimpleDelimitedValuesParser csv = new SimpleDelimitedValuesParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "   alpha", sa[0] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "alpha", sa[0] ) ;
		}

		[Test]
		public void StringTest2()
		{
			string str = " ,   alpha, bravo   ,,, charlie,delta  ," ;

			ac.SimpleDelimitedValuesParser csv = new SimpleDelimitedValuesParser() ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 8, sa.Length ) ;
			Assert.AreEqual( " ",		 sa[0] ) ;
			Assert.AreEqual( "   alpha", sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 8, sa.Length ) ;
			Assert.AreEqual( "",	  sa[0] ) ;
			Assert.AreEqual( "alpha", sa[1] ) ;
		}

		[Test]
		public void StringTest3()
		{
			string str = " |   alpha| bravo   ,,, charlie|delta  " ;

			ac.SimpleDelimitedValuesParser csv = new SimpleDelimitedValuesParser() ;
			csv.Delimiter = ac.SimpleDelimitedValuesParser.FATBAR ;
			string[] sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( " ",		 sa[0] ) ;
			Assert.AreEqual( "   alpha", sa[1] ) ;

			csv.Trim = true ;
			sa = csv.ParseString( str ) ;
			this.Dump( sa ) ;

			Assert.AreEqual( 4, sa.Length ) ;
			Assert.AreEqual( "",	  sa[0] ) ;
			Assert.AreEqual( "alpha", sa[1] ) ;
		}

		[Test]
		public void FileTest1_Unicode()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.unicode.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.SimpleDelimitedValuesParser csv = new SimpleDelimitedValuesParser() ;
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
		public void FileTest1_Utf8()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.utf8.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.SimpleDelimitedValuesParser csv = new SimpleDelimitedValuesParser() ;
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
		public void FileTest1_Ascii()
		{
            string filename = @"c:\albyStuff\development\alby.core.2015\alby.core.test\misc\csvFiles\csv.ascii.txt";
			Assert.IsTrue( File.Exists( filename ) ) ;

			FileInfo fo = new FileInfo( filename ) ;
			Console.WriteLine( "[{0}] {1} bytes", fo.Name, fo.Length ) ;

			ac.SimpleDelimitedValuesParser csv = new SimpleDelimitedValuesParser() ;
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

	} // end class
}