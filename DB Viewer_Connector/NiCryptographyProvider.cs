using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public interface NiCryptographyProvider
    {
        byte[] Decrypt(byte[] pUserData);
        string Decrypt(string pUserData);
        byte[] Encrypt(byte[] pEncryptedUserData);
        string Encrypt(string pEncryptedUserData);
    }
}
