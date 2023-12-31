using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LClaproth.MyFinancialTracker.Portfolio.MongoDB;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
}