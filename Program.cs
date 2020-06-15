// Program.cs //

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SrcMake
{
	public static class Version
	{
		public const int Major = 0;
		public const int Minor = 1;
		public const int Patch = 0;
		public const int Build = 1;

		public static readonly string String = Major.ToString() + "." + Minor.ToString() + "." +
											   Patch.ToString() + "." + Build.ToString();
	}
	public static class Help
	{
		public const string Usage =
			"SrcMake usage:\n" +
			"SrcMake [language] [file type] [name] ([arguments])\n" +
			"SrcMake set [setting name] [value]\n" +
			"SrcMake reset [setting name]";

		public const string Language =
			"The language flag determines which language to generate files for.\nPossible values:\n" +
			"c           - Generate files for the C language.\n" +
			"cpp         - Generate files for the C++ language.\n" +
			"csharp | c# - Generate files for the C# language.\n" +
			"rust        - Generate files for the Rust language.";

		public const string FileType =
			"The file type flag determines which kind(s) of file(s) to generate.\nPossible values:\n" +
			"main             - Generate a main file.\n" +
			"struct           - Generate a struct.\n" +
			"header           - Generate a C/C++ header file.\n" +
			"source           - Generate a C/C++ source file.\n" +
			"class            - Generate a C++/C# class.\n" +
			"singleton        - Generate a C++11/C# singleton class.\n" +
			"singleton03      - Generate a pre-C++11 singleton class.\n" +
			"interface        - Generate a C# interface.\n" +
			"monobehaviour    - Generate a C# Unity MonoBehaviour script.\n" +
			"scriptableobject - Generate a C# Unity ScriptableObject script.\n" +
			"lib              - Generate a Rust lib file.\n" +
			"trait            - Generate a Rust trait file.";

		public const string Naming = 
			"The name given must be a valid type name as it used not just for the file name(s), but also class and structure names.";

		public const string Arguments =
			"Arguments can enable/disable certain flags.\nPossible values compatible with all languages:\n" +
			"-a=[value] | -author=[value]  - Sets the author for just the files being generated.\n" +
			"-e=[value] | -email=[value]   - Sets the author email for just the files being generated.\n" +
			"-l=[value] | -lineend=[value] - Sets the line endings for just the files being generated.\n" + 
			"Possible values compatible with C/C++/C#:\n" +
			"-ns=[value] | -namespace=[value] - Sets the enclosing namespace.\n" +
			"Possible values compatible with C/C++:\n" +
			"-i=[value]   | -include=[value] - Adds an include file.\n" +
			"Possible values compatible with C++:\n" +
			"-v            | -virtual           - Makes the generated class virtual.\n" +
			"-pub=[value]  | -public=[value]    - Adds a class for public inheritance.\n" +
			"-prot=[value] | -protected=[value] - Adds a class for protected inheritance.\n" +
			"-priv=[value] | -private=[value]   - Adds a class for private inheritance.\n" +
			"Possible values compatible with C#:\n" +
			"-u=[value]  | -using=[value]   - Adds value to the using statement.\n" +
			"-ac=[value] | -access=[value]  - Sets the access level of the generated structure.\n" +
			"-pr         | -partial         - Makes the generated class partial.\n" +
			"-ab         | -abstract        - Makes the generated class abstract.\n" +
			"-se         | -sealed          - Makes the generated class sealed.\n" +
			"-st         | -static          - Makes the generated class static.\n" +
			"-i=[value]  | -inherit=[value] - Adds a class/interface the structure should inherit from.";

		public const string Settings = 
			"Possible settings values:\n" + 
			"author  = \"<author name>\"  - Sets the default author.\n" +
			"email   = \"<author email>\" - Sets the default author contact email.\n" +
			"lineend = \"<windows|unix>\" - Sets the default file line endings.";
	}


	class Program
	{
		// Argument indicies.
		public const int LangArg = 0;
		public const int FileArg = 1;
		public const int NameArg = 2;
		public const int FlagArg = 3;
		//
		public const int FuncArg = 0;
		public const int SettArg = 1;
		public const int ValArg  = 2;
		// Minimum argument count.
		public const int MinArgs    = 2;
		public const int MinGenArgs = 3;

		public static string MainFolder
		{
			get
			{
				string path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase );

				try
				{
					string s5  = path.Substring( 0,  5 ).ToLower();
					string s6  = path.Substring( 0,  6 ).ToLower();
					string s8  = path.Substring( 0,  8 ).ToLower();
					string s10 = path.Substring( 0, 10 ).ToLower();

					if( s5 == "dir:/" || s5 == "dir:\\" )
						path = path.Substring( 5 );
					else if( s6 == "file:/" || s6 == "file:\\" || s6 == "path:/" || s6 == "path:\\" )
						path = path.Substring( 6 );
					else if( s8 == "folder:/" || s8 == "folder:\\" )
						path = path.Substring( 8 );
					else if( s10 == "directory:/" || s10 == "directory:\\" )
						path = path.Substring( 10 );
				}
				catch( Exception e )
				{
					Console.WriteLine( e.Message );
				}

				return path.Replace( '/', '\\' );
			}
		}
		public static readonly string TemplateFolder = MainFolder + "\\templates\\";
		public static readonly string SettingsPath   = MainFolder + "\\settings.ini";

		static void Main( string[] args )
		{
			Console.WriteLine( "SrcMake version " + Version.String + " by Michael Furlong." );

			if( args.Length < MinArgs )
			{
				Console.WriteLine( "Invalid argument count." );
				Console.WriteLine( Help.Usage );
				return;
			}

			string funcstr = args[ FuncArg ].Trim().ToLower();

			if( funcstr == "set" )
			{
				if( args.Length != 3 )
				{
					Console.WriteLine( "Invalid argument count." );
					Console.WriteLine( Help.Usage );
					return;
				}

				if( !SetSetting( args[ SettArg ], args[ ValArg ] ) )
				{
					Console.WriteLine( "Unabke to set setting." );
					Console.WriteLine( Help.Usage );
					Console.WriteLine( Help.Settings );
					return;
				}

				return;
			}
			else if( funcstr == "reset" )
			{
				if( args.Length != 2 )
				{
					Console.WriteLine( "Invalid argument count." );
					Console.WriteLine( Help.Usage );
					return;
				}

				if( !ResetSetting( args[ SettArg ] ) )
				{
					Console.WriteLine( "Unabke to reset setting." );
					Console.WriteLine( Help.Usage );
					Console.WriteLine( Help.Settings );
					return;
				}

				return;
			}

			string langstr = args[ LangArg ].ToLower(),
			       filestr = args[ FileArg ].ToLower(),
				   name    = args[ NameArg ];

			GeneratorInfo info = new GeneratorInfo();

			if( !File.Exists( SettingsPath ) )
				info.Settings.SaveToFile( SettingsPath );
			else if( !info.Settings.LoadFromFile( SettingsPath ) )
				Console.WriteLine( "Unable to load user settings. Defaults will be used." );

			// Language flag
			{
				Language lang = (Language)( -1 );

				if( langstr == "c#" )
					lang = Language.CSharp;
				else
				{
					for( Language l = 0; (int)l < Enum.GetNames( typeof( Language ) ).Length; l++ )
					{
						if( langstr == Enum.GetName( typeof( Language ), l ).ToLower() )
						{
							lang = l;
							break;
						}
					}

					if( (int)lang == -1 )
					{
						Console.WriteLine( "Invalid language argument." );
						Console.WriteLine( Help.Language );
						return;
					}
				}

				info.Language = lang;
			}
			// FileType flag
			{
				FileType file = (FileType)( -1 );

				for( FileType f = 0; (int)f < Enum.GetNames( typeof( FileType ) ).Length; f++ )
				{
					if( filestr == Enum.GetName( typeof( FileType ), f ).ToLower() )
					{
						file = f;
						break;
					}
				}

				if( (int)file == -1 )
				{
					Console.WriteLine( "Invalid file type argument." );
					Console.WriteLine( Help.FileType );
					return;
				}

				info.FileType = file;
			}

			if( !Flags.IsValidName( name ) )
			{
				Console.WriteLine( "Invalid name argument." );
				Console.WriteLine( Help.Naming );
				return;
			}

			info.Name = name;

			if( args.Length > MinGenArgs )
				for( int i = MinGenArgs + 1; i < args.Length; i++ )
					info.Args.Add( StringTools.RemoveUnquotedWhitespace( args[ i ] ) );

			if( !info.IsValid() )
			{
				Console.WriteLine( "Incompatible or invalid arguments given." );
				Console.WriteLine( Help.Arguments );
				return;
			}

			if( !Generate( info ) )
			{
				Console.WriteLine( "Generating failed. Try running as admin and ensuring no files are locked." );
				return;
			}
		}

		static bool Generate( GeneratorInfo g )
		{
			string templatebase = TemplateFolder + Enum.GetName( typeof( Language ), g.Language );

			FileType[] types = Flags.GetTypes( g.Language, g.FileType );

			if( types == null )
				return false;

			foreach( FileType f in types )
			{
				GeneratorInfo info = new GeneratorInfo( g )
				{
					FileType = f
				};

				string template = templatebase + "_" + Enum.GetName( typeof( FileType ), f ) + "." + Flags.TemplateExt;
				string content  = string.Empty;

				try
				{
					content = File.ReadAllText( template );
				}
				catch
				{
					Console.WriteLine( "Unable to load template file \"" + template + "\"." );
					return false;
				}

				content = ReplaceUniversalMacros( content, info );

				if( info.Language == Language.C )
					content = ReplaceCMacros( content, info );
				else if( info.Language == Language.Cpp )
					content = ReplaceCppMacros( ReplaceCMacros( content, info ), info );
				else if( info.Language == Language.CSharp )
					content = ReplaceCSharpMacros( content, info );
				else if( info.Language == Language.Rust )
					content = ReplaceRustMacros( content, info );

				content = CleanupSpacing( content );

				if( info.Settings.LineEnd.ToLower() == "windows" )
					content = content.Replace( "\n", "\r\n" );

				string filepath = info.Name + "." + Flags.GetFileExtention( info.Language, f );

				try
				{
					File.WriteAllText( filepath, content );
				}
				catch
				{
					Console.WriteLine( "Unable to write file \"" + filepath + "\"." );
					return false;
				}
			}

			return true;
		}

		static bool SetSetting( string setting, string val )
		{
			Settings set = new Settings();

			if( !File.Exists( SettingsPath ) )
			{
				if( !set.SaveToFile( SettingsPath ) )
				{
					Console.WriteLine( "Unable to create settings file. Try running as admin?" );
					return false;
				}
			}
			else if( !set.LoadFromFile( SettingsPath ) )
			{
				Console.WriteLine( "Settings file exists but unable to load from it. Try deleting or correcting file?" );
				return false;
			}

			string s = StringTools.RemoveSurroundingQuotes( setting ).Trim().ToLower();
			string v = string.IsNullOrWhiteSpace( val ) ? string.Empty : StringTools.RemoveSurroundingQuotes( val ).Trim();

			if( s == "author" )
				set.Author = v;
			else if( s == "email" )
				set.Email = v;
			else if( s == "lineend" )
				set.LineEnd = v;
			else
				return false;

			return set.SaveToFile( SettingsPath );
		}
		static bool ResetSetting( string setting )
		{
			return SetSetting( setting, null );
		}

		static string ReplaceUniversalMacros( string content, GeneratorInfo g )
		{
			if( string.IsNullOrWhiteSpace( content ) )
				return content;

			DateTime now   = DateTime.Now;
			string author  = g.Settings.Author,
				   email   = g.Settings.Email,
				   lineend = g.Settings.LineEnd;
			{
				foreach( string a in g.Args )
				{
					int len = a.Length;

					string low = a.ToLower();

					if( ( len > 3 && low.Substring( 0, 3 ) == "-a=" ) ||
						( len > 8 && low.Substring( 0, 8 ) == "-author=" ) )
					{
						author = a.Substring( a.IndexOf( '=' ) + 1 );

						if( author.Length > 2 && author[ 0 ] == '"' && author[ author.Length - 1 ] == '"' )
							author = author.Substring( 1, author.Length - 2 );
					}
					else if( ( len > 3 && low.Substring( 0, 3 ) == "-e=" ) ||
					         ( len > 7 && low.Substring( 0, 7 ) == "-email=" ) )
					{
						email = a.Substring( a.IndexOf( '=' ) + 1 );

						if( email.Length > 2 && email[ 0 ] == '"' && email[ email.Length - 1 ] == '"' )
							email = email.Substring( 1, email.Length - 2 );
					}
					else if( ( len > 3 && low.Substring( 0, 3 ) == "-l=" ) ||
							 ( len > 9 && low.Substring( 0, 9 ) == "-lineend=" ) )
					{
						lineend = low.Substring( a.IndexOf( '=' ) + 1 );

						if( lineend.Length > 2 && lineend[ 0 ] == '"' && lineend[ lineend.Length - 1 ] == '"' )
							lineend = lineend.Substring( 1, lineend.Length - 2 );
					}
				}
			}

			content = content.Replace( "$NAME$",      g.Name )
			                 .Replace( "$FILE_EXT$",  Flags.GetFileExtention( g.Language, g.FileType ) )
							 .Replace( "$FILE_NAME$", g.Name + "." + Flags.GetFileExtention( g.Language, g.FileType ) )
							 .Replace( "$DATE$",      now.ToShortDateString() )
							 .Replace( "$YEAR$",      now.Year.ToString() )
							 .Replace( "$MONTH$",     now.ToString( "MMMM", CultureInfo.InvariantCulture ) )
							 .Replace( "$TIME$",      now.ToShortTimeString() )
							 .Replace( "$DATETIME$",  now.ToString() )
							 .Replace( "$AUTHOR$",    author )
							 .Replace( "$EMAIL$",     email );

			return content;
		}
		static string ReplaceCMacros( string content, GeneratorInfo g )
		{
			if( string.IsNullOrWhiteSpace( content ) )
				return content;

			string ns = string.Empty;
			List<string> include = new List<string>();

			foreach( string s in g.Args )
			{
				if( string.IsNullOrWhiteSpace( s ) )
					continue;

				int len = s.Length;

				if( ( len > 4  && s.Substring( 0,  4 ).ToLower() == "-ns=" ) ||
					( len > 11 && s.Substring( 0, 11 ).ToLower() == "-namespace=" ) )
				{
					ns = s.Substring( s.IndexOf( '=' ) + 1 ).Trim();

					if( ns.Length > 2 && ns[ 0 ] == '"' && ns[ ns.Length - 1 ] == '"' )
						ns = ns.Substring( 1, ns.Length - 2 );
				}
				else if( ( len > 3 && s.Substring( 0, 3 ).ToLower() == "-i=" ) ||
				         ( len > 9 && s.Substring( 0, 9 ).ToLower() == "-include=" ) )
				{
					string i = s.Substring( s.IndexOf( '=' ) + 1 ).Trim();

					if( i.Length > 2 && i[ 0 ] == '"' && i[ ns.Length - 1 ] == '"' )
						i = i.Substring( 1, ns.Length - 2 );

					include.Add( i );
				}
			}

			content = content.Replace( "$HEADER_GUARD$", Flags.GetHeaderGuard( g.Language, g.Name ) );
			content = content.Replace( "$HEADER_EXT$",   Flags.GetFileExtention( g.Language, FileType.Header ) );
			content = content.Replace( "$SOURCE_EXT$",   Flags.GetFileExtention( g.Language, FileType.Source ) );

			if( !string.IsNullOrWhiteSpace( ns ) )
				content = content.Replace( "$NAMESPACE_BEGIN$", "namespace " + ns + "\n{" )
				                 .Replace( "$NAMESPACE_END$", "}" );
			else
				content = content.Replace( "$NAMESPACE_BEGIN$", string.Empty )
				                 .Replace( "$NAMESPACE_END$",   string.Empty );

			if( include.Count > 0 )
			{
				string inc = string.Empty;

				foreach( string i in include )
					inc += "#include<" + i + ">\n";

				content = content.Replace( "$INCLUDES$", inc );
			}
			else
				content = content.Replace( "$INCLUDES$", string.Empty );

			return content;
		}
		static string ReplaceCppMacros( string content, GeneratorInfo g )
		{
			if( string.IsNullOrWhiteSpace( content ) )
				return content;

			bool virt = false;
			List<string> pub  = new List<string>(),
			             prot = new List<string>(),
						 priv = new List<string>();

			foreach( string a in g.Args )
			{
				int len = a.Length;

				if( ( len == 2 && a.ToLower() == "-v" ) || ( len == 8 && a.ToLower() == "-virtual" ) )
					virt = true;
				else if( ( len > 5 && a.Substring( 0, 5 ).ToLower() == "-pub=" ) ||
				         ( len > 8 && a.Substring( 0, 8 ).ToLower() == "-public=" ) )
				{
					string s = a.Substring( a.IndexOf( '=' ) + 1 ).Trim();

					if( s.Length > 2 && s[ 0 ] == '"' && s[ s.Length - 1 ] == '"' )
						s = s.Substring( 1, s.Length - 2 );

					pub.Add( s );
				}
				else if( ( len > 6  && a.Substring( 0,  6 ).ToLower() == "-prot=" ) ||
						 ( len > 11 && a.Substring( 0, 11 ).ToLower() == "-protected=" ) )
				{
					string s = a.Substring( a.IndexOf( '=' ) + 1 ).Trim();

					if( s.Length > 2 && s[ 0 ] == '"' && s[ s.Length - 1 ] == '"' )
						s = s.Substring( 1, s.Length - 2 );

					prot.Add( s );
				}
				else if( ( len > 6  && a.Substring( 0, 6 ).ToLower() == "-priv=" ) ||
						 ( len > 11 && a.Substring( 0, 9 ).ToLower() == "-private=" ) )
				{
					string s = a.Substring( a.IndexOf( '=' ) + 1 ).Trim();

					if( s.Length > 2 && s[ 0 ] == '"' && s[ s.Length - 1 ] == '"' )
						s = s.Substring( 1, s.Length - 2 );

					priv.Add( s );
				}
			}

			bool hasinherit = pub.Count > 0 || prot.Count > 0 || priv.Count > 0;

			string inherit = hasinherit ? ":" : "";

			foreach( string p in pub )
				inherit += " public " + p + ",";
			foreach( string p in prot )
				inherit += " protected " + p + ",";
			foreach( string p in priv )
				inherit += " private " + p + ",";

			if( !hasinherit )
				content = content.Replace( ": $INHERITANCE$", "" ).Replace( ":$INHERITANCE$", "" ).Replace( "$INHERITANCE$", "" );
			else
			{
				// Remove excess comma.
				inherit = inherit.Substring( 0, inherit.Length - 1 );
				content = content.Replace( "$INHERITANCE$", inherit );
			}

			content = content.Replace( "$VIRTUAL$", virt ? "virtual" : string.Empty )
			                 .Replace( "$INLINE_EXT$", "inl" );

			return content;
		}
		static string ReplaceCSharpMacros( string content, GeneratorInfo g )
		{
			if( string.IsNullOrWhiteSpace( content ) )
				return content;

			string ns     = string.Empty;
			string access = string.Empty;
			bool   abst   = false,
			       seal   = false,
				   part   = false,
				   stat   = false;

			List<string> usings   = new List<string>(),
			             inherits = new List<string>();

			foreach( string a in g.Args )
			{
				int len = a.Length;

				if( ( len == 3 && a.ToLower() == "-pr" ) || ( len == 7 && a.ToLower() == "-partial" ) )
					part = true;
				else if( ( len == 3 && a.ToLower() == "-ab" ) || ( len == 9 && a.ToLower() == "-abstract" ) )
				{
					abst = true;
					seal = false;
					stat = false;
				}
				else if( ( len == 3 && a.ToLower() == "-se" ) || ( len == 7 && a.ToLower() == "-sealed" ) )
				{
					abst = false;
					seal = true;
					stat = false;
				}
				else if( ( len == 3 && a.ToLower() == "-st" ) || ( len == 7 && a.ToLower() == "-static" ) )
				{
					abst = false;
					seal = false;
					stat = true;
				}
				else if( ( len > 4 && a.Substring( 0, 4 ).ToLower() == "-ac=" ) ||
						 ( len > 8 && a.Substring( 0, 8 ).ToLower() == "-access=" ) )
				{
					access = a.Substring( a.IndexOf( '=' ) + 1 ).Trim().ToLower();

					if( access.Length > 2 && access[ 0 ] == '"' && access[ access.Length - 1 ] == '"' )
						access = access.Substring( 1, access.Length - 2 );
				}
				else if( ( len > 4 && a.Substring( 0, 4 ).ToLower() == "-ns=" ) ||
						 ( len > 11 && a.Substring( 0, 11 ).ToLower() == "-namespace=" ) )
				{
					ns = a.Substring( a.IndexOf( '=' ) + 1 ).Trim();

					if( ns.Length > 2 && ns[ 0 ] == '"' && ns[ ns.Length - 1 ] == '"' )
						ns = ns.Substring( 1, ns.Length - 2 );
				}
				else if( ( len > 3 && a.Substring( 0, 3 ).ToLower() == "-u=" ) ||
						 ( len > 7 && a.Substring( 0, 7 ).ToLower() == "-using=" ) )
				{
					string s = a.Substring( a.IndexOf( '=' ) + 1 ).Trim();

					if( s.Length > 2 && s[ 0 ] == '"' && s[ s.Length - 1 ] == '"' )
						s = s.Substring( 1, s.Length - 2 );

					usings.Add( s );
				}
				else if( ( len > 3 && a.Substring( 0, 3 ).ToLower() == "-i=" ) ||
						 ( len > 9 && a.Substring( 0, 9 ).ToLower() == "-inherit=" ) )
				{
					string s = a.Substring( a.IndexOf( '=' ) + 1 ).Trim();

					if( s.Length > 2 && s[ 0 ] == '"' && s[ s.Length - 1 ] == '"' )
						s = s.Substring( 1, s.Length - 2 );

					inherits.Add( s );
				}
			}

			

			if( !string.IsNullOrWhiteSpace( ns ) )
			{
				content = content.Replace( "$NAMESPACE_BEGIN$", "namespace " + ns + "\n{" );
				content = content.Replace( "$NAMESPACE_END$", "}" );
			}
			else
			{
				content = content.Replace( "$NAMESPACE_BEGIN$", string.Empty );
				content = content.Replace( "$NAMESPACE_END$", string.Empty );
			}

			if( access == "pub" || access == "public" )
				content = content.Replace( "$ACCESS$", "public" );
			else if( access == "prot" || access == "protected" )
				content = content.Replace( "$ACCESS$", "protected" );
			else if( access == "priv" || access == "private" )
				content = content.Replace( "$ACCESS$", "private" );
			else
				content = content.Replace( "$ACCESS$", string.Empty );

			string classmod = string.Empty;

			if( abst )
				classmod += "abstract ";
			if( seal )
				classmod += "sealed ";
			if( stat )
				classmod += "static ";
			if( part )
				classmod += "partial ";

			classmod = classmod.Trim();

			if( !string.IsNullOrWhiteSpace( classmod ) )
				content = content.Replace( "$CLASS_MODIFIERS$", classmod );
			else
				content = content.Replace( "$CLASS_MODIFIERS$", string.Empty );

			if( usings.Count > 0 )
			{
				string u = string.Empty;

				foreach( string s in usings )
					u += "using " + s + ";\n";

				content = content.Replace( "$USING_STATEMENTS$", u );
			}
			else
				content = content.Replace( "$USING_STATEMENTS$", string.Empty );

			if( inherits.Count > 0 )
			{
				string i = ":";

				foreach( string s in inherits )
					i += " " + s + ",";

				i = i.Substring( 0, i.Length - 1 );
				content = content.Replace( "$INHERITANCE$", i );
			}
			else
				content = content.Replace( "$INHERITANCE$", string.Empty );

			return content;
		}
		static string ReplaceRustMacros( string content, GeneratorInfo g )
		{
			if( string.IsNullOrWhiteSpace( content ) )
				return content;

			return content;
		}

		static string CleanupSpacing( string content )
		{
			if( string.IsNullOrWhiteSpace( content ) )
				return string.Empty;

			content = content.Replace( "\r\n", "\n" );
			content = content.Trim();

			while( content.Contains( "  " ) )
				content = content.Replace( "  ", " " );
			while( content.Contains( " \t" ) )
				content = content.Replace( " \t", "\t" );
			while( content.Contains( "\t " ) )
				content = content.Replace( "\t ", "\t" );
			while( content.Contains( " \n" ) )
				content = content.Replace( " \n", "\n" );
			while( content.Contains( "\n " ) )
				content = content.Replace( "\n ", "\n" );
			while( content.Contains( "\n\n\n" ) )
				content = content.Replace( "\n\n\n", "\n\n" );

			if( content[ content.Length - 1 ] != '\n' )
				content += '\n';

			return content;
		}
	}
}
