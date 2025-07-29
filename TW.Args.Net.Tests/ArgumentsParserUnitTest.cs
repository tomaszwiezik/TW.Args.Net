using System.Reflection;

namespace TW.Args.Net.Tests
{
    public class ArgumentsParserUnitTest
    {
        private string[] ToArgs(string args) => args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        private ArgumentsParser GetParser() => new ArgumentsParser(assembly: Assembly.GetExecutingAssembly());


        [Fact]
        public void TestHelp()
        {
            var exception = Assert.Throws<HelpRequestedException>(() => GetParser()
                .Parse(ToArgs("--help")));
        }

        [Fact]
        public void TestHelpShortcut()
        {
            var exception = Assert.Throws<HelpRequestedException>(() => GetParser()
                .Parse(ToArgs("-h")));
        }

        [Fact]
        public void TestNoArguments()
        {
            var exception = Assert.Throws<SyntaxException>(() => GetParser()
                .Parse(ToArgs("")));
            Assert.True(!string.IsNullOrWhiteSpace(exception.Message));
        }

        [Fact]
        public void TestPositionalArguments()
        {
            var arguments = GetParser()
                .Parse<SamplePositionalArguments>(ToArgs("command file"));
            Assert.Equal("command", arguments.Command);
            Assert.Equal("file", arguments.File);
            Assert.Null(arguments.OutputFile);
        }

        [Fact]
        public void TestOptionalPositionalArguments()
        {
            var arguments = GetParser()
                .Parse<SamplePositionalArguments>(ToArgs("command file output"));
            Assert.Equal("command", arguments.Command);
            Assert.Equal("file", arguments.File);
            Assert.Equal("output", arguments.OutputFile);
        }

        [Fact]
        public void TestTooManyPositionalArguments()
        {
            Assert.Throws<SyntaxException>(() => GetParser()
                .Parse<SamplePositionalArguments>(ToArgs("command file output some others")));
        }

        [Fact]
        public void TestInsufficientPositionalArguments()
        {
            Assert.Throws<SyntaxException>(() => GetParser()
                .Parse<SamplePositionalArguments>(ToArgs("command")));
        }

        [Fact]
        public void TestUnsupportedCommandInPositionalArguments()
        {
            Assert.Throws<SyntaxException>(() => GetParser()
                .Parse<SamplePositionalArguments>(ToArgs("unsupportedCommand file")));
        }

        [Fact]
        public void TestOptionArguments()
        {
            var arguments = GetParser()
                .Parse<SampleOptionArguments>(ToArgs("--boolRequired --stringRequired=required-string --intRequired=999 --boolOptional --stringOptional=optional-string --intOptional=200"));
            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(true, arguments.BoolOptional);
            Assert.Equal("optional-string", arguments.StringOptional);
            Assert.Equal(200, arguments.IntOptional);
        }

        [Fact]
        public void TestRequiredOnlyOptionArguments()
        {
            var arguments = GetParser()
                .Parse<SampleOptionArguments>(ToArgs("--boolRequired --stringRequired=required-string --intRequired=999"));
            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(false, arguments.BoolOptional);
            Assert.Equal(string.Empty, arguments.StringOptional);
            Assert.Equal(100, arguments.IntOptional);
        }

        [Fact]
        public void TestOptionShortcutArguments()
        {
            var arguments = GetParser()
                .Parse<SampleOptionArguments>(ToArgs("-br -sr=required-string -ir=999 -bo -so=optional-string -io=200"));
            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(true, arguments.BoolOptional);
            Assert.Equal("optional-string", arguments.StringOptional);
            Assert.Equal(200, arguments.IntOptional);
        }

        [Fact]
        public void TestRequiredOnlyOptionShortcutArguments()
        {
            var arguments = GetParser()
                .Parse<SampleOptionArguments>(ToArgs("-br -sr=required-string -ir=999"));
            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(false, arguments.BoolOptional);
            Assert.Equal(string.Empty, arguments.StringOptional);
            Assert.Equal(100, arguments.IntOptional);
        }

        [Fact]
        public void TestMixedArguments()
        {
            var arguments = GetParser()
                .Parse<SampleMixedArguments>(ToArgs("command file output --boolRequired --stringRequired=required-string --intRequired=999 --boolOptional --stringOptional=optional-string --intOptional=200"));
            Assert.Equal("command", arguments.Command);
            Assert.Equal("file", arguments.File);
            Assert.Equal("output", arguments.OutputFile);

            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(true, arguments.BoolOptional);
            Assert.Equal("optional-string", arguments.StringOptional);
            Assert.Equal(200, arguments.IntOptional);
        }

        [Fact]
        public void TestMixedArgumentsWithOptionsBeforeArguments()
        {
            var arguments = GetParser()
                .Parse<SampleMixedArguments>(ToArgs("--boolRequired --stringRequired=required-string --intRequired=999 --boolOptional --stringOptional=optional-string --intOptional=200 command file output"));
            Assert.Equal("command", arguments.Command);
            Assert.Equal("file", arguments.File);
            Assert.Equal("output", arguments.OutputFile);

            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(true, arguments.BoolOptional);
            Assert.Equal("optional-string", arguments.StringOptional);
            Assert.Equal(200, arguments.IntOptional);
        }

        [Fact]
        public void TestRequiredOnlyMixedArguments()
        {
            var arguments = GetParser()
                .Parse<SampleMixedArguments>(ToArgs("command file --boolRequired --stringRequired=required-string --intRequired=999"));
            Assert.Equal("command", arguments.Command);
            Assert.Equal("file", arguments.File);
            Assert.Null(arguments.OutputFile);

            Assert.Equal(true, arguments.BoolRequired);
            Assert.Equal("required-string", arguments.StringRequired);
            Assert.Equal(999, arguments.IntRequired);
            Assert.Equal(false, arguments.BoolOptional);
            Assert.Equal(string.Empty, arguments.StringOptional);
            Assert.Equal(100, arguments.IntOptional);
        }

    }
}
