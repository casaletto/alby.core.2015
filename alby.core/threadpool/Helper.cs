//
// Helper.cs
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

namespace alby.core.threadpool
{
	public class Helper 
	{	
		//
		//
		//
		protected static object __lock = new object() ;
		
		//
		//
		//
		public Helper()
		{
		}
		
		//
		//
		//
		public static void WriteLine( object obj )
		{
			if ( obj == null ) 
					obj = "[NULL]" ;
			
			MtWriteLine( obj.ToString() ) ;
		}
		
		//
		//
		//
		public static void MtWriteLine( string msg, params object[] args )
		{
			lock( __lock )
			{
				if ( msg == null ) 
					msg = "[NULL]" ;
				
				//Console.WriteLine( msg, args ) ;
			}
		}
						
		//
		//
		//
		public static bool IsWindows() 
		{
			return System.Environment.OSVersion.ToString().ToUpper().IndexOf( "WINDOWS" ) >= 0 ;
		}		
		
		//
		//
		//
		public static bool IsLinux() 
		{
			return ! Helper.IsWindows() ;
		}		
				
	} // end class	
	
} // end namespace
