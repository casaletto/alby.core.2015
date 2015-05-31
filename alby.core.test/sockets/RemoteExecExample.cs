//
// Main.cs
//

using System ;
using System.IO ;
using System.Collections ;
using System.Collections.Generic ;
using System.Text ;
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Diagnostics ;	
using System.Reflection ;
using System.Threading ;
using System.Net ;
using SNS=System.Net.Sockets ;

using NUnit.Framework ;
using alby.core.threadpool ;
using alby.core.sockets ;


namespace alby.core.sockets.test 
{

	[TestFixture]
	public class TestRemoteExec
	{
		[Test]
		public void MyRemoteExec6Example_Localhost()
        {
            RunMyRemoteExecExample( 6, "localhost" ) ; 
        }

		[Test]
		public void MyRemoteExec4Example_Localhost()
        {
            RunMyRemoteExecExample( 4, "localhost" ) ;
        }

		[Test]
		public void MyRemoteExec6Example_MachineName()
        {
            RunMyRemoteExecExample( 6, Environment.MachineName ) ; 
        }

		[Test]
		public void MyRemoteExec4Example_MachineName()
        {
            RunMyRemoteExecExample( 4, Environment.MachineName ) ;
        }


		protected void RunMyRemoteExecExample( int ipversion, string theServer )
		{
			Console.WriteLine("@@@ [tid {0}] TestRemoteExec - start unit test. ", Thread.CurrentThread.ManagedThreadId);

			// server
			using ( MyThreadPoolManager tpm = new MyThreadPoolManager( 1, 1 ) )
			{
				Console.WriteLine("@@@ [tid {0}] TestRemoteExec - starting server on [{1}]. ", Thread.CurrentThread.ManagedThreadId, theServer );

				tpm.Queue(new TestRemoteExecThreadPoolItem( ipversion ));
				tpm.WaitUntilAllStarted();

				// client
				Console.WriteLine("@@@ [tid {0}] TestRemoteExec - starting client to [{1}]. ", Thread.CurrentThread.ManagedThreadId, theServer );

				RemoteExecClient rec ; //= new RemoteExecClient6("localhost", TestRemoteExecThreadPoolItem.Port);
                if ( ipversion == 4 )
                    rec = new RemoteExecClient4( theServer, TestRemoteExecThreadPoolItem.Port);
                else
                    rec = new RemoteExecClient6( theServer, TestRemoteExecThreadPoolItem.Port);

				rec.ExecuteCommand( "ls", "-al", true ) ;
				Console.WriteLine( rec.Stdout ) ;

				Console.WriteLine("@@@ [tid {0}] TestRemoteExec - finishing client. ", Thread.CurrentThread.ManagedThreadId);
			 
				File.WriteAllText(TestRemoteExecThreadPoolItem.StopFile, "you can finish now");
				Console.WriteLine("@@@ [tid {0}] TestRemoteExec - finishing server. ", Thread.CurrentThread.ManagedThreadId);
			}
			Console.WriteLine("@@@ [tid {0}] TestRemoteExec - finish unit test. ", Thread.CurrentThread.ManagedThreadId);
		}
				
	} // end class		

	public class TestRemoteExecThreadPoolItem : MyThreadPoolItemBase
	{
		public const int Port = 10001;
		public const int Timeout = 25;
		public const string StopFile = @"c:\temp\stopRunMyRemoteExecExample.txt";

        protected int _ipversion ;

		public TestRemoteExecThreadPoolItem( int ipversion )
			: base()
		{
            _ipversion = ipversion ; 
		}

		public override void Run()
		{
			RemoteExecServer fws ; //= new RemoteExecServer6(Port, Timeout, StopFile);

            if ( _ipversion == 4 )
                fws = new RemoteExecServer4(Port, Timeout, StopFile );
            else
                fws = new RemoteExecServer6(Port, Timeout, StopFile );

			fws.Listen();
		}

	}

}
