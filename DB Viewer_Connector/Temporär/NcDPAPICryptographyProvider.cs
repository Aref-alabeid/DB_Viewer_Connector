using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public class NcDPAPICryptographyProvider : NiCryptographyProvider
    {
        #region Properties

        public byte[] Entropy { get; private set; }
        public DataProtectionScope DataProtectionScope { get; private set; }

        #endregion

        #region Constructors

        public NcDPAPICryptographyProvider(DataProtectionScope pDataProtectionScope, byte[] pEntropy)
        {
            Entropy = pEntropy;
            DataProtectionScope = pDataProtectionScope;
        }

        #endregion

        #region Methods

        public byte[] Encrypt(/*[NaNotNull]*/byte[] pUserData)
        {
            return ProtectedData.Protect(pUserData, Entropy, DataProtectionScope);
        }

        public string Encrypt(string pUserData)
        {
            byte[] tmpUserData = new byte[0];
            if (pUserData != null)
                tmpUserData = Encoding.UTF8.GetBytes(pUserData);

            byte[] tmpEncryptetData = this.Encrypt(tmpUserData);
            string tmpEncryptetDataString = Convert.ToBase64String(tmpEncryptetData);

            return tmpEncryptetDataString;
        }

        public byte[] Decrypt(/*[NaNotNull]*/ byte[] pEncryptedUserData)
        {
            return ProtectedData.Unprotect(pEncryptedUserData, Entropy, DataProtectionScope);
        }

        public string Decrypt(string pEncryptedUserData)
        {
            byte[] tmpEncryptedUserData = new byte[0];
            if (pEncryptedUserData != null)
                tmpEncryptedUserData = Convert.FromBase64String(pEncryptedUserData);

            byte[] tmpDecryptetData = this.Decrypt(tmpEncryptedUserData);
            string tmpDecryptetDataString = Encoding.UTF8.GetString(tmpDecryptetData);

            return tmpDecryptetDataString;
        }

        #endregion
    }
}