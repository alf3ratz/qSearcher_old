using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QSearcher_
{
    class CosmosLoad
    {
        public CosmosLoad() { }
        // Данные для подключения к БД
        const string DataBaseName = "QSearcher";
        const string UserCollection = "Users";
        static readonly Uri UserCollectionURI = UriFactory.CreateDocumentCollectionUri(DataBaseName, UserCollection);
        const string EventCollection = "Events";
        static readonly Uri EventCollectionURI = UriFactory.CreateDocumentCollectionUri(DataBaseName, EventCollection);
        static DocumentClient Client = new DocumentClient(new System.Uri(Keys.URL), Keys.AuthKey);
        /// <summary>
        /// Инициализация бвзы данных
        /// </summary>
        /// <returns>The initialize.</returns>
        static async Task<bool> Initialize()
        {
            if (Client != null)
                return true;
            try
            {
                Client = new DocumentClient(new Uri(Keys.URL), Keys.AuthKey);
                // Create the database 
                await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DataBaseName });
                // Create the collection
                await Client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(DataBaseName),
                    new DocumentCollection { Id = UserCollection },
                    new RequestOptions { OfferThroughput = 400 }
                );
                // Create the collection
                await Client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(DataBaseName),
                    new DocumentCollection { Id = EventCollection },
                    new RequestOptions { OfferThroughput = 400 }
                );
            }
            catch (Exception)
            {
                Client = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Добавление записи в базу данных
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="user">User.</param>
        public async static Task InsertObject<T>(T obj)
        {
            if (!await Initialize())
                return;
            Func<T, bool> func;
            if (obj is PersonLogin)
                func = o => (o as PersonLogin).Email == (obj as PersonLogin).Email;
            else
                func = o => (o as Data.Event).Title == (obj as Data.Event).Title;
            var uri = obj is PersonLogin ? UserCollectionURI : EventCollectionURI;
            var query = Client.CreateDocumentQuery<T>(uri,
                new FeedOptions { EnableCrossPartitionQuery = true }).Where(func).ToList();
            if (query.Count == 0)
                await Client.CreateDocumentAsync(uri, obj);
        }

        /// <summary>
        /// Обновление записи в бвзе данных
        /// </summary>
        /// <returns>User.</returns>
        /// <param name="user">User.</param>
        public async static Task UpdateEvent(Data.Event obj)
        {
            if (!await Initialize())
                return;
            var query = Client.CreateDocumentQuery<Data.Event>(EventCollectionURI,
               new FeedOptions { EnableCrossPartitionQuery = true }).Where(o => o.Title == obj.Title).ToList();
            if (query.Count != 0)
            {
                var docUri = UriFactory.CreateDocumentUri(DataBaseName, EventCollection, query[0].Id);
                obj.Id = query[0].Id;
                await Client.ReplaceDocumentAsync(docUri, obj);
            }
            else
                await InsertObject(obj);
        }

        /// <summary>
        /// Удаляет запись о пользователе из базы данных
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="user">User.</param>
        public async static Task DeleteObject<T>(T obj)
        {
            if (!await Initialize())
                return;
            var id = obj is PersonLogin ? (obj as PersonLogin).Id : (obj as Data.Event).Id;
            var collection = obj is PersonLogin ? UserCollection : EventCollection;
            var docUri = UriFactory.CreateDocumentUri(DataBaseName, collection, id);
            await Client.DeleteDocumentAsync(docUri);
        }

        /// <summary>
        /// Получает данные о пользователе с заданным именем
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userName">User name.</param>
        public async static Task<T> GetObject<T>(string q) where T : class
        {
            var objects = await GetObjects<T>();

            if (typeof(T).Equals(typeof(PersonLogin)))
            {
                foreach (var user in objects)
                    if ((user as PersonLogin).Id == q || (user as PersonLogin).Email == q)
                        return user;
            }
            else
            {
                foreach (var e in objects)
                    if ((e as Data.Event).Title == q)
                        return e;
            }
            return null;
        }

        /// <summary>
        /// Получает весь список пользователей 
        /// </summary>
        /// <returns>The users.</returns>
        public async static Task<List<T>> GetObjects<T>()
        {
            var objects = new List<T>();

            if (!await Initialize())
                return objects;

            var collection = typeof(T).Equals(typeof(PersonLogin)) ? UserCollection : EventCollection;

            objects = Client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DataBaseName, collection),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                                   .ToList();
            return objects;
        }
    }
}

