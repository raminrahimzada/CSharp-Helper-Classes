using System;
using System.Data;
using System.Web;

namespace raminrahimzada
{
    public class SessionManager
    {
        private static T Get<T>(string key)
        {
            object sessionObject = HttpContext.Current.Session[key];
            if (sessionObject == null)
            {
                return default(T);
            }
            return (T)HttpContext.Current.Session[key];
        }
        private static void Save<T>(string key, T entity)
        {
            HttpContext.Current.Session[key] = entity;
        }

        private static void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }
        public static string LoggedUserName
        {
            set
            {
                if (value != null) Save("LoggedUserName", value);
                else
                    Remove("LoggedUserName");
            }
            get { return Get<string>("LastErrorString"); }
        }         
        public static void RemoveAllSession()
        {
            HttpContext.Current.Session.Clear();
        }
    }    
}