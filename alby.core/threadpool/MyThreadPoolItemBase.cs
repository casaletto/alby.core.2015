//
// MyThreadPoolItemBase.cs
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
	public abstract class MyThreadPoolItemBase : IDisposable
	{
		//
		// static state
		//
		protected static int __nextID = 0 ;

		//
		// state
		//
		protected object 					_lock 				= new object() ;
		protected int 						_ID 	   			= 0     ;
		protected bool 						_started   			= false ;
		protected bool 						_finished  			= false ;
		protected MyThreadPoolManager  	 	_threadPoolManager 	= null  ;		
		protected Dictionary<string,string>	_params				= new Dictionary<string,string>() ;		
		
		//
		//
		//
		public MyThreadPoolItemBase()
		{
			__nextID ++ ;
			_ID = __nextID ;
		}
				
		//
		//
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
		public bool IsStarted // synced
		{
			get
			{
				lock( _lock )
				{
					return _started ;
				}
			}
			set
			{
				lock( _lock )
				{
					_started = value ;
				}
			}
		}
		
		//
		//
		//
		public bool IsFinished // synced
		{
			get
			{
				lock( _lock )
				{
					return _finished ;
				}
			}
			set
			{
				lock( _lock )
				{
					_finished = value ;
				}
			}
		}
				
		//
		//
		//
		public bool IsRunning // synced
		{
			get
			{
				lock( _lock )
				{
					return _started && ( ! _finished ) ;
				}
			}
		}
		
		//
		//
		//
		public Dictionary<string,string> Parameters
		{
			get
			{
				return _params ;
			}
			set
			{
				_params = value ;
			}
		}
		
		//
		//
		//
		public MyThreadPoolManager ThreadPoolManager
		{
			get
			{
				return _threadPoolManager ;
			}
			set
			{
				_threadPoolManager = value ;
			}
		}
				
		//
		// blocking wit call -wait until my finished flag is true
		// ie, my Run() method has finshed - probably in another thread
		//
		// Note: thread items must check .IsShutdown regularly and finish asap.
		//
		// this Wait returns immediately for items still on the wait queue [ie those never started yet]
		//
		public void WaitUntilFinished()
		{						
			if ( ! this.IsStarted ) // item never got started, so nothing to wait for
			{
				Helper.MtWriteLine( "ThreadPoolItem.WaitUntilFinished - [{0}] was never started.", _ID ) ; 				
				return ; 	
			}
			
			//Helper.WriteLine( "ThreadPoolItem [{0}] - start - wait until finished.", _ID ) ; 
			
			while ( true )
			{
				//Helper.WriteLine( "ThreadPoolItem [{0}] - waiting until finished....", _ID ) ; 
				if ( this.IsFinished ) break ;
				
				ST.Thread.Sleep( 100 ) ; // sleep a while and try again 
			}
			
			//Helper.WriteLine( "ThreadPoolItem [{0}] - end - wait until finished.", _ID ) ; 
		}
		
		//
		//
		//
		public void WaitUntilStarted()
		{	
			while ( true )
			{				
				if ( this.IsStarted ) break ;
				ST.Thread.Sleep( 100 ) ; // sleep a while and try again 
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
		//
		//	
		public void Dispose()
		{
			//Helper.WriteLine( "ThreadPoolItem [{0}] - start dispose.", _ID ) ; 
			
			this.WaitUntilFinished() ; // dont dispose me until the underlying Run() method is finished 
		
			//Helper.WriteLine( "ThreadPoolItem [{0}] - end dispose.", _ID ) ; 
		}
		
		//
		// override in the derived class with what you want to do in another thread
		//
		public virtual void Run() 
		{
			throw new NotImplementedException() ;
		}
		
	} // end class
	
} // end namespace
	
	
	
