using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ScraperService.Dto;
using ScraperService.Dto.TvMaze;
using ScraperService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScraperService.Helpers
{
    public interface ITvMazeWebAccessor
    {
        Task<IEnumerable<TvMazeShowDto>> GetShows(int page);
        Task<IEnumerable<TvMazeCastPersonDto>> GetCast(int showId);
    }
    public class TvMazeWebAccessor: ITvMazeWebAccessor
    {

        private readonly IWebClientHelper _webClientHelper;
        private readonly AppSettings _appSettings;
        public TvMazeWebAccessor(IWebClientHelper webClientHelper, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            _webClientHelper = webClientHelper;
            _appSettings = optionsSnapshot.Value;
        }

        private async Task<IEnumerable<T>> ProcessResult<T>(HttpResponseMessage response)
        {            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<T>();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException();
            string responseMessage = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(responseMessage);
        }

        public async Task<IEnumerable<TvMazeShowDto>> GetShows(int page)
        {            
            HttpResponseMessage response = await _webClientHelper.GetResult($"{_appSettings.TvMazeBaseUrl}/shows?page={page}");
            return await ProcessResult<TvMazeShowDto>(response);
        }

        public async Task<IEnumerable<TvMazeCastPersonDto>> GetCast(int showId)
        {
            HttpResponseMessage response = await _webClientHelper.GetResult($"{_appSettings.TvMazeBaseUrl}/shows/{showId}/cast");
            return await ProcessResult<TvMazeCastPersonDto>(response);
        }
    }
}
