using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

namespace alby.core
{
	public class SimpleDelimitedValuesParser : DelimitedValuesParserBase
	{
		//
		// this is a simple parse - just split on , or whatever the delimiter is
		//
		protected override string[] StringToFields( string aString )
		{
			if ( aString == null )
				return null ;

			if ( this.Trim )
				aString = aString.Trim() ;

			if ( aString.Length == 0 )
				return null ;

			string[] fieldArray = aString.Split( new char[] { this.Delimiter } ) ; 

			if ( fieldArray == null )
				return null ;

			if ( this.Trim )
				for ( int i = 0 ; i < fieldArray.Length ; i++ )
					fieldArray[i] = fieldArray[i].Trim() ; 

			return fieldArray ;
		}

	}

}