namespace QuickDoc
{
    //@qdclass(Class that handles formatting an analyzed source into a certain format (such as html).)
    public abstract class Formatter

    {
        //@qdmfunction(Formats the source depending on the desired output type.*void)
        //@qdparam(source*The analyzed source.*AnalyzedSource)
        //@qdparam(executionRequest*The execution request.*ExecutionRequest)
        public abstract void FormatSource(AnalyzedSource source, ExecutionRequest executionRequest);

    }
    //@qdend

}