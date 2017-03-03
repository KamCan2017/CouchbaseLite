using Couchbase.Lite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CBLiteApplication
{
    public class CBLModelManager<T> where T : BaseModel
    {
        private readonly string _databaseName = "cbl-cache-localhost";

        public CBLModelManager()
        {
            LoadStoredObjectInCache();
        }

        public async Task<T> Store(T obj)
        {
            return await Task.Run(() => 
            {
                Manager manager = Manager.SharedInstance;
                Database db = manager.GetDatabase(_databaseName);
                Document document = db.GetDocument(obj.ID.ToString());
                DateTime now = DateTime.Now;

                IDictionary<string, object> properties = new Dictionary<string, object>();
                properties["Model"] = JsonConvert.SerializeObject(obj);
                properties["CacheTime"] = now;
                properties["Type"] = obj.GetType().Name;

                var savedRevision = document.PutProperties(properties);
                if (savedRevision == null)
                    return null;

                return PutToCache(obj, now);
            }
            );           
        }

        public async Task<IEnumerable<Guid>> GetAllStoredObjIds()
        {
            return await Task.Run(() => 
            {
                Manager manager = Manager.SharedInstance;
                Database db = manager.GetDatabase(_databaseName);

                var query = db.CreateAllDocumentsQuery();
                query.AllDocsMode = AllDocsMode.AllDocs;
                var rows = query.Run();
                var ids = new List<Guid>();

                foreach (var row in rows)
                {
                    if (row.Document == null || row.Document.Deleted)
                        continue;
                    var obj = JsonConvert.DeserializeObject<T>(row.Document.GetProperty<string>("Model"));
                    if(obj != null)
                      ids.Add(Guid.Parse(row.DocumentId));
                }

                return ids;
            }
            
            );
            
        }


        private void LoadStoredObjectInCache()
        {
            Manager manager = Manager.SharedInstance;
            Database db = manager.GetDatabase(_databaseName);

            var query = db.CreateAllDocumentsQuery();
            query.AllDocsMode = AllDocsMode.AllDocs;
            var rows = query.Run();

            foreach (var row in rows)
            {
                if (row.Document == null || row.Document.Deleted)
                    return;

                DateTime cacheTime = row.Document.GetProperty<DateTime>("CacheTime");
                var obj = JsonConvert.DeserializeObject<T>(row.Document.GetProperty<string>("Model"));
                if(obj != null)
                  PutToCache(obj, cacheTime);
            }
        }
        

        private T PutToCache(T obj, DateTime cacheTime)
        {
            var itemToCache = new Tuple<T, DateTime>(obj, cacheTime);

            CacheItemPolicy policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(1) };

            ObjectCache cache = MemoryCache.Default;
            cache.Set(obj.ID.ToString(), itemToCache, policy);

            return itemToCache.Item1;
        }
    }
}
