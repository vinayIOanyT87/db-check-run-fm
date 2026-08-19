namespace FMBusinessObjects.DataObjects
{
	using System.Data.SqlClient;

	public class FMFatalErrorHandlerClass
	{
		public static readonly string Header = "<h1>FuelsManager Fatal Error Notification</h1>";
		public static readonly string ShutdownMessage = "Shutting down SQL Server and stopping FuelsManager.";
		public static readonly string ContactMessage = "Please contact the system administrator.";
		public static readonly string NotificationFormatter = "<h2 style=\"width:100%\">{0} " + ShutdownMessage + "</h2>";
		public static readonly string Footer = "<h2>" + ContactMessage + "</h2>";


		public void ShutDownSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblSessionToSQLProcess DELETE FROM tblSessions  SHUTDOWN ";
		}
	}
}
