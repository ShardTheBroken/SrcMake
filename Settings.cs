// Settings.cs //

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SrcMake
{
	public static class StringTools
	{
		/// <summary>
		///   Removes all whitespace in a string that is not contained in double quotes.
		/// </summary>
		/// <param name="str">
		///   The string.
		/// </param>
		/// <returns>
		///   The given string with all unquoted whitespace removed.
		/// </returns>
		public static string RemoveUnquotedWhitespace( string str )
		{
			if( string.IsNullOrWhiteSpace( str ) )
				return string.Empty;

			string s = str.Trim();
			string result = string.Empty;

			for( int i = 0; i < s.Length; i++ )
			{
				if( s[ i ] == '"' && i < s.Length - 1 )
				{
					int q = s.IndexOf( '"', i + 1 );

					if( q > 0 )
					{
						result += s.Substring( i, ( q - i ) + 1 );
						i = q;
					}
					else
						result += s[ i ];
				}
				else if( !char.IsWhiteSpace( s[ i ] ) )
					result += s[ i ];
			}

			return result;
		}
		/// <summary>
		///   Removes the surrounding double quotes from a string.
		/// </summary>
		/// <param name="str">
		///   The string.
		/// </param>
		/// <returns>
		///   The given string with the surrounding quotes removed.
		/// </returns>
		public static string RemoveSurroundingQuotes( string str )
		{
			int len = str.Length;

			if( str == null || len == 1 || str.IndexOf( '"' ) < 0 )
				return str;

			if( len >= 2 && str[0] == '"' && str[ len - 1 ] == '"' )
				str = len == 2 ? string.Empty : str.Substring( 1, len - 2 );

			return str;
		}
	}
	
	public class Settings
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		public Settings()
		{
			Author  = string.Empty;
			Email   = string.Empty;
			LineEnd = "windows";
		}
		/// <summary>
		///   Deep copy constructor.
		/// </summary>
		/// <param name="s">
		///   The object to deep copy.
		/// </param>
		public Settings( Settings s )
		{
			Author  = new string( s.Author.ToCharArray() );
			Email   = new string( s.Email.ToCharArray() );
			LineEnd = new string( s.LineEnd.ToCharArray() );
		}

		/// <summary>
		///   The author.
		/// </summary>
		public string Author
		{
			get { return m_author; }
			set { m_author = string.IsNullOrWhiteSpace( value ) ? string.Empty : StringTools.RemoveSurroundingQuotes( value ).Trim(); }
		}
		/// <summary>
		///   The authors' contact email.
		/// </summary>
		public string Email
		{
			get { return m_email; }
			set { m_email = string.IsNullOrWhiteSpace( value ) ? string.Empty : StringTools.RemoveSurroundingQuotes( value ).Trim().ToLower(); }
		}
		/// <summary>
		///   Line ending type used when generating files ("windows" or "unix").
		/// </summary>
		public string LineEnd
		{
			get { return m_lineend; }
			set
			{
				bool invalid = string.IsNullOrWhiteSpace( value );

				string v = invalid ? "windows" : StringTools.RemoveSurroundingQuotes( value ).Trim().ToLower();

				if( !invalid && v != "windows" && v != "unix" )
					invalid = true;

				m_lineend = invalid ? "windows" : v;
			}
		}

		/// <summary>
		///   Loads settings from the file at the given path.
		/// </summary>
		/// <param name="path">
		///   The path of the file to load.
		/// </param>
		/// <returns>
		///   True if settings were loaded successfully, otherwise false.
		/// </returns>
		public bool LoadFromFile( string path )
		{
			List<string> lines = new List<string>();
			{
				try
				{
					string[] ls = File.ReadAllLines( path );

					foreach( string l in ls )
					{
						string line = StringTools.RemoveUnquotedWhitespace( l );

						if( line.Length == 0 || line[ 0 ] == ';' || line[ 0 ] == '#' )
							continue;

						lines.Add( line );
					}
				}
				catch
				{
					return false;
				}
			}

			if( lines[ 0 ].ToLower() != "[user]" )
			{
				Console.WriteLine( "Settings file does not start with [User] header." );
				return false;
			}

			for( int i = 1; i < lines.Count; i++ )
			{
				if( lines[i].Length >= 7 && lines[i].Substring( 0, 7 ).ToLower() == "author=" )
				{
					if( lines[ i ].Length == 7 )
						Author = string.Empty;
					else
					{
						Author = lines[ i ].Substring( 7 );

						if( Author.Length > 2 && Author[ 0 ] == '"' && Author[ Author.Length - 1 ] == '"' )
							Author = Author.Substring( 1, Author.Length - 2 );
					}
				}
				else if( lines[ i ].Length >= 6 && lines[ i ].Substring( 0, 6 ).ToLower() == "email=" )
				{
					if( lines[ i ].Length == 6 )
						Email = string.Empty;
					else
					{
						Email = lines[ i ].Substring( 6 );

						if( Email.Length > 2 && Email[ 0 ] == '"' && Email[ Email.Length - 1 ] == '"' )
							Email = Email.Substring( 1, Email.Length - 2 );
					}
				}
				else if( lines[ i ].Length >= 8 && lines[ i ].Substring( 0, 8 ).ToLower() == "lineend=" )
				{
					if( lines[ i ].Length == 8 )
						LineEnd = string.Empty;
					else
					{
						LineEnd = lines[ i ].Substring( 8 );

						if( LineEnd.Length > 2 && LineEnd[ 0 ] == '"' && LineEnd[ LineEnd.Length - 1 ] == '"' )
							LineEnd = LineEnd.Substring( 1, LineEnd.Length - 2 );
					}
				}
			}

			return true;
		}
		/// <summary>
		///   Saves settings from the file at the given path.
		/// </summary>
		/// <param name="path">
		///   The path of the file to save.
		/// </param>
		/// <returns>
		///   True if settings were saved successfully, otherwise false.
		/// </returns>
		public bool SaveToFile( string path )
		{
			StringBuilder str = new StringBuilder();

			str.Append( "#\n# SrcMake User Settings\n#\n\n" );

			str.Append( "[User]\n" );
			str.Append( "author  = \"" );
			str.Append( Author );
			str.Append( "\"\n" );

			str.Append( "email   = \"" );
			str.Append( Email );
			str.Append( "\"\n" );

			str.Append( "lineend = \"" );
			str.Append( LineEnd );
			str.Append( "\"\n" );

			if( LineEnd.ToLower() == "windows" )
				str = str.Replace( "\n", "\r\n" );

			try
			{
				File.WriteAllText( path, str.ToString() );
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		///   Constructs a new object by loading from file.
		/// </summary>
		/// <param name="path">
		///   The path of the file to load.
		/// </param>
		/// <returns>
		///   A valid object on success or null on failure.
		/// </returns>
		public static Settings FromFile( string path )
		{
			Settings s = new Settings();

			if( !s.LoadFromFile( path ) )
				return null;

			return s;
		}

		private string m_author,
		               m_email,
		               m_lineend;
	}
}
