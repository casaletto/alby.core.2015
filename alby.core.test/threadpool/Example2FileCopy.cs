//
// Example2FileCopy.cs
//

using System ;
using System.IO ;
using System.Data;
using System.Text ;	
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Collections ;
using System.Collections.Generic ;
using System.Reflection ;
using System.Threading ;

using NUnit.Framework ;
using alby.core.threadpool;

namespace alby.core.threadpool.test
{
	[TestFixture]
	public class Example2FileCopy 
	{
		public Example2FileCopy() : base()
		{
		}		

		[Test] 		
		public void RunMyExample()
		{
			string sourceDirectory = @"c:\temp\spacePictures";
			string destDirectory = @"c:\temp\spacePicturesCopy";
            int    maxFiles = 10 ;

			// how many files to copy
			DirectoryInfo sourceDirectoryInfo = new DirectoryInfo( sourceDirectory ) ;
            int sourceFileCount = 0 ;
			long bytesSource = 0 ;

			// recreate dest directory ;
			//if ( Directory.Exists(destDirectory) )
			//	Directory.Delete(destDirectory, true ) ;
            if ( !Directory.Exists(destDirectory))
                Directory.CreateDirectory(destDirectory);

            Assert.IsTrue(Directory.Exists(destDirectory));

            Array.ForEach( 
                Directory.GetFiles(destDirectory), 
                x => File.Delete( x ) ) ;
    
            Assert.IsTrue( Directory.GetFiles(destDirectory).Length == 0);
            
            //-------------------------------------------

			DateTime start = DateTime.Now;
			Console.WriteLine("*** RunMyExample - start, copying [{0}] files \nfrom [{1}] \nto [{2}]", sourceFileCount, sourceDirectory, destDirectory);

			using (MyThreadPoolManager tpm = new MyThreadPoolManager( 100, 100 ) ) // max 100 threads, queue 100
			{
                int i = 0 ;
                sourceFileCount = 0 ;
				foreach ( FileInfo fi in sourceDirectoryInfo.GetFiles() ) 
				{
					Example2ThreadPoolItem mtpi = new Example2ThreadPoolItem() ; 
					mtpi.SourceMachinePath      = sourceDirectory +  @"\" + fi.Name ;
					mtpi.DestinationMachinePath = destDirectory   +  @"\" + fi.Name ;
					tpm.Queue( mtpi ) ;

                    sourceFileCount++ ;
                    bytesSource += fi.Length;

                    i++ ;
                    if ( i > maxFiles ) break ;
				}
											
				Helper.WriteLine( "*** RunMyExample - waiting for all threads to start..." ) ;
				tpm.WaitUntilAllStarted() ;
				
				Helper.WriteLine( "*** RunMyExample - finish - waiting for threads to finish..." ) ;
			}

			Helper.WriteLine("*** RunMyExample - finish - threads finished");
			DateTime finish = DateTime.Now;

			// see if all files copied over
			DirectoryInfo destDirectoryInfo = new DirectoryInfo(destDirectory);
			int destFileCount = destDirectoryInfo.GetFiles().Length;
			Assert.AreEqual( sourceFileCount, destFileCount ) ;

			// see if all files copied over
			long bytesDest = 0;			
			foreach ( FileInfo fi in destDirectoryInfo.GetFiles() ) 
				bytesDest += fi.Length ;
			Assert.AreEqual(bytesSource, bytesDest);

			// dump timing info 
			double seconds = ((double)finish.Subtract(start).TotalMilliseconds) / 1000.0;
			double rate = Math.Round( ((double) bytesDest ) / ((double) seconds ) / 1000000.0, 2 ) ;

			Helper.MtWriteLine("*** RunMyExample - finish - [{0}] files copied in [{1}] sec - rate [{2}] bytes/sec",
								sourceFileCount,
								seconds, 
								rate);
		}		
				
	} // end class

} // end namespace
