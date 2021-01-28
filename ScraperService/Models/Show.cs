using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperService.Models
{    
    public class Show
    {        
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public int ShowId { get; set; }
        
        [BsonElement("Name")]
        public string ShowName { get; set; }
        public IEnumerable<Actor> Cast { get; set; }
    }
}
