using System.Collections.Generic;

namespace QuickDoc
{
    //@qdclass(Class representing data that can be analyzed from source.)
    public abstract class DataType
    {
        //@qdmfield(The name of the data.)
        public string name;
        //@qdmfield(The description of the data.)
        public string description;
    }
    //@qdend
    //@qdclass(Represents a field, such as a variable.)
    public class Field : DataType
    {
        //@qdmfield(The type of variable, for example string or int.)
        public string type;
    }
    //@qdend
    //@qdclass(Represents a method.)
    public class Method : DataType
    {
        //@qdmfield(The parameters of this method.)
        public List<Field> parameters;
        //@qdmfield(The return type of this method.)
        public string returnType;

        public Method()
        {
            parameters = new List<Field>();
        }
    }
    //@qdend
    //@qdclass(Represents a class.)
    public class Class : DataType
    {
        //@qdmfield(The fields nested within the class)
        public List<Field> memberFields;
        //@qdmfield(The methods nested within the class)
        public List<Method> memberMethods;

        public Class()
        {
            memberFields = new List<Field>();
            memberMethods = new List<Method>();
        }
    }
    //@qdend

}