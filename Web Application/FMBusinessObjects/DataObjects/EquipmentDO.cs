
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Xml.Serialization;
	using System.Runtime.Serialization;

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(EquipmentDO))]
    public class EquipmentDOCollection : List<EquipmentDO>
    {
    }

    [XmlRoot("Equipment")]
	[XmlType("Equipment")]
	[DebuggerDisplay("EquipmentDO ID={RegistrationID}")]
	[DataContract]
    [Serializable]
	public class EquipmentDO : DataObject
	{
		#region Attributes
		[DataMember] private string registrationID = string.Empty;
		[DataMember] private string serialNumber = string.Empty;
		[DataMember] private string equipmentType = string.Empty;
		[DataMember] private string equipmentModel = string.Empty;
		[DataMember] private string companyEquipmentID = string.Empty;
		[DataMember] private Guid equipmentGuid = Guid.Empty;
		[DataMember] private string equipmentRefID = string.Empty;
        [DataMember] private Guid equipmentTypeGuid = Guid.Empty;
        [DataMember] private string equipmentSecondaryTypeName = string.Empty;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public EquipmentDO()
		{
		}

		/// <summary>
		/// Constructor for populating with equipment class.
		/// </summary>
		/// <param name="equipment">Equipment Class.</param>
		public EquipmentDO(EquipmentClass equipment)
		{
			RegistrationID		= equipment.ID;
			EquipmentModel		= equipment.Model;
			EquipmentType		= EquipmentTypeClass.TypeID(equipment.Type);
			SerialNumber		= equipment.SerialNumber;
			CompanyEquipmentID	= equipment.CompanyEquipmentID;
			EquipmentGuid		= equipment.MasterRecordGuid;
			EquipmentRefID		= equipment.Xref;
			EquipmentTypeGuid	= equipment.EquipmentTypeGuid;
            EquipmentSecondaryTypeName = GetEquipmentSecondaryTypeName(equipment.EquipmentTypeGuid);
		}

		/// <summary>
		/// Copy Constructor
		/// </summary>
		/// <param name="src">Equipment DO</param>
		public EquipmentDO(EquipmentDO src)
		{
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            this.RegistrationID		= src.RegistrationID;
			this.EquipmentModel		= src.EquipmentModel;
			this.EquipmentType		= src.EquipmentType;
			this.SerialNumber		= src.SerialNumber;
			this.CompanyEquipmentID = src.CompanyEquipmentID;
			this.EquipmentGuid		= src.EquipmentGuid;
			this.EquipmentRefID		= src.EquipmentRefID;
			this.EquipmentTypeGuid	= src.EquipmentTypeGuid;
            this.EquipmentSecondaryTypeName = src.EquipmentSecondaryTypeName;
		}
		#endregion

		#region Properties
		public string EquipmentRefID
		{
			get { return this.equipmentRefID; }
			set { this.equipmentRefID = value; }
		}

		public string RegistrationID
		{
			get { return registrationID; }
			set { registrationID = value; }
		}

		public string SerialNumber
		{
			get { return serialNumber; }
			set { serialNumber = value; }
		}

		public string EquipmentType
		{
			get { return equipmentType; }
			set { equipmentType = value; }
		}

		public string EquipmentModel
		{
			get { return equipmentModel; }
			set { equipmentModel = value; }
		}

		public string CompanyEquipmentID
		{
			get { return companyEquipmentID; }
			set { companyEquipmentID = value; }
		}

		[XmlIgnore]
		public Guid EquipmentGuid
		{
			get { return equipmentGuid; }
			set { equipmentGuid = value; }
		}

        [XmlIgnore]
        public Guid EquipmentTypeGuid
        {
            get { return equipmentTypeGuid; }
            set { equipmentTypeGuid = value; }
        }

        public string EquipmentSecondaryTypeName
        {
            get { return GetEquipmentSecondaryTypeName(equipmentTypeGuid); }
            set { equipmentSecondaryTypeName = value; }
        }
        #endregion Properties

        #region Methods

        private string GetEquipmentSecondaryTypeName(Guid equipmentTypeGuid)
        {
            return string.Empty;
        }

        #endregion
        #region Abstract members
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Abstract members
	}
}
