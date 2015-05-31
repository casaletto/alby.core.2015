//
// Example1MyThreadPoolItem.cs
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

using alby.core.threadpool ;

namespace alby.core.threadpool.test
{
	public class Example1MyThreadPoolItem : MyThreadPoolItemBase
	{
		protected int			_sleepTime = 0;

		protected static object __lock	= new object();
		protected static bool	__go	 = false;
		
		public static bool Go
		{
			get
			{
				lock( __lock )
				{
					return __go ;
				}
			}
			set
			{
				lock( __lock )
				{
					__go = value ;
				}
			}	
		}	
			
		public Example1MyThreadPoolItem() : base()
		{
		}		
		
		public Example1MyThreadPoolItem( int sleepTime  ) : base()
		{
			_sleepTime = sleepTime ;
		}		
		
		public override void Run() 
		{
			Helper.MtWriteLine( "\tExample1MyThreadPoolItem.Run ################# [{0}] starting -  waiting for go flag.", this.ID ) ;
			while ( true )
			{
				if ( __go ) break ;
				if ( this.ThreadPoolManager.IsShutdown ) break ;
				Thread.Sleep( 50 ) ;
			}
			Helper.MtWriteLine("\tExample1MyThreadPoolItem.Run ################# [{0}] we are go ...", this.ID);
			
			DateTime t1 = System.DateTime.Now ;
			
			if ( this.ThreadPoolManager.IsShutdown )
			{
				Helper.MtWriteLine( "\tExample1MyThreadPoolItem.Run ################# [{0}] SHUTDOWN notification.", this.ID ) ;
			}
			else
			{
				if ( _sleepTime == 0 )
				{	
					Random ran = new Random() ;	
					_sleepTime = ran.Next( 500, 5000 ) ;
					_sleepTime = ran.Next( 500, 5000 ) ;
				}
			
				Helper.MtWriteLine("\tExample1MyThreadPoolItem.Run ################# [{0}] - sleeping for [{1}] ms...", this.ID, _sleepTime );						
				Thread.Sleep( _sleepTime ) ;
			}
					
			DateTime t2 = System.DateTime.Now ;
			TimeSpan ts = t2.Subtract( t1 ) ;

			Helper.MtWriteLine("\tExample1MyThreadPoolItem.Run ################# [{0}] stopping , which ran for [{1}] ms, SHUTDOWN notification = [{2}]",
				this.ID, ts.TotalMilliseconds, this.ThreadPoolManager.IsShutdown );
		}
		
		
	} // end class

} // end namespace
