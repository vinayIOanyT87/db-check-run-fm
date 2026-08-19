using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Constants
{
	public class ReportTypesClass
	{
		public enum ReportTypes
		{
			AVIATION_RPT,
			OIL_GAS_RPT,
			QUERY_RPT, BOL_RPT,
			SECURE_RPT,
			FESS_RPT,
			DOD_SHIPMENT_RCV_RPT,
			DOD_EOM_RPT,
			ADF_BULK_RPT,
			NONE_RPT,
			VARIABLE_PARAMETERS,
			DOD_TDR_RPT,
			METER_RECONCILIATION_RPT,
			OVRDUE_TST_RPRT
		};
	}
}
