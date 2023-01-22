using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteMeta.Scraper
{
    public class CompareMetaData
    {
        private readonly IFindMetaData _findMetaData;

        public CompareMetaData(IFindMetaData findMetaData)
        {
            _findMetaData = findMetaData;
        }

        /// <summary>
        /// Scrape the two sites, and return a dictionary of differences
        /// </summary>
        /// <param name="url"></param>
        /// <param name="url2"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// 
        /*
        public async Task<Dictionary<string, string>> Run(string url, string url2, Encoding encoding = null)
        {
            var site1 = await _findMetaData.Run(url);
            var site2 = await _findMetaData.Run(url2);

            foreach (var metadata in site1.Metadata.Meta)
            {

            }
        }
        */
    }
}
