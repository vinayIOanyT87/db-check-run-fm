using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
	[Serializable ( )]
	public class BaseObjectClass
	{
		public static string SystemKey = "System";
		public static string LoadRackKey = "Load Rack";
		public static string TransactionKey = "Transactions";
		public static string DataSynchronization = "Data Synchronization";
		public static string WebApplicationKey = "Web Application";
		public static string PointManagerKey = "Point Manager";
        public static string FCEE = "FCEE";
        public static string License = "License";
    }
}