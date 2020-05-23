using System.Collections.Generic;
using System.IO;

namespace QuickDoc

{
    //@qdclass(Formats to a text file, mainly for debugging purposes.)
    public class TextFormatter : Formatter
    {

        private List<string> buffer = new List<string>();
        
        public override void FormatSource(AnalyzedSource source, ExecutionRequest executionRequest)
        {

            foreach(Field field in source.fields)
            {
                buffer.Add("**FIELD**");
                buffer.Add(FormatField(field));
                AddLineBreak();
            }    
            foreach(Method method in source.methods)
            {
                buffer.Add("**METHOD**");
                buffer.Add(FormatMethod(method));
                AddLineBreak();
            }
            foreach(Class c in source.classes)
            {
                buffer.Add("**CLASS**");
                buffer.Add(FormatClass(c));
                AddLineBreak();
            }

            SaveData(executionRequest.outputDirectory);

        }

        public void SaveData(string path)
        {
            string[] lines = new string[buffer.Count];

            for(int i = 0; i < buffer.Count; i++)
                lines[i] = buffer[i];

            File.WriteAllLines(path + ".txt", lines);

        }

        public List<string> FormatField(Field field)
        {
            List<string> formattedField = new List<string>();

            formattedField.Add(field.name + " : " + field.type + "\n");
            formattedField.Add("Description: " + field.description);
            
            return formattedField;
        }

        public List<string> FormatMethod(Method method)
        {
            List<string> formattedMethod = new List<string>();

            formattedMethod.Add(method.name + " returns " + method.returnType + "\n");
            formattedMethod.Add("Description: " + method.description);

            if(method.parameters.Count != 0)
                formattedMethod.Add("Parameters:");

            foreach(Field parameter in method.parameters)
            {
                formattedMethod.Add("\n");
                formattedMethod.Add(FormatField(parameter));
                formattedMethod.Add("\n");
            }

            return formattedMethod;

        }

        public List<string> FormatClass(Class c)
        {
            List<string> formattedClass = new List<string>();

            formattedClass.Add(c.name);
            formattedClass.Add(c.description);
            
            if(c.memberFields.Count > 0)
                formattedClass.Add("Member Fields:\n");

            foreach(Field field in c.memberFields)
            {
                formattedClass.Add("\n");
                formattedClass.Add(FormatField(field));
                formattedClass.Add("\n");
            }

            foreach(Method m in c.memberMethods)
            {
                formattedClass.Add("\n");
                formattedClass.Add(FormatMethod(m));
                formattedClass.Add("\n");
            }

            return formattedClass;

        }

        public void AddLineBreak()
        {
            string br = "------------------------------------------------------------";

            buffer.Add("\n");
            buffer.Add(br);
            buffer.Add("\n");
        }

    }
    //@qdend
}