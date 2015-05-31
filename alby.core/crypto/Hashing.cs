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

	public class Hashing
	{
		public Hashing() 
		{
		}

		protected FileStream GetFileStream( string filename )
		{
			return new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read ) ;
		}
		
		//
		//
		//		
		public byte[] Sha512Hash( Stream stream )
		{
			SHA512Managed hash = new SHA512Managed() ;
			return hash.ComputeHash( stream ) ;
		}
		
		public byte[] Sha512Hash( byte[] bytes )
		{
			SHA512Managed hash = new SHA512Managed() ;
			return hash.ComputeHash( bytes ) ;
		}
		
		public byte[] Sha512Hash( string str )
		{
			return this.Sha512Hash( System.Text.Encoding.Unicode.GetBytes( str )  ) ;		
		}

		public byte[] Sha512HashFile( string filename )
		{
			using ( Stream stream = this.GetFileStream( filename ) )
				return this.Sha512Hash( stream ) ;		
		}
		
		//
		//
		//
		public byte[] MD5Hash( Stream stream )
		{
			MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider() ;
			return hash.ComputeHash( stream ) ;
		}
		
		public byte[] MD5Hash( byte[] bytes )
		{
			MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider() ;
			return hash.ComputeHash( bytes ) ;
		}
				
		public byte[] MD5Hash( string str )
		{
			return this.MD5Hash( System.Text.Encoding.Unicode.GetBytes( str )  ) ;		
		}

		public byte[] MD5HashFile( string filename )
		{
			using (Stream stream = this.GetFileStream(filename))
				return this.MD5Hash( stream );		
		}
		
	}
	
}
