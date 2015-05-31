using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;

using alby.core;
using alby.core.crypto;

namespace alby.core.crypto.test 
{
	[TestFixture]
	public class RsaAesEncryptionTest
	{
		public const string TestFile			= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt";
		public const string TestFileEncrypted	= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt.aes";
		public const string TestFileDecrypted	= @"c:\albyStuff\development\alby.core.2015\doc\alp-ch04-threads.txt.aes.decrypted";

		[SetUp]
		public void SetUp()
		{
			Init();
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
		public void SslLikeConversationUseStory()
		{
			// use story: alice send a file (encypted) to bob using run-time generated keys
			//
			// sequence:
			//
			// 1.  alice generated rsa private/public key pair 
			// 2.  bob generates rsa private/private key pair 
			// 3.  alice generates symmetrical aes key and iv 
			// 4.  alice encryptes file with aes symmetric key and iv (3)
			// 5.  alice encrypts aes symmetrical key and iv (3) with bob's public key (2)
			// 6.  alice sends encrypted aes key and iv (5) to bob  
			// 7.  alice sends encrypted file (4) to bob
			// 8.  bob decrypts aes key and iv (6) with his private rsa key
			// 9.  bob generates symmetrical key [aes] with decrypted aes key and iv
			// 10. bob decrypts file (7) with aes key (8)
			//	

			// 1. 3.
			AesEncryption aliceAes = new AesEncryption();
			RsaEncryption aliceRsa = new RsaEncryption();
			
			// 2.
			RsaEncryption bobRsa = new RsaEncryption();
			
			// 4.
			aliceAes.EncryptFile( TestFile, TestFileEncrypted ) ;
			
			// 5.
			byte[] rsaEncryptedAesKey = aliceRsa.Encrypt( aliceAes.Key, bobRsa.PublicKey ) ;
			byte[] rsaEncryptedAesIV  = aliceRsa.Encrypt( aliceAes.IV,  bobRsa.PublicKey ) ;
			
			// 6. 7.
			// some over aether somehow, usually tcp/ip sockets
			
			// 8.
			byte[] decryptedAesKey = bobRsa.Decrypt( rsaEncryptedAesKey ) ;
			byte[] decryptedAesIV  = bobRsa.Decrypt( rsaEncryptedAesIV  ) ;
			
			// 9.
			AesEncryption bobAes = new AesEncryption( decryptedAesKey, decryptedAesIV ) ;
			
			// 10.
			bobAes.DecryptFile( TestFileEncrypted, TestFileDecrypted ) ; 
						
			// assert keys
			Assert.AreNotEqual( Convert.ToBase64String( rsaEncryptedAesKey ), Convert.ToBase64String( decryptedAesKey ));
			Assert.AreNotEqual( Convert.ToBase64String( rsaEncryptedAesIV  ), Convert.ToBase64String( decryptedAesIV  ));
			
			// assert files
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
		public void SslLikeConversation()
		{
			RsaAesEncryptor alice = new RsaAesEncryptor();
			RsaAesDecryptor bob   = new RsaAesDecryptor();

			alice.EncryptFile( bob, TestFile, TestFileEncrypted ) ;
			bob.DecryptFile( alice, TestFileEncrypted, TestFileDecrypted ) ;

			// assert files
			Hashing h = new Hashing();

			byte[] b = h.Sha512HashFile(TestFile);
			string hash1 = HexHelper.ByteArrayToHexString(b); 
			Console.WriteLine(hash1);

			b = h.Sha512HashFile(TestFileEncrypted);
			string hash2 = HexHelper.ByteArrayToHexString(b);
			Console.WriteLine("[{0}] bytes {1}, bits {2}", hash2, hash2.Length, hash2.Length * 4);

			b = h.Sha512HashFile(TestFileDecrypted);
			string hash3 = HexHelper.ByteArrayToHexString(b);
			Console.WriteLine(hash3);

			Assert.AreEqual(hash1, hash3);
			Assert.AreNotEqual(hash1, hash2);
		}

		
	}
	
}	
			
