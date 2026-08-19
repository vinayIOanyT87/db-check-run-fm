namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;

    public enum PANELTYPE
    {
        Standard = 0,

        Detail = 1
    };

	[Serializable]
	public class DrawingCollection : List<Drawing>
	{
	}

	[DataContract]
	[Serializable]
	public class Drawing : BaseDataObject
	{
		[DataMember]
		[FMPersistedField]
		public string Image { get; set; }

		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		[FMPersistedField]
		public Guid DrawingGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		public List<Guid> AnimationGuidList { get; set; }
			
		[DataMember]
      [FMPersistedField]
      public Guid? PointTemplateGuid { get; set; }

      [DataMember]
      [FMPersistedField]
      public PANELTYPE PanelType { get; set; }

		[DataMember]
		[FMPersistedField]
		public Boolean? Published { get; set; }


		public void SelectSQL(SqlCommand cmd, Guid drawingGuid)
		{
			cmd.CommandText = "SELECT * FROM tblDrawings WHERE DrawingGuid = @DrawingGuid";
			cmd.Parameters.AddWithValue("@DrawingGuid", drawingGuid);
		}

		public static void SelectByIdSQL(SqlCommand cmd, Guid siteGuid, string id)
		{
			cmd.CommandText = "SELECT * FROM tblDrawings WHERE SiteGuid = @SiteGuid AND UPPER(ID) = UPPER(@ID)";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@ID", id);
		}
	}
}
