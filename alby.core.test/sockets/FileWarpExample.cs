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
	public class TestFileWarp
	{
		[Test]
		public void IpAddressesOnLocalhost()
		{
            string dns = "" ;
            foreach( IPAddress address in Dns.GetHostAddresses( dns ) )
                Console.WriteLine( "[{2}]\t\t\t[{1}]\t\t[{0}]", address.ToString(), address.AddressFamily, dns ) ; 

            dns = Environment.MachineName ;
            foreach( IPAddress address in Dns.GetHostAddresses( dns ) )
                Console.WriteLine( "[{2}]\t\t[{1}]\t\t[{0}]", address.ToString(), address.AddressFamily, dns ) ; 

            dns = "localhost" ;
            foreach( IPAddress address in Dns.GetHostAddresses( dns ) )
                Console.WriteLine( "[{2}]\t\t[{1}]\t\t[{0}]", address.ToString(), address.AddressFamily, dns ) ; 

            dns = "::1" ;
            foreach( IPAddress address in Dns.GetHostAddresses( dns ) )
                Console.WriteLine( "[{2}]\t\t\t[{1}]\t\t[{0}]", address.ToString(), address.AddressFamily, dns ) ; 

            dns = "127.0.0.1" ;
            foreach( IPAddress address in Dns.GetHostAddresses( dns ) )
                Console.WriteLine( "[{2}]\t\t[{1}]\t\t[{0}]", address.ToString(), address.AddressFamily, dns ) ; 

            foreach( IPAddress address1 in Dns.GetHostAddresses( "" ) )
            {
                dns = address1.ToString() ;
                foreach( IPAddress address in Dns.GetHostAddresses( dns ) )
                    Console.WriteLine( "[{2}]\t\t[{1}]\t\t[{0}]", address.ToString(), address.AddressFamily, dns ) ; 
            }
        }

		[Test]
		public void IsIp4Up() // see if ip4 is up 
		{
            string str = this.GetLocalIp4Address() ; 
            Console.WriteLine( "[{0}]", str ) ;
            Assert.IsTrue( str.Length > 0 ) ;
        }

		[Test]
		public void IsIp6Up()
		{
            string str = this.GetLocalIp6Address() ; 
            Console.WriteLine( "[{0}]", str ) ;
            Assert.IsTrue( str.Length > 0 ) ;
        }


		[Test]
		public void RunMyFileWarpExample4_LocalMachineName()
		{
            RunMyFileWarpExample( 4, Environment.MachineName ) ;
        }

		[Test]
		public void RunMyFileWarpExample6_LocalMachineName()
		{
            RunMyFileWarpExample( 6, Environment.MachineName ) ;
        }

        [Test]
		public void RunMyFileWarpExample6_Localhost()
		{
            RunMyFileWarpExample( 6, "localhost" ) ;
        }

        [Test]
		public void RunMyFileWarpExample6_ColonColon1()
		{
            RunMyFileWarpExample( 6, "::1" ) ;
        }

        [Test]
		public void RunMyFileWarpExample4_127dot0dot0dot1()
		{
            RunMyFileWarpExample( 4, "127.0.0.1" ) ;
        }

        [Test]
		public void RunMyFileWarpExample4_LocalIp4Address()
		{
            RunMyFileWarpExample( 4, this.GetLocalIp4Address() ) ;
        }

        [Test]
		public void RunMyFileWarpExample6_LocalIp6Address()
		{
            RunMyFileWarpExample( 6, this.GetLocalIp6Address() ) ;
        }

		protected void RunMyFileWarpExample( int ipversion, string theServer )
		{
			string sourceFile			= @"alp-ch04-threads.txt";
			string sourceFileFullPath	= TestFileWarpThreadPoolItem.ServerFileSite + @"\" + sourceFile;
			
			Assert.IsTrue( File.Exists(sourceFileFullPath ));
			FileInfo fiSource = new FileInfo( sourceFileFullPath ) ;
			Assert.IsTrue( fiSource.Length > 100000 ) ;
			
			alby.core.crypto.Hashing hashing = new alby.core.crypto.Hashing() ;
			byte[] hashSource = hashing.Sha512HashFile( sourceFileFullPath ) ;
			
			string destFile	= @"c:\temp\unittestcopy.txt" ;
			File.Delete(destFile);
			Assert.IsFalse( File.Exists( destFile ) ) ;

			Console.WriteLine("@@@ [tid {0}] TestFileWarp - start unit test. ", Thread.CurrentThread.ManagedThreadId);

			// server
			using ( MyThreadPoolManager tpm = new MyThreadPoolManager( 1, 1 ) )
			{
				Console.WriteLine("@@@ [tid {0}] TestFileWarp - starting server [{1}]. ", Thread.CurrentThread.ManagedThreadId, theServer );

				tpm.Queue(new TestFileWarpThreadPoolItem( ipversion ));
				tpm.WaitUntilAllStarted();

				// client
				Console.WriteLine("@@@ [tid {0}] TestFileWarp - starting client to server [{1}]. ", Thread.CurrentThread.ManagedThreadId, theServer);

				FileWarpClient fwc ;
                if ( ipversion == 4 )
                    fwc = new FileWarpClient4( theServer, TestFileWarpThreadPoolItem.Port);
                else
                    fwc = new FileWarpClient6( theServer, TestFileWarpThreadPoolItem.Port);

				fwc.DownloadFile(sourceFile, destFile);
				Console.WriteLine("@@@ [tid {0}] TestFileWarp - finishing client. ", Thread.CurrentThread.ManagedThreadId);
			 
				File.WriteAllText(TestFileWarpThreadPoolItem.StopFile, "you can finish now");
				Console.WriteLine("@@@ [tid {0}] TestFileWarp - finishing server. ", Thread.CurrentThread.ManagedThreadId);
			}

			Assert.IsTrue( File.Exists( destFile ) ) ;
			FileInfo fiDest = new FileInfo(destFile);
			Assert.AreEqual( fiSource.Length, fiDest.Length ) ;

			byte[] hashDest = hashing.Sha512HashFile(destFile);
			Assert.AreEqual( Convert.ToBase64String( hashDest ), Convert.ToBase64String( hashSource ) ) ;
			
			Console.WriteLine("@@@ [tid {0}] TestFileWarp - finish unit test. ", Thread.CurrentThread.ManagedThreadId);
		}

        protected string GetLocalIp4Address() 
        {
            foreach( IPAddress address in Dns.GetHostAddresses( "" ) )
            {
                if ( address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
                    return address.ToString() ;
            }
            return "" ;
        }

        protected string GetLocalIp6Address() 
        {
            foreach( IPAddress address in Dns.GetHostAddresses( "" ) )
            {
                if ( address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 )
                    return address.ToString() ;
            }
            return "" ;
        }
				
	} // end class		

	public class TestFileWarpThreadPoolItem : MyThreadPoolItemBase
	{
		public const int	Port			= 10001;
		public const int	Timeout			= 25;
		public const string StopFile		= @"c:\temp\stopRunMyFileWarpExample.txt";
		public const string ServerFileSite	= @"c:\albyStuff\development\alby.core.2015\doc" ;

        protected int _ipversion ;

		public TestFileWarpThreadPoolItem( int ipversion )
			: base()
		{
            _ipversion = ipversion ;
		}

		public override void Run()
		{
			FileWarpServer fws ;
        
            if ( _ipversion == 4 )
                fws = new FileWarpServer4(Port, Timeout, StopFile, ServerFileSite);
            else
                fws = new FileWarpServer6(Port, Timeout, StopFile, ServerFileSite);

			fws.Listen();
		}

	}

}
