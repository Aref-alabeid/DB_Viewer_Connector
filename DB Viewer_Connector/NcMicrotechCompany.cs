using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Viewer_Connector
{
    public class NcMicrotechCompany
    {

        #region Properties

        public string Name { get; set; }
        public string MandantenNr { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
