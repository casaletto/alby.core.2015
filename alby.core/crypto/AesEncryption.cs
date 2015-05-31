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
	public class AesEncryption
	{
		protected const int DEFAULT_KEY_SIZE = 256   ;
		protected const int BUFFER_SIZE      = 10000000 ;
		
		protected RijndaelManaged  _aes ;
		protected ICryptoTransform _encryptor ;
		protected ICryptoTransform _decryptor ;
				
		public byte[] Key
		{
			get
			{
				return this._aes.Key ;
			}
		}
		
		public byte[] IV
		{
			get
			{
				return this._aes.IV ;
			}
		}
		
		public AesEncryption() : 
			this( DEFAULT_KEY_SIZE, null, null )
		{			
		}
		
		public AesEncryption( int keySize ) : 
			this(  keySize, null, null )
		{
		}

		public AesEncryption( byte[] key, byte[] IV ) : 
			this( DEFAULT_KEY_SIZE, key, IV )
		{
		}

		public AesEncryption( int keysize, byte[] key, byte[] iv )
		{
			Init(keysize, key, iv);
		}

		public AesEncryption( string passPhrase )
		{
			byte[] key = new byte[ 32 ] ;
			byte[] iv  = new byte[ 16 ] ;

			// do sha512 on passphrase - generates 64 byte array
			Hashing h = new Hashing() ;
			byte[] hash = h.Sha512Hash( passPhrase ) ;

			// key is first 32 bytes of hash
			Array.Copy(hash, 0, key, 0, key.Length);

			// iv is last 16 bytes
			Array.Copy(hash, hash.Length - iv.Length, iv, 0, iv.Length);

			Init( DEFAULT_KEY_SIZE, key, iv ) ;
		}

		protected void Init(int keySize, byte[] key, byte[] IV)
		{
			this._aes = new RijndaelManaged();
			this._aes.Mode = CipherMode.CBC;
			this._aes.KeySize = keySize;

			if (key == null)
			{
				this._aes.GenerateKey();
				this._aes.GenerateIV();
			}
			else
			{
				this._aes.Key = key;
				this._aes.IV = IV;
			}

			this._encryptor = this._aes.CreateEncryptor();
			this._decryptor = this._aes.CreateDecryptor();
		}

		//
		// inStream --> encrypt --> outStream
		//
		public void Encrypt( Stream inStream, Stream outStream )
		{
			try
			{	
				if ( inStream  == null ) return  ;
				if ( outStream == null ) return  ;
				
				byte[] buffer = new byte[ BUFFER_SIZE ] ;
				
				using ( CryptoStream cs = new CryptoStream( outStream, this._encryptor, CryptoStreamMode.Write ) )
				{
					while ( true )
					{
						int bytesRead = inStream.Read( buffer, 0, buffer.Length ) ;
						if ( bytesRead <= 0 ) break ;
						
						cs.Write( buffer, 0, bytesRead ) ;
					}
					cs.FlushFinalBlock() ;
				}				
			}
			catch( Exception ex )
			{
				throw new CryptographicException( "AES encryption error", ex ) ;
			}
		}		
				
		//
		// 
		//
		public MemoryStream Encrypt( Stream stream )
		{
			MemoryStream outStream = new MemoryStream() ;
			this.Encrypt( stream, outStream ) ;
			
			outStream.Position = 0 ; 	
			return outStream ;		
		}
			
								
		//
		//
		//
		public byte[] Encrypt( byte[] bytes )
		{
			if ( bytes == null ) bytes = new byte[0] ;
			
			using ( MemoryStream outStream = this.Encrypt( new MemoryStream( bytes ) ) )
				return outStream.ToArray() ;
		}
	
		//
		//
		//
		public byte[] Encrypt( string str )
		{
			if ( str == null ) str = "" ;
			
			return this.Encrypt( System.Text.Encoding.Unicode.GetBytes( str )  ) ;
		}

		//
		//
		//
		public void EncryptFile( string inFilename, string outFilename )
		{
			if ( inFilename  == null ) return  ;
			if ( outFilename == null ) return  ;
		
			using ( Stream inStream = new FileStream( inFilename, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				using ( Stream outStream = new FileStream( outFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.None ) )
					this.Encrypt( inStream, outStream ) ;
		}
									
		//
		// inStream --> decrypt --> outStream
		//
		public void Decrypt( Stream inStream, Stream outStream )
		{
			try
			{
				if ( inStream  == null ) return  ;
				if ( outStream == null ) return  ;
				
				byte[] buffer = new byte[ BUFFER_SIZE ] ;
				
				using ( CryptoStream cs = new CryptoStream( inStream, this._decryptor, CryptoStreamMode.Read ) )
				{
					while (true)
					{
						int bytesRead = cs.Read(buffer, 0, buffer.Length);
						if (bytesRead <= 0) break;

						outStream.Write(buffer, 0, bytesRead);
					}
				}
			}
			catch( Exception ex )
			{
				throw new CryptographicException( "AES decryption error", ex ) ;
			}
			
		}		
		
		//
		// 
		//
		public MemoryStream Decrypt( Stream stream )
		{
			if ( stream  == null ) return new MemoryStream() ;
			
			MemoryStream outStream = new MemoryStream() ;
			this.Decrypt( stream, outStream ) ;
			
			outStream.Position = 0 ; 	
			return outStream ;		
		}
		
		//
		//
		//
		public byte[] Decrypt( byte[] bytes )
		{
			if ( bytes == null ) bytes = new byte[0] ;
		
			using ( MemoryStream outStream = this.Decrypt( new MemoryStream( bytes ) ) )
				return outStream.ToArray() ;
		}

		//
		//
		//
		public void DecryptFile( string inFilename, string outFilename )
		{
			if ( inFilename  == null ) return  ;
			if ( outFilename == null ) return  ;
				
			using ( Stream inStream = new FileStream( inFilename, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				using ( Stream outStream = new FileStream( outFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.None ) )
					this.Decrypt( inStream, outStream ) ;
		}
											
		////
		//// unit testing follows
		////
		//public static void Main( string[] args )
		//{
		//    Test( "" ) ;
		//    Test( "hello charlie" )  ;	
		//    Test( "the fat cat sat on the mat on the hat with a bat in his hat." )  ;
			
		//    string file = @"/home/albert/albyStuff/documents/albert/LukaAtBambi2005.html" ;			
		//    TestFile( file ) ;
		//}
		
		////
		//// unit testing follows
		////
		//protected static void Test( string str )
		//{
		//    Base64String base64 = new Base64String() ;
		//    AesEncryption aes = new AesEncryption() ;
		//    Console.WriteLine( aes ) ;

		//    byte[] bytes    = aes.Encrypt( str ) ;
		//    byte[] bytesOut = aes.Decrypt( bytes ) ;
			
		//    Console.WriteLine( base64.BytesToString( bytesOut ) ) ;
			
			
		//    string stringOut = System.Text.Encoding.Unicode.GetString( bytesOut ) ;
		//    Console.WriteLine( "@@@" + str + "@@@" ) ;		
		//    Console.WriteLine( "@@@" + stringOut + "@@@" ) ;					
		//}
		
		////
		////
		////
		//protected static void TestFile( string file )
		//{
		//    Base64String base64 = new Base64String() ;
		//    HashingHelper hh = new HashingHelper() ;		

		//    string fileEncrypted = @"/tmp/encrypted.html.aes" ;
		//    string fileDecrypted = @"/tmp/encrypted.html" ;

		//    AesEncryption aes = new AesEncryption() ;
		//    byte[] hash = hh.Sha256HashFile( file ) ;			
		//    Console.WriteLine( file + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
			
		//    Console.WriteLine( "Encrypting " + file ) ;		
		//    aes.EncryptFile( file, fileEncrypted ) ;	

		//    AesEncryption aes2 = new AesEncryption( aes.Key, aes.IV ) ;
		//    Console.WriteLine( "Decrypting " + fileEncrypted ) ;					
		//    aes2.DecryptFile( fileEncrypted, fileDecrypted ) ;		
			
		//    hash = hh.Sha256HashFile( fileDecrypted ) ;			
		//    Console.WriteLine( fileDecrypted + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
		//}
		
		
	}
	
}
