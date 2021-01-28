using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScraperService.Dto;
using ScraperService.Dto.TvMaze;
using ScraperService.Helpers;
using ScraperService.Models;
using ScraperService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowsRepositroy _showsRepositroy;
        private readonly ITvMazeWebAccessor _tvMazeWebAccessor;
        private readonly IStateMonitor _stateMonitor;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(IShowsRepositroy showsRepositroy, 
            ITvMazeWebAccessor tvMazeWebAccessor, IStateMonitor stateMonitor, ILogger<ShowsController> logger)
        {
            _showsRepositroy = showsRepositroy;
            _tvMazeWebAccessor = tvMazeWebAccessor;
            _stateMonitor = stateMonitor;
            _logger = logger;
        }

        [HttpGet]
        [Route("pagecount")]
        public async Task<long> GetPagesCount(int pageSize = 100)
        {
            return (int)Math.Floor((double)(await _showsRepositroy.GetShowsCount()) / pageSize) + 1;
        }

        [HttpGet]
        public async Task<IEnumerable<ShowDto>> Get(int page=1, int pageSize=100)
        {
            int offset = (page - 1) * pageSize;
            return 
                (await _showsRepositroy.GetShows(offset, pageSize))
                .Select(s => new ShowDto
                {
                    Id = s.ShowId,
                    Name = s.ShowName,
                    Cast = s.Cast.OrderByDescending(c=>c.Birthday).Select(c=>new CastPersonDto
                    {
                        Id = c.ActorId,
                        Name = c.ActorName,
                        Birthday = c.Birthday
                    })
                }
                ).ToList();
        }

        [HttpPost]
        public async Task<ActionResult> Sync()
        {
            if (!_stateMonitor.OptainLock())
                return BadRequest("Sync is already working");
            try
            {
                var maxId = await _showsRepositroy.GetShowsMaxId();
                int page = ((int)Math.Floor((double)maxId / 250)) + 1;
                IEnumerable<TvMazeShowDto> newShowsList;
                do
                {
                    newShowsList = await _tvMazeWebAccessor.GetShows(page);
                    var showList = new List<Show>();
                    foreach (var show in newShowsList)
                    {
                        var castList = await _tvMazeWebAccessor.GetCast(show.Id);
                        var showModel = new Show
                        {
                            ShowId = show.Id,
                            ShowName = show.Name,
                            Cast = castList.Select(c => new Actor
                            {
                                ActorId = c.Person.Id,
                                ActorName = c.Person.Name,
                                Birthday = c.Person.Birthday
                            }).ToList()
                        };
                        showList.Add(showModel);                        
                    }

                    await _showsRepositroy.AddShows(showList);                   
                    page++;

                } while (newShowsList.Any());
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
            finally
            {
                _stateMonitor.ReleaseLock();
            }
        }
    }
}
