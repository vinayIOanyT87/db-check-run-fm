using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
	[Serializable]
	public class FlightSelectionDO
	{
		[DataMember]
		protected string flight;
		[DataMember]
		protected string gate;
		[DataMember]
		protected DateTimeOffset etd;
		[DataMember]
		protected DateTimeOffset eta;
		[DataMember]
		protected string load;		
		 
		public string Flight
		{
			get { return flight; }
			set { flight = value; }
		}
		public string Gate
		{
			get { return gate; }
			set { gate = value; }
		}
		public DateTimeOffset ETD
		{
			get { return etd; }
			set { etd = value; }
		}
		public DateTimeOffset ETA
		{
			get { return eta; }
			set { eta = value; }
		}
		public string Load
		{
			get { return load; }
			set { load = value; }
		}

		public void LoadRow(DataRow row)
		{
			if (row != null)
			{
				this.Flight = row.IsNull("Flight") ? "" : (string)row["Flight"];
				this.Gate = row.IsNull("Gate") ? "" : (string)row["Gate"];
				this.ETA = row.IsNull("ETA") ? DateTimeOffset.Now : DataObject.getValue<DateTimeOffset>(row["ETA"], TimeConverter.Today());
				this.ETD = row.IsNull("ETD") ? DateTimeOffset.Now : DataObject.getValue<DateTimeOffset>(row["ETA"], TimeConverter.Today());
				this.Load = row.IsNull("Load") ? "" : (string)row["Load"];
			}
		}
	}

	[Serializable]
	[CollectionDataContract]
	public class FlightSelectionCollectionDO : List<FlightSelectionDO> 
	{
		public void Get(SqlCommand sqlcommand, 
								SecurityClass security, 
								string operatorID,
								bool filterByOperatorID,
								string vehicleID,
								bool filterByVehicleID,
								string gateID,
								bool filterByGateID,
								int hoursInPast,
								int hoursInFuture)
		{
			sqlcommand.CommandText = "exec usp_MobileFlightSelctionSelectIssueTransactions 	@OperatorID, @filterOperatorID, @VehicleID," +
				" @filterVehicleID, @GateID, @filterGateID, @HoursInPast, @HoursInFuture";

			SqlParameter parm1 = new SqlParameter("@OperatorID", SqlDbType.NVarChar, 100) { Value = operatorID };
			sqlcommand.Parameters.Add(parm1);
			SqlParameter parm2 = new SqlParameter("@filterOperatorID", SqlDbType.Bit) { Value = filterByOperatorID };
			sqlcommand.Parameters.Add(parm2);
			SqlParameter parm3 = new SqlParameter("@VehicleID", SqlDbType.NVarChar, 100) { Value = vehicleID };
			sqlcommand.Parameters.Add(parm3);
			SqlParameter parm4 = new SqlParameter("@filterVehicleID", SqlDbType.Bit) { Value = filterByVehicleID };
			sqlcommand.Parameters.Add(parm4);
			SqlParameter parm5 = new SqlParameter("@GateID", SqlDbType.NVarChar, 100) { Value = gateID };
			sqlcommand.Parameters.Add(parm5);
			SqlParameter parm6 = new SqlParameter("@filterGateID", SqlDbType.Bit) { Value = filterByGateID };
			sqlcommand.Parameters.Add(parm6);
			SqlParameter parm7 = new SqlParameter("@HoursInPast", SqlDbType.Int) { Value = hoursInPast };
			sqlcommand.Parameters.Add(parm7);
			SqlParameter parm8 = new SqlParameter("@HoursInFuture", SqlDbType.Int) { Value = hoursInFuture };
			sqlcommand.Parameters.Add(parm8);
		}

		public void Load(DataSet dataSet)
		{
			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if ((table != null) && (table.Rows != null) && (table.Rows.Count > 0))
				{
					for (int i = 0; i < table.Rows.Count; i++)
					{
						DataRow row = table.Rows[i];
						FlightSelectionDO flight = new FlightSelectionDO();
						flight.LoadRow(row);
						Add(flight);
					}
				}
			}
		}
	}
}
