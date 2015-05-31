//
// MyThreadPoolThread.cs
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

namespace alby.core.threadpool
{
	public class MyThreadPoolThread 
	{
		//
		// static state
		//
		protected static int __nextID = 0 ;
		
		//
		// state
		//
		protected int 					_ID     = 0 ;
		protected MyThreadPoolManager 	_tpm    = null ;
		protected ST.Thread			    _thread = null ; // the mananage .NET CLR thread abstraction of the physical kernal thread
		
		//
		// no sync reqd
		//
		public MyThreadPoolThread( MyThreadPoolManager tpm )
		{
			__nextID++ ;
			_ID  = __nextID ; // my thread id
			_tpm = tpm      ; // pointer back to manager
			
			// create the underlying thread and run it
			_thread = new ST.Thread( new ST.ThreadStart( this.Run ) ) ;
			_thread.Start() ;			
		}
								
		//
		// this method runs in another thread, 
		// picking off the next thread item to run off the queue
		// until manager tells us to stop
		//
		public void Run()
		{
			//Helper.MtWriteLine( "ThreadPoolThread [{0}] - entered run().", _ID ) ; 
			
			while ( true )
			{
				if ( _tpm.IsShutdown ) 
				{
					//Helper.MtWriteLine( "ThreadPoolThread [{0}] - shutdown - exited run().", _ID ) ; 
					break ; // synced internally - quick exit if manager says enough is enough
				}
				
				using ( MyThreadPoolItemBase tpi = _tpm.NextThreadPoolItem() ) // synced internally - get next item off the queue ;
				{
					if ( tpi == null ) 
					{
						ST.Thread.Sleep( 100 ) ; // sleep 1/10 second - try again
						continue ; 
					}
						
					//Helper.WriteLine( "ThreadPoolThread [{0}] - start run - item [{1}].", _ID, tpi.ID ) ; 
					
					try
					{
						tpi.Run() ; // found something to run -run thread pool item's code
					}
					catch( Exception ex ) // cant let exceptions escape - or else thread will die
					{
						Helper.MtWriteLine( "ThreadPoolThread [{0}], item [{1}] - EXCEPTION:\n\t{2}\n\t\t{3}", 
										  _ID, tpi.ID, ex.Message, ex.StackTrace ) ;
					}
					tpi.IsFinished = true ; // synced - this tpi now marked for deletion
				
					//Helper.WriteLine( "ThreadPoolThread [{0}] - end	 run - item [{1}].", _ID, tpi.ID ) ; 
				}
			}
			
			//Helper.MtWriteLine( "ThreadPoolThread [{0}] - exited run().", _ID ) ; 
		}				
		
		//
		// no sync reqd
		//
		public int ID
		{
			get
			{
				return _ID ;				
			}
		}
		
		//
		//
		//
		public override string ToString()
		{
			return base.ToString() ;
		}		
		
		//
		// call this when the thread pool manager is disposed
		//		
		public void Join()
		{
			_thread.Join() ;
		}

	} // end class
	
} // end namespace
	
	