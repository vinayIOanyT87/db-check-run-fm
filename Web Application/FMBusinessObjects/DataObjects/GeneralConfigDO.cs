using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	[KnownType(typeof(GeneralConfigAlias))]
	[KnownType(typeof(DropdownValuePairDO))]
	public class GeneralConfigDO : DataObject
	{
		#region private attributes
		[DataMember]
		private Guid _GeneralConfigurationGuid;
		[DataMember]
		private Guid _SiteGuid;
		[DataMember]
		private bool _ConsortiumFlag;
		[DataMember]
		private bool _ShowDeletedTrxFlag;
		[DataMember]
		private bool _SetBeginInvToZeroFlag;
		[DataMember]
		private string _ReverseTrxDateMode;
		[DataMember]
		private int _ForceCloseout;
		[DataMember]
		private string _SecurityCode;
		[DataMember]
		private string _AuthorizationCode;
		[DataMember]
		private double _MeterTolerance;
		[DataMember]
		private string _CreatedBy;
		[DataMember]
		private string _UpdatedBy;
		[DataMember]
		private GeneralConfigSR.GeneralConfigAdjustMethod _Method;
		[DataMember]
		private List<GeneralConfigAlias> _AdjustmentAliasList;
		[DataMember]
		private ArrayList _UnassignedAliasList;
		[DataMember]
		private System.DateTimeOffset _CreatedDate;
		[DataMember]
		private System.DateTimeOffset _UpdatedDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the general configuration DO class.
		/// </summary>
		public GeneralConfigDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set and get the general configuration Guid attribute.
		/// </summary>
		public Guid GeneralConfigurationGuid
		{
			get { return this._GeneralConfigurationGuid; }
			set { this._GeneralConfigurationGuid = value; }
		}

		/// <summary>
		/// This property will set and get the site guid attribute.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this._SiteGuid; }
			set { this._SiteGuid = value; }
		}

		/// <summary>
		/// This property will set and get the adjustment distribution method attribute.
		/// </summary>
		public GeneralConfigSR.GeneralConfigAdjustMethod AdjustmentMethod
		{
			get { return this._Method; }
			set { this._Method = value; }
		}

		/// <summary>
		/// This property will set and get the adjustment distribution method attribute.
		/// </summary>
		public string AdjustmentMethodString
		{
			get
			{
				switch (this._Method)
				{
					case GeneralConfigSR.GeneralConfigAdjustMethod.ALLOCATION:
						return "Allocation";
					case GeneralConfigSR.GeneralConfigAdjustMethod.MANUAL:
						return "Manual";
					case GeneralConfigSR.GeneralConfigAdjustMethod.THROUGHPUT:
						return "Throughput";
					default:
						return "Manual";
				}
			}

			set
			{
				string temp = value;
				this._Method = GeneralConfigSR.GeneralConfigAdjustMethod.MANUAL;

				if ((temp != null) && (temp.Length > 0))
				{
					if (temp == "Allocation")
						this._Method = GeneralConfigSR.GeneralConfigAdjustMethod.ALLOCATION;

					if (temp == "Throughput")
						this._Method = GeneralConfigSR.GeneralConfigAdjustMethod.THROUGHPUT;
				}
			}
		}

		/// <summary>
		/// This property will set and get the consortium flag attribute.
		/// </summary>
		public bool UseConsortium
		{
			get { return this._ConsortiumFlag; }
			set { this._ConsortiumFlag = value; }
		}

		/// <summary>
		/// This property will set and get the show deleted transaction flag attribute.
		/// </summary>
		public bool ShowDeletedTransactions
		{
			get { return this._ShowDeletedTrxFlag; }
			set { this._ShowDeletedTrxFlag = value; }
		}

		/// <summary>
		/// This property will set and get the set begin inventory to zero flag attribute.
		/// True means to set the 1st day of the month beginning inventory to zero if 
		/// the last day of the previous month did not have a physical inventory.
		/// </summary>
		public bool SetBeginInventoryToZeroFlag
		{
			get { return this._SetBeginInvToZeroFlag; }
			set { this._SetBeginInvToZeroFlag = value; }
		}

		/// <summary>
		/// This property will set and get the current reverse transaction date mode (current/original) attribute.
		/// </summary>
		public string ReverseTransactionDateMode
		{
			get { return this._ReverseTrxDateMode; }
			set { this._ReverseTrxDateMode = value; }
		}

		/// <summary>
		/// This property will set and get the forced closeout number of days attribute.
		/// </summary>
		public int ForceCloseout
		{
			get { return this._ForceCloseout; }
			set
			{
				this._ForceCloseout = value;

				// Ensure the forced closeout days is between 1 and 180.
				if ((this._ForceCloseout < 1) || (this._ForceCloseout > 180))
					this._ForceCloseout = 1;
			}
		}

		/// <summary>
		/// This property will set and get the forced closeout number of days attribute.
		/// </summary>
		public string ForceCloseoutString
		{
			get
			{
				string temp = "";

                if (this._ForceCloseout == 0)
                    temp = "Disabled";
                else if (this._ForceCloseout == 1)
                    temp = "  1 day";
                else
                    temp = string.Format("{0,3} days", this._ForceCloseout);

				return temp;
			}

            set
            {
                string temp = value;
                int index = temp.LastIndexOf(" day");

				if (index > 0)
					temp = temp.Substring(0, index);
				else
					temp = "0";

				this._ForceCloseout = System.Convert.ToInt32(temp);

				// Ensure the forced closeout days is between 1 and 180.
				if ((this._ForceCloseout < 0) || (this._ForceCloseout > 180))
					this._ForceCloseout = 1;
			}
		}

		/// <summary>
		/// This property will set and get the ExSTAR security code attribute.
		/// </summary>
		public string SecurityCode
		{
			get { return this._SecurityCode; }
			set { this._SecurityCode = value; }
		}

		/// <summary>
		/// This property will set and get the ExSTAR authorization code attribute.
		/// </summary>
		public string AuthorizationCode
		{
			get { return this._AuthorizationCode; }
			set { this._AuthorizationCode = value; }
		}

		public double MeterTolerance
		{
			get { return this._MeterTolerance; }
			set { this._MeterTolerance = value; }
		}

		/// <summary>
		/// This property will get the assigned adjustment alias list attribute.
		/// </summary>
		public List<GeneralConfigAlias> AdjustmentAliasList
		{
			get { return this._AdjustmentAliasList; }
		}

		/// <summary>
		/// This property will get the unassigned adjustment alias list attribute.
		/// </summary>
		public ArrayList UnassignedAliasList
		{
			get { return this._UnassignedAliasList; }
		}

		/// <summary>
		/// This property will set and get the created by attribute.
		/// </summary>
		public string CreatedBy
		{
			get { return this._CreatedBy; }
			set { this._CreatedBy = value; }
		}

		/// <summary>
		/// This property will set and get the updated by attribute.
		/// </summary>
		public string UpdatedBy
		{
			get { return this._UpdatedBy; }
			set { this._UpdatedBy = value; }
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will return a SQL string that will retrieve the general configuration
		/// data for a given site.
		/// </summary>
		/// <param name="siteGuid"></param>
		/// <returns></returns>
		public void GetGeneralConfigSQL(SqlCommand cmd, Guid siteGuid)
		{
			string select = "SELECT SiteGuid, Method, ConsortiumFlag, ShowDeletedTrxFlag, SetBeginInventoryToZeroFlag, " +
							 "ReverseTrxDateMode, ForcedCloseout, SecurityCode, AuthorizationCode, MeterTolerance, " +
							 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, GeneralConfigurationGuid ";
			string from = "FROM tblGeneralConfiguration ";
			string where = "WHERE SiteGuid = @SiteGuid ";

			cmd.CommandText = select + from + where;
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		/// <summary>
		/// This method will return an insert SQL string to insert the general configuration
		/// data.
		/// </summary>
		/// <returns></returns>
		public void InsertGeneralConfigSQL(SqlCommand cmd)
		{
			DateTimeOffset currentDate = DateTimeOffset.Now;

			cmd.CommandText = "INSERT INTO tblGeneralConfiguration (" +
							 "SiteGuid, Method, ConsortiumFlag, ShowDeletedTrxFlag, SetBeginInventoryToZeroFlag, " +
							 "ReverseTrxDateMode, ForcedCloseout, SecurityCode, AuthorizationCode, MeterTolerance, " +
							 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, GeneralConfigurationGuid " +
							 ") VALUES (" +
							 "@SiteGuid, " +
							 "@Method, " +
							 "@ConsortiumFlag, " +
							 "@ShowDeletedTrxFlag, " +
							 "@SetBeginInventoryToZeroFlag, " +
							 "@ReverseTrxDateMode, " +
							 "@ForcedCloseout, " +
							 "@SecurityCode, " +
							 "@AuthorizationCode, " +
							 "@MeterTolerance, " +
							 "@CreatedBy, " +
							 "@CreatedDate, " +
							 "@UpdatedBy, " +
							 "@UpdatedDate," +
							 "@GeneralConfigurationGuid)";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@Method", (int)_Method);
			cmd.Parameters.AddWithValue("@ConsortiumFlag", _ConsortiumFlag);
			cmd.Parameters.AddWithValue("@ShowDeletedTrxFlag", _ShowDeletedTrxFlag);
			cmd.Parameters.AddWithValue("@SetBeginInventoryToZeroFlag", _SetBeginInvToZeroFlag);
			cmd.Parameters.AddWithValue("@ReverseTrxDateMode", _ReverseTrxDateMode);
			cmd.Parameters.AddWithValue("@ForcedCloseout", _ForceCloseout);
			cmd.Parameters.AddWithValue("@SecurityCode", _SecurityCode);
			cmd.Parameters.AddWithValue("@AuthorizationCode", _AuthorizationCode);
			cmd.Parameters.AddWithValue("@MeterTolerance", _MeterTolerance);
			cmd.Parameters.AddWithValue("@CreatedDate", currentDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", currentDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@GeneralConfigurationGuid", _GeneralConfigurationGuid);
		}

		/// <summary>
		/// This method will return an update SQL string to udpate the general configuration
		/// data.
		/// </summary>
		/// <returns></returns>
		public void UpdatedGeneralConfigSQL(SqlCommand cmd)
		{
			DateTimeOffset currentDate = DateTimeOffset.Now;

			cmd.CommandText = "UPDATE tblGeneralConfiguration SET SiteGuid = @SiteGuid " +
									", Method = @Method " +
									", ConsortiumFlag = @ConsortiumFlag " +
									", ShowDeletedTrxFlag = @ShowDeletedTrxFlag" +
									", SetBeginInventoryToZeroFlag = @SetBeginInventoryToZeroFlag" +
									", ReverseTrxDateMode = @ReverseTrxDateMode " +
									", ForcedCloseout = @ForcedCloseout " +
									", SecurityCode = @SecurityCode " +
									", AuthorizationCode = @AuthorizationCode " +
									", MeterTolerance = @MeterTolerance " +
									", UpdatedBy = @UpdatedBy " +
									", UpdatedDate = @UpdatedDate " +
									" WHERE SiteGuid = @SiteGuid ";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@Method", (int)_Method);
			cmd.Parameters.AddWithValue("@ConsortiumFlag", _ConsortiumFlag);
			cmd.Parameters.AddWithValue("@ShowDeletedTrxFlag", _ShowDeletedTrxFlag);
			cmd.Parameters.AddWithValue("@SetBeginInventoryToZeroFlag", _SetBeginInvToZeroFlag);
			cmd.Parameters.AddWithValue("@ReverseTrxDateMode", _ReverseTrxDateMode);
			cmd.Parameters.AddWithValue("@ForcedCloseout", _ForceCloseout);
			cmd.Parameters.AddWithValue("@SecurityCode", _SecurityCode);
			cmd.Parameters.AddWithValue("@AuthorizationCode", _AuthorizationCode);
			cmd.Parameters.AddWithValue("@MeterTolerance", _MeterTolerance);
			cmd.Parameters.AddWithValue("@UpdatedDate", currentDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
		}


		/// <summary>
		/// This method will return an update SQL string to udpate the general configuration
		/// data.
		/// </summary>
		/// <returns></returns>
		public void PurgeGeneralConfigSQL(SqlCommand cmd)
		{
			DateTimeOffset currentDate = DateTimeOffset.Now;

			cmd.CommandText =	"DELETE FROM dbo.tblGeneralConfigurationAliases"
								+	" WHERE GeneralConfigurationGuid = (SELECT GeneralConfigurationGuid FROM dbo.tblGeneralConfiguration WHERE SiteGuid = @SiteGuid)"
								+	" DELETE FROM dbo.tblGeneralConfiguration "
								+	" WHERE SiteGuid = @SiteGuid ";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
		}


		#endregion

		#region SQL Load Methods
		/// <summary>
		/// This method will accept a data set that contains the general configuration
		/// data retrieved from the database and load the object members with the data.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public void LoadGeneralConfigSQL(System.Data.DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					this._GeneralConfigurationGuid = DataObject.getValue<Guid>(row["GeneralConfigurationGuid"], Guid.Empty);
					this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
					this._Method = DataObject.getValue<GeneralConfigSR.GeneralConfigAdjustMethod>(row["Method"],
																	GeneralConfigSR.GeneralConfigAdjustMethod.MANUAL);
					this._ConsortiumFlag = DataObject.getValue<bool>(row["ConsortiumFlag"], false);
					this._ShowDeletedTrxFlag = DataObject.getValue<bool>(row["ShowDeletedTrxFlag"], false);
					this._SetBeginInvToZeroFlag = DataObject.getValue<bool>(row["SetBeginInventoryToZeroFlag"], false);
					this._ReverseTrxDateMode = DataObject.getValue<string>(row["ReverseTrxDateMode"], "Current");
					this._ForceCloseout = DataObject.getValue<int>(row["ForcedCloseout"], 1);
					this._SecurityCode = DataObject.getValue<string>(row["SecurityCode"], "");
					this._AuthorizationCode = DataObject.getValue<string>(row["AuthorizationCode"], "");
					this._MeterTolerance = DataObject.getValue<double>(row["MeterTolerance"], 0.0);
					this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
					this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
					this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
					this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);

					// Ensure that the forced closeout days is between 1 and 180.
					if ((this._ForceCloseout < 0) || (this._ForceCloseout > 180))
					{
						this._ForceCloseout = 1;
					}
				}
			}
		}

		#endregion

		#region private methods
		/// <summary>
		/// This method will initialize this object to its initialize state.
		/// </summary>
		private void Init()
		{
			this._GeneralConfigurationGuid = Guid.Empty;
			this._SiteGuid = Guid.Empty;
			this._Method = GeneralConfigSR.GeneralConfigAdjustMethod.MANUAL;
			this._ConsortiumFlag = false;
			this._ShowDeletedTrxFlag = false;
			this._SetBeginInvToZeroFlag = false;
			this._ReverseTrxDateMode = "Current";
			this._ForceCloseout = 1;
			this._SecurityCode = "";
			this._AuthorizationCode = "";
			this._MeterTolerance = 0.0;
			this._CreatedBy = BaseDataObject.ADMIN;
			this._UpdatedBy = BaseDataObject.ADMIN;
			this._AdjustmentAliasList = new List<GeneralConfigAlias>();
			this._UnassignedAliasList = new ArrayList();
			this._CreatedDate = System.DateTimeOffset.Now;
			this._UpdatedDate = _CreatedDate;
		}
		#endregion

		#region Override Methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}
		#endregion
	}
}
