using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

   public class LocalStorage
    {
        private readonly ConcurrentDictionary<string, string> _db = new ConcurrentDictionary<string, string>();
        public static LocalStorage Instance { get; set; }
        private readonly object _sync = new object();
        private LocalStorage()
        { }
        static LocalStorage()
        {
            Instance=new LocalStorage();
        }

        public void Set(string key, string value)
        {
            lock (_sync)
            {
                _db[key] = value;
            }
        }
        public string Get(string key)
        {
            lock (_sync)
            {
                string cavab;
                var isok = _db.TryGetValue(key, out cavab);
                return isok ? cavab : "";
            }
        }

        public void Remove(string key)
        {
            lock (_sync)
            {
                string cavab;
                _db.TryRemove(key, out cavab);
            }
        }

        //public void Save(string fileLocation)
        //{
        //    File.WriteAllText("SomeFile.Txt", new JavaScriptSerializer().Serialize(_db));
        //}

        //public void LoadFrom(string fileLocation)
        //{
        //    _db= new JavaScriptSerializer()
        //        .Deserialize<ConcurrentDictionary<string, string>>(File.ReadAllText(fileLocation));
        //}
    }
