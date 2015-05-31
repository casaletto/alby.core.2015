using System ;
using System.IO ;
using ICSharpCode.SharpZipLib.BZip2 ;

namespace alby.core.crypto 
{
	public class Bzip2Compression
	{
		//
		//
		//
		protected const int BUFFER_SIZE = 1000000 ;
		
		//
		//
		//
		public Bzip2Compression()
		{
		}
	
		//
		// inStream --> compress --> outStream
		//
		public void Compress( Stream inStream, Stream outStream )
		{	
			if ( inStream  == null ) return  ;
			if ( outStream == null ) return  ;

			byte[] buffer = new byte[ BUFFER_SIZE ] ;

			using ( BZip2OutputStream bz2out = new BZip2OutputStream( outStream ) )
			{
				while ( true )
				{
					int bytesRead = inStream.Read( buffer, 0, buffer.Length ) ;
					if ( bytesRead <= 0 ) break ;
					
					bz2out.Write( buffer, 0, bytesRead ) ;
				}			
				bz2out.Flush() ;	
			}
		}
					
		//
		//
		//
		public MemoryStream Compress( Stream stream )
		{
			if ( stream == null ) return null ;

			byte[] buffer = new byte[ BUFFER_SIZE ] ;
						
			MemoryStream outStream = new MemoryStream() ;

			using ( BZip2OutputStream bz2Stream = new BZip2OutputStream( outStream ) )
			{
				while ( true )
				{
					int bytesRead = stream.Read( buffer, 0, buffer.Length ) ;
					if ( bytesRead <= 0 ) break ;
					
					bz2Stream.Write( buffer, 0, bytesRead ) ;
				}			
				bz2Stream.Flush() ;	
			} // OUCH - doesnt work if you dont close the output stream

			outStream.Position = 0 ;
			return outStream ;			
		}		

		//
		// Because SharpZipLib needs to close the compressed stream to save its compressed bytes, 
		// you cant leave the output stream open, so if you need to get at the stream afterwards, you cant.
		// So here I just copy the compressed bytes to a new stream - UGLY and wont work on big streams. 
		//
		public MemoryStream Compress2( Stream stream )
		{
			using ( MemoryStream outStream = new MemoryStream() )
			{
				this.Compress( stream, outStream ) ;			

				MemoryStream ms = new MemoryStream() ; 
				byte[] bytes = outStream.ToArray() ;
				
				ms.Write( bytes, 0, bytes.Length ) ;
				ms.Position = 0 ;
				return ms ;
			}
		}
				
		//
		//
		//
		public byte[] Compress( byte[] bytes )
		{
			if ( bytes == null ) bytes = new byte[0] ;
			
			using ( MemoryStream outStream = this.Compress( new MemoryStream( bytes ) ) )
				return outStream.ToArray() ;
		}
		
		//
		//
		//
		public byte[] Compress( string str )
		{
			if ( str == null ) str = "" ;
			
			return this.Compress( System.Text.Encoding.Unicode.GetBytes( str )  ) ;
		}

		//
		//
		//
		public void CompressFile( string inFilename, string outFilename )
		{
			if ( inFilename  == null ) return  ;
			if ( outFilename == null ) return  ;
		
			using ( Stream inStream = new FileStream( inFilename, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				using ( Stream outStream = new FileStream( outFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.None ) )
					this.Compress( inStream, outStream ) ;
		}
		
		//
		//
		//
		public void Decompress( Stream inStream, Stream outStream )
		{	
			if ( inStream  == null ) return  ;
			if ( outStream == null ) return  ;

			byte[] buffer = new byte[ BUFFER_SIZE ] ;
			
			using ( BZip2InputStream bz2Stream = new BZip2InputStream( inStream ) )
			{
				while ( true )
				{
					int bytesRead = bz2Stream.Read( buffer, 0, buffer.Length ) ;
					if ( bytesRead <= 0 ) break ;
					
					outStream.Write( buffer, 0, bytesRead ) ;
				}
				outStream.Flush() ;
			}
		}
		
		//
		//
		//
		public MemoryStream Decompress( Stream stream )
		{
			if ( stream == null ) return null ;	

			byte[] buffer = new byte[ BUFFER_SIZE ] ;
			
			MemoryStream outStream = new MemoryStream() ;

			using ( BZip2InputStream bz2Stream = new BZip2InputStream( stream ) )
			{
				while ( true )
				{
					int bytesRead = bz2Stream.Read( buffer, 0, buffer.Length ) ;
					if ( bytesRead <= 0 ) break ;
					
					outStream.Write( buffer, 0, bytesRead ) ;
				}
				outStream.Flush() ;
			}

			outStream.Position = 0 ;
			return outStream ;
		}
				
		//
		//
		//
		public byte[] Decompress( byte[] bytes )
		{
			if ( bytes == null ) bytes = new byte[0] ;
		
			using ( MemoryStream outStream = this.Decompress( new MemoryStream( bytes ) ) )
				return outStream.ToArray() ;
		}
			
		//
		//
		//
		public void DecompressFile( string inFilename, string outFilename )
		{
			if ( inFilename  == null ) return  ;
			if ( outFilename == null ) return  ;
		
			using ( Stream inStream = new FileStream( inFilename, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				using ( Stream outStream = new FileStream( outFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.None ) )
					this.Decompress( inStream, outStream ) ;		
		}		
		
		////
		//// unit testing follows
		////
		//public static void Main( string[] args )
		//{
		//    Test( "" ) ;
		//    Test( "hello charlie" )  ;	
		//    Test( "the fat cat sat on the mat on the hat with a bat in his hat." )  ;
			
		//    //string file = @"/home/albert/albyStuff/documents/albert/LukaAtBambi2005.html" ;			
		//    //TestFile( file ) ;
		//    //TestStream( file ) ;
			
		//    //string bigfile = @"/home/albert/albyStuff/downloads/knoppix/KNOPPIX_V4.0.2CD-2005-09-23-EN.iso" ;	
		//    //TestBigFile( bigfile ) ;

		//    //TestBigFileDecompress() ;
					
		//}
		
		////
		////
		////
		//protected static void Test( string str )
		//{
		//    Console.WriteLine( "@@@" + str + "@@@" ) ;		
			
		//    Bzip2Compression bz2 = new Bzip2Compression() ;

		//    byte[] bytes    = bz2.Compress( str ) ;
		//    byte[] bytesOut = bz2.Decompress( bytes ) ;
			
		//    string stringOut = System.Text.Encoding.Unicode.GetString( bytesOut ) ;
		//    Console.WriteLine( "@@@" + stringOut + "@@@" ) ;					
		//}
	
		////
		////
		////
		//protected static void TestFile( string file )
		//{
		//    Base64String base64 = new Base64String() ;
		//    HashingHelper hh = new HashingHelper() ;		
		//    Bzip2Compression bz2 = new Bzip2Compression() ;
		
		//    string fileCompressed   = @"/tmp/compressed.html.bz2" ;
		//    string fileDecompressed = @"/tmp/decompressed.html" ;

		//    byte[] hash = hh.Sha256HashFile( file ) ;			
		//    Console.WriteLine( file + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
			
		//    Console.WriteLine( "Compressing " + file ) ;		
		//    bz2.CompressFile( file, fileCompressed ) ;	
			
		//    Console.WriteLine( "Decompressing " + fileCompressed ) ;					
		//    bz2.DecompressFile( fileCompressed, fileDecompressed ) ;		
			
		//    hash = hh.Sha256HashFile( fileDecompressed ) ;			
		//    Console.WriteLine( fileDecompressed + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
		//}
		
		////
		////
		////		
		//protected static void TestStream( string file )
		//{
			
		//    Base64String base64 = new Base64String() ;
		//    HashingHelper hh = new HashingHelper() ;		
		//    Bzip2Compression bz2 = new Bzip2Compression() ;
		
		//    Stream stream = new FileStream( file, FileMode.Open, FileAccess.Read, FileShare.Read ) ;
			
		//    MemoryStream compressedStream   = bz2.Compress( stream ) ;
		//    MemoryStream decompressedStream = bz2.Decompress( compressedStream ) ;

		//    stream.Close() ;
			

		//    string fileOut2 = "/tmp/decompressed2.html" ;			
		//    using ( FileStream fs = new FileStream( fileOut2, FileMode.Create, FileAccess.ReadWrite ) )	
		//    {
		//        byte[] bytes = decompressedStream.ToArray() ;
		//        fs.Write( bytes, 0, bytes.Length ) ;
		//    }

		//    byte[] hash = hh.Sha256HashFile( file ) ;			
		//    Console.WriteLine( file + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
				
		//    hash = hh.Sha256HashFile( fileOut2 ) ;			
		//    Console.WriteLine( fileOut2 + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
			
		//    byte[] bytes2 = decompressedStream.ToArray() ;
		//    bytes2 = bz2.Compress( bytes2 ) ;
		//    bytes2 = bz2.Decompress( bytes2 ) ;
			
		//    string fileOut3 = "/tmp/decompressed3.html" ;			
		//    using ( FileStream fs = new FileStream( fileOut3, FileMode.Create, FileAccess.ReadWrite ) )	
		//    {
		//        fs.Write( bytes2, 0, bytes2.Length ) ;
		//    }
			
		//    hash = hh.Sha256HashFile( fileOut3 ) ;			
		//    Console.WriteLine( fileOut3 + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
			
			
		//}
		
		////
		////
		////
		//protected static void TestBigFile( string file )
		//{
		//    Base64String base64 = new Base64String() ;
		//    HashingHelper hh = new HashingHelper() ;		
		//    Bzip2Compression bz2 = new Bzip2Compression() ;
			
		//    string fileCompressed   = @"/tmp/big_compressed.bz2" ;
		//    string fileDecompressed = @"/tmp/big_decompressed.bin" ;

		//    byte[] hash = hh.Sha256HashFile( file ) ;			
		//    Console.WriteLine( file + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
			
		//    Console.WriteLine( "Compressing " + file ) ;		
		//    bz2.CompressFile( file, fileCompressed ) ;	
			
		//    Console.WriteLine( "Decompressing " + fileCompressed ) ;					
		//    bz2.DecompressFile( fileCompressed, fileDecompressed ) ;		
			
		//    hash = hh.Sha256HashFile( fileDecompressed ) ;			
		//    Console.WriteLine( fileDecompressed + " sha256 hash [" + base64.BytesToString( hash ) + "]" ) ;
			
		//}

	}

}
