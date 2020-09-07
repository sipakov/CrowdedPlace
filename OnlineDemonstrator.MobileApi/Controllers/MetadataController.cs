using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineDemonstrator.Libraries.Domain.Entities;

namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {

        IMongoCollection<Contact> Contacts;

        public MetadataController()
        {
            string connectionString = "mongodb://134.209.30.64:27017/OnlineDemonstratorDb";
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            // обращаемся к коллекции Products
            Contacts = database.GetCollection<Contact>("Contacts");
        }

        [HttpGet("getContacts")]
        public async Task<ActionResult<Contact>> GetAsync()
        {
            if (!ModelState.IsValid) return BadRequest();

            var results = await Contacts.Find(x => x.Author == "Sipakov S").FirstOrDefaultAsync();

            return results;
        }
    }
}