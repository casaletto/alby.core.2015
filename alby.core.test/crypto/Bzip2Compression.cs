using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;
using alby.core.crypto ;

namespace alby.core.crypto.test 
{
	[TestFixture]
	public class Bzip2CompressionTest
	{
		public const string TestFile				= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt";
		public const string TestFileCompressed		= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt.bzip2";
		public const string TestFileDecompressed	= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt.bzip2.decrypted";

		[SetUp]
		public void SetUp()
		{
			Init() ;
		}

		[TearDown]
		public void TearDown()
		{
			Init() ;
		}
	
		protected void Init()
		{
			File.Delete(TestFileCompressed);
			File.Delete(TestFileDecompressed);

			Assert.IsTrue(File.Exists(TestFile));
			Assert.IsFalse(File.Exists(TestFileCompressed));
			Assert.IsFalse(File.Exists(TestFileDecompressed));		
		}
	
		[Test]
		public void TestCompression()
		{
			// compress and decompress
			Bzip2Compression bzip = new Bzip2Compression() ;
			bzip.CompressFile( TestFile, TestFileCompressed ) ;
			bzip.DecompressFile(TestFileCompressed, TestFileDecompressed);

			// test results
			FileInfo fi = new FileInfo(TestFile);
			FileInfo fiCompressed = new FileInfo(TestFileCompressed);
			FileInfo fiDecompressed = new FileInfo(TestFileDecompressed);

			Console.WriteLine( "File [{0}] size [{1}]", TestFile, fi.Length ) ;
			Console.WriteLine( "File [{0}] size [{1}]", TestFileCompressed, fiCompressed.Length);
			Console.WriteLine( "File [{0}] size [{1}]", TestFileDecompressed, fiDecompressed.Length);

			Assert.AreEqual( fi.Length, fiDecompressed.Length ) ;
			Assert.Greater( fi.Length, fiCompressed.Length ) ;			 

			// test results
			Hashing h = new Hashing();

			byte[] b = h.Sha512HashFile(TestFile);
			string hash1 = Convert.ToBase64String(b);

			b = h.Sha512HashFile(TestFileCompressed);
			string hash2 = Convert.ToBase64String(b);

			b = h.Sha512HashFile(TestFileDecompressed);
			string hash3 = Convert.ToBase64String(b);

			Assert.AreEqual(hash1, hash3);
			Assert.AreNotEqual(hash1, hash2);
		}
		
	}
	
}	
			
