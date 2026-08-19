/*
	DROP PROCEDURE [dbo].[usp_GetAuditLogIdByFilter]

	EXEC [dbo].[usp_GetAuditLogIdByFilter] NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetAuditLogIdByFilter] 'mstironek', '1B787F5D-2853-43ED-9740-CB80F6F4430D', NULL, NULL, NULL, 'Personnel'
	EXEC [dbo].[usp_GetAuditLogIdByFilter] 'ServiceAccount_KAF', '1B787F5D-2853-43ED-9740-CB80F6F4430D', '2019-04-17 00:28:29.5673517 +02:00', '2019-04-17 11:28:29.5673517 +02:00', NULL, 'Tank - Process Variable'
		
*/

CREATE PROCEDURE [dbo].[usp_GetAuditLogIdByFilter]
(
	@UserId nvarchar(100),
	@SiteGuid uniqueidentifier,
	@AuditedDateTimeStart DateTimeOffset,
	@AuditedDateTimeEnd DateTimeOffset,
	@ActionId nvarchar(20),
	@TypeId nvarchar(50)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_GetAuditLogIdByFilter] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns a distinct list of Audit Log Ids for a given set of filter values
	-- Notes:
	-- 1. @UserId: UserId to be used to set the AuditLog.CreatedBy filter
	-- 2. @SiteGuid : SiteGuid to be used to set the AuditLog.SiteGuid filter.
	-- 3. @AuditdDateTimeStart: Start filtering date on the AuditLog.AuditedDate field
	-- 4. @AuditdDateTimeEnd: End filtering date on the AuditLog.AuditedDate field
	-- 5. @ActionId: Filter Value on the AuditLog.ActionId field
	-- 6. @TypeId: Filter on the AuditLog.TypeId and AuditLog.ParentTypeId fields
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		
		SELECT DISTINCT a.ID
		FROM tblAuditLog a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.SiteGuid
		WHERE ((a.CreatedBy = @UserId) OR (@UserId IS NULL))
		AND ((a.SiteGuid = @SiteGuid) OR (@SiteGuid IS NULL))
		AND ((a.AuditedDate >= @AuditedDateTimeStart) OR (@AuditedDateTimeStart IS NULL))
		AND ((a.AuditedDate <= @AuditedDateTimeEnd) OR (@AuditedDateTimeEnd IS NULL))
		AND ((a.TypeID = @TypeId) OR (a.ParentTypeID = @TypeId) OR (@TypeId IS NULL))
		AND ((a.ActionID = @ActionId) OR (@ActionId IS NULL))
		ORDER BY a.ID DESC

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
						+ 'Procedure Name: dbo.usp_GetAuditLogIdByBatch' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END