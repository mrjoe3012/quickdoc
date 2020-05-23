using System.Collections.Generic;
using System.IO;
using System;

namespace QuickDoc
{
    //@qdclass(Analyzes python code.)
    public class PythonAnalyzer : SourceAnalyzer
    {

        protected override List<Flag> GetFlags(string[] lines)
        {
            List<Flag> flags = new List<Flag>();

            int counter = 0;

            foreach (string line in lines)
            {
            
                    string thisLine = RemoveIndent(line);
                try
                {
                    if(thisLine.Substring(0,4)== "#@qd")
                    {
                        Flag thisFlag = new Flag();
                        if(thisLine.Substring(0,9) == "#@qdfield")
                        {
                            thisFlag.type = Flag.FlagType.FIELD;
                            thisFlag.line = counter;
                            flags.Add(thisFlag);
                        }
                        else if(thisLine.Substring(0, 9) == "#@qdclass")
                        {
                            thisFlag.type = Flag.FlagType.CLASS;
                            thisFlag.line = counter;
                            flags.Add(thisFlag);
                        }
                        else if(thisLine.Substring(0,12) == "#@qdfunction")
                        {
                            thisFlag.type = Flag.FlagType.METHOD;
                            thisFlag.line = counter;
                            flags.Add(thisFlag);
                        }
                        
                    }
                }
                catch(ArgumentOutOfRangeException)
                {

                }
                counter++;
            }

            return flags;

        }

        protected override List<Flag> GetMemberFlags(string[] lines)
        {
            List<Flag> flags = new List<Flag>();

            int counter = 0;

            foreach (string line in lines)
            {
            
                    string thisLine = RemoveIndent(line);
                try
                {
                    if(thisLine.Substring(0,4)== "#@qd")
                    {
                        Flag thisFlag = new Flag();
                        if(thisLine.Substring(0,10) == "#@qdmfield")
                        {
                            thisFlag.type = Flag.FlagType.FIELD;
                            thisFlag.line = counter;
                            flags.Add(thisFlag);
                        }
                        else if(thisLine.Substring(0, 13) == "#@qdmfunction")
                        {
                            thisFlag.type = Flag.FlagType.METHOD;
                            thisFlag.line = counter;
                            flags.Add(thisFlag);
                        }
                        
                    }
                }
                catch(ArgumentOutOfRangeException)
                {

                }
                counter++;
            }

            return flags;

        }

        public override AnalyzedSource AnalyzeCode(ExecutionRequest request)
        {
            AnalyzedSource analyzedSource = new AnalyzedSource();


            string[] lines = new string[0];
            foreach(string sourcePath in request.sourceFilePaths)
            {
                string[] file = File.ReadAllLines(sourcePath);
                lines = lines.Join(file);
            }

            List<Flag> flags = GetFlags(lines);

            foreach(Flag flag in flags)
            {
                switch(flag.type)
                {
                    case(Flag.FlagType.CLASS):
                        Class c = InterpretClassFlag(flag, lines);
                        analyzedSource.classes.Add(c);
                        break;
                    case(Flag.FlagType.FIELD):
                        Field fi = InterpretFieldFlag(flag, lines);
                        analyzedSource.fields.Add(fi);
                        break;
                    case(Flag.FlagType.METHOD):
                        Method m = InterpretMethodFlag(flag, lines);
                        analyzedSource.methods.Add(m);
                        break;
                }
            }

            return analyzedSource;
        }

        protected override Method InterpretMethodFlag(Flag flag, string[] lines)
        {
            Method method = new Method();

            string decLine = RemoveIndent(lines[flag.line]);

            string data = GetDescription(decLine);
            string[] dataArray = data.Split('*');

            method.description = dataArray[0];
            method.returnType = dataArray[1];

            bool parameter = false;
            int offSet = 1;

            do
            {
                parameter = false;
                string line = RemoveIndent(lines[flag.line+offSet]);
                if(line.Substring(0, 9) == "#@qdparam")
                {
                    parameter = true;
                    method.parameters.Add(GetParameter(line));
                }
                offSet++;


            }while(parameter);

            var name = "";
            string nameLine = RemoveIndent(lines[flag.line + offSet-1]);
            int nameBeginIndex = 1;

            foreach (char c in nameLine)
            {
                if(c == ' ')
                    break;

                nameBeginIndex++;
            }

            for(;nameBeginIndex<nameLine.Length;nameBeginIndex++)
            {
                if(nameLine[nameBeginIndex] != ':' && nameLine[nameBeginIndex] != '(')
                    name += nameLine[nameBeginIndex];
                else
                    break;
            }


            method.name = name;


            return method;

        }

        protected Field GetParameter(string line)
        {
            string data = GetDescription(line);
            string[] dataArray = data.Split("*");

            Field result = new Field();

            result.name = dataArray[0];
            result.description = dataArray[1];
            result.type = dataArray[2];

            return result;
        }

        protected override Class InterpretClassFlag(Flag flag, string[] lines)
        {
            Class classData = new Class();

            string decLine = lines[flag.line];

            string description = GetDescription(decLine);
            classData.description = description;
            
            string declLine = RemoveIndent(lines[flag.line+1]);
            string name = "";

            int nameBeginIndex = 1;

            foreach (char c in declLine)
            {
                if(c == ' ')
                    break;

                nameBeginIndex++;
            }

            for(;nameBeginIndex<declLine.Length;nameBeginIndex++)
            {
                if(declLine[nameBeginIndex] != ':' && declLine[nameBeginIndex] != '(')
                    name += declLine[nameBeginIndex];
                else
                    break;
            }

            classData.name = name;

            int endIndex = GetNextEndIndex(lines, flag.line, "#");

            string[] body = new string[(endIndex-1) - (flag.line+1)];
            for (int i  = flag.line+2, j = 0; i < endIndex; i++, j++)
            {
                body[j] = RemoveIndent(lines[i]);
            }

            List<Flag> nestedFlags = GetMemberFlags(body);
            foreach(Flag f in nestedFlags)
            {
                DataType t = InterpretFlag(f, body);

                if(t.GetType() == typeof(Field))
                {
                    classData.memberFields.Add((Field)t);
                }
                else if(t.GetType() == typeof(Method))
                {
                    classData.memberMethods.Add((Method)t);
                }
                else
                {
                    throw(new Exception("Could not identify flag type."));
                }

            }

            return classData;
        }

        private string GetDescription(string line)
        {
            int startIndex = line.IndexOf('(');
            int lastIndex = line.LastIndexOf(')');
            string data = line.SubstringRange(startIndex+1, lastIndex-1);

            return data;
        }

        protected override Field InterpretFieldFlag(Flag flag, string[] lines)
        {
            Field field = new Field();

            string data = GetDescription(lines[flag.line]);
            string[] dataArray = data.Split('*');

            string declarationString = lines[flag.line+1];
            declarationString = RemoveIndent(declarationString);

            foreach(char c in declarationString)
            {
                if(c == ' ')
                {
                    break;
                }
                else
                {
                    field.name += c;
                }
            }
            try
            {
                if(field.name.Substring(0, 5) == "self.")
                {
                    field.name = field.name.SubstringRange(5, field.name.Length-1);
                }
            }
            catch(ArgumentOutOfRangeException){}

            field.description = dataArray[0];
            field.type = dataArray[1];
            

            return field;
        }
            


    }
    //@qdend
}