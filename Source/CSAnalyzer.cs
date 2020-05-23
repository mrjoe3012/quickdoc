using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

namespace QuickDoc
{
    //@qdclass(Analyzes C# code.)
    public class CSAnalyzer : SourceAnalyzer
    {

        public override AnalyzedSource AnalyzeCode(ExecutionRequest request)
        {
            AnalyzedSource source = new AnalyzedSource();

            string[] lines = new string[0];

            foreach(string filePath in request.sourceFilePaths)
            {
                string[] fileLines = File.ReadAllLines(filePath);
                lines = lines.Join(fileLines);
            }

            List<Flag> flags = GetFlags(lines);

            foreach(Flag flag in flags)
            {
                if(flag.type == Flag.FlagType.FIELD)
                {
                    source.fields.Add(InterpretFieldFlag(flag, lines));
                }
                else if(flag.type == Flag.FlagType.METHOD)
                {
                    source.methods.Add(InterpretMethodFlag(flag, lines));
                }
                else if(flag.type == Flag.FlagType.CLASS)
                {
                    source.classes.Add(InterpretClassFlag(flag, lines));
                }
                else
                {
                    throw new Exception("Could not identify flag type.");
                }
            }

            return source;
        }

        protected override List<Flag> GetMemberFlags(string[] lines)
        {
            List<Flag> flags = new List<Flag>();

            int lineNumber = 0;

            foreach(string line in lines)
            {   
                try{
                if(RemoveIndent(line).Substring(0,2) == "//")
                {
                    Flag flag = new Flag();
                    flag.line = lineNumber;

                    if(RemoveIndent(line).Substring(0, 11) == "//@qdmfield")
                    {
                        flag.type = Flag.FlagType.FIELD;
                        flags.Add(flag);
                    }
                    else if(RemoveIndent(line).Substring(0, 14) == "//@qdmfunction")
                    {
                        flag.type = Flag.FlagType.METHOD;
                        flags.Add(flag);
                    }
                }
                }
                catch(ArgumentOutOfRangeException){}
                lineNumber++;
            }

            return flags;

        }

        protected override List<Flag> GetFlags(string[] lines)
        {
            List<Flag> flags = new List<Flag>();
            int lineNumber = 0;

            foreach(string line in lines)
            {
                try
                {
                if(RemoveIndent(line).Substring(0,2) == "//")
                {
                    Flag flag = new Flag();
                    flag.line = lineNumber;

                    if(RemoveIndent(line).Substring(0,10) == "//@qdfield")
                    {
                        flag.type = Flag.FlagType.FIELD;
                        flags.Add(flag);
                    }
                    else if(RemoveIndent(line).Substring(0,13) == "//@qdfunction")
                    {
                        flag.type = Flag.FlagType.METHOD;
                        flags.Add(flag);
                    }
                    else if(RemoveIndent(line).Substring(0,10) == "//@qdclass")
                    {
                        flag.type = Flag.FlagType.CLASS;
                        flags.Add(flag);
                    }
                }
                }
                catch(ArgumentOutOfRangeException){}

                lineNumber++;
            }

            return flags;

        }

        protected string[] GetData(string line)
        {
            int startIndex = line.IndexOf('(');

            string dataLine = line.SubstringRange(startIndex+1, line.Length-2);
            string[] data = dataLine.Split('*');

            return data;

        }

        protected override Field InterpretFieldFlag(Flag flag, string[] lines)
        {
            Field field = new Field();

            string flagLine = lines[flag.line];

            string[] data = GetData(flagLine);

            field.description = data[0];

            string declLine = lines[flag.line+1];

            if(declLine.Contains('='))
            {
                int eqIndex = declLine.IndexOf('=');
                if(char.IsWhiteSpace(declLine[eqIndex-1]))
                    eqIndex--;

                declLine = declLine.SubstringRange(0, eqIndex-1);
            }

            string[] temp = declLine.Split(' ');
            field.name = temp[temp.Length-1];
            field.name = field.name.Replace(';', ' ');

            for(int i = 0; i < temp.Length-1;i++)
            {
                field.type += temp[i] + " ";
            }

            return field;
        }

        protected override Method InterpretMethodFlag(Flag flag, string[] lines)
        {
            Method method = new Method();

            string[] data = GetData(RemoveIndent(lines[flag.line]));

            method.description = data[0];
            method.returnType = data[1];

            bool foundDeclaration = false;
            int offset = 0;
            do
            {
                offset++;

                string thisLine = RemoveIndent(lines[flag.line+offset]);

                if(thisLine.Substring(0,10) == "//@qdparam")
                {
                    string[] paramInfo = GetData(thisLine);
                    Field param = new Field();
                    param.name = paramInfo[0];
                    param.description = paramInfo[1];
                    param.type = paramInfo[2];

                    method.parameters.Add(param);
                }
                else
                {
                    foundDeclaration = true;
                }

            }while(!foundDeclaration);

            int declarationIndex = flag.line + offset;

            string declaration = RemoveIndent(lines[declarationIndex]);

            int nameEndIndex = declaration.IndexOf('(')-1;
            int nameBeginIndex = 0;

            int i = nameEndIndex;
            bool foundNameBegin = false;

            while(!foundNameBegin && i >= 0)
            {
                if(char.IsWhiteSpace(declaration[i]))
                {
                    foundNameBegin = true;
                    nameBeginIndex = i;
                }

                i--;
            }

            method.name = declaration.SubstringRange(nameBeginIndex, nameEndIndex);

            return method;

        }

        protected override Class InterpretClassFlag(Flag flag, string[] lines)
        {
            Class c = new Class();

            string description = GetData(RemoveIndent(lines[flag.line]))[0];

            string declaration = RemoveIndent(lines[flag.line+1]);

            string[] inherits = null;

            if(declaration.Contains(':'))
            {
                
                int colonIndex = declaration.IndexOf(':');
                int endIndex = colonIndex-1;
                char thisChar = '0';
                do
                {

                    thisChar = declaration[endIndex]; 

                    endIndex--;
  

                }while(char.IsWhiteSpace(thisChar));
                endIndex++;

                inherits = declaration.SubstringRange(endIndex+3, declaration.Length).RemoveAllWhitespace().Split(',');
                declaration = declaration.SubstringRange(0, endIndex);
            }


            string[] keyWords = declaration.Split(' ');

            c.name = keyWords[keyWords.Length-1];


            if(keyWords.Contains("abstract"))
            {
                c.description += "(abstract)";
            }
            if(keyWords.Contains("struct"))
            {
                c.description += "(struct)";
            }
            if(keyWords.Contains("static"))
            {
                c.description += "(static)";
            }
            if(keyWords.Contains("sealed"))
            {
                c.description += "(sealed)";
            }
            if(keyWords.Contains("interface"))
            {
                c.description += "(interface)";
            }

            if(inherits != null && inherits.Length > 0)
            {
                c.description += "Inherits from ";

                foreach(string s in inherits)
                {
                    c.description +=  s + ", ";
                }

                c.description = c.description.SubstringRange(0, c.description.Length-3);

                c.description+=". ";
            }


            c.description += description;

            int classEnd = GetNextEndIndex(lines, flag.line, "//");

            string[] body = new string[(classEnd-1) - (flag.line+1)];

            for(int i = 0, j = flag.line+2;j<classEnd;j++,i++)
            {
                body[i] = lines[j];
            }

            List<Flag> memberFlags = GetMemberFlags(body);

            foreach(Flag memberFlag in memberFlags)
            {
                DataType t = InterpretFlag(memberFlag, body);

                if(t.GetType() == typeof(Field))
                {
                    c.memberFields.Add((Field)t);
                }
                else if(t.GetType() == typeof(Method))
                {
                    c.memberMethods.Add((Method)t);
                }
            }
           
           return c;

        }
    

    }
    //@qdend
}