namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Runtime.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
	[CollectionDataContract]
	[KnownType(typeof(AirplaneTankClass))]
	public class AirplaneTankCollectionClass : List<AirplaneTankClass> { }

	[EntityImportExportWorksheet("EQUIPMENT TYPE CLASSES")]
	[DataContract]
	[Serializable]
	public class AirplaneTankClass : BaseDataObject, IComparable
	{
		#region Protected data members

		[DataMember]
		protected string _CustomerTankID;

		[DataMember]
		protected int _Position;

		[DataMember]
		protected int _GuiOrder;

		[DataMember]
		protected EQUIPMENT_TYPE_LOCATION _LocationIndex;

		[DataMember]
		protected Guid _ParentGuid = Guid.Empty;
		
		[DataMember]
		protected string _Description;

		[DataMember]
		protected SIDouble _Capacity;

		#endregion

		#region Properties
		[EntityImportExport("TYPECLASSID*", 100, "ID")]
		public override string ID { get { return this._ID; } set {
		    this.SetString("ID", 50, value, ref this._ID); } }

		public string Alias { get { return this._ID; } set {
		    this.SetString("ID", 50, value, ref this._ID); } }

		[EntityImportExport("DESCRIPTION", 100)]
		public string Description { get { return this._Description; } set {
		    this.SetString("Description", 50, value, ref this._Description); } }

		[EntityImportExport("CAPACITY", 80)]
		public string Capacity { get { return this._Capacity.ToString(); } set {
		    this.SetSIDouble("Capacity", value, ref this._Capacity); } }

		public double CapacityValue { get { return this._Capacity.Value; } }

		[EntityImportExport("CUSTOMERTANKID", 100)]
		public string CustomerTankID { get { return this._CustomerTankID; } set {
		    this._CustomerTankID = value; } }

		[EntityImportExport("POSITION", 50)]
		public int Position { get { return this._Position; } set {
		    this._Position = value; } }

		[EntityImportExport("GUIORDER", 50)]
		public int GuiOrder { get { return this._GuiOrder; } set {
		    this._GuiOrder = value; } }

		[EntityImportExport("LOCATIONINDEX", 50)]
		public EQUIPMENT_TYPE_LOCATION LocationIndex { get { return this._LocationIndex; } set {
		    this._LocationIndex = value; } }

		[EntityImportExport("LOCATION", 50)]
		public string Location { get { return TypeLocation(this._LocationIndex); } set {
		    this._LocationIndex = TypeLocation(value); } }

		[EntityImportExport("PARENTGUID*", 100)]
		public Guid ParentGuid { get { return this._ParentGuid; } set {
		    this._ParentGuid = value; } }

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.EQUIPMENT_TYPE; }
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO dbo.tblAirplaneTank " +
					"(EquipmentTypeGuid," +
					"Alias," +
					"Description," +
					"Capacity," +
					"AirlineTankID," +
					"Position," +
					"DisplayOrder," +
					"Location," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"TankGuid" +
					") VALUES (" +
					"@ParentGuid," +
					"@ID," +
					"@Description," +
					"@Capacity," +
					"@CustomerTankID," +
					"@Position," +
					"@GuiOrder," +
					"@LocationIndex," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@TankGuid)";


			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Capacity", SqlDbType.Float);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CustomerTankID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Position", SqlDbType.SmallInt);
			cmd.Parameters.Add("@GuiOrder", SqlDbType.SmallInt);
			cmd.Parameters.Add("@LocationIndex", SqlDbType.Int);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@Description"].Value = this._Description;
			cmd.Parameters["@Capacity"].Value = this._Capacity.SIValue;
			cmd.Parameters["@CustomerTankID"].Value = this._CustomerTankID;
			cmd.Parameters["@Position"].Value = this._Position;
			cmd.Parameters["@GuiOrder"].Value = this._GuiOrder;
			cmd.Parameters["@LocationIndex"].Value = (int)this.LocationIndex;
			cmd.Parameters["@ParentGuid"].Value = this._ParentGuid;
			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@TankGuid"].Value = this._IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE dbo.tblAirplaneTank " +
						"SET EquipmentTypeGuid = @ParentGuid," +
						"Alias = @ID," +
						"Description = @Description," +
						"AirlineTankID = @CustomerTankID," +
						"Position = @Position," +
						"DisplayOrder = @GuiOrder," +
						"Location = @LocationIndex," +
						"Capacity = @Capacity," +
						"UpdatedDate = @UpdatedDate," +
						"UpdatedBy = @UpdatedBy " +
				  "WHERE TankGuid = @TankGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Capacity", SqlDbType.Float);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CustomerTankID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Position", SqlDbType.SmallInt);
			cmd.Parameters.Add("@GuiOrder", SqlDbType.SmallInt);
			cmd.Parameters.Add("@LocationIndex", SqlDbType.Int);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@Description"].Value = this._Description;
			cmd.Parameters["@Capacity"].Value = this._Capacity.SIValue;
			cmd.Parameters["@CustomerTankID"].Value = this._CustomerTankID;
			cmd.Parameters["@Position"].Value = this._Position;
			cmd.Parameters["@GuiOrder"].Value = this._GuiOrder;
			cmd.Parameters["@LocationIndex"].Value = (int)this.LocationIndex;
			cmd.Parameters["@ParentGuid"].Value = this._ParentGuid;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@TankGuid"].Value = this.IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM dbo.tblAirplaneTank WHERE TankGuid = @TankGuid";

			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TankGuid"].Value = this.IdentityGuid;
		}


		#endregion

		#region Comparable method
		int IComparable.CompareTo(object obj)
		{
			AirplaneTankClass AirplaneTank = obj as AirplaneTankClass;
			if (AirplaneTank == null)
				throw new Exception("Invalid AirplaneTank");

			if(this.ParentGuid == AirplaneTank.ParentGuid && this.ID.CompareTo(AirplaneTank.ID) == 0)
			{
				return 0;
			}
			if(this.ID.CompareTo(AirplaneTank.ID) != 0)
			{
				return this.ID.CompareTo(AirplaneTank.ID);
			}
			return -1;
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the equipment type class.
		/// </summary>
		public AirplaneTankClass()
			: this(EngineeringUnit.FmvUsGal,0)
		{
			NumberFormatInfo currentInfo = NumberFormatInfo.CurrentInfo;
			EngineeringUnit units = EngineeringUnit.FmvUsGal;
		    this._Capacity = new SIDouble(units, currentInfo, 0.0);
		    this._Capacity.numberDecimalDigits = 0;
		}

		/// <summary>
		/// This constructor will initialize the equipment type class based on the site.
		/// </summary>
		/// <param name="Site"></param>
		public AirplaneTankClass(EngineeringUnit units, int decimalPlaces)
		{
			
			NumberFormatInfo CurrentInfo = NumberFormatInfo.CurrentInfo;
			this.Reset();
		    this._Capacity = new SIDouble(units, CurrentInfo, 0.0);
		    this._Capacity.numberDecimalDigits = decimalPlaces;
		}
		#endregion

		#region Public methods

		public void SetCapacityParameters(EngineeringUnit units, int decimalPlaces)
		{
		    this._Capacity.Units = units;
		    this._Capacity.numberDecimalDigits = decimalPlaces;
		}

		public static EQUIPMENT_TYPE_LOCATION TypeLocation(string location)
		{
			if (location == "Center")
				return EQUIPMENT_TYPE_LOCATION.Center;
			else if (location == "Right")
				return EQUIPMENT_TYPE_LOCATION.Right;
			else if (location == "Left")
				return EQUIPMENT_TYPE_LOCATION.Left;
			else
				return EQUIPMENT_TYPE_LOCATION.MAX_EQUIPMENT_TYPE_LOCATION;
		}

		public static string TypeLocation(EQUIPMENT_TYPE_LOCATION location)
		{
			switch (location)
			{
				case EQUIPMENT_TYPE_LOCATION.Center:
					return "Center";
				case EQUIPMENT_TYPE_LOCATION.Right:
					return "Right";
				case EQUIPMENT_TYPE_LOCATION.Left:
					return "Left";
				default:
					return "Undefined";
			}
		}

		public override void Load(Object o)
		{
			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

			    this.IdentityGuid = DataObject.getValue<Guid>(Row["TankGuid"], Guid.Empty);
			    this._ParentGuid = DataObject.getValue<Guid>(Row["EquipmentTypeGuid"], Guid.Empty);
			    this.ID = DataObject.getValue<string>(Row["Alias"], "");
			    this._Description = DataObject.getValue<string>(Row["Description"], "");
			    this._CustomerTankID = DataObject.getValue<string>(Row["AirlineTankID"], "");
			    this._Position = DataObject.getValue<int>(Row["Position"], 0);
			    this._GuiOrder = DataObject.getValue<int>(Row["DisplayOrder"], 0);
			    this._LocationIndex = DataObject.getValue<EQUIPMENT_TYPE_LOCATION>(Row["Location"], EQUIPMENT_TYPE_LOCATION.Center);
			    this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			    this._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
			    this._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			    this._Capacity.SIValue = DataObject.getValue<double>(Row["Capacity"], 0.0);

			}

		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * " +
					" FROM dbo.tblAirplaneTank  " + SQLUpdateLock(bInTransaction) +
				" WHERE dbo.tblAirplaneTank.TankGuid = @AirplaneTankGuid";

			cmd.Parameters.Add("@AirplaneTankGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AirplaneTankGuid"].Value = this.IdentityGuid;
		}


		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * " +
					" FROM dbo.tblAirplaneTank " + SQLUpdateLock(bInTransaction) +
					" WHERE dbo.tblAirplaneTank.EquipmentTypeGuid = @ParentGuid AND dbo.tblAirplaneTank.Alias = @ID";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ParentGuid"].Value = this.ParentGuid;
		}

		public void SelectByParentGuid(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * " +
					" FROM dbo.tblAirplaneTank " + SQLUpdateLock(bInTransaction) +
					" WHERE dbo.tblAirplaneTank.EquipmentTypeGuid = @ParentGuid" +
					" ORDER BY dbo.tblAirplaneTank.DisplayOrder";

			cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier, 100);
			if (this.ParentGuid == null || this.ParentGuid == Guid.Empty)
			{
				cmd.Parameters["@ParentGuid"].Value = Guid.Empty;
			}
			else
			{
				cmd.Parameters["@ParentGuid"].Value = this.ParentGuid;
			}
		}

		#endregion

		#region Private and internal methods

		public override void Reset()
		{
			base.Reset();
		    this._ParentGuid = Guid.Empty;
		    this._Description = "";
		    this._CustomerTankID = "";
		    this._Position = 0;
		    this._GuiOrder = 0;
		    this._LocationIndex = EQUIPMENT_TYPE_LOCATION.Center;
		}
		#endregion
	}
}
