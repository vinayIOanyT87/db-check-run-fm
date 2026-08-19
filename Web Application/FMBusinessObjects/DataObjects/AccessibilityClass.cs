using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for Accessibility.
	/// </summary>
	[Serializable()]
	[CollectionDataContract]
	[KnownType(typeof(AccessibilityClass))]
	public class AccessibilityCollectionClass : CollectionBase
	{
		public void Add(AccessibilityClass Accessibility)
		{
			List.Add(Accessibility);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public void Remove(AccessibilityClass Accessibility)
		{
			int index = 0;
			foreach (AccessibilityClass Item in List)
			{
				if (Item.IdentityGuid == Accessibility.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public AccessibilityClass Item(int Index)
		{
			return (AccessibilityClass)List[Index];
		}
	}

	[Serializable()]
	[DataContract]
	public class AccessibilityClass :  IDataDictionary
	{
		[DataMember]
		protected Guid identityGuid;

		[DataMember]
		protected Guid accessibilityGuid;

		[DataMember]
		protected Guid userGuid;

		[DataMember]
		protected string settingKey;

		[DataMember]
		protected string settingValue;

		[DataMember]
		protected string valueRange;

		[DataMember]
		protected string valueType;

		[DataMember]
		protected string description;

		[DataMember]
		protected string displayName;

		[DataMember]
		protected DateTimeOffset _CreatedDate;

		[DataMember]
		protected string _CreatedBy;	
	
		[DataMember]
		protected DateTimeOffset _UpdatedDate;

		[DataMember]
		protected string _UpdatedBy;

		[XmlIgnore]
		public DateTimeOffset CreatedDate { get { return _CreatedDate; } set { _CreatedDate = value; } }

		[XmlIgnore]
		public string CreatedBy { get { return _CreatedBy; } set { _CreatedBy = value; } }

		[XmlIgnore]
		public DateTimeOffset UpdatedDate { get { return _UpdatedDate; } set { _UpdatedDate = value; } }

		[XmlIgnore]
		public string UpdatedBy { get { return _UpdatedBy; } set { _UpdatedBy = value; } }

		[XmlIgnore]
		public Guid IdentityGuid { get { return identityGuid; } set { identityGuid = value; } }

		[XmlIgnore]
		public Guid AccessibilityGuid { get { return accessibilityGuid; } set { accessibilityGuid = value; } }

		[XmlIgnore]
		public Guid UserGuid { get { return userGuid; } set { userGuid = value; } }

		[XmlIgnore]
		public string SettingKey { get { return settingKey; } set { settingKey = value; } }

		[XmlIgnore]
		public string SettingValue { get { return settingValue; } set { settingValue = value; } }

		[XmlIgnore]
		public string ValueRange { get { return valueRange; } set { valueRange = value; } }

		[XmlIgnore]
		public string ValueType { get { return valueType; } set { valueType = value; } }

		[XmlIgnore]
		public string DisplayName { get { return displayName; } set { displayName = value; } }

		[XmlIgnore]
		public string Description { get { return description; } set { description = value; } }

		public AccessibilityClass()
		{
			Initialize();
		}

		public AccessibilityClass(Guid _userGuid)
		{
			Initialize();
			this.userGuid = _userGuid;
		}

		string[] IDataDictionary.Keys(SecurityClass security)
		{
			string[] Keys ={	"Enable accessibility features",				
								 "Enable solid outlining of focused elements",				
								 "Enable -Please Wait- audio",					
								 "Enable session time out notification",			
								 "Minutes before session time out notification",	
								 "Enable keyboard access to menus",				
								 "Please Wait audio delay"						
								};
			return Keys;
		}

		private void Initialize()
		{
			this.IdentityGuid		= Guid.Empty;
			this.userGuid			= Guid.Empty;
			this.accessibilityGuid	= Guid.Empty;
			this.settingKey			= string.Empty;
			this.settingValue		= string.Empty;
			this.valueType			= string.Empty;
			this.valueRange			= string.Empty;
			this.description		= string.Empty;
			this.displayName		= string.Empty;

		}

		public void Reset()
		{
			Initialize();
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = Table.Rows[0];
			this.IdentityGuid = DataObject.getValue<Guid>(row["AccessibilityConfigurationSettingGuid"], Guid.Empty);
			this.userGuid = DataObject.getValue<Guid>(row["UserGuid"], Guid.Empty);
			this.accessibilityGuid = DataObject.getValue<Guid>(row["AccessibilityGuid"], Guid.Empty);
			this.settingKey = DataObject.getValue<string>(row["SettingKey"], string.Empty);
			this.settingValue = DataObject.getValue<string>(row["SettingValue"], string.Empty);
			this.valueType = DataObject.getValue<string>(row["ValueType"], string.Empty);
			this.valueRange = DataObject.getValue<string>(row["ValueRange"], string.Empty);
			this.description = DataObject.getValue<string>(row["Description"], string.Empty);
			this.displayName = DataObject.getValue<string>(row["DisplayName"], string.Empty);


		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblAccessibilityConfigurationSettings (AccessibilityConfigurationSettingGuid, UserGuid, AccessibilityGuid, SettingValue, UpdatedBy, UpdatedDate, CreatedBy, CreatedDate)  " +
				"SELECT @AccessibilityConfigurationSettingGuid, @UserGuid,  @AccessibilityGuid, @SettingValue,@UpdatedBy,GetDate(),@UpdatedBy,GetDate() FROM lookup.tblAccessibilities " +
				" WHERE AccessibilityGuid = @AccessibilityGuid";

			cmd.Parameters.Add("@AccessibilityConfigurationSettingGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AccessibilityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar);

			cmd.Parameters["@AccessibilityConfigurationSettingGuid"].Value = this.identityGuid;
			cmd.Parameters["@AccessibilityGuid"].Value = this.AccessibilityGuid;
			cmd.Parameters["@UserGuid"].Value = this.userGuid;
			cmd.Parameters["@SettingValue"].Value = this.settingValue;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblAccessibilityConfigurationSettings " +
				"SET SettingValue = @SettingValue," +
				"UpdatedBy    = @UpdatedBy" +
				" WHERE AccessibilityConfigurationSettingGuid = @AccessibilityConfigurationSettingGuid";

			cmd.Parameters.Add("@AccessibilityConfigurationSettingGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar);

			cmd.Parameters["@AccessibilityConfigurationSettingGuid"].Value = this.identityGuid;
			cmd.Parameters["@SettingValue"].Value = this.settingValue;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

		}
		/// <summary>
		/// Provide SQL to delete from DB
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		public void PurgeByUserSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblAccessibilityConfigurationSettings WHERE UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue("@UserGuid", UserGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblAccessibilityConfigurationSettings " +
				"WHERE AccessibilityConfigurationSettingGuid = @AccessibilityConfigurationSettingGuid";

			cmd.Parameters.Add("@AccessibilityConfigurationSettingGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AccessibilityConfigurationSettingGuid"].Value = IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT AccessibilityConfigurationSettingGuid, a.AccessibilityGuid, UserGuid, Settingkey, SettingValue, valueType, valueRange, Description, DisplayName " +
				" FROM tblAccessibilityConfigurationSettings s JOIN lookup.tblAccessibilities a ON s.AccessibilityGuid=a.AccessibilityGuid " +
				" WHERE UserGuid = @UserGuid AND SettingKey = @SettingKey";

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SettingKey", SqlDbType.NVarChar);
			cmd.Parameters["@UserGuid"].Value = this.userGuid;
			cmd.Parameters["@SettingKey"].Value = this.settingKey;
		}

		/// <summary>
		/// Returns all assessibility settings assigned to a user. If an accessibility setting is not assigned to user, default value is returned with a new guid.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT AccessibilityConfigurationSettingGuid, a.AccessibilityGuid,  UserGuid, Settingkey, SettingValue, valueType, valueRange, [Description], DisplayName " + 
							"FROM lookup.tblAccessibilities a  JOIN tblAccessibilityConfigurationSettings s ON s.AccessibilityGuid=a.AccessibilityGuid WHERE UserGuid =@userguid " + 
							"UNION " + 
							"SELECT newid() AS AccessibilityConfigurationSettingGuid, a.AccessibilityGuid,  @UserGuid AS UserGuid, Settingkey, DefaultSettingValue AS SettingValue,valueType, valueRange, [Description], DisplayName  " +
							"FROM lookup.tblAccessibilities a WHERE NOT EXISTS(SELECT TOP 1 1 FROM tblAccessibilityConfigurationSettings WHERE  a.AccessibilityGuid = a.AccessibilityGuid AND UserGuid=@UserGuid) " +
							"ORDER BY Settingkey";

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@UserGuid"].Value = userGuid;
		}



	}
}
