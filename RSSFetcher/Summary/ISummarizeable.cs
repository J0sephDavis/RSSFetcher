namespace RSSFetcher.Summary
{
    internal record SummaryItem(string Field, string Value);
    internal interface ISummarizeable
    {
        public string Name { get; }
        public List<SummaryItem> GetSummary();
    }
}
