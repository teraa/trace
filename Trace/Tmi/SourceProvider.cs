namespace Trace.Tmi;

public interface ISourceProvider
{
    short SourceId { get; }
}

public class SourceProvider : ISourceProvider
{
    public short SourceId { get; set; }
}
