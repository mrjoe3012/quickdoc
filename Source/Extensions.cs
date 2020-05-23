using System.Collections.Generic;

namespace QuickDoc
{
    //@qdclass(Class containing extension methods.)
    public static class Extensions
    {
        //@qdmfunction(Returns a new string with both indexes (inclusive).*string)
        //@qdparam(startIndex*The beginning of the substring (inclusive).*int)
        //@qdparam(endIndex*The end of the substring (inclusive).*int)
        public static string SubstringRange(this string str, int startIndex, int endIndex)
        {
            int counter = 0;
            string newString = "";

            foreach (char c in str)
            {
                if(counter >= startIndex && counter <= endIndex)
                {
                    newString += c;
                }

                counter++;
            }
            return newString;
        }


        //@qdmfunction(Adds the list to this list.*void)
        //@qdparam(otherList*The list to add.*List<T>)
        public static void Add<T>(this List<T> thisList, List<T> otherList)
        {
            foreach(T t in otherList)
                thisList.Add(t);
        }


        //@qdmfunction(Adds the array to this list.*void)
        //@qdparam(array*The array to add.*T[])
        public static void Add<T>(this List<T> list, T[] array)
        {
            foreach (T t in array)
                list.Add(t);
        }


        //@qdmfunction(Joins two arrays together*T[])
        //@qdparam(other*The other array.*T[])
        public static T[] Join<T>(this T[] array, T[] other)
        {
            T[] newArray = new T[array.Length + other.Length];

            int i;

            for(i = 0;i<array.Length;i++)
            {
                newArray[i] = array[i];
            }

            for(int j = 0;j<other.Length;j++,i++)
            {
                newArray[i] = other[j];
            }

            array = newArray;

            return array;

        }
        //@qdmfunction(Returns this string with no whitespace.*string)
        public static string RemoveAllWhitespace(this string str)
        {
            string newString = "";

            foreach(char c in str)
            {
                if(!char.IsWhiteSpace(c))
                    newString+=c;
            }
            str = newString;
            return newString;
        }

    }
    //@qdend
}