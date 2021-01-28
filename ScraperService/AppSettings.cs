using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService
{
    public class AppSettings
    {
        public string TvMazeBaseUrl { get; set; }
        public string ShowsCollectionName { get; set; }
        public string ShowsDatabaseConnectionString { get; set; }
        public string ShowsDatabaseName { get; set; }
    }
}
