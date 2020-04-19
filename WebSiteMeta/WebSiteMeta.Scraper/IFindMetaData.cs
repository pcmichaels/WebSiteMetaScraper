using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteMeta.Scraper
{
    public interface IFindMetaData
    {
        Task<FindMetaDataResult> Run(string url);
    }
}
