//
// RemoteExecServerThreadPoolItem.cs
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
using System.Net ;
using System.Diagnostics ;
using SNS=System.Net.Sockets ;

using alby.core.threadpool;

namespace alby.core.sockets
{
	public class RemoteExecServerThreadPoolItem : SocketServerThreadPoolItem
	{		
		//
		//
		//
		public RemoteExecServerThreadPoolItem() : base()
		{	
		}		
				
		//
		// 
		//
		public override void RunSocket()
		{		
			Console.WriteLine( "RemoteExecServerThreadPoolItem.Run ################# #{0} starting", this.ID ) ;
								
			// call 0. get header 
			Dictionary<string,string> request = this.Socket.ReceiveDictionary() ;

			string command     = request[ "Command" ] ;
			string args        = request[ "Args" ] ;
			bool   waitForExit = bool.Parse( request[ "WaitForExit" ] ) ;

			ExecuteCommand( command, args, waitForExit ) ;			
		}		
		
		//
		//
		//
		protected void ExecuteCommand( string command, string args, bool waitForExit )
		{
			Dictionary<string,string> response = new Dictionary<string,string>() ;
			response[ "ResponseStatus" ] = "OK" ;
			response[ "ExitCode" ]       = "-1" ;
			response[ "Stdout"   ]       = ""   ;
			response[ "Stderr"   ]       = ""   ;

			Console.WriteLine("RemoteExecServerThreadPoolItem.Run ################# #{0} Exec [{1}] [{2}]...", this.ID, command, args);

            ProcessStartInfo psi = new ProcessStartInfo();

			try
			{
            	psi.WindowStyle = ProcessWindowStyle.Hidden;
            	psi.CreateNoWindow = true;
            	psi.UseShellExecute = false;
            	psi.RedirectStandardOutput = true;
            	psi.RedirectStandardError = true;

            	psi.FileName = command ;
            	psi.Arguments = args;

            	// fire away and wait
            	Process process = Process.Start(psi);
              
              	if ( waitForExit )
              	{
					process.WaitForExit();
	
					response[ "ExitCode" ] = process.ExitCode.ToString() ;				
					response[ "Stdout"   ] = process.StandardOutput.ReadToEnd();
					response[ "Stderr"   ] = process.StandardError.ReadToEnd();

					Console.WriteLine("RemoteExecServerThreadPoolItem.Run ################# #{0} Exit code [{1}]", this.ID, response["ExitCode"]);
              	}
              	else
					Console.WriteLine("RemoteExecServerThreadPoolItem.Run ################# #{0} Now detached - running async...", this.ID);				
			}
			catch( Exception ex )
			{
				Console.WriteLine("RemoteExecServerThreadPoolItem.Run ################# #{0} EXCEPTION\n{1}", this.ID, ex.ToString());
				
				response[ "ResponseStatus" ] = "Error" ;
				response[ "ResponseError"  ] = ex.Message ;
			}

			this.Socket.Send( response ) ; // send it back to client
		}
				
		
	} // end class

} // end namespace
