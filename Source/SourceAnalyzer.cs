using System.IO;
using System.Collections.Generic;
using System;

namespace QuickDoc
{
    //@qdclass(Abstract class that provides the core functionality for analyzing a source file. Language Specific analyzers should be derived from this class.)
    public abstract class SourceAnalyzer
    {
        public SourceAnalyzer()
        {
        }

        protected struct Flag
        {
            public enum FlagType
            {
                FIELD,
                CLASS,
                METHOD
            }

            public FlagType type;

            public int line;
        }


        //@qdmfunction(Removes indent before text, such as tabs or spaces.*string)
        //@qdparam(line*The line to modify*string)
        protected string RemoveIndent(string line)
        {
            int startIndex = 0;
            int counter = 0;

            foreach (char c in line)
            {
                if (!char.IsWhiteSpace(c))
                {
                    startIndex = counter;
                    break;
                }


                counter++;
            }

            string newString = line.Substring(startIndex);
            return newString;
        }

        //@qdmfunction(Returns the index of the first @qdend flag found in the file.*int)
        //@qdparam(lines*The lines to search through.*string[])
        //@qdparam(startIndex*The index to start searching from.*int)
        //@qdparam(commentText*The text that specifies a comment in the chosen programming language.*string)
        protected int GetNextEndIndex(string[] lines, int startIndex, string commentText)
        {
            int endIndex = -1;
            for (int i = startIndex; i < lines.Length; i++)
            {
                try
                {
                        if(RemoveIndent(lines[i]).Substring(0, 6+commentText.Length) == commentText + "@qdend")
                        {
                            endIndex = i;
                            break;
                        }
                }
                catch(ArgumentOutOfRangeException)
                {}
            }
            return endIndex;
        }

        //@qdmfunction(Returns flags found in the passed lines*List<Flag>)
        //@qdparam(lines*The lines in whic to search.*string[])
        protected abstract List<Flag> GetFlags(string[] lines);

        //@qdmfunction(Returns member flags found in the passed lines.*List<Flag>)
        //@qdparam(lines*The lines in which to search*string[])
        protected abstract List<Flag> GetMemberFlags(string[] lines);

        //@qdmfunction(Interprets the flag depending on the type.*DataType)
        //@qdparam(flag*The flag to interpret.*Flag)
        //@qdparam(lines*The lines to look through.*string[])
        protected DataType InterpretFlag(Flag flag, string[] lines)
        {
            DataType result = null;
            if(flag.type == Flag.FlagType.FIELD)
            {
                result = InterpretFieldFlag(flag, lines);
            }
            else if(flag.type == Flag.FlagType.CLASS)
            {
                result = InterpretClassFlag(flag, lines);
            }
            else if(flag.type == Flag.FlagType.METHOD)
            {
                result = InterpretMethodFlag(flag, lines);
            }

            return result;
        }


        //@qdmfunction(Analyzes the sourcefiles in the request.*AnalyzedSource)
        //@qdparam(request*The execution request containing source file information.*ExecutionRequest)
        public abstract AnalyzedSource AnalyzeCode(ExecutionRequest request);

        //@qdmfunction(Interprets a field flag.*Field)
        //@qdparam(flag*The flag to interpret.*Flag)
        //@qdparam(lines*The lines to look through.*string[])
        protected abstract Field InterpretFieldFlag(Flag flag, string[] lines);

        //@qdmfunction(Interprets a class flag.*Field)
        //@qdparam(flag*The flag to interpret.*Flag)
        //@qdparam(lines*The lines to look through.*string[])
        protected abstract Class InterpretClassFlag(Flag flag, string[] lines);

        //@qdmfunction(Interprets a method flag.*Field)
        //@qdparam(flag*The flag to interpret.*Flag)
        //@qdparam(lines*The lines to look through.*string[])
        protected abstract Method InterpretMethodFlag(Flag flag, string[] lines);

    }
    //@qdend
}
