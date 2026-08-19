using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;
using System;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessServices.ServiceClasses
{

    public class GaugeTypes : IGaugeTypes
	{
		#region Private data Members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public GaugeTypes()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public GaugeTypeClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var gaugeType = new GaugeTypeClass ();

			using (var cmd = new SqlCommand())
			{
				gaugeType.SelectSQL(cmd, identityGuid);
				gaugeType.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return gaugeType;
		}

		public GaugeTypeClass GetByID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var gaugeType = new GaugeTypeClass();

			using (var cmd = new SqlCommand())
			{
				gaugeType.SelectByIDSQL(cmd, id);
				gaugeType.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return gaugeType;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			return GetByID( security, id).IdentityGuid;
		}

		public GaugeTypeClass GetByIndex(SecurityClass security, int gaugeTypeIndex)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var gaugeType = new GaugeTypeClass();

			using (var cmd = new SqlCommand())
			{
				gaugeType.SelectByIindexSQL(cmd, gaugeTypeIndex);
				gaugeType.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return gaugeType;
		}

		public GaugeTypeCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var gaugeType = new GaugeTypeClass();
			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				gaugeType.EnumerateSQL(cmd, security);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var gaugeTypeCollectionClass = new GaugeTypeCollectionClass();

			DataTable table = dataSet.Tables[0];

			while (table.Rows.Count != 0)
			{
				gaugeType = new GaugeTypeClass();
				gaugeType.Load(dataSet);
				gaugeTypeCollectionClass.Add(gaugeType);
				table.Rows.RemoveAt(0);
			}

			return gaugeTypeCollectionClass;
		}

	}
}