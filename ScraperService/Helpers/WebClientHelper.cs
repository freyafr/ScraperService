using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ScraperService.Helpers
{
    public interface IWebClientHelper
    {
        Task<HttpResponseMessage> GetResult(string url);
    }
    public class WebClientHelper: IWebClientHelper
    {        
        private readonly HttpMessageHandler _messageHandler;

        public WebClientHelper(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public async Task<HttpResponseMessage> GetResult(string url)
        {
            using (var httpClient = new HttpClient(_messageHandler, false))
            {
                var result = await httpClient.GetAsync(new Uri(url));

                if (result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(2000);
                    return await GetResult(url);
                }
                return result;
            }
        }
    }
}
