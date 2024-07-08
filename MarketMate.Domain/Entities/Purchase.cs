using Google.Cloud.Firestore;
using MarketMate.Domain.Attributes;

namespace MarketMate.Domain.Entities;


[FirestoreData]
[CollectionName("purchases")]
public class Purchase
{
    [FirestoreDocumentId]
    public string Id { get; private set; }

    [FirestoreProperty("name")]
    public string Name { get; private set; }

    [FirestoreProperty("description")]
    public string Description { get; private set; }

    [FirestoreProperty("start-date")]
    public DateTime StartDate { get; private set; }

    [FirestoreProperty("end-date")]
    public DateTime EndDate { get; private set; }

    [FirestoreProperty("community-id")]
    public long CommunityId { get; private set; }

    [FirestoreProperty("agent-id")]
    public long AgentId { get; set; }

    [FirestoreProperty("album-id")]
    public long AlbumId { get; private set; }
}
