using BandAssistantBot.Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace BandAssistantBot.Services
{
    public class RehearsalService
    {
        private readonly IMongoCollection<Rehearsal> _rehearsals;

        public RehearsalService(IBandAssistantDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _rehearsals = database.GetCollection<Rehearsal>(settings.RehearsalsCollectionName);
        }

        public List<Rehearsal> Get() =>
            _rehearsals.Find(rehearsal => true).ToList();

        public Rehearsal Get(string id) =>
            _rehearsals.Find<Rehearsal>(rehearsal => rehearsal.Id == id).FirstOrDefault();

        public Rehearsal Create(Rehearsal rehearsal)
        {
            _rehearsals.InsertOne(rehearsal);
            return rehearsal;
        }

        public void Update(string id, Rehearsal rehearsalIn) =>
            _rehearsals.ReplaceOne(rehearsal => rehearsal.Id == id, rehearsalIn);

        public void Remove(Rehearsal rehearsalIn) =>
            _rehearsals.DeleteOne(rehearsal => rehearsal.Id == rehearsalIn.Id);

        public void Remove(string id) =>
            _rehearsals.DeleteOne(rehearsal => rehearsal.Id == id);
    }
}
