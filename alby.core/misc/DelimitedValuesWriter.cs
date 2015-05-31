// Delimiter		,
// Delimiter2		"
// UseDelimiter2OnlyWhenNecessary	true [only use 2 if 1 found in field]
//


using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;

namespace alby.core
{
	public class DelimitedValuesWriter  
	{
		public const string COMMA	= "," ;
		public const string FATBAR	= "|" ;
		public const string QUOTE	= "\"" ;

		#region state

		protected bool		_trim		= false ;
		protected Encoding	_enc		= Encoding.UTF8 ;
		protected string	_delimiter	= COMMA ;
		protected string	_delimiter2	= QUOTE ;
		protected bool		_useDelimter2OnlyWhenNecessary = true ;

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

		public string Delimiter
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

		public string Delimiter2
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

		public bool Delimter2UseOnlyWhenNecessary
		{
			get
			{
				return _useDelimter2OnlyWhenNecessary ;
			}
			set
			{
				_useDelimter2OnlyWhenNecessary = value ;
			}
		}

		#endregion

		public string FieldsToString( string[] fields )
		{
			string final = "" ;

			if ( fields == null ) 
				return null ;

			if ( fields.Length == 0 ) 
				return null ;

			for ( int i = 0 ; i < fields.Length ; i++ )
			{
				string s = fields[i] ?? "" ;
				if ( this.Trim )
					s = s.Trim() ;

				if ( this.Delimter2UseOnlyWhenNecessary )
				{
					if ( s.Contains( this.Delimiter ) )
						s = this.Delimiter2 + s + this.Delimiter2 ;
				}
				else
					s = this.Delimiter2 + s + this.Delimiter2 ;

				final += s ;
				
				if ( i + 1 < fields.Length ) // no delimiter on last field
					final += this.Delimiter ;
			}

			return final ;
		}

		public void WriteFile( string filename, List< string[] > list )
		{			
			if ( list == null || list.Count == 0 )
			{
				using ( FileStream fs = File.Create( filename ) )
					return ;
			}

			List<string> lines = new List<string>() ;

			foreach ( string[] fields in list ) 
			{
				string line = this.FieldsToString( fields ) ;
				if ( line != null )
					lines.Add( line ) ;
			}

			if ( lines.Count == 0 )
			{
				using ( FileStream fs = File.Create( filename ) )
					return ;
			}

			File.WriteAllLines( filename, lines, this.Encoding ) ; 
		} 

	}

}