// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldLevelConfigClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldLevelConfigClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [Serializable]
   [CollectionDataContract]
	public class FieldLevelConfigCollectionClass : List<FieldLevelConfigClass> { }

	[DataContract]
   [Serializable]
	public class FieldLevelConfigClass : BaseDataObject
	{
        public const string FLCModeGSOnly = "GlobalSpecific";
        public const string FLCModeVSandGS = "VersionSpecificAndGlobalSpecific";

        public enum FIELD_CONTROL_MODE
        {
            Unknown,
            Configurable,
            ParentSpecific,
            VersionSpecific,
            GlobalSpecific
        }

        [DataMember]
        public int FieldLevelConfigMatrixIndex { get; set; }

        [DataMember]
        public Guid EntitySegmentTemplateGuid { get; set; }

        [DataMember]
		public string EntityTypeId { get; set; }

        [DataMember]
		public string EntityTypeDisplayName { get; set; }

        [DataMember]
        public Guid SiteGroupGuid { get; set; }

        [DataMember]
        public string SiteGroupId { get; set; }

        [DataMember]
        public int HierarchyLevel { get; set; }

        [DataMember]
        public string FilterFieldName { get; set; }

        [DataMember]
        public string FilterDisplayName { get; set; }

        [DataMember]
        public Guid FilterValueGuid { get; set; }

        [DataMember]
        public string FilterValueName { get; set; }

        [DataMember]
        public string TargetField { get; set; }

        [DataMember]
        public bool IsExternalAttribute { get; set; }

        [DataMember]
        public string InternalFieldName { get; set; }

        [DataMember]
        public FIELD_CONTROL_MODE InheritedControlMode { get; set; }

        [DataMember]
        public FIELD_CONTROL_MODE ForwardControlMode { get; set; }

        //[DataMember]
        //public bool IsFCMVerSpecific { get; set; }

        public void Load(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            this.Reset();

            this.IdentityGuid = DataObject.getValue<Guid>(row["FieldConfigGuid"], Guid.Empty);
            this.EntitySegmentTemplateGuid = DataObject.getValue<Guid>(row["EntitySegmentTemplateGuid"], Guid.Empty);
            this.EntityTypeId = DataObject.getValue<string>(row["EntityTypeId"], null);
            this.EntityTypeDisplayName = DataObject.getValue<string>(row["EntityTypeDisplayName"], null);
            this.SiteGroupGuid = DataObject.getValue<Guid>(row["SiteGroupGuid"], Guid.Empty);
            this.SiteGroupId = DataObject.getValue<string>(row["SiteGroupId"], null);
            this.HierarchyLevel = DataObject.getValue<int>(row["HierarchyLevel"], 0);
            this.FilterFieldName = DataObject.getValue<string>(row["FilterFieldName"], null);
            this.FilterDisplayName = DataObject.getValue<string>(row["FilterDisplayName"], null);
            this.FilterValueGuid = DataObject.getValue<Guid>(row["FilterValueGuid"], Guid.Empty);
            this.FilterValueName = DataObject.getValue<string>(row["FilterValueName"], null);
            this.TargetField = DataObject.getValue<string>(row["TargetField"], null);
            this.IsExternalAttribute = DataObject.getValue<bool>(row["IsExternalAttribute"], false);
            this.InternalFieldName = DataObject.getValue<string>(row["InternalFieldName"], null);
            string inheritedControlModeStr = DataObject.getValue<string>(row["InheritedControlMode"], null);
            if (string.IsNullOrEmpty(inheritedControlModeStr)) this.InheritedControlMode = FIELD_CONTROL_MODE.Unknown;
            else this.InheritedControlMode = (FIELD_CONTROL_MODE)Enum.Parse(typeof(FIELD_CONTROL_MODE),inheritedControlModeStr);
            string forwardControlModeStr = DataObject.getValue<string>(row["ForwardControlMode"], null);
            this.ForwardControlMode = (FIELD_CONTROL_MODE)Enum.Parse(typeof(FIELD_CONTROL_MODE), forwardControlModeStr);
            //this.IsFCMVerSpecific = (this.ForwardControlMode == FIELD_CONTROL_MODE.VersionSpecific || this.ForwardControlMode == FIELD_CONTROL_MODE.GlobalSpecific);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
            this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
            //RowVersion = DataObject.getValue<Byte[]>(Row["_RowVersion"], null);
		}


	}
}
