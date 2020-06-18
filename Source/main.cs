using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickDoc
{
    //@qdclass(Handles the entry of the program.)
    class Entry
    {
        public static ExecutionRequest Request { get; set; }

        static int Main(string[] args)
        {
            string[] options = GetOptions(args);          

            if(options.Length == 0)
            {
                Console.WriteLine("Invalid arguments passed.");
                PrintHelp(true);
                return -1;
            }

            string[] sourcefiles = null;
            string output = "";

            ExecutionRequest request;

            try
            { request = new ExecutionRequest(options, null, null); }
            catch (ExecutionRequest.InvalidOptionException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
            

            if(request.options.Contains(ExecutionRequest.Option.HELP))
            {
                PrintHelp();
            }
            else if(args.Length < 3)
            {
                Console.WriteLine("Invalid arguments passed.");
                PrintHelp(true);
                return -1;
            }
            else
            {
            GetArguments(args, out sourcefiles, out output);
            request = new ExecutionRequest(options, sourcefiles, output);
            Request = request;

            SourceAnalyzer analyzer = null;

            if(sourcefiles[0].Substring(sourcefiles[0].Length-3, 3) == ".py")
            {
                analyzer = new PythonAnalyzer();
            }
            else if(sourcefiles[0].Substring(sourcefiles[0].Length-3, 3) == ".cs")
            {
                analyzer = new CSAnalyzer();
            }

            if(analyzer == null)
            {
                Console.WriteLine("Cannot create documentation for these types of files. Check the documentation for supported file extensions.");
                return -1;
            }

            AnalyzedSource source;

            try
            {
                source = analyzer.AnalyzeCode(request);
            }
            catch(SourceAnalyzer.UnclosedClassFlagException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            if (source == null)
                return -1;

            Formatter formatter = GetFormatter(request);
            
            formatter.FormatSource(source, request);
            }
            return 0;
        }
        //@qdmfunction(Returns the formatter depending on the option.*Formatter)
        //@qdparam(request*The execution request*ExecutionRequest)
        static Formatter GetFormatter(ExecutionRequest request)
        {
            if(request.options.Contains(ExecutionRequest.Option.TEXT))
            {
                return new TextFormatter();
            }
            else if(request.options.Contains(ExecutionRequest.Option.HTML))
            {
                return new HTMLFormatter();
            }
            return null;
        }

        //@qdmfunction(Prints out the help message.*void)
        //@qdparam(shortHelp=false*If true, a shorter version of the help text will be printed.*bool)
        static void PrintHelp(bool shortHelp = false)
        {
            if(!shortHelp)
                Console.WriteLine("QuickDoc version " + QuickDoc.VERSION);
            Console.WriteLine("For help, type quickdoc -h or quickdoc -help");
            if(!shortHelp)
            {            
            Console.WriteLine("\nUsage: quickdoc {options} {sourcefiles} {output}");
            Console.WriteLine("Example: quickdoc -text myfile.py myfile2.py output - will create documentation for two source files and output it to a file name output.txt.");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("-h -help : display help");
            Console.WriteLine("-text : export documentation as text file");
            Console.WriteLine("-html : export documentation in html format");
            }
        }
        //@qdmfunction(Returns the amount of options present in the command arguments.*void)
        //@qdparam(args*The command line arguments.*string[])
        static int GetOptionCount(string[] args)
        {
            int count = 0;

            foreach(string arg in args)
            {
                if(arg[0] == '-')
                    count++;
            }

            return count;

        }

        //@qdmfunction(Returns source files and destinatin path from arguments.*void)
        //@qdparam(args*Arguments.*string[])
        //@qdparam(sourceFiles*The source files extracted from the arguments.*out string[])
        //@qdparam(outPath*The output directory extracted from the arguments.*out string)
        static void GetArguments(string[] args, out string[] sourceFiles, out string outPath)
        {
            int optionCount = GetOptionCount(args);

            sourceFiles = new string[(args.Length-optionCount)-1];
            outPath = "";

            

            for (int i = optionCount, index = 0; i < args.Length; i++, index++)
            {
                if (i != args.Length-1)
                {
                    sourceFiles[index] = args[i];
                }
                else
                {
                    outPath = args[i];
                }
            }
        }


        //@qdmfunction(Returns options from the arguments.*string[])
        //@qdparam(args*The command line arguments.*string[])
        static string[] GetOptions(string[] args)
        {
            int optionCount = GetOptionCount(args);

            string[] options = new string[optionCount];
            
            int index = 0;

            foreach(string arg in args)
            {
                if(arg[0] == '-')
                {
                    options[index] = arg;
                    index++;
                }
            }

            return options;

        }

    }
    //@qdend
}
