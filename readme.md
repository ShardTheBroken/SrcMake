# SrcMake
A simple template-based source code generator.

## Usage
To generate a file:
```
SrcMake [language] [filetype] [name] ([args])
```

To set a persisting user argument:
```
SrcMake set [argument] [value]
```

To reset (remove) a persisting user argument:
```
SrcMake reset [argument]
```

### Language
Possible language flags:
`c`					- Generate C files.
`cpp`				- Generate C++ files.
`c#` | 'csharp'		- Generate C# files.
`rust`				- Generate Rust files.

### File Types
Possible file type flags:

#### Universal
Flags that are compatible with all languages:

`main`				- Main source file.
`struct`			- Struct.

#### C/C++
`header`			- Header file.
`source`			- Source file.

#### C++
`class`				- Header and source files for a class.
`singleton`			- Header and source files for a C++11 singleton class.
`template`			- Header and inline files for a templated class.
`singleton03`		- Header and source files for a pre-C++11 singleton class.

#### C#
`class`				- Class.
`singleton`			- Threadsafe singleton class.
`interface`			- Interface.
`monobehaviour`		- Unity MonoBehaviour script.
`scriptableobject`	- Unity ScriptableObject script.

#### Rust
`lib`				- Lib file.
`trait`				- Trait.

### Name
The given name must be a valid type name. It must start with either a letter or underscore and contain only letters,
numbers and underscores thereafter.

### Arguments

#### Universal
Arguments that are compatible with all languages:

`-a=[value]`  | `-author=[value]`		- Sets the author for just the files being generated.
`-e=[value]`  | `-email=[value]`		- Sets the author email for just the files being generated.
`-l=[value]` | `-lineend=[value]`		- Sets the line endings for just the files being generated.

#### C/C++/C#
`-ns=[value]` | `-namespace=[value]`	- Sets the enclosing namespace.

#### C/C++
`-i=[value]`  | `-include=[value]`		- Adds an include file.

#### C++
`-v`            | `-virtual`			- Makes the generated class virtual.
`-pub=[value]`  | `-public=[value]`		- Adds a class for public inheritance.
`-prot=[value]` | `-protected=[value]`	- Adds a class for protected inheritance.
`-priv=[value]` | `-private=[value]`	- Adds a class for private inheritance.

#### C#
`-u=[value]`    | `-using=[value]`		- Adds value to the using statement.
`-ac=[value]`   | `-access=[value]`		- Sets the access level of the generated structure.
`-pr`           | `-partial`			- Makes the generated class partial.
`-ab`           | `-abstract`			- Makes the generated class abstract.
`-se`           | `-sealed`				- Makes the generated class sealed.
`-st`           | `-static`				- Makes the generated class static.
`-i=[value]`    | `-inherit=[value]`	- Adds a class/interface the structure should inherit from.

## User Settings
User settings can be found in "settings.ini". Please note this file is not parsed properly, comment lines with either
the `;` or `#` character are supported, but inline comments are not be removed and will become part of the keys' value.
Comments will also be removed when setting values with the `set` or `reset` commands.

The first non-comment, non-empty line in this file must be the user header "[User]" or SrcMake will return an error.

### Possible Values
`author=[value]`		- Sets the default author.
`email=[value]`			- Sets the default author contact email.
`lineend=[value]`		- Sets the default line endings for generated files. Can be "windows" "unix".

## Template Macros

### Universal
Template macros that are compatible with all languages:

`$NAME$`				- The given name for the file/class.
`$FILE_EXT$`			- The file extention of the generated file.
`$FILE_NAME$`			- The name of the file with the extention.
`$DATE$`				- The date the file was generated on.
`$YEAR$`				- The year the file was generated on.
`$MONTH$`				- The name of the month the file was generated on.
`$TIME$`				- The time the file was generated on.
`$DATETIME$`			- The full date and time the file was generated on.
`$AUTHOR$`				- The author.
`$EMAIL$`				- The author contact email.

### C/C++/C#
`$NAMESPACE_BEGIN$`		- Opening namespace statement.
`$NAMESPACE_END$`		- Closing namespace statement.

### C/C++
`$INCLUDES$`			- Include statements added with `-i=`|`-include=`.
`$HEADER_GUARD$`		- Replaced with header guard strings generated from file name.
`$HEADER_EXT$`			- The file extention for header files.
`$SOURCE_EXT$`			- The file extention for source files.

### C++/C#
`$INHERITANCE$`			- Inheritance statement.

### C++
`$VIRTUAL$`				- Virtual statement enabled with `-v`|`-virtual`.
`$INLINE_EXT$`			- The file extention for inline files.

### C#
`$ACCESS$`				- The access level for the structure, set with `-ac=`|`-access=`.
`$CLASS_MODIFIERS$`		- Class modifiers, set with `-pr`|`-partial`, `-ab`|`-abstract`, `-se`|`-sealed`, `-st`|`-static`.
`$USING_STATEMENTS$`	- Using statements set with `-u=`|`-using=`.
