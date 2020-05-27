using System;
using System.Collections.Generic;

namespace WebSiteMeta.Scraper
{
    public class Metadata
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Charset { get; set; }
        public Dictionary<string, string> Meta { get; internal set; }
    }
}
