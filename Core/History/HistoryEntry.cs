namespace RestAdventure.Core.History;

public abstract class HistoryEntry
{
    protected HistoryEntry(long tick)
    {
        Tick = tick;
    }

    public long Tick { get; }
}
