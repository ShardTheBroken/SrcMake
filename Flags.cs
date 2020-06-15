// Flags.cs //

using System;
using System.Linq;

namespace SrcMake
{
	/// <summary>
	///   Languages SrcMake can generate in.
	/// </summary>
	public enum Language
	{
		C,
		Cpp,
		CSharp,
		Rust
	}
	/// <summary>
	///   File types SrcMake can generate.
	/// </summary>
	public enum FileType
	{
		// Universal
		Main,
		Struct,

		// C#
		Class,
		Interface,
		Singleton,
		MonoBehaviour,
		ScriptableObject,

		// C
		Header,
		Source,

		// C++
		ClassHeader,
		ClassSource,
		Template,
		TemplateHeader,
		TemplateSource,
		SingletonHeader,
		SingletonSource,
		Singleton03,
		Singleton03Header,
		Singleton03Source,

		// Rust
		Lib,
		Trait
	}

	public static class Flags
	{
		private const string Numbers = "0123456789";

		/// <summary>
		///   The file extention used by template files.
		/// </summary>
		public const string TemplateExt = "src";

		/// <summary>
		///   Checks if a string is a valid name.
		/// </summary>
		/// <param name="name">
		///   The string to check.
		/// </param>
		/// <returns>
		///   True if the given string is a valid name and false otherwise.
		/// </returns>
		public static bool IsValidName( string name )
		{
			if( string.IsNullOrWhiteSpace( name ) || ( !char.IsLetter( name[ 0 ] ) && name[ 0 ] != '_' ) )
				return false;

			for( int i = 1; i < name.Length; i++ )
				if( !char.IsLetter( name[ i ] ) && name[ i ] != '_' && !Numbers.Contains( name[ i ] ) )
					return false;

			return true;
		}

		/// <summary>
		///   Checks if a FileType is compatible with a Language.
		/// </summary>
		/// <param name="l">
		///   The language.
		/// </param>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   True if the given file type is compatible with the given language and false otherwise.
		/// </returns>
		public static bool IsCompatible( Language l, FileType f )
		{
			int lang = (int)l,
			    file = (int)f;

			if( lang < 0 || lang >= Enum.GetNames( typeof( Language ) ).Length ||
				file < 0 || file >= Enum.GetNames( typeof( FileType ) ).Length )
				return false;

			if( f == FileType.Main || f == FileType.Struct )
				return true;

			switch( l )
			{
				case Language.C:
					return f == FileType.Header || f == FileType.Source;
				case Language.Cpp:
					return f == FileType.Header || f == FileType.Source || f == FileType.Class ||
						   f == FileType.ClassHeader || f == FileType.ClassSource || f == FileType.Template ||
						   f == FileType.TemplateHeader || f == FileType.TemplateSource || f == FileType.Singleton ||
						   f == FileType.SingletonHeader || f == FileType.SingletonSource || f == FileType.Singleton03 ||
						   f == FileType.Singleton03Header || f == FileType.Singleton03Source;
				case Language.CSharp:
					return f == FileType.Class || f == FileType.Interface || f == FileType.Singleton ||
						   f == FileType.MonoBehaviour || f == FileType.ScriptableObject;
				case Language.Rust:
					return f == FileType.Lib || f == FileType.Trait;
			}

			return false;
		}
		/// <summary>
		///   If a FileType is a proxy type in the given language and thus needs extra processing.
		/// </summary>
		/// <param name="l">
		///   The language.
		/// </param>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   True if the given file type is a proxy type in the given language.
		/// </returns>
		public static bool IsProxy( Language l, FileType f )
		{
			if( l != Language.Cpp )
				return false;

			return f == FileType.Class || f == FileType.Singleton || f == FileType.Singleton03 || f == FileType.Template;
		}
		/// <summary>
		///   If a FileType is a partial type in the given language.
		/// </summary>
		/// <param name="l">
		///   The language.
		/// </param>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   True if the given file type is a partial type in the given language.
		/// </returns>
		public static bool IsPartial( Language l, FileType f )
		{
			if( l != Language.Cpp )
				return false;

			return f == FileType.ClassHeader       || f == FileType.ClassSource ||
			       f == FileType.SingletonHeader   || f == FileType.SingletonSource ||
				   f == FileType.Singleton03Header || f == FileType.Singleton03Source ||
				   f == FileType.TemplateHeader    || f == FileType.TemplateSource;
		}
		/// <summary>
		///   If a FileType is concidered a header file in C/C++.
		/// </summary>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   True if the given file type is a header file and false otherwise.
		/// </returns>
		public static bool IsCHeader( FileType f )
		{
			return f == FileType.Header || f == FileType.SingletonHeader   || f == FileType.ClassHeader || 
			       f == FileType.Struct || f == FileType.Singleton03Header || f == FileType.TemplateHeader;
		}
		/// <summary>
		///   If a FileType is concidered a source file in C/C++.
		/// </summary>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   True if the given file type is a source file and false otherwise.
		/// </returns>
		public static bool IsCSource( FileType f )
		{
			return f == FileType.Source || f == FileType.SingletonSource   || f == FileType.ClassSource ||
				   f == FileType.Main   || f == FileType.Singleton03Source || f == FileType.TemplateSource;
		}

		/// <summary>
		///   Returns the file types that need generated based on a Language and FileType, resolving proxy types.
		/// </summary>
		/// <param name="l">
		///   The language.
		/// </param>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   The file types that need generated based on the given language and file type or null if invalid.
		/// </returns>
		public static FileType[] GetTypes( Language l, FileType f )
		{
			bool proxy = IsProxy( l, f );

			FileType[] types = new FileType[ !proxy ? 1 : 2 ];

			if( proxy )
			{
				switch( f )
				{
					case FileType.Class:
						types[ 0 ] = FileType.ClassHeader;
						types[ 1 ] = FileType.ClassSource;
						break;
					case FileType.Singleton:
						types[ 0 ] = FileType.SingletonHeader;
						types[ 1 ] = FileType.SingletonSource;
						break;
					case FileType.Singleton03:
						types[ 0 ] = FileType.Singleton03Header;
						types[ 1 ] = FileType.Singleton03Source;
						break;
					case FileType.Template:
						types[ 0 ] = FileType.TemplateHeader;
						types[ 1 ] = FileType.TemplateSource;
						break;
					default:
						Console.WriteLine( "Unimplemented proxy FileType given." );
						return null;
				}
			}
			else
				types[ 0 ] = f;

			return types;
		}
		/// <summary>
		///   Returns the file extention for the Language and FileType.
		/// </summary>
		/// <param name="l">
		///   The language.
		/// </param>
		/// <param name="f">
		///   The file type.
		/// </param>
		/// <returns>
		///   The file extention for the given language and file type or null if they are invalid.
		/// </returns>
		public static string GetFileExtention( Language l, FileType f )
		{
			if( l == Language.CSharp )
				return "cs";
			else if( l == Language.Rust )
				return "rs";
			else if( l == Language.Cpp && f == FileType.TemplateSource )
				return "inl";
			else if( IsCHeader( f ) )
				return l == Language.C ? "h" : "hpp";
			else if( IsCSource( f ) )
				return l == Language.C ? "c" : "cpp";

			return null;
		}
		/// <summary>
		///   Returns a header guard to be used in C/C++ files or null if invalid.
		/// </summary>
		/// <param name="l">
		///   The language.
		/// </param>
		/// <param name="name">
		///   The file name.
		/// </param>
		/// <returns>
		///   A header guard to be used in the file.
		/// </returns>
		public static string GetHeaderGuard( Language l, string name )
		{
			if( l != Language.C && l != Language.Cpp || !IsValidName( name ) )
				return null;

			string n     = name.Trim();
			string guard = string.Empty;

			bool lowerlast = false;

			for( int i = 0; i < n.Length; i++ )
			{
				if( char.IsLetter( n[i] ) )
				{
					if( lowerlast && char.IsUpper( n[i] ) )
						guard += "_" + n[ i ];
					else
						guard += char.ToUpper( n[ i ] );

					lowerlast = char.IsLower( n[ i ] );
				}
				else
				{
					guard += n[ i ];
					lowerlast = false;
				}
			}

			if( l == Language.C )
				guard += "_H";
			else
				guard += "_HPP";

			return guard;
		}
	}
}
