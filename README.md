# Introduction

## Terminology

* `Positional arguments` - arguments which must be provided in a specific order.
* `Options` - usually optional arguments which can be provided in any order. By default, options are recognized by prepending `--` (double dash) characters. Option can be a boolean switch (i.e., `--force`), or have a value (i.e., --retries=5).
* `Option shortcut` - alternative short option name. By default, shortcuts are recognized by prepending `-` (single dash) character.
* `Argument definition class` - a class decorated with argument-related attributes, the properties of which get values from parsed arguments.
* `Simple syntax` - a scenario in which all arguments can be defined in a single argument definition class.
* `Syntax variants` - in this scenario arguments are divided into separate sets, each set can have a different positional arguments and options.


# Quick start

Quick start examples probably cover most cases required from command-line applications. The following conditions must be met to apply one of those patterns:
* The parser uses default settings.
* Argument definition class(-es) are defined in the entry assembly of the application.

## Argument definition class

Argument definition class properties are mapped to parsed command-line arguments. Both the class and the properties must be docorated with obligatory attributes, which are listed below.

Required class attributes:
* `[Arguments]` - denotes the class as an argument definition class.
* `[Doc("Syntax general description")]` - required by the help generator.

Required property attributes for positional arguments:
* `[Argument(Name="argument_name", Required=true|false, RequiredValue="argument_value", Position=0,1,...)]`
	- `Name` - argument name, used by the help generator.
	- `Required` - determines if the argument is obligatory, or not. Non-obligatory arguments should be assigned with a default value.
	- `RequiredValue` - use this parameter when the argument expects a specific value.
	- `Position` - zero-based index of the positional argument.
* `[Doc("Argument description")]` - required by the help generator.

Required property attributes for options:
* `[Option(Name="option_name", Required=true|false, ShortcutName="shortcut_name")]`
	- `Name` - option name without prepending `--` characters.
	- `Required` - determines if the option is obligatory, or not. Non-obligatory options should be assigned with a default value.
	- `ShortcutName` - alternative short name of the option without prepending `-` character. This parameter is not obligatory and is usually used with the most commonly used options.
* `[Doc("Argument description")]` - required by the help generator.

> [!IMPORTANT]
> All properties must be nullable types.
> Allowed types: `bool`, `int`, `long`, `short`, `string`
> All non-obligatory arguments or options must have a default value.

See examples of argument definition classes in the following sections.

## Simple syntax

Simple syntax can be used when exactly one argument definition class exists in the application and it covers all possible combinations of arguments and/or options.

### Example

`myApp` application is a command-line app which copies files and accepts the following arguments:

```
myApp <src_file> <dest_file> [--quiet | -q] [--retry=N]
```

Argument definition class:

```cs
namespace TW.Args.Net.Sample
{
    [Arguments]
    [Doc("Copy file to another location.")]
    internal class Arguments
    {
        [Argument(Name = "<source_file>", Required = true, Position = 0)]
        [Doc("Source file name.")]
        public string? SourceFile { get; set; }

        [Argument(Name = "<destination_file>", Required = true, Position = 1)]
        [Doc("Destination file name.")]
        public string? DestinationFile { get; set; }


        [Option(Name = "quiet", Required = false, ShortcutName = "q")]
        [Doc("Don't ask for confirmation.")]
        public bool? Quiet { get; set; } = false;

        [Option(Name = "retry", Required = false)]
        [Doc("Number of retries.")]
        public int? Retry { get; set; } = 0;
    }
}
```

Application:

```cs
namespace TW.Args.Net.Sample
{
    internal class Program
    {
        static int Main(string[] args)
        {
            return new ArgumentsParser().Run<Arguments>(args, (arguments) =>
            {
                Console.WriteLine($"SourceFile = {arguments.SourceFile}, DestinationFile = {arguments.DestinationFile}, Quiet = {arguments.Quiet}, Retry = {arguments.Retry}");
                return 0;
            });
        }
    }
}
```

## Syntax variants

Syntax variants should be used when an application accepts a few argument sets of different positional arguments and/or options.

### Example
`myApp` works with files and accepts the following arguments:

```
myApp copy <src_file> <dest_file> [--quiet | -q] [--retry=N]
myApp delete <file> [--force]
```

Here the parameters for `copy` and `delete` operations are different and it is not possible to define them in a single argument definition class. That is why two such classes are defined, one for `copy` and one for `delete` commands:

```cs
namespace TW.Args.Net.SampleVariants
{
    [Arguments]
    [Doc("Copies a source file to the destination.")]
    internal class CopyArguments
    {
        [Argument(Name = "copy", Required = true, RequiredValue = "copy", Position = 0)]
        [Doc("Copy command.")]
        public string? Action { get; set; }

        [Argument(Name = "<source_file>", Required = true, Position = 1)]
        [Doc("Source file name.")]
        public string? SourceFile { get; set; }

        [Argument(Name = "<destination_file>", Required = true, Position = 2)]
        [Doc("Destination file name.")]
        public string? DestinationFile { get; set; }


        [Option(Name = "quiet", Required = false, ShortcutName = "q")]
        [Doc("Don't ask for confirmation.")]
        public bool? Quiet { get; set; } = false;

        [Option(Name = "retry", Required = false)]
        [Doc("Number of retries.")]
        public int? Retry { get; set; } = 0;
    }
}

namespace TW.Args.Net.SampleVariants
{
    [Arguments]
    [Doc("Deletes a file.")]
    internal class DeleteArguments
    {
        [Argument(Name = "delete", Required = true, RequiredValue = "delete", Position = 0)]
        [Doc("Delete command.")]
        public string? Action { get; set; }

        [Argument(Name = "<file>", Required = true, Position = 1)]
        [Doc("File name.")]
        public string? File { get; set; }


        [Option(Name = "force", Required = false, ShortcutName = "f")]
        [Doc("Force file deletion.")]
        public bool? Force { get; set; } = false;
    }
}
```

Application:

```cs
namespace TW.Args.Net.SampleVariants
{
    internal class Program
    {
        static int Main(string[] args)
        {
            return new ArgumentsParser().Run(args, (arguments) =>
                {
                    if (arguments is CopyArguments copyArguments) return Copy(copyArguments);
                    if (arguments is DeleteArguments deleteArguments) return Delete(deleteArguments);
                    return 3;
                });
        }

        static int Copy(CopyArguments arguments)
        {
            Console.WriteLine($"Action = {arguments.Action}, SourceFile = {arguments.SourceFile}, DestinationFile = {arguments.DestinationFile}, Quiet = {arguments.Quiet}, Retry = {arguments.Retry}");
            return 0;
        }

        static int Delete(DeleteArguments arguments)
        {
            Console.WriteLine($"Action = {arguments.Action}, File = {arguments.File}, Force = {arguments.Force}");
            return 0;
        }

    }
}
```


# Advanced topics

## `ArgumentsParser.Run()`

Runs the parser. Exists in two variants:

1. Simple syntax; `T` is the type of argument definition class:
```cs
public int Run<T>(
	string[] args, 
	Func<T, int> handler, 
	Func<int>? onHelpRequested = null, 
	Func<string, int>? onSyntaxError = null, 
	Func<Exception, int>? onError = null)
```

2. Syntax variants:
```cs
public int Run(
	string[] args, 
	Func<object, int> handler, 
	Func<int>? onHelpRequested = null, 
	Func<string, int>? onSyntaxError = null, 
	Func<Exception, int>? onError = null)
```

* `args` - an array of arguments received by the application.
* `handler` - a function that receives parsed arguments. This is the heart of the application, all further processing starts here when argumenents are successfully processed.
* `onHelpRequested` - an optional function for overriding the default behavior for help text display.
* `onSyntaxError` - an optional function for overriding the reaction on syntax error. It receives the error message in `string` parameter.
* `onError` - an optional function for overriding the reaction on any other error. It receives the error in `Exception` parameter.

## Configuring the parser

`ArgumentsParser` default configuration can be changed by passing appropriate parameters to the parser's constructor:

```cs
public ArgumentsParser(Assembly? assembly = null, ParserOptions? options = null)
```

Parameters:
* `assembly` - the assembly where argument definition classes are defined. By default the entry assembly is used. This parameter shoul be only used when arguments definition class(-es) is in another assembly.
* `options` - parser options:
	- `ApplicationName` - the application name to use in help text. By default, the entry assembly name is used.
	- `OptionPrefix` - the option prefix. The default value is `--` (double dash characters).
	- `OptionShortcutPrefix` - the option shortcut prefix. The default value is `-` (single dash character).

### Example

The example shows how to change the default option prefix from `--` to `**`.

```cs
var options = new ParserOptions()
{
	OptionPrefix = "**"
};
var result = ArgumentsParser(options: options)
	.Run<args), (arguments) =>
	{
		return 0;
	});
```

## Changing default arguments processing flow

By default, arguments are processed by one of `ArgumentsParser.Run()` methods and produce the following result:
* If option `--help` or `-h` is provided, the help text is displayed and error code 0 is returned.
* If provided arguments/options do not match any of argument definition classes, then the appropriate error message is displayed and error code 1 is returned.
* If arguments are correctly parsed, then the error code returned from the `handler` function is returned. It should be 0 on success, and some other value on failure.

It is possible to overwrite the default behavior by using custom error handlers with `ArgumentsParser.Run()`:
* When `onHelpRequested` handler is used with `Run()`, then it is executed instead of the default handler.
* When `onSyntaxError` handler is used with `Run()`, then it is executed instead of the default handler. The handler accepts a `string` paramter, which contains the error message.
* When `onError` handler is used with `Run()`, then it is executed instead of the default handler. The handler accepts a paramter of type `Exception`.

### Example

The example illustrates using of all types ot error handlers.

```cs
var result = new ArgumentsParser().Run<SamplePositionalArguments>(ToArgs("command file"), 
	(arguments) =>
	{
		throw new ApplicationException("Test exception")
	},
	onHelpRequested: () =>
	{
		// --help or -h option was provided as argument
		return 97;
	},
	onSyntaxError: (message) =>
	{
		// message is string and contains a descriptive syntax error message
		return 98;
	},
	onError: (exception) =>
	{
		// exception is of type ApplicationException, exception.Message is 'Test exception'
		return 99;
	}
);
```

## The help text generator

The help text generator is implemented in `ArgumentsHelp` class. Its constructor accepts the following optional parameters:

```cs
public ArgumentsHelp(Assembly? assembly = null, ParserOptions? options = null)
```

* `assembly` - the assembly where argument definition classes are declared. The entry assembly is used by default.
* `options` - parser options. Values passed here must be consistent with the parser.

To display the help, use the following code snippet:

```cs
Console.WriteLine(new ArgumentsHelp().GetText());
```

### Example

The help text generated for the simple syntax sample:

```
SYNTAX:

TW.Args.Net.Sample <source_file> <destination_file> [--quiet] [--retry=<int32>]

    Copy file to another location.

    <source_file>        Source file name.

    <destination_file>   Destination file name.

    -q  --quiet          Don't ask for confirmation.

        --retry=<int32>  Number of retries.
```