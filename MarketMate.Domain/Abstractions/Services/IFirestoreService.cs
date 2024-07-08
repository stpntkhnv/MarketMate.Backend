namespace MarketMate.Domain.Abstractions.Services;

public interface IFirestoreService
{
    Task<T> GetDocumentAsync<T>(string collectionName, string documentId);
    Task<IEnumerable<T>> GetDocumentsAsync<T>(string collectionName);
    Task AddDocumentAsync<T>(string collectionName, T document);
    Task AddDocumentAsync<T>(string collectionName, string documentId, T document);
    Task UpdateDocumentAsync<T>(string collectionName, string documentId, T document);
    Task DeleteDocumentAsync(string collectionName, string documentId);
}
