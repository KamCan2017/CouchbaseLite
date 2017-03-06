using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CBLiteApplication
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        private CBLModelManager<ProductModel> _cblManager;

        private List<ProductModel> _models;


        public MainWindowViewModel()
        {
            _cblManager = new CBLModelManager<ProductModel>();
            _models = new List<ProductModel>();
        }

        public List<ProductModel> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                OnPropertyChanged("Models");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void CreateProducts()
        {
            ProductModel pd = new ProductModel();
            pd.ID = Guid.NewGuid();
            pd.Name = "Computer";
            pd.Price = 500;
            _models.Add(pd);
            await _cblManager.Store(pd);

            pd = new ProductModel();
            pd.ID = Guid.NewGuid();
            pd.Name = "Phone";
            pd.Price = 800;
            _models.Add(pd);
            await _cblManager.Store(pd);

            pd = new ProductModel();
            pd.ID = Guid.NewGuid();
            pd.Name = "House";
            pd.Price = 1400000;
            _models.Add(pd);
            await _cblManager.Store(pd);
        }

        public async Task LoadDataFromCache()
        {
            ObjectCache cache = MemoryCache.Default;
            var ids = await _cblManager.GetAllStoredObjIds();
            IList<ProductModel> models = new List<ProductModel>();
            foreach (Guid id in ids)
            {
                var cachedObj = cache.Get(id.ToString());
                var t = cachedObj as Tuple<ProductModel, DateTime>;
                if (t != null)
                    models.Add(t.Item1);
            }

            Models = new List<ProductModel>(models);
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
