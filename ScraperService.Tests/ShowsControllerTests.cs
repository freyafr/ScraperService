using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ScraperService.Controllers;
using ScraperService.Dto;
using ScraperService.Dto.TvMaze;
using ScraperService.Helpers;
using ScraperService.Models;
using ScraperService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Tests
{
    [TestClass]
    public class ShowsControllerTests
    {
        private readonly Mock<IShowsRepositroy> _repoMock = new Mock<IShowsRepositroy>();
        private readonly Mock<ITvMazeWebAccessor> _webAccessorMock = new Mock<ITvMazeWebAccessor>();
        private readonly Mock<IStateMonitor> _stateMonitorMock = new Mock<IStateMonitor>();

        [TestInitialize]
        public void Init()
        {
            _repoMock.Setup(e => e.GetShowsCount()).ReturnsAsync(1);
            _repoMock.Setup(e => e.GetShows(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Show>
                {
                    new Show
                    {
                        ShowId=1,
                        ShowName="Name1",
                        Cast = new List<Actor>
                        {
                            new Actor
                            {
                                ActorId=1,
                                ActorName="tt2",
                                Birthday = new DateTime(1900,1,1)
                            }
                        }
                    },
                    new Show
                    {
                        ShowId=2,
                        ShowName="Name2",
                        Cast = new List<Actor>
                        {
                            new Actor
                            {
                                ActorId=2,
                                ActorName="tt2"
                            },
                            new Actor
                            {
                                ActorId=1,
                                ActorName="tt2",
                                Birthday = new DateTime(2000,1,1)
                            },                           
                        }
                    }
                });
            _stateMonitorMock.Setup(s => s.OptainLock()).Returns(true);
            _webAccessorMock.Setup(e => e.GetShows(1)).ReturnsAsync(new List<TvMazeShowDto>
            {
                new TvMazeShowDto
                {
                    Id = 1,
                    Name = "Test"
                }
            });
            _webAccessorMock.Setup(e => e.GetCast(1))
                .ReturnsAsync(new List<TvMazeCastPersonDto>
                {
                    new TvMazeCastPersonDto
                    {
                        Person = new TvMazePersonDto
                        {
                            Id = 1,
                        }
                    }
                });
        }


        [TestMethod]
        public async Task GetCountOk()
        {
            var controller = new ShowsController(_repoMock.Object, _webAccessorMock.Object, _stateMonitorMock.Object,
                Mock.Of<ILogger<ShowsController>>());
            var result = await controller.GetPagesCount();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task GetOk()
        {
            var controller = new ShowsController(_repoMock.Object, _webAccessorMock.Object, _stateMonitorMock.Object,
                Mock.Of<ILogger<ShowsController>>());
            var result = await controller.Get(0,10);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task SyncOk()
        {
            var controller = new ShowsController(_repoMock.Object, _webAccessorMock.Object, _stateMonitorMock.Object,
                Mock.Of<ILogger<ShowsController>>());
            var result = await controller.Sync();
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task SyncException()
        {            
            _repoMock.Setup(r => r.AddShows(It.IsAny<List<Show>>())).ThrowsAsync(new Exception());
            var controller = new ShowsController(_repoMock.Object, _webAccessorMock.Object, _stateMonitorMock.Object,
               Mock.Of<ILogger<ShowsController>>());
            var result = await controller.Sync();
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task SyncLocked()
        {
            _stateMonitorMock.Setup(s => s.OptainLock()).Returns(false);            
            var controller = new ShowsController(_repoMock.Object, _webAccessorMock.Object, _stateMonitorMock.Object,
               Mock.Of<ILogger<ShowsController>>());
            var result = await controller.Sync();
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
