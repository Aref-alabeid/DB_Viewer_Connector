using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public class NcRegistryKeyValueStorageProvider : NiKeyValueStorageProvider
    {
        #region Member

        public RegistryKey RegistryKeyLocation { get; internal set; }

        #endregion

        #region Constructors

        public NcRegistryKeyValueStorageProvider(/*[NaNotNull]*/RegistryKey pRegistryKey)
        {
            RegistryKeyLocation = pRegistryKey;
        }

        #endregion

        #region Methods

        public void SaveEntry(/*[NaNotNullOrEmpty]*/string pKey, string pValue)
        {
            string[] tmpValue = new string[0];
            if (pValue != null)
                tmpValue = new string[] { pValue };

            NcRegistryHelper.SetValue(RegistryKeyLocation, pKey, tmpValue, RegistryValueKind.MultiString);
        }

        public string LoadEntry(/*[NaNotNullOrEmpty]*/string pKey)
        {
            string[] tmpMultiStringValue = (string[])NcRegistryHelper.GetValue(RegistryKeyLocation, pKey);

            string tmpValue = null;
            if (tmpMultiStringValue.Length > 0)
                tmpValue = tmpMultiStringValue[0];

            return tmpValue;
        }

        public Dictionary<string, string> LoadAllEntries()
        {
            Dictionary<string, string> tmpEntries = new Dictionary<string, string>();

            List<string> tmpValueNames = NcRegistryHelper.GetValueNames(RegistryKeyLocation);
            foreach (string tmpValueName in tmpValueNames)
            {
                string tmpValue = LoadEntry(tmpValueName);
                tmpEntries.Add(tmpValueName, tmpValue);
            }

            return tmpEntries;
        }

        public bool RemoveEntry(/*[NaNotNullOrEmpty]*/string pKey)
        {
            try
            {
                NcRegistryHelper.DeleteValue(RegistryKeyLocation, pKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ContainsEntry(/*[NaNotNullOrEmpty]*/string pKey)
        {
            return NcRegistryHelper.ContainsValue(RegistryKeyLocation, pKey);
        }

        #endregion
    }
}
