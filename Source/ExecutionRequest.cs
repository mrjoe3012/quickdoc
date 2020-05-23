using System.Collections.Generic;

namespace QuickDoc
{
    //@qdclass(Stores data about a program execution.)
    public class ExecutionRequest
    {
        public static Dictionary<string, Option> OptionArgument = new Dictionary<string, Option>()
        {
            {"-h", Option.HELP},
            {"-help", Option.HELP},
            {"-text", Option.TEXT},
            {"-html", Option.HTML}
        };

        //@qdmfield(The types of options that can be entered from the command line.)
        public enum Option
        {
            HELP,
            HTML,
            TEXT
        }

        //@qdmfield(The source file locations)
        public readonly string[] sourceFilePaths;
        //@qdmfield(The path and name of the output file.)
        public readonly string outputDirectory;
        //@qdmfield(The options enabled by the user.)
        public readonly Option[] options;
        public ExecutionRequest(string[] options, string[] sourceFiles, string outputDir)
        {
            Option[] convertedOptions = new Option[options.Length];

            for (int i = 0; i < options.Length; i++)
            {
                convertedOptions[i] = OptionArgument[options[i]];
            }

            this.options = convertedOptions;

            this.sourceFilePaths = sourceFiles;
            this.outputDirectory = outputDir;
        }


    }
    //@qdend
}