using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.BusinessInterfaces;

namespace FMBusinessServices.ServiceClasses
{
	public class ADOFMSReverseLookupProcessorClass : IADOFMSReverseLookupProcessor
	{
		private ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		public ADOFMSReverseLookupProcessorClass ( )
		{
		}

		#region 
		public StringDO Process ( ADOFMSReverseLookupSR sr )
		{
			StringDO result = null;

			if (sr != null)
			{
				DataSet ds = null;

				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "EXEC fm_ADF_ADOFMSEntityLookup @EntityIdentifier, @EntityValue";
					cmd.Parameters.Add("@EntityIdentifier", SqlDbType.Int);
					cmd.Parameters.Add("@EntityValue", SqlDbType.NVarChar, 128);

					cmd.Parameters["@EntityIdentifier"].Value = (int)sr.EntityIdentifier;
					cmd.Parameters["@EntityValue"].Value = sr.EntityValue;

					ds = this.consolidatedDA.GetDataSet(cmd, sr.Security);
				}

				if (ds.Tables[0].Rows.Count > 0)
				{
					DataRow dr = ds.Tables[0].Rows[0];
					if (!dr.IsNull ( "Result" ))
					{
						result = new StringDO ( );
						result.Value = dr["Result"] as string;
					}
				}
			}

			return result;
		}
		#endregion
	}
}