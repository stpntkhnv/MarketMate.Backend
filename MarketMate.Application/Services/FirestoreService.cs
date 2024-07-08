using Google.Cloud.Firestore;
using MarketMate.Domain.Abstractions.Services;

namespace MarketMate.Application.Services;

public class FirestoreService : IFirestoreService
{
    private readonly FirestoreDb _firestoreDb;

    public FirestoreService(string projectId)
    {
        _firestoreDb = FirestoreDb.Create(projectId);
    }

    public async Task<T> GetDocumentAsync<T>(string collectionName, string documentId)
    {
        DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }
        else
        {
            return default;
        }
    }

    public async Task<IEnumerable<T>> GetDocumentsAsync<T>(string collectionName)
    {
        CollectionReference collection = _firestoreDb.Collection(collectionName);
        QuerySnapshot snapshot = await collection.GetSnapshotAsync();
        List<T> documents = new List<T>();

        foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
        {
            documents.Add(documentSnapshot.ConvertTo<T>());
        }

        return documents;
    }

    public async Task AddDocumentAsync<T>(string collectionName, T document)
    {
        CollectionReference collection = _firestoreDb.Collection(collectionName);
        await collection.AddAsync(document);
    }

    public async Task AddDocumentAsync<T>(string collectionName, string documentId, T document)
    {
        DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentId);
        await docRef.SetAsync(document);
    }

    public async Task UpdateDocumentAsync<T>(string collectionName, string documentId, T document)
    {
        DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentId);
        await docRef.SetAsync(document, SetOptions.Overwrite);
    }

    public async Task DeleteDocumentAsync(string collectionName, string documentId)
    {
        DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentId);
        await docRef.DeleteAsync();
    }
}
