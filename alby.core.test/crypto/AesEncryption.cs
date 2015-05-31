using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;
using alby.core.crypto ;

namespace alby.core.crypto.test 
{
	[TestFixture]
	public class AesEncryptionTest
	{
		public const string TestFile			= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt";
		public const string TestFileEncrypted	= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt.aes";
		public const string TestFileDecrypted	= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt.aes.decrypted";

		[SetUp]
		public void SetUp()
		{
			Init() ;
		}

		[TearDown]
		public void TearDown()
		{
			Init();
		}
	
		protected void Init()
		{
			File.Delete(TestFileEncrypted);
			File.Delete(TestFileDecrypted);

			Assert.IsTrue(File.Exists(TestFile));
			Assert.IsFalse(File.Exists(TestFileEncrypted));
			Assert.IsFalse(File.Exists(TestFileDecrypted));
		}

        [Test]
        public void ClrVersion()
        {
            Console.WriteLine( Environment.Version.ToString() );

			// 4.0.30319.34014
/*
4.5

4.0.30319.17626 = .NET 4.5 RC
4.0.30319.17929 = .NET 4.5 RTM
4.0.30319.18010 = .NET 4.5 on Windows 8
4.0.30319.18052 = .NET 4.5 on Windows 7 SP1 64-bit

4.5.1

4.0.30319.18408 = .NET 4.5.1 on Windows 7 SP1 64-bit
4.0.30319.18444 = .NET 4.5.1 on Windows 7 SP1 64-bit (with MS14-009 security update)
4.0.30319.34014 = .NET 4.5.1 on Windows 8.1 64-bit

4.5.2

4.0.30319.34209 = .NET 4.5.2 on Windows 7 SP1 64-bit
4.0.30319.34209 = .NET 4.5.2 on Windows 8.1 64-bit
*/
        }

		[Test]
		public void TestWithRandomKey()
		{
			// encrypt and decrypt
			AesEncryption aes = new AesEncryption() ;
			aes.EncryptFile( TestFile, TestFileEncrypted ) ;
			aes.DecryptFile( TestFileEncrypted, TestFileDecrypted ) ;

			// test results
			Hashing h = new Hashing();

			byte[] b = h.Sha512HashFile(TestFile);
			string hash1 = Convert.ToBase64String(b);
			Console.WriteLine(hash1);

			b = h.Sha512HashFile(TestFileEncrypted);
			string hash2 = Convert.ToBase64String(b);
			Console.WriteLine(hash2);

			b = h.Sha512HashFile(TestFileDecrypted);
			string hash3 = Convert.ToBase64String(b);
			Console.WriteLine(hash3);

			Assert.AreEqual(hash1, hash3);
			Assert.AreNotEqual(hash1, hash2);
		}

		[Test]
		public void TestPassPhrase()
		{
			// encrypt and decrypt
			AesEncryption aes = new AesEncryption( "AustralianOpenTennis2008");
			aes.EncryptFile(TestFile, TestFileEncrypted);
			aes.DecryptFile(TestFileEncrypted, TestFileDecrypted);

			// test results
			Hashing h = new Hashing();

			byte[] b = h.Sha512HashFile(TestFile);
			string hash1 = Convert.ToBase64String(b);
			Console.WriteLine(hash1);

			b = h.Sha512HashFile(TestFileEncrypted);
			string hash2 = Convert.ToBase64String(b);
			Console.WriteLine(hash2);

			b = h.Sha512HashFile(TestFileDecrypted);
			string hash3 = Convert.ToBase64String(b);
			Console.WriteLine(hash3);

			Assert.AreEqual(hash1, hash3);
			Assert.AreNotEqual(hash1, hash2);
		}


		[Test, ExpectedException( typeof(System.Security.Cryptography.CryptographicException) )]
		public void NegativeTestDecryptyWithDifferentKey()
		{
			AesEncryption aes = new AesEncryption();
			aes.EncryptFile(TestFile, TestFileEncrypted);
			aes.DecryptFile(TestFileEncrypted, TestFileDecrypted);

			AesEncryption aes2 = new AesEncryption();
			aes2.DecryptFile(TestFileEncrypted, TestFileDecrypted);
		}
		
		
	}
	
}	
			
