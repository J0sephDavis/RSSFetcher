namespace RSSFetcher.Summary
{
    public record SummaryItem(string Field, string Value);
    public interface ISummarizeable
    {
        public string Name { get; }
        public List<SummaryItem> GetSummary();
    }
}
