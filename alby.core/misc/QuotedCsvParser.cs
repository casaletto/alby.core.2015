using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

namespace alby.core
{
	public class QuotedCsvParser : DelimitedValuesParserBase 
	{
		//
		// csv splitting by , or "xxxx,xxx"
		//

		#region state

		protected char _delimiter2 = QUOTE  ;

		#endregion

		#region properties

		public char Delimiter2
		{
			get
			{
				return _delimiter2 ;
			}
			set
			{
				_delimiter2 = value ;
			}
		}

		#endregion

		protected override string[] StringToFields( string aString )
		{
			if ( aString == null )
				return null ;

			if ( this.Trim )
				aString = aString.Trim() ;

			if ( aString.Trim().Length == 0 ) // always reject blank lines
				return null ;

			string[] fieldArray = this.StringToFields2( aString ).ToArray() ; 
	
			if ( fieldArray == null )
				return null ;

			if ( fieldArray.Length == 0 )
				return null ;

			if ( fieldArray.Length == 1 )
				if ( fieldArray[0].Trim().Length == 0 ) // reject blank lines
					return null ;

			if ( this.Trim )
				for ( int i = 0 ; i < fieldArray.Length ; i++ )
					fieldArray[i] = fieldArray[i].Trim() ; 

			return fieldArray ;
		}

		public List<string> StringToFields2( string aString )
		{
			List<string> fieldList = new List<string>() ;

			List<int> commaList = GetCommaPositions( aString ) ;

			// no commas, so whole string is one field
			if ( commaList.Count == 0 ) 
			{
				fieldList.Add( aString ) ;
				return TidyUpFields( fieldList ) ;
			}

			// split line into fields on each comma position
			int start  = 0 ;
			foreach( int commaPos in commaList )
			{
				int    end    = commaPos ;
				int    length = Math.Max( end - start + 1, 0 ) ;
				string str    = aString.Substring( start, length ) ;

				fieldList.Add( str ) ;
				start = end + 1 ;
			}

			// last field is whatever is beyond the last comma
			string last = "" ;               
			if ( start < aString.Length )   
				last = aString.Substring( start ) ;

			fieldList.Add( last ) ;

			// remove fluff
			return TidyUpFields( fieldList ) ;
		}

		public List<int> GetCommaPositions( string aString ) 
		{
			List<int> commaPositions = new List<int>() ;
			
			if ( aString == null )
				return commaPositions ;

			if ( aString.Length == 0 )
				return commaPositions ;

			if ( aString.Trim().Length == 0 )
				return commaPositions ;

			char[] charArray = aString.ToCharArray() ;

			bool normalField = true ;
			
			for ( int i = 0 ; i < aString.Length ; i++ ) 
			{
				if ( charArray[i] == this._delimiter ) // found a comma
				{
					if ( normalField )
						commaPositions.Add( i ) ;
					else
					{} // ignore commas if in a quoted field

					continue ;
				}

				if ( charArray[i] == this._delimiter2 ) // found a quote
				{
					if ( normalField )
						normalField = false ; // entering a quoted field
					else
						normalField = true ; // leaving a quoted field
					continue ;
				}
			}

			return commaPositions ;
		}

		protected List<string> TidyUpFields( List<string> untidy )
		{
			List<string> tidy = new List<string>() ;

			foreach( string field in untidy )
			{
				string str = RemoveTrailingComma( field ) ;
				str = RemoveTrailingQuote( str ) ;
				str = RemoveLeadingQuote( str ) ;
				
				tidy.Add( str ) ;
			}

			return tidy ;
		}

		protected string RemoveTrailingComma( string str )
		{
			if ( str == null )
				return "" ;

			if ( ! str.Contains( this._delimiter.ToString() ) )
				return str ;

			if ( ! str.Trim().EndsWith( this._delimiter.ToString() ) )
				return str ;

			int pos = str.LastIndexOf( this._delimiter ) ;

			if ( pos < 1 )
				return "" ;

			return str.Substring( 0, pos ) ;
		}

		protected string RemoveTrailingQuote( string str )
		{
			if ( str == null )
				return "" ;

			if ( ! str.Contains( this._delimiter2.ToString() ) )
				return str ;

			if ( ! str.Trim().EndsWith( this._delimiter2.ToString() ) )
				return str ;

			int pos = str.LastIndexOf( this._delimiter2 ) ;

			if ( pos < 1 )
				return "" ;

			return str.Substring( 0, pos ) ;
		}

		protected string RemoveLeadingQuote( string str )
		{
			if ( str == null )
				return "" ;

			if ( ! str.Contains( this._delimiter2.ToString() ) )
				return str ;

			if ( ! str.Trim().StartsWith( this._delimiter2.ToString() ) )
				return str ;

			int pos = str.IndexOf( this._delimiter2 ) ;

			if ( pos + 1 > str.Length - 1 )
				return "" ;

			return str.Substring( pos + 1 ) ;
		}

	} // end class

}