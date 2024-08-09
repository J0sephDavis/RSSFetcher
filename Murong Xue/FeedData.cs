using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class FeedData
    {
        protected string Title { get; set; }
        protected string FileName { get; set; }
        protected Uri URL { get; set; } //TODO how to use this Uri data type
        protected string Expression { get; set; }
        protected string History { get; set; }

        public FeedData() //accept RSS data here?
        {
            this.Title = "subspleases - slime";
            this.FileName = "subsplease_tensei_shitara.xml";
            this.URL = new Uri("https://nyaa.si/?page=rss&q=1080+%5BSubsPlease%5D+Tensei+Shitara+Slime+Datta+Ken+-&c=0_0&f=0");
            this.Expression = "[SubsPlease] Tensei Shitara Slime Datta Ken -.*";
            this.History = "[SubsPlease] Tensei Shitara Slime Datta Ken - 65 (1080p) [7C636E3C].mkv";
        }

        public void Print()
        {
            Console.WriteLine($"Title: {this.Title}\t" +
                $"FileName:{this.FileName}\t" +
                $"URL:{this.URL}\t" +
                $"Expr:{this.Expression}\t" +
                $"History:{this.History}");
        }
    }
}
