using Google.Cloud.Firestore;
using MarketMate.Domain.Abstractions.Repositories;
using MarketMate.Domain.Attributes;
using MarketMate.Domain.Settings;
using System.Reflection;

namespace MarketMate.Infrastructure.Repositories;

public class FirestoreRepositoryBase<T> : IRepository<T> where T : class
{
    private readonly FirestoreDb _firestoreDb;
    private readonly string _collectionName;

    public FirestoreRepositoryBase(FirebaseSettings firestoreSettings)
    {
        _collectionName = GetCollectionName() ?? throw new ArgumentNullException("Collection name not found");
        _firestoreDb = FirestoreDb.Create(firestoreSettings.ProjectId);
    }

    private string GetCollectionName()
    {
        var collectionNameAttribute = typeof(T).GetCustomAttribute<CollectionNameAttribute>();
        return collectionNameAttribute?.Name;
    }

    public async Task<T> GetByIdAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection(_collectionName).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }
        else
        {
            return null;
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        CollectionReference collection = _firestoreDb.Collection(_collectionName);
        QuerySnapshot snapshot = await collection.GetSnapshotAsync();
        List<T> documents = new List<T>();

        foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
        {
            documents.Add(documentSnapshot.ConvertTo<T>());
        }

        return documents;
    }

    public async Task<T> AddAsync(T entity)
    {
        CollectionReference collection = _firestoreDb.Collection(_collectionName);
        DocumentReference docRef = await collection.AddAsync(entity);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }
        else
        {
            throw new Exception("Failed to retrieve the added document.");
        }
    }

    public async Task<T> UpdateAsync(string id, T entity)
    {
        DocumentReference docRef = _firestoreDb.Collection(_collectionName).Document(id);
        await docRef.SetAsync(entity, SetOptions.Overwrite);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<T>();
        }
        else
        {
            throw new Exception("Failed to retrieve the updated document.");
        }
    }

    public async Task<T> DeleteAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection(_collectionName).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            T entity = snapshot.ConvertTo<T>();
            await docRef.DeleteAsync();
            return entity;
        }
        else
        {
            throw new Exception("Failed to retrieve the document before deletion.");
        }
    }

}
