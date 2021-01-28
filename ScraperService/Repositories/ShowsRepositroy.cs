using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ScraperService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Repositories
{
    public interface IShowsRepositroy
    {
        Task<IEnumerable<Show>> GetShows(int offset, int limit);
        Task<long> GetShowsCount();
        Task<int> GetShowsMaxId();
        Task AddShows(List<Show> showsList);
    }

    public class ShowsRepositroy : IShowsRepositroy
    {             
        private readonly IMongoCollection<Show> _shows;

        public ShowsRepositroy(IOptionsSnapshot<AppSettings> optionsSnapshot,
            IMongoClient mongoClient)
        {           
            var appSettings = optionsSnapshot.Value;
            var database = mongoClient.GetDatabase(appSettings.ShowsDatabaseName);          
            _shows = database.GetCollection<Show>(appSettings.ShowsCollectionName);
        }

        public async Task AddShows(List<Show> showsList)
        {
           await _shows.InsertManyAsync(showsList);
        }

        public async Task<IEnumerable<Show>> GetShows(int offset, int limit)
        {           
           return await _shows.Find(c => true).Skip(offset).Limit(limit).ToListAsync();
        }

        public async Task<long> GetShowsCount()
        {            
            return await _shows.CountDocumentsAsync(b=>true);
        }

        public async Task<int> GetShowsMaxId()
        {
            if ((await GetShowsCount())==0)
                return 0;
            return _shows.Find(c => true).SortByDescending(s => s.ShowId).First().ShowId;           
        }        
    }
}
