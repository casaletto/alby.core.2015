using System;
using System.Collections.Generic;
using System.Text;

namespace alby.core
{
	public class HexHelper
	{
		public static string ByteArrayToHexString( byte[] b ) 
		{
			if ( b == null ) return "" ;
			
			return BitConverter.ToString( b ).Replace( "-", "" ) ;
		}
	
	}
}
