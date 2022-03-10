using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public static class NcRegistryHelper
    {
        #region Methodes

        #region Key Methods

        public static RegistryKey CreateKey(/*[NaNotNull]*/RegistryKey pRegistryBaseKey,
                                            /*[NaNotNullOrEmpty]*/string pSubKey)
        {
            return pRegistryBaseKey.CreateSubKey(pSubKey);
        }

        public static RegistryKey GetKey(/*[NaNotNull]*/RegistryKey pRegistryBaseKey,
                                         /*[NaNotNullOrEmpty]*/string pSubKey, bool pWritable)
        {
            return pRegistryBaseKey.OpenSubKey(pSubKey, pWritable);
        }

        public static List<string> GetSubKeyNames(/*[NaNotNull]*/RegistryKey pRegistryBaseKey,
                                                  /*[NaNotNullOrEmpty]*/string pSubKey)
        {
            RegistryKey tmpKey = GetKey(pRegistryBaseKey, pSubKey, false);
            List<string> tmpSubKeyNames = tmpKey.GetSubKeyNames().ToList();

            return tmpSubKeyNames;
        }

        public static List<RegistryKey> GetSubKeys(/*[NaNotNull]*/RegistryKey pRegistryBaseKey,
                                                   /*[NaNotNullOrEmpty]*/string pSubKey)
        {
            List<string> tmpSubKeyNames = GetSubKeyNames(pRegistryBaseKey, pSubKey);
            List<RegistryKey> tmpSubKeys = new List<RegistryKey>();

            foreach (string tmpSubKeyName in tmpSubKeyNames)
            {
                string tmpSubKeyPath = string.Format(@"{0}\{1}", pSubKey, tmpSubKeyName);
                RegistryKey tmpSubKey = GetKey(pRegistryBaseKey, tmpSubKeyPath, false);

                tmpSubKeys.Add(tmpSubKey);
            }

            return tmpSubKeys;
        }

        public static void DeleteKey(/*[NaNotNull]*/RegistryKey pRegistryBaseKey, /*[NaNotNullOrEmpty]*/string pSubKey)
        {
            pRegistryBaseKey.DeleteSubKey(pSubKey, true);
        }

        #endregion

        #region ValueMethods

        public static void SetValue(/*[NaNotNull]*/RegistryKey pRegistryKey, /*[NaNotNullOrEmpty]*/string pName,
                                    string pValue)
        {
            pRegistryKey.SetValue(pName, pValue, RegistryValueKind.String);
        }

        public static void SetValue(/*[NaNotNull]*/RegistryKey pRegistryKey, /*[NaNotNullOrEmpty]*/string pName,
                                    object pValue, RegistryValueKind pValueKind)
        {
            pRegistryKey.SetValue(pName, pValue, pValueKind);
        }

        public static object GetValue(/*[NaNotNull]*/RegistryKey pRegistryKey, /*[NaNotNullOrEmpty]*/string pName)
        {
            object tmpValue = pRegistryKey.GetValue(pName, DBNull.Value);
            if (tmpValue == DBNull.Value)
                throw new KeyNotFoundException();

            return tmpValue;
        }

        public static void DeleteValue(/*[NaNotNull]*/RegistryKey pRegistryKey, /*[NaNotNullOrEmpty]*/string pName)
        {
            try
            {
                pRegistryKey.DeleteValue(pName, true);
            }
            catch (ArgumentException)
            {
                throw new KeyNotFoundException();
            }
        }

        public static bool ContainsValue(/*[NaNotNull]*/RegistryKey pRegistryKey, /*[NaNotNullOrEmpty]*/string pName)
        {
            return pRegistryKey.GetValueNames().Contains(pName);
        }

        public static List<string> GetValueNames(/*[NaNotNull]*/RegistryKey pRegistryKey)
        {
            return pRegistryKey.GetValueNames().ToList();
        }

        #endregion

        #endregion



    }
}
