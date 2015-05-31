//
// Example1Waiting.cs
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
using ST=System.Threading ;

using NUnit.Framework;
using alby.core.threadpool;

namespace alby.core.threadpool.test
{
	[TestFixture]
	public class Example1Waiting 
	{
		public Example1Waiting() : base()
		{
		}		
		
		[Test]
		public void RunMyExample()
		{
			Example1MyThreadPoolItem.Go = false;
			
			Helper.WriteLine("@@@  RunMyExample - start");
			
			using ( MyThreadPoolManager tpm = new MyThreadPoolManager( 100, 100 ) ) // max 100 threads, queue 200
			{
				Console.WriteLine("@@@  RunMyExample - creating threads");
				
				for (int i = 1 ; i <= 100 ; i++)
					tpm.Queue( new Example1MyThreadPoolItem() ) ;

				Console.WriteLine("@@@  RunMyExample - all threads created, waiting for all threads to start");
				tpm.WaitUntilAllStarted();

				Console.WriteLine("@@@  RunMyExample - all threads started, sleeping 5 sec");
				ST.Thread.Sleep( 5000);

				Console.WriteLine("@@@  RunMyExample - all threads created - setting go flag");
				Example1MyThreadPoolItem.Go = true;
				Console.WriteLine("@@@  RunMyExample - all threads created - go flag set");

				Console.WriteLine("@@@  RunMyExample - sleeping for 2 sec");
				ST.Thread.Sleep( 2000 );
				
				//Helper.WriteLine( "@@@  RunMyExample - finish - waiting for threads to finish" ) ;
				//tpm.WaitUntilAllFinished() ;
				Helper.WriteLine("@@@  RunMyExample - finish - SHUTDOWN notification.");
			}	
					
			Helper.WriteLine( "@@@  RunMyExample - finish - threads finished after SHUTDOWN notification" ) ;
		}
			
				
	} // end class

} // end namespace
