using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LClaproth.MyFinancialTracker.Portfolio.MongoDB;


public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; set; }

    DateTime CreatedAt { get; }
}
