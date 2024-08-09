using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class FeedData
    {
        protected string _sTitle { get; set; }
        protected string _sFileName { get; set; }
        protected Uri _URL { get; set; } //TODO how to use this Uri data type
        protected string _sexpression { get; set; }
        protected string _sHistory { get; set; }

        FeedData() //accept RSS data here?
        {

        } 
    }
}
