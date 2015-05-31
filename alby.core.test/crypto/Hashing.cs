using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework ;
using alby.core.crypto ;

namespace alby.core.crypto.test 
{
	[TestFixture]
	public class HashingTest
	{
        public const string TestFile = @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt";
	
		[Test]
		public void TestSha512()
		{
			string expectedHash = "VkmVPRf0nD+NV9WEKt2NVOX62OCvmmQeU9sYcH/ip58PXVEZR0dpn94CBX7V/iOBKj2Jfw5VJmatf4dVcSIOxA==";
		
			Hashing h = new Hashing() ;
			byte[] b = h.Sha512HashFile(TestFile);
			
			string hash = Convert.ToBase64String( b ) ;
			Console.WriteLine( hash ) ;		
			
			Assert.AreEqual( expectedHash, hash ) ;
		}

		[Test]
		public void TestMd5()
		{
			string expectedHash = "mx9Q3Y4xwx71xIjNdSacyw==";

			Hashing h = new Hashing();
			byte[] b = h.MD5HashFile(TestFile) ;

			string hash = Convert.ToBase64String(b);
			Console.WriteLine(hash);

			Assert.AreEqual(expectedHash, hash);
		}


	}
	
}	
			
