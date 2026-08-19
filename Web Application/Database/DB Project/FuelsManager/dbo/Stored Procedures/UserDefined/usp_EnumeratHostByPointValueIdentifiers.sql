CREATE PROCEDURE [dbo].[usp_EnumerateHostByPointValueIdentifiers]
(
	@PointValueIdentityTable dbo.GuidListType   READONLY
)
AS
BEGIN
	BEGIN TRY

		SET NOCOUNT ON
		SELECT DISTINCT ps.HostName, pvit.Guid FROM @PointValueIdentityTable pvit
			INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = pvit.Guid
			LEFT JOIN map.tblPointToPointService ptps ON ptps.PointGuid = pt.PointGuid
			LEFT JOIN dbo.tblPointService ps ON ps.PointServiceGuid = ptps.PointServiceGuid
			UNION
			SELECT DISTINCT ps.HostName, pvit.Guid FROM @PointValueIdentityTable pvit
			INNER JOIN dbo.tblPointProperty pp ON pp.PointPropertyGuid = pvit.Guid
			LEFT JOIN map.tblPointToPointService ptps ON ptps.PointGuid = pp.PointGuid
			LEFT JOIN dbo.tblPointService ps ON ps.PointServiceGuid = ptps.PointServiceGuid
			UNION
			SELECT DISTINCT ps.HostName, pvit.Guid FROM @PointValueIdentityTable pvit
			INNER JOIN dbo.tblPoint p ON p.PointGuid = pvit.Guid
			LEFT JOIN map.tblPointToPointService ptps ON ptps.PointGuid = p.PointGuid
			LEFT JOIN dbo.tblPointService ps ON ps.PointServiceGuid = ptps.PointServiceGuid
			UNION
			SELECT DISTINCT 'Deleted', pvit.Guid FROM @PointValueIdentityTable pvit
			LEFT JOIN dbo.tblPointTag pt ON pt.PointTagGuid = pvit.Guid
			LEFT JOIN dbo.tblPointProperty pp ON pp.PointPropertyGuid = pvit.Guid
			LEFT JOIN dbo.tblPoint p ON p.PointGuid = pvit.Guid
			WHERE pt.PointTagGuid IS NULL AND pp.PointPropertyGuid IS NULL AND p.PointGuid IS NULL
			ORDER BY Hostname
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;      
				      
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: usp_EnumerateHostByPointValueIdentifier' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      

	END CATCH    
END
GO


