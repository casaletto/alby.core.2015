//
// RemoteExecClient.cs
//

using System ;
using System.IO ;
using System.Text ;
using System.Collections ;
using System.Collections.Generic ;
using System.Xml ;
using System.Xml.Xsl ;
using System.Xml.XPath ;
using System.Xml.Schema ;
using System.Diagnostics ;	
using System.Reflection ;
using System.Threading ;
using System.Net ;
using SNS=System.Net.Sockets ;

using alby.core.threadpool;

namespace alby.core.sockets
{
	public class RemoteExecClient
	{
		protected SNS.AddressFamily _addressFamily  ;

		public SNS.AddressFamily AddressFamily
		{
			get
			{
				return _addressFamily ;
			}
		}
        
		protected string _server = "" ;
		
		public string Server
		{
			get
			{
				return _server ;
			}
		}
		
		protected int _port = 0  ;
		
		public int Port
		{
			get
			{
				return _port ;
			}
		}

		protected string _command = "" ;
		
		public string Command
		{
			get
			{
				return _command ;
			}
		}
		
		protected string _stdout = "" ;
		
		public string Stdout
		{
			get
			{
				return _stdout ;
			}
		}
		
		protected string _stderr = "" ;
		
		public string Stderr
		{
			get
			{
				return _stderr ;
			}
		}
		
		protected int _exitCode = -1 ;
		
		public int ExitCode
		{
			get
			{
				return _exitCode ;
			}
		}
		
		public RemoteExecClient( SNS.AddressFamily addressFamily, string server, int port )
		{
            _addressFamily = addressFamily ;
			_server = server ;
			_port   = port ;
		}
	
		public void ExecuteCommand( string command, string args, bool waitForExit )
		{
			using ( SocketClient client = new SocketClient() )
			{
				// set up request
				Dictionary<string,string> request = new Dictionary<string,string>()    ;
				request[ "Command"     ] = command ;
				request[ "Args"        ] = args ;
				request[ "WaitForExit" ] = waitForExit.ToString() ;
			
				// connect to server
				client.Connect( _addressFamily, _server, _port ) ;
							
				// over the aether we go...
				Dictionary<string,string> response = client.Call( request ) ; 				
				string rc = response[ "ResponseStatus" ] ;
				if ( rc != "OK" ) 
					throw new SocketException( "Error from RemoteExec server: " + response[ "ResponseError" ] ) ;  
					 
				if ( waitForExit )
				{	 
           			_exitCode = int.Parse( response[ "ExitCode" ] ) ;
           			_stdout   = response[ "Stdout"   ] ;
            		_stderr   = response[ "Stderr"   ] ;
				}	 
			}
		} // end exec cmd	
					
	} // end class		

    public class RemoteExecClient4 : RemoteExecClient 
    {
		public RemoteExecClient4( string server, int port ) 
            : base( SNS.AddressFamily.InterNetwork, server, port ) 
        {
        }
    }

    public class RemoteExecClient6 : RemoteExecClient 
    {
		public RemoteExecClient6( string server, int port ) 
            : base( SNS.AddressFamily.InterNetworkV6, server, port ) 
        {
        }
    }

}
