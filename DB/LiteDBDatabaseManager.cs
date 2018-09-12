// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class LiteDBDatabaseManager<T>
    {
        private readonly LiteDatabase _db;
        private readonly string _fileLocation;
        private readonly object _sync = new object();

        public LiteDBDatabaseManager(string fileLocation)
        {
            _fileLocation = fileLocation;
            _db = new LiteDatabase(_fileLocation);
        }

        private LiteCollection<T> Collection => _db.GetCollection<T>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T Get(BsonValue key)
        {
            lock (_sync)
            {
                var exists = Collection.FindById(key);
                return exists;
            }
        }

        public bool Has(BsonValue key)
        {
            lock (_sync)
            {
                var exists = Collection.FindById(key);
                return !EqualityComparer<T>.Default.Equals(exists, default(T));
            }
        }

        public void Remove(BsonValue key)
        {
            lock (_sync)
            {
                Collection.Delete(key);
            }
        }

        public IEnumerable<T> GetAll()
        {
            lock (_sync)
            {
                return Collection.FindAll();
            }
        }

        public void Set(T obj)
        {
            lock (_sync)
            {
                Collection.Upsert(obj);
            }
        }

        public void DropDatabase()
        {
            if (File.Exists(_fileLocation))
                File.Delete(_fileLocation);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            // free native resources
        }
    }
