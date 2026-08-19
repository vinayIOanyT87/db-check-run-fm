namespace FMPointTagArchive.Core.Interfaces.ServiceRequests
{
    using System;
    using System.Data.SqlTypes;

    public struct CurrentTankInventoryProcessorSR
    {
        public SqlGuid SiteGuid;
        public string SiteID;
        public DateTimeOffset BeginDate;
        public string refDataTableAsXML;
        public string CassandraConfiguration;
        public bool UseSmallFieldNames;
		public bool useDateOnly;
		public SqlGuid UserGuid;

	    public string CassandraUsername;

	    public string CassandraPassword;
    }
}
