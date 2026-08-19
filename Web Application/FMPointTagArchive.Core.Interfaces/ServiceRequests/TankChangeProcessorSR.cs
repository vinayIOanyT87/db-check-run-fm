namespace FMPointTagArchive.Core.Interfaces.ServiceRequests
{
	using System;
	using System.Data.SqlTypes;

	public struct TankChangeProcessorSR
	{
		public SqlGuid SiteGuid;
		public string SiteID;
		public DateTimeOffset BeginDate;
		public DateTimeOffset EndDate;
		public string refDataTableAsXML;
		public string CassandraConfiguration;
		public bool UseSmallFieldNames;
		public SqlGuid UserGuid;

		public string CassandraUsername;
		public string CassandraPassword;
	}
}
