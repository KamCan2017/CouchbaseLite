using Couchbase.Lite;
using System.Windows;

namespace CBLiteApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            InitializeLocalCouchbaseDatabase();
        }
        private void InitializeLocalCouchbaseDatabase()
        {
            string databaseName = "cbl-cache-localhost";
            Couchbase.Lite.Storage.ForestDB.Plugin.Register();
            Manager manager = Manager.SharedInstance;
            Database db = manager.OpenDatabase(databaseName, new DatabaseOptions
            {
                Create = true,
                StorageType = StorageEngineTypes.ForestDB
            });

            //can be keept low, we are not syncing the cache
            db.SetMaxRevTreeDepth(3);

            var view = db.GetView("Types");
            view.SetMap((doc, emit) =>
            {
                emit(doc["Type"], doc["CacheTime"]);
            }, "1.0");
        }

    }
}
