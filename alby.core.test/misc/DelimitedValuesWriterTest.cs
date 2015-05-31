using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;
using ac = alby.core ;

namespace alby.core.misc.test 
{
	[TestFixture]
	public class DelimitedValuesWriterTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test]
		public void StringTest1_Basic()
		{
			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;

			string[] sa = new string[] { "  alpha ", "   bravo  ", "   charlie ", "   delta "  } ;
			string str = csv.FieldsToString( sa ) ;

			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "  alpha ,   bravo  ,   charlie ,   delta ", str ) ;

			csv.Trim = true ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "alpha,bravo,charlie,delta", str ) ;
		}

		[Test]
		public void StringTest2_OnlyWhenNecessary()
		{
			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;

			string[] sa = null ;
			string str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.IsNull( str ) ;

			sa = new string[] { null } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "", str ) ;

			sa = new string[] { "" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "", str ) ;

			sa = new string[] { "a" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "a", str ) ;

			sa = new string[] { "a", "b" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "a,b", str ) ;

			sa = new string[] { "a", "b", "c" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "a,b,c", str ) ;

			csv.Delimter2UseOnlyWhenNecessary = false ;

			sa = null ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.IsNull( str ) ;

			sa = new string[] { null } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "\"\"", str ) ;

			sa = new string[] { "" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "\"\"", str ) ;

			sa = new string[] { "a" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "\"a\"", str ) ;

			sa = new string[] { "a", "b" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "\"a\",\"b\"", str ) ;

			sa = new string[] { "a", "b", "c" } ;
			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "\"a\",\"b\",\"c\"", str ) ;
		}

		[Test]
		public void StringTest3_RabbitEars()
		{
			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;

			string[] sa = new string[] { "fred", "o,malley" } ;
			string str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "fred,\"o,malley\"", str ) ;

			csv.Delimter2UseOnlyWhenNecessary = false ;

			str = csv.FieldsToString( sa ) ;
			Console.WriteLine( "[{0}]", str ) ;
			Assert.AreEqual( "\"fred\",\"o,malley\"", str ) ;
		}

		[Test]
		public void FileTest_ZeroByteFile()
		{
			string filename = @"c:\temp\ft1.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = null ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 0, fo.Length ) ;
		}

		[Test]
		public void FileTest_ZeroByteFile2()
		{
			string filename = @"c:\temp\csvw_ft1.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 0, fo.Length ) ;
		}

		[Test]
		public void FileTest_ZeroByteFile3()
		{
			string filename = @"c:\temp\csvw_ft2.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;

			list.Add( null ) ;
			list.Add( null ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 0, fo.Length ) ;
		}

		[Test]
		public void FileTest_Utf8_NoPreamble()
		{
			UTF8Encoding enc = new UTF8Encoding() ;

			string filename = @"c:\temp\csvw_ft3.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.Encoding = enc ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 2, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 13, 10 }, bytes ) ;
		}

		[Test]
		public void FileTest_Utf8_Preamble()
		{
			UTF8Encoding enc = new UTF8Encoding( true ) ;

			string filename = @"c:\temp\csvw_ft4.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.Encoding = enc ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 5, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 0xEF, 0xBB, 0xBF, 13, 10 }, bytes ) ;
		}

		[Test]
		public void FileTest_Utf8()
		{
			string filename = @"c:\temp\csvw_ft5.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { " " } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 6, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 0xEF, 0xBB, 0xBF, 0x20, 13, 10 }, bytes ) ;
		}

		[Test]
		public void FileTest_Utf8_Omega()
		{
			string filename = @"c:\temp\csvw_ft6.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "ϴ" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 7, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 0xEF, 0xBB, 0xBF, 0xCF, 0xB4, 13, 10 }, bytes ) ;
		}

		[Test]
		public void FileTest_Utf16_Preamble_Omega()
		{
			string filename = @"c:\temp\csvw_ft7.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "ϴ" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.Encoding = new UnicodeEncoding() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 8, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 0xFF, 0xFE, 0xF4, 0x03, 13, 0, 10, 0 }, bytes ) ;
		}

		[Test]
		public void FileTest_Utf16_NoPremamble_Omega()
		{
			string filename = @"c:\temp\csvw_ft8.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "ϴ" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.Encoding = new UnicodeEncoding( false, false ) ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 6, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 0xF4, 0x03, 13, 0, 10, 0 }, bytes ) ;
		}

		[Test]
		public void FileTest_Utf16_BigEndian_Omega()
		{
			string filename = @"c:\temp\csvw_ft9.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "ϴ" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.Encoding = new UnicodeEncoding( true, true ) ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 8, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { 0xFE, 0xFF, 0x03, 0xF4, 0, 13, 0, 10 }, bytes ) ;
		}

		[Test]
		public void FileTest_Ansi()
		{
			byte question = (byte) '?' ; // 63

			string filename = @"c:\temp\csvw_ft10.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { "ϴ" } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.Encoding = new ASCIIEncoding() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;
			
			FileInfo fo = new FileInfo( filename ) ;
			Assert.AreEqual( 3, fo.Length ) ;

			byte[] bytes = File.ReadAllBytes( filename ) ;
			Assert.AreEqual( fo.Length, bytes.Length ) ;

			//Array.ForEach<byte>( bytes, p => Console.WriteLine( p ) ) ;
			Assert.AreEqual( new byte[] { question, 13, 10 }, bytes ) ; 
		}


		[Test]
		public void FileTest_MultiLine()
		{
			string filename = @"c:\temp\csvw_ft11.txt" ;
			File.Delete( filename ) ;
			Assert.IsFalse( File.Exists( filename ) ) ;

			List< string[] > list = new List<string[]> () ;
			list.Add( new string[] { " the ", " fat ", " catch ", " of ", "the", "  dayחטיי " } ) ;
			list.Add( new string[] { " go ", " you ", " good ", " thing " , " ", null } ) ;

			ac.DelimitedValuesWriter csv = new DelimitedValuesWriter() ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;

			string actual = File.ReadAllText( filename ) ;
			Console.WriteLine( "[{0}]", actual ) ;

			string expected = 
@" the , fat , catch , of ,the,  dayחטיי 
 go , you , good , thing , ,
";
			Assert.AreEqual( expected, actual ) ;

			// trim me
			csv.Trim = true ;
			csv.Delimiter = "|" ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;

			actual = File.ReadAllText( filename ) ;
			Console.WriteLine( "[{0}]", actual ) ;

			expected = 
@"the|fat|catch|of|the|dayחטיי
go|you|good|thing||
";
			Assert.AreEqual( expected, actual ) ;

			// rabbit ears always
			csv.Delimter2UseOnlyWhenNecessary = false ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;

			actual = File.ReadAllText( filename ) ;
			Console.WriteLine( "[{0}]", actual ) ;

			expected = 
@"""the""|""fat""|""catch""|""of""|""the""|""dayחטיי""
""go""|""you""|""good""|""thing""|""""|""""
";
			Assert.AreEqual( expected, actual ) ;

			// rabbit ears sometimes
			list = new List<string[]> () ;
			list.Add( new string[] { " the ", " fat ", " cat|ch ", " of ", "the", "  dayחטיי " } ) ;
			list.Add( new string[] { " go ", " you ||| ", " go|od ", " thing " , " ", null, "|" } ) ;

			csv.Delimter2UseOnlyWhenNecessary = true ;
			csv.WriteFile( filename, list ) ;
			Assert.IsTrue( File.Exists( filename ) ) ;

			actual = File.ReadAllText( filename ) ;
			Console.WriteLine( "[{0}]", actual ) ;

			expected = 
@"the|fat|""cat|ch""|of|the|dayחטיי
go|""you |||""|""go|od""|thing|||""|""
";
			Assert.AreEqual( expected, actual ) ;
		}

	} // end class
}