using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Dto.TvMaze
{
    public class TvMazeCastPersonDto
    {
        public TvMazePersonDto Person { get; set; }
    }

    public class TvMazePersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
