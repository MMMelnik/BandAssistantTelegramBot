using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BandAssistantBot.Models
{
    public class Rehearsal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
    }
}

//{ 'DateTime' : '9/2/2020 7:00 PM', 'Duration' : 2, 'Location' : 'Olimpiska/red room' },
//
//db.Rehearsals.insertMany([{ 'DateTime' : '', 'Duration' : 2, 'Location' : 'Olimpiska/red room' },{ 'DateTime' : '', 'Duration' : 1, 'Location' : 'Olimpiska/violet room' }])