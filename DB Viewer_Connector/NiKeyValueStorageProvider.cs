using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public interface NiKeyValueStorageProvider
    {
        void SaveEntry(string pKey, string pValue);
        string LoadEntry(string pKey);
        Dictionary<string, string> LoadAllEntries();
        bool RemoveEntry(string pKey);
        bool ContainsEntry(string pKey);
    }
}