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
	//----------------------------------------------------------------------

	public class RsaAesEncryptor
	{
		protected byte[] _aesEncryptedKey;
		protected byte[] _aesEncryptedIV;

		public RsaAesEncryptor()
		{
		}

		public byte[] AesEncryptedKey
		{
			get
			{
				return _aesEncryptedKey;
			}
		}

		public byte[] AesEncryptedIV
		{
			get
			{
				return _aesEncryptedIV;
			}
		}

		public void EncryptFile( RsaAesDecryptor decryptor, string file, string encryptedFile) 
		{
			AesEncryption aes = new AesEncryption();
			RsaEncryption rsa = new RsaEncryption();
			
			_aesEncryptedKey = rsa.Encrypt( aes.Key, decryptor.PublicKey ) ;
			_aesEncryptedIV  = rsa.Encrypt( aes.IV,  decryptor.PublicKey ) ;

			File.Delete( encryptedFile ) ;
			aes.EncryptFile( file, encryptedFile ) ;
		}
	}

	//----------------------------------------------------------------------

	public class RsaAesDecryptor
	{
		RsaEncryption _rsa = new RsaEncryption();
		
		public RsaAesDecryptor()
		{
		}
		public string PublicKey
		{
			get 
			{
				return _rsa.PublicKey ;
			}
		}

		public void DecryptFile( RsaAesEncryptor encryptor, string encryptedFile, string decryptedFile )
		{
			byte[] decryptedAesKey = _rsa.Decrypt( encryptor.AesEncryptedKey );
			byte[] decryptedAesIV  = _rsa.Decrypt( encryptor.AesEncryptedIV  );

			AesEncryption aes = new AesEncryption( decryptedAesKey, decryptedAesIV );

			File.Delete( decryptedFile );
			aes.DecryptFile( encryptedFile, decryptedFile );	
		}
	}
	
}
