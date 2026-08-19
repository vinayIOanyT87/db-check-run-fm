using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMExportService
{
    public class LegacyCompanyNameRecord
    {
        public string FMEnterpriseCompanyID { get; set; }
        public string LegacyCompanyID { get; set; }
        public string LegacyCompanyCode { get; set; }
        public string LegacyCustomCode { get; set; }
        

        public string getValue()
        {
            return LegacyCompanyID + "," + LegacyCompanyCode + "," + LegacyCustomCode;
        }
    }
}
