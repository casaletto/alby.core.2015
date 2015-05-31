using System ;
using System.IO ;
using System.Collections ;
using System.Collections.Generic ;
using System.Threading ;
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Configuration ;
using System.Diagnostics ;
using System.Reflection ;
using System.Text ;
using System.Security.Cryptography ;

namespace alby.core.crypto
{
	public class RsaEncryption
	{
		//
		//
		//
		protected const int DEFAULT_KEY_SIZE = 1024 ; // 2048
	
		//
		//
		//	
		protected RSACryptoServiceProvider _rsa ;
				
		//
		//
		//
		public string PublicKey
		{
			get
			{
				return this._rsa.ToXmlString( false ) ;
			}
			
			set
			{
				this._rsa.FromXmlString( value ) ;
			}
		}

		//
		//
		//
		public string PrivatePublicKey
		{
			get
			{
				return this._rsa.ToXmlString( true ) ;
			}
			
			set
			{
				this._rsa.FromXmlString( value ) ;
			}
		}
				
		//
		// 
		//
		public RsaEncryption() : this( DEFAULT_KEY_SIZE, "" )
		{
		}
		
		//
		// 
		//
		public RsaEncryption( int keySize ) : this( keySize, "" )
		{
		}
		
		//
		// 
		//
		public RsaEncryption( string publicKey ) : this( DEFAULT_KEY_SIZE, publicKey )
		{
		}
				
		//
		//
		//
		public RsaEncryption( int keySize, string publicKey )
		{
			this._rsa = new RSACryptoServiceProvider( keySize ) ;
			
			if ( publicKey != null && publicKey.Length > 0 )
				this._rsa.FromXmlString( publicKey ) ;
		}
		
		//
		//
		//
		public RsaEncryption GetEncryptor( string receiverPublicKey )
		{
			return new RsaEncryption( this._rsa.KeySize, receiverPublicKey ) ;
		}	
				
		//
		// encrpty with the message receiver's public key - only he can decrypt it
		//
		public byte[] Encrypt( byte[] bytes )
		{
			if ( bytes == null ) bytes = new byte[0] ;
			
			try
			{		
				return this._rsa.Encrypt( bytes, false ) ;
			}
			catch( Exception ex )
			{
				throw new CryptographicException( "RSA encryption error", ex ) ;
			}
		}
		
		//
		// decrypt with my own public / private key pair
		//
		public byte[] Decrypt( byte[] bytes )
		{
			if ( bytes == null ) bytes = new byte[0] ;

			try
			{
				byte[] result = this._rsa.Decrypt( bytes, false ) ;
				if ( result == null ) throw new Exception() ;
				
				return result ;
			}
			catch ( Exception ex )
			{
				throw new CryptographicException( "RSA decryption error", ex ) ;
			}
		}
		
		//
		//
		//
		public byte[] Encrypt( string str ) 
		{
			if ( str == null ) str = "" ;

			return this.Encrypt( System.Text.Encoding.Unicode.GetBytes( str ) ) ; 
		}


		public byte[] Encrypt( string str, string receiverPublicKey ) 
		{
			if ( str == null ) str = "" ;

			RsaEncryption rsa = new RsaEncryption(this._rsa.KeySize, receiverPublicKey);
						
			return rsa.Encrypt( System.Text.Encoding.Unicode.GetBytes( str ) ) ; 
		}

		public byte[] Encrypt( byte[] bytes, string receiverPublicKey )
		{
			if (bytes == null) bytes = new byte[0];

			RsaEncryption rsa = new RsaEncryption(this._rsa.KeySize, receiverPublicKey);

			return rsa.Encrypt( bytes );
		}

			
		//
		//
		//		
		public override string ToString()
		{
			string xml = this._rsa.ToXmlString( true ) ;
			
			string str = 	"keysize: 	" + this._rsa.KeySize + 
							"\nkey:		" + xml ; 			
			return str ;
		}
		
		////
		//// unit testing follows
		////
		//public static void Main( string[] args )
		//{
		//    //Test( "" ) ;
		//    Test( "<hello yogi>" )  ;	
		//    //Test( "the fat cat sat on the mat on the hat fancy that silly cat" )  ; // about 58 chars max for RSA = 58*2*8 = 928 bits 
		//}
		
		////
		//// 
		////
		//protected static void Test( string str )
		//{
		//    Base64String base64 = new Base64String() ;
			
		//    RsaEncryption alice   = new RsaEncryption() ;
		//    RsaEncryption bob     = new RsaEncryption() ;
		//    RsaEncryption charlie = new RsaEncryption() ;
		
		//    Console.WriteLine( "\nalice public key:\n" + alice.PublicKey  ) ;
		//    Console.WriteLine( "\nalice whole  key:\n" + alice.ToString() + "\n" ) ;
			
		//    byte[] bytesEnc ;
		//    byte[] bytesDec ;
		//    string stringOut ;
			
		//    string strReply = str + ", Good thanks." ;
			
		//    Console.WriteLine( "----------------------------------  pass 1 ---------------------------------------------" ) ;
			
			
			
		//    // alice sends a message to bob
		//    Console.WriteLine( "1 alice @@@" + str + "@@@ (" + str.Length + " chars)" ) ;		
		//    RsaEncryption encryptorAlice = alice.GetEncryptor( bob.PublicKey ) ;
		//    bytesEnc = encryptorAlice.Encrypt( str ) ; //, bob.PublicKey ) ;
		//    Console.WriteLine( "1 alice encoded bytes: " + base64.BytesToString( bytesEnc ) + " (" + bytesEnc.Length + " bytes)" ) ;
			
		//    // bob has to decrypt it
		//    bytesDec = bob.Decrypt( bytesEnc ) ;			
		//    Console.WriteLine( "1 bob decryped bytes: " + base64.BytesToString( bytesDec ) ) ;
			
		//    stringOut = System.Text.Encoding.Unicode.GetString( bytesDec ) ;
		//    Console.WriteLine( "1 bob @@@" + stringOut + "@@@" ) ;					

		//    // charlie tries to decrypt it - should be error
		//    try
		//    {
		//        bytesDec = charlie.Decrypt( bytesEnc ) ;			
		//        Console.WriteLine( "1 charlie decryped bytes: " + base64.BytesToString( bytesDec ) ) ;
				
		//        stringOut = System.Text.Encoding.Unicode.GetString( bytesDec ) ;
		//        Console.WriteLine( "1 charlie @@@" + stringOut + "@@@" ) ;					
		//    }
		//    catch( Exception ex )
		//    {
		//        Console.WriteLine( "1 charlie: " + ex.ToString() ) ;					
		//    }
			
		//    // bob sends reply to alice	
		//    Console.WriteLine( "1 bob @@@" + strReply + "@@@ (" + strReply.Length + " chars)" ) ;		
		//    RsaEncryption encryptorBob = bob.GetEncryptor( alice.PublicKey ) ;
		//    bytesEnc = encryptorBob.Encrypt( strReply ) ;//, alice.PublicKey ) ;
		//    Console.WriteLine( "1 bob encoded bytes: " + base64.BytesToString( bytesEnc ) + " (" + bytesEnc.Length + " bytes)" ) ;
			
		//    // alice has to decrypt it
		//    bytesDec = alice.Decrypt( bytesEnc ) ;			
		//    Console.WriteLine( "1 alice decryped bytes: " + base64.BytesToString( bytesDec ) ) ;
			
		//    stringOut = System.Text.Encoding.Unicode.GetString( bytesDec ) ;
		//    Console.WriteLine( "1 alice @@@" + stringOut + "@@@" ) ;					

			
						
		
		//    Console.WriteLine( "----------------------------------  pass 2 ---------------------------------------------" ) ;
	
		//    RsaEncryption alice2   = new RsaEncryption( alice.PrivatePublicKey ) ;
		//    RsaEncryption bob2     = new RsaEncryption( bob.PrivatePublicKey ) ;
					
		//    // alice sends a message to bob
		//    Console.WriteLine( "2 alice @@@" + str + "@@@" ) ;		
			
		//    RsaEncryption encryptorAlice2 = alice2.GetEncryptor( bob.PublicKey ) ;
		//    bytesEnc = encryptorAlice2.Encrypt( str ) ;//, bob.PublicKey ) ;
		//    Console.WriteLine( "2 alice encoded bytes: " + base64.BytesToString( bytesEnc ) ) ;
			
		//    // bob has to decrypt it
		//    bytesDec = bob2.Decrypt( bytesEnc ) ;			
		//    Console.WriteLine( "2 bob decryped bytes: " + base64.BytesToString( bytesDec ) ) ;
			
		//    stringOut = System.Text.Encoding.Unicode.GetString( bytesDec ) ;
		//    Console.WriteLine( "2 bob @@@" + stringOut + "@@@" ) ;					

		//    // charlie tries to decrypt it - should be error
		//    try
		//    {
		//        bytesDec = charlie.Decrypt( bytesEnc ) ;			
		//        Console.WriteLine( "2 charlie decryped bytes: " + base64.BytesToString( bytesDec ) ) ;
				
		//        stringOut = System.Text.Encoding.Unicode.GetString( bytesDec ) ;
		//        Console.WriteLine( "2 charlie @@@" + stringOut + "@@@" ) ;					
		//    }
		//    catch( Exception ex )
		//    {
		//        Console.WriteLine( "2 charlie: " + ex.ToString() ) ;					
		//    }
			
		//    // bob sends reply to alice	
		//    Console.WriteLine( "2 bob @@@" + strReply + "@@@ (" + strReply.Length + " chars)" ) ;		
		//    RsaEncryption encryptorBob2 = bob2.GetEncryptor( alice.PublicKey ) ;
		//    bytesEnc = encryptorBob2.Encrypt( strReply ) ;//, alice.PublicKey ) ;
		//    Console.WriteLine( "2 bob encoded bytes: " + base64.BytesToString( bytesEnc ) + " (" + bytesEnc.Length + " bytes)" ) ;
			
		//    // alice has to decrypt it
		//    bytesDec = alice2.Decrypt( bytesEnc ) ;			
		//    Console.WriteLine( "2 alice decryped bytes: " + base64.BytesToString( bytesDec ) ) ;
			
		//    stringOut = System.Text.Encoding.Unicode.GetString( bytesDec ) ;
		//    Console.WriteLine( "2 alice @@@" + stringOut + "@@@" ) ;					
						
		//}
		
				
	}
	
}
