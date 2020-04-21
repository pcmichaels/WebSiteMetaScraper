using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteMeta.Scraper
{
    public interface IFindMetaData
    {
        string CleanUrl(string url);
        bool ValidateUrl(string url);
        Task<FindMetaDataResult> Run(string url);
    }
}
