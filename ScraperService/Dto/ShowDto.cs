using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Dto
{
    public class ShowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<CastPersonDto> Cast { get; set; }
    }
}
