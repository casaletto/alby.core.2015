using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

using NUnit.Framework ;
using alby.core.crypto ;

namespace alby.core.crypto.test 
{
	[TestFixture]
	public class RsaEncryptionTest
	{
		public const string TestMessage			= @"The eagle has landed.";
		public const string TestMessageReply	= @"Big mamma is in the house.";

		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}
	
		[Test]
		public void TestPublicPrivateKeyConversation()
		{
			// alice sends message to bob
			// bob send message back to alica
			//
			// failures:
			//
			// charlie tries to read alices's message
			// charlie tries to read bob's message
			// alice tries to read a message she encrypted
			
			RsaEncryption alice		= new RsaEncryption();
			RsaEncryption bob		= new RsaEncryption();
			RsaEncryption charlie	= new RsaEncryption();
			
			byte[] aliceToBobMessage = alice.Encrypt( TestMessage, bob.PublicKey ) ;
			byte[] decodedAliceMessage = bob.Decrypt( aliceToBobMessage );
			string decodedAliceToBobString = System.Text.Encoding.Unicode.GetString( decodedAliceMessage );
			
			Assert.AreEqual( TestMessage, decodedAliceToBobString ) ;
			Assert.AreNotEqual( Convert.ToBase64String( aliceToBobMessage ), Convert.ToBase64String( decodedAliceMessage ) ) ;
			
			byte[] bobToAliceMessage = bob.Encrypt( TestMessageReply, alice.PublicKey ) ;
			byte[] decodedBobMessage = alice.Decrypt( bobToAliceMessage );
			string decodedBobToAliceString = System.Text.Encoding.Unicode.GetString(decodedBobMessage);

			Assert.AreEqual(TestMessageReply, decodedBobToAliceString);
			Assert.AreNotEqual( Convert.ToBase64String( bobToAliceMessage ), Convert.ToBase64String( decodedBobMessage ) ) ;
			 
			try
			{
				charlie.Decrypt( aliceToBobMessage ) ;
				Assert.Fail( "charlie shouldnt be able to see alice's message to bob" ) ;
			}
			catch (System.Security.Cryptography.CryptographicException ) 
			{}

			try
			{
				charlie.Decrypt( bobToAliceMessage );
				Assert.Fail("charlie shouldnt be able to see bob's message to alice");
			}
			catch (System.Security.Cryptography.CryptographicException)
			{}

			try
			{
				alice.Decrypt(aliceToBobMessage);
				Assert.Fail("alice shouldnt be able to see her own message to bob - go figure!");
			}
			catch (System.Security.Cryptography.CryptographicException)
			{}
		}

		[Test]
		public void TestPublicPrivateKeyConversationRehydratedKey()
		{
			// same as above , but also use serialized key to rehydrate alice and bob		
			RsaEncryption alice = new RsaEncryption();
			RsaEncryption bob = new RsaEncryption();

			byte[] aliceToBobMessage = alice.Encrypt(TestMessage, bob.PublicKey);
			byte[] decodedAliceMessage = bob.Decrypt(aliceToBobMessage);
			Assert.AreEqual(TestMessage, System.Text.Encoding.Unicode.GetString(decodedAliceMessage) );

			byte[] bobToAliceMessage = bob.Encrypt(TestMessageReply, alice.PublicKey);
			byte[] decodedBobMessage = alice.Decrypt(bobToAliceMessage);
			Assert.AreEqual(TestMessageReply, System.Text.Encoding.Unicode.GetString(decodedBobMessage));

			// rehydrate bob
			RsaEncryption alice2 = new RsaEncryption( alice.PrivatePublicKey ) ;
			RsaEncryption bob2 = new RsaEncryption( bob.PrivatePublicKey ) ;

			Assert.AreEqual( alice.PrivatePublicKey, alice2.PrivatePublicKey ) ;
			Assert.AreEqual( bob.PrivatePublicKey, bob2.PrivatePublicKey);

			aliceToBobMessage = alice2.Encrypt(TestMessage, bob2.PublicKey);
			decodedAliceMessage = bob2.Decrypt(aliceToBobMessage);
			Assert.AreEqual(TestMessage, System.Text.Encoding.Unicode.GetString(decodedAliceMessage));

			bobToAliceMessage = bob2.Encrypt(TestMessageReply, alice2.PublicKey);
			decodedBobMessage = alice2.Decrypt(bobToAliceMessage);
			Assert.AreEqual(TestMessageReply, System.Text.Encoding.Unicode.GetString(decodedBobMessage));

			// decrypt with old objects
			decodedAliceMessage = bob.Decrypt(aliceToBobMessage);
			Assert.AreEqual(TestMessage, System.Text.Encoding.Unicode.GetString(decodedAliceMessage));

			decodedBobMessage = alice.Decrypt(bobToAliceMessage);
			Assert.AreEqual(TestMessageReply, System.Text.Encoding.Unicode.GetString(decodedBobMessage));		
		}
	}
	
}	
			
