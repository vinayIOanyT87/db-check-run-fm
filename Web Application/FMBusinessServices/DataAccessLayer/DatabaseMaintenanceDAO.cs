
namespace FMBusinessServices.DataAccessLayer
{
	using System.Data;
	using System.Data.SqlClient;

	internal static class DatabaseMaintenanceDAO
	{
		internal static void ReindexDatabaseSQL(SqlCommand cmd)
		{
			const string SQL = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[maint].[usp_ReindexDatabase_UpdateStats]') AND type in (N'P', N'PC'))
BEGIN 
	EXEC maint.usp_ReindexDatabase_UpdateStats 
END
ELSE
BEGIN
    RAISERROR ('Stored procedure usp_ReindexDatabase_UpdateStats does not exist', 16, 1, 'usp_ReindexDatabase_UpdateStats')
END
";

			cmd.CommandText = SQL;
			cmd.CommandTimeout = 1800;
			cmd.CommandType = CommandType.Text;
		}
	}
}