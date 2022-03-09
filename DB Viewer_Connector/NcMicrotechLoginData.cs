using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public class NcMicrotechLoginData
    {
        #region Properties

        public int ModuleID { get; set; }
        public string Firma { get; set; }
        public string Benutzername { get; set; }
        public string Passwort { get; set; }
        public string Mandant { get; set; }

        #endregion
    }
}
