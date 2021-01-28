using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Dto
{
    public class CastPersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
