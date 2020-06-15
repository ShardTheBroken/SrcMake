// GeneratorInfo.cs //

using System.Collections.Generic;

namespace SrcMake
{
	public class GeneratorInfo
	{
		public GeneratorInfo()
		{
			Language = 0;
			FileType = 0;
			Name     = string.Empty;
			Settings = new Settings();
			Args     = new List<string>();
		}
		public GeneratorInfo( GeneratorInfo g )
		{
			Language = g.Language;
			FileType = g.FileType;
			Name     = new string( g.Name.ToCharArray() );
			Settings = new Settings( g.Settings );
			Args     = new List<string>( g.Args.Count );

			foreach( string s in g.Args )
				Args.Add( new string( s.ToCharArray() ) );
		}
		public GeneratorInfo( Language lang, FileType file, string name, params string[] args )
		{
			Language = lang;
			FileType = file;
			Name     = name;
			Settings = new Settings();
			Args     = new List<string>();

			foreach( string s in args )
				Args.Add( s );
		}

		public Language Language
		{
			get; set;
		}
		public FileType FileType
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public List<string> Args
		{
			get; set;
		}

		public Settings Settings
		{
			get; set;
		}

		public bool IsValid()
		{
			return Flags.IsCompatible( Language, FileType ) && Flags.IsValidName( Name );
		}
	}
}
