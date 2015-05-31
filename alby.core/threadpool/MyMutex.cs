//
// MyMutex.cs
// 

using System ;
using System.IO ;
using System.Data;
using System.Configuration ;
using System.Text ;	
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Collections ;
using System.Collections.Generic ;
using System.Reflection ;
using System.Threading ;

//
// usage:
//
//
// MyMutex myMutex = new MyMutex() ;
//
// using ( MyMutexLock lock = myMutex.Lock() ) // do Mutex.WaitOne() ;
// {
//
//		// do non-reentrant task
//
// }	// do Mutex.ReleaseMutex() in Dispose() 
//
//

namespace alby.core.threadpool
{
	//
	//
	//
	public class MyMutex 
	{	
		//
		//
		//
		protected Mutex _mutex = new Mutex() ;
		
		//
		//
		//
		public MyMutex()
		{
		}
		
		//
		//
		//
		public MyMutexLock Lock()
		{
			Helper.WriteLine( "*** MyMutex - waiting to acquire lock." ) ;
			
			_mutex.WaitOne() ;

			Helper.WriteLine( "*** MyMutex - lock acquired." ) ;

			return new MyMutexLock( _mutex ) ;
		}

	} // end class	

	//
	//
	//
	public class MyMutexLock : IDisposable
	{
		//
		//
		//
		protected Mutex _mutex = null ;
		
		//
		//
		//
		public MyMutexLock( Mutex aMutex )
		{
			_mutex = aMutex ;
		}

		//
		//
		//
		public void Dispose()
		{
			_mutex.ReleaseMutex() ;
			
			Helper.WriteLine( "*** MyMutex - lock released." ) ;
		}
	
	} // end class
	
} // end namespace
