using System;
using System.Collections.Generic;
using System.Text;

namespace WebSiteMeta.Scraper
{
    public class FindMetaDataResult
    {
        public bool IsSuccess { get; set; }
        public string[] Errors { get; set; }
        public Metadata Metadata { get; set; }
    }
}
