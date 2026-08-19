namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;

	[DataContract]
	[Serializable]
	public class TrendName
	{
		[DataMember]
		[FMPersistedField]
		public Guid TrendGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public string ID { get; set; }

		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		static public void EnumerateAvailableTrendNames(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT TrendGuid, ID, Description FROM tblTrend WHERE SiteGuid = @SiteGuid AND PointTemplateGuid IS NULL ORDER BY [ID]";
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}
	}
}
