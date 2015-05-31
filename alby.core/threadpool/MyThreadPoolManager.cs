//
// MyThreadPoolManager.cs
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
	public class MyThreadPoolManager : IDisposable
	{
		//
		// state
		//
		
		protected object 	_lock 					= new object() ;		
		protected object 	_lockShutdownFlag 		= new object() ;		
		
		protected int 		_defaultThreadPoolSize 	= 30    ;
		protected int 		_threadPoolSize 		= 0     ;
		protected int 		_threadPoolQueueMaxSize = 100   ;
		protected bool		_shutdown				= false ; // sync				
		
		protected List<MyThreadPoolThread>    	_threads  = new List<MyThreadPoolThread>()   ; 
		protected List<MyThreadPoolItemBase> 	_tpiQueue = new List<MyThreadPoolItemBase>() ; // sync 
		
		//
		//
		//
		public MyThreadPoolManager() 
		{
			this.Init( _defaultThreadPoolSize, _threadPoolQueueMaxSize ) ;
		}
				
		//
		//
		//
		public MyThreadPoolManager( int threadPoolSize, int threadPoolQueueMaxSize )
		{
			this.Init( threadPoolSize, threadPoolQueueMaxSize ) ;
		}
		
		//
		//
		//
		protected void Init( int threadPoolSize, int threadPoolQueueMaxSize )
		{
			_threadPoolSize         = threadPoolSize ;
			_threadPoolQueueMaxSize = threadPoolQueueMaxSize ;
			
			for ( int i = 1 ; i <= _threadPoolSize ; i++ ) // create the threads
			{
				_threads.Add( new MyThreadPoolThread( this ) ) ;
			}
		}
		
		//
		//
		//
		public int ThreadPoolSize
		{
			get
			{
				return _threadPoolSize ;	
			}
		}
		
		//
		//
		//
		public int ThreadPoolQueueMaxSize
		{
			get
			{
				return _threadPoolQueueMaxSize ;	
			}
		}
				
		//
		// called from WORKER threads
		//
		public bool IsShutdown
		{
			get
			{
				lock( _lockShutdownFlag )
				{
					return _shutdown ;
				}
			}			
		}
						
		//
		// called from main thread
		//
		public void Shutdown() 
		{
			lock( _lock )
			{
				lock( _lockShutdownFlag )
				{
					_shutdown = true ;			
				}			
			}
		}				
												
		//
		// called from main thread
		//
		public void Queue( MyThreadPoolItemBase tpi )
		{
			if ( this.IsShutdown )
			{
				Helper.MtWriteLine( "Thread pool is shutting down. Can't queue new item [{0}] that will never be run, dude.", tpi.ID ) ;
				return ;
			}
			
			if ( tpi.IsStarted )
				throw new Exception( "ThreadPoolItem #" + tpi.ID + " has already been started. Create a new one." ) ;
				
			this.ThrottleThreadPool() ; // wait for thread count to be less than maximum unstarted + running queue length
									
			tpi.IsFinished        = false ;
			tpi.ThreadPoolManager = this  ;
			
			lock( _lock )
			{
				_tpiQueue.Add( tpi ) ;			 
			}
		}
						
		//
		// called from WORKER threads
		//
		// get next unstarted tpi from _tpiQueue -sync
		// 
		public MyThreadPoolItemBase NextThreadPoolItem()
		{
			int queueLength    = 0 ;
			int unstartedItems = 0 ;
			int finishedItems  = 0 ;
			int runningItems   = 0 ;
			
			this.ThreadPoolStats( out queueLength, out unstartedItems, out finishedItems, out runningItems ) ; 
							
			// this is probably a good time to reduce the array size
			if ( finishedItems > 100 )
				this.CleanupFinishedThreadPoolItems() ;
			
			lock( _lock )
			{
				foreach ( MyThreadPoolItemBase tpi in _tpiQueue )
				{
					if ( ! tpi.IsStarted ) 
					{
						//Helper.WriteLine( "ThreadPoolManager - NextThreadPoolItem to run is [{0}]", tpi.ID ) ; 			
						
						tpi.IsStarted = true ;
						return tpi ;
					}
				}
				return null ; // no waiting thread items
			}
		}
		
		//
		// called from main thread
		// wait here until queue length is sane
		// wait 'till unstarted + running <= max queue length
		//
		public void ThrottleThreadPool()
		{
			int queueLength    = 0 ;
			int unstartedItems = 0 ;
			int finishedItems  = 0 ;
			int runningItems   = 0 ;
			
			while ( true )
			{
				this.ThreadPoolStats( out queueLength, out unstartedItems, out finishedItems, out runningItems ) ; 
							
				if ( unstartedItems + runningItems <= _threadPoolQueueMaxSize ) return ;
				
				ST.Thread.Sleep( 500 ) ; // let busy cpu do stuff for a while				
			}	
		}
		
		//
		// called from main thread
		// other threads will use _threadQueue
		//
		public void ThreadPoolStats( out int queueLength, out int unstartedItems, out int finishedItems, out int runningItems )
		{
			queueLength    = 0 ;
			unstartedItems = 0 ;
			finishedItems  = 0 ;
			runningItems   = 0 ;
			
			lock( _lock )
			{
				queueLength = _tpiQueue.Count ;
				
				foreach ( MyThreadPoolItemBase tpi in _tpiQueue )
				{
					if ( tpi == null )
					{}
					else
					if ( ! tpi.IsStarted ) 
						unstartedItems ++ ;
					else
					if ( tpi.IsFinished ) 
						finishedItems ++ ;
					else
						runningItems ++ ;
				}
			}	
					
			//Helper.WriteLine( "ThreadPoolStats - threads [{0}] || items running [{1}], unstarted [{2}], finished [{3}], total [{4}]", 
			//				  _threads.Count, runningItems, unstartedItems, finishedItems, queueLength ) ;			
		}		
		
		//
		// called  from main thread
		// wait until all thread items finished
		// if they havet started yet , then they are considered already finished
		//
		public void WaitUntilAllFinished()  
		{			
			//Helper.WriteLine( "ThreadPoolManager - start - WaitUntilAllFinished" ) ; 			
			
			lock( _lock ) 
			{
				foreach ( MyThreadPoolItemBase tpi in _tpiQueue )
					if ( tpi != null )
						tpi.WaitUntilFinished() ;

			}
			
			//Helper.WriteLine( "ThreadPoolManager - end - WaitUntilAllFinished" ) ; 						
		}
		
		//
		// callled from main thread
		//		
		public void WaitUntilAllStarted() 
		{
			int queueLength    = 0 ;
			int unstartedItems = 0 ;
			int finishedItems  = 0 ;
			int runningItems   = 0 ;
			
			while ( true )
			{
				if ( this.IsShutdown ) return ; // no point waiting ;
				
				this.ThreadPoolStats( out queueLength, out unstartedItems, out finishedItems, out runningItems ) ; 
			
				if ( unstartedItems == 0 ) return ;
				
				//Helper.WriteLine( "ThrottleThreadPool - running [{0}], unstarted [{1}], finished [{2}], total [{3}]", 
				//				  runningItems, unstartedItems, finishedItems, queueLength ) ;
				
				ST.Thread.Sleep( 500 ) ; // let busy cpu do stuff for a while				
			}				
		}
		
		//
		// called from main thread
		//	
		public void Dispose()
		{
			//Helper.WriteLine( "ThreadPoolManager - start - dispose" ) ; 
			
			this.Shutdown() ;
			this.WaitUntilAllFinished() ;		
			
			lock( _lock )
			{
				_tpiQueue.Clear() ;
			}		
		
			//ALBY 

			// really wait for worker threads to finish
			foreach( MyThreadPoolThread tpt in _threads )
				tpt.Join() ;	
			
			//Helper.WriteLine( "ThreadPoolManager - end - dispose" ) ; 
		}
		
		//
		//
		//
		public override string ToString()
		{
			return base.ToString() ;
		}		
		
		//
		// called from main thread
		// this is called by the user as required
		// other threads will look at _threadQueue
		//
		public void CleanupFinishedThreadPoolItems() 
		{
			//Helper.WriteLine( "#### CleanupFinishedThreadPoolItems ####" ) ;
			
			lock( _lock )
			{
				List<MyThreadPoolItemBase> liveTpiQueue = new List<MyThreadPoolItemBase>() ;
				
				foreach ( MyThreadPoolItemBase tpi in _tpiQueue )
					if ( ! tpi.IsFinished )
						liveTpiQueue.Add( tpi ) ;
				
				_tpiQueue = liveTpiQueue ;
			}
		}

	} // end class
	
} // end namespace
	
	
