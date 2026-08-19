
CREATE FUNCTION [dbo].[udf_GetSyncContext]()
RETURNS 
	varbinary(128)
AS
BEGIN
	DECLARE @syncContext uniqueidentifier
	SELECT @syncContext = s.SynchronizationNodeGuid 
		FROM map.tblSessionToSQLProcess stsp
			INNER JOIN dbo.tblSessions s
				ON stsp.SessionGuid = s.SessionGuid
			WHERE stsp.SqlServerSessionID = @@SPID;

	RETURN(CAST(@syncContext AS Varbinary(128)));
END