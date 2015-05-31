using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO ;

namespace alby.core
{
	public abstract class DelimitedValuesParserBase
	{
		public const char COMMA		= ',' ;
		public const char FATBAR	= '|' ;
		public const char QUOTE		= '"' ;

		#region state

		protected bool		_trim		= false ;
		protected Encoding	_enc		= Encoding.UTF8 ;
		protected char		_delimiter	= COMMA ;

		#endregion

		#region properties

		public bool Trim
		{
			get
			{
				return _trim ;
			}
			set
			{
				_trim = value ;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return _enc ;
			}
			set
			{
				_enc = value ;
			}
		}

		public char Delimiter
		{
			get
			{
				return _delimiter ;
			}
			set
			{
				_delimiter = value ;
			}
		}

		#endregion

		public string[] ParseString( string aString )
		{
			return this.StringToFields( aString ) ;
		}

		public List< string[] > Parse( string filename )
		{
			using ( FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				return this.Parse( fs ) ;
		}

		public List< string[] > Parse( Stream stream )
		{
			List< string[] > list = new List<string[]> () ;

			using ( StreamReader sr = new StreamReader( stream, this.Encoding ) )
			{
				while ( true )
				{
					string line = sr.ReadLine() ; 
					if ( line == null ) break ; // eof

					if ( line.Trim().Length == 0 ) continue ; // skip blank lines

					string[] fieldArray = this.StringToFields( line ) ;
					if ( fieldArray != null )
						list.Add( fieldArray ) ;
				}
			}
			return list ;
		}
		
		//
		// over ride me in derived classes
		//
		protected abstract string[] StringToFields( string aString ) ;
		
	}
}
