using Microsoft.Azure.Documents.Client; 
using Microsoft.Azure.Documents.Linq; 
using System.Configuration; 
using System.Threading.Tasks; 
using System.Linq.Expressions; 
using Microsoft.Azure.Documents;

public static class CosmosDbRepository<T> where T : class 
{ 
    private static readonly string dbId = ConfigurationManager.AppSettings["databaseId"]; 
    private static readonly string CollectionId = ConfigurationManager.AppSettings["collectionId"]; 
    private static DocumentClient client; 
    
    public static void Initialize() 
    { 
        client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["uri"]), ConfigurationManager.AppSettings["authenticationKey"]);
    } 
    public static async Task<IEnumerable<T>> GetItemsAsync() 
    { 
        IDocumentQuery<T> query = client.CreateDocumentQuery<T> (UriFactory.CreateDocumentCollectionUri(dbId, CollectionId)) .AsDocumentQuery(); 
        List<T> results = new List<T>(); 
        while (query.HasMoreResults) 
        { 
            results.AddRange(await query.ExecuteNextAsync<T>()); 
        } 
        return results; 
    } 
    public static async Task<Document> CreateItemAsync(T item) 
    {
            return await client.CreateDocumentAsync( UriFactory.CreateDocumentCollectionUri(dbId, CollectionId), item); 
    }
}