using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Com.Efrata.Service.Purchasing.Lib
{
    public class MongoDbContext
    {
        public static string connectionString;

        private readonly IMongoDatabase database;

        public MongoDbContext()
        {
            MongoUrl mongoUrl = new MongoUrl(connectionString);

            MongoClientSettings mongoClientSettings = new MongoClientSettings();
            mongoClientSettings.Server = mongoUrl.Server;
            if (mongoUrl.DatabaseName != null && mongoUrl.Username != null && mongoUrl.Password != null)
                mongoClientSettings.Credential = MongoCredential.CreateCredential(mongoUrl.DatabaseName, mongoUrl.Username, mongoUrl.Password);
            // mongoClientSettings.UseSsl = mongoUrl.UseSsl;
            // mongoClientSettings.VerifySslCertificate = false;

            MongoClient mongoClient = new MongoClient(mongoClientSettings);

            database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<BsonDocument> UnitReceiptNote => database.GetCollection<BsonDocument>("unit-receipt-notes");
        public IMongoCollection<UnitReceiptNoteViewModel> UnitReceiptNoteViewModel => database.GetCollection<UnitReceiptNoteViewModel>("unit-receipt-notes");
        public IMongoCollection<BsonDocument> UnitPaymentOrder => database.GetCollection<BsonDocument>("unit-payment-orders");
    }
}
