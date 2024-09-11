using System.Text;

namespace RSSFetcher.Summary
{
    internal class SummaryBuilder()
    {
        private readonly List<ISummarizeable> Summaries = [];
        public void Add(ISummarizeable obj) => Summaries.Add(obj);
        public void AddRange(ISummarizeable[] summaries) => Summaries.AddRange(summaries);
        private string GetSummary()
        {
            StringBuilder response = new();
            foreach (ISummarizeable summary in Summaries)
            {
                List<SummaryItem> items = summary.GetSummary();
                response.Append($"Summary ({summary.Name}),");
                foreach (var item in items)
                {
                    response.Append($" {item.Field} {item.Value},");
                }
                response.Append("\n");
            }
            return response.ToString();
            /* EventTicker.cs:
             * Summary| FIELD VALUE, FIELD VALUE, FIELD, VALUE
             * New:
             * Summary (CATEGORY-1): FIELD VALUE, FIELD VALUE, FIELD VALUE
             * Summary (CATEGORY-2): FIELD VALUE, FIELD VALUE, FIELD VALUE
             * Summary (CATEGORY-3): FIELD VALUE, FIELD VALUE, FIELD VALUE
             */
        }
        public override string ToString() => GetSummary();
    }
}
