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

        public FeedData(string title, string fileName, string url, string expression, string history)
        {
            this.Title = title;
            this.FileName = fileName;
            this.URL = new Uri(url);
            //TODO add checks on URL validity
            this.Expression = expression;
            this.History = history;
        }

        public void Print()
        {
            Console.WriteLine("FeedData Obj:" +
                $"\n\t{this.Title}" +
                $"\n\t{this.FileName}" +
                $"\n\t{this.URL}" +
                $"\n\t{this.Expression}" +
                $"\n\t{this.History}");
        }
    }
}
