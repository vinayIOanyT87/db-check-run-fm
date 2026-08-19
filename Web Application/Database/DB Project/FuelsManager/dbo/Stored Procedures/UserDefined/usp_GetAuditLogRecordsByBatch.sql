/*
	DROP PROCEDURE [dbo].[usp_GetAuditLogRecordsByBatch]
	DECLARE @i int
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 5000, 1, @i output
	Select @i
	DECLARE @i int
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'B6B0C108-C9F2-438F-9917-3189E471A3C2', NULL, NULL, NULL, NULL, NULL, NULL, 0, 5000, 1, @i output
	Select @i
	DECLARE @i int
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'B6B0C108-C9F2-438F-9917-3189E471A3C2', NULL, NULL, NULL, NULL, NULL, NULL, 1, 5000, 1, @i output
	Select @i

	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'B6B0C108-C9F2-438F-9917-3189E471A3C2', NULL, NULL, NULL, NULL, NULL, NULL, 1, 5000, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'D4F28A85-BD5B-4543-8C9B-32F0894FAFF1', NULL, NULL, NULL, NULL, NULL, NULL, 1, 50, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] '9E1344A3-86D0-48A3-8B5C-DD0BA02A23F4', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 50, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] '9E1344A3-86D0-48A3-8B5C-DD0BA02A23F4', 'E6D517E7-88BC-4575-ADF5-51F4E61F5733', NULL, NULL, NULL, NULL, NULL, NULL, 1, 50, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] '9E1344A3-86D0-48A3-8B5C-DD0BA02A23F4', 'E6D517E7-88BC-4575-ADF5-51F4E61F5733', '2019-05-17 09:06:04.0661691 +02:00', '2019-05-17 11:05:54.3525754 +02:00', NULL, NULL, NULL, NULL, 50, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] '9E1344A3-86D0-48A3-8B5C-DD0BA02A23F4', 'E6D517E7-88BC-4575-ADF5-51F4E61F5733', '2019-05-17 09:06:04.0661691 +02:00', '2019-05-17 11:05:54.3525754 +02:00', NULL, 'Transactions', NULL, NULL, 1, 50, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] '9E1344A3-86D0-48A3-8B5C-DD0BA02A23F4', 'E6D517E7-88BC-4575-ADF5-51F4E61F5733', '2019-05-17 09:06:04.0661691 +02:00', '2019-05-17 11:05:54.3525754 +02:00', NULL, 'Transactions', 'd7e7373d9cf44f36a23c8ab003ebb2d4', NULL, 1, 50, 1, @i output
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] '9E1344A3-86D0-48A3-8B5C-DD0BA02A23F4', 'E6D517E7-88BC-4575-ADF5-51F4E61F5733', '2019-05-17 09:06:04.0661691 +02:00', '2019-05-17 11:05:54.3525754 +02:00', 'Add', 'Transactions', NULL, NULL, 1, 50, 1, @i output	
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'D4F28A85-BD5B-4543-8C9B-32F0894FAFF1', NULL, NULL, NULL, NULL, NULL, NULL, 1, 500, 1, @i output
	DECLARE @i int
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'D4F28A85-BD5B-4543-8C9B-32F0894FAFF1', NULL, NULL, NULL, NULL, NULL, NULL, 50, 1, @i output
	Select @i
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'D4F28A85-BD5B-4543-8C9B-32F0894FAFF1', NULL, NULL, NULL, NULL, NULL, NULL, 1, 50, 2, @i output
	DECLARE @i int
	EXEC [dbo].[usp_GetAuditLogRecordsByBatch] NULL, 'D4F28A85-BD5B-4543-8C9B-32F0894FAFF1', NULL, NULL, NULL, NULL, NULL, NULL, 1, 50, 3, @i output
	Select @i
	
*/
CREATE PROCEDURE [dbo].[usp_GetAuditLogRecordsByBatch]
(
	@UserGuid uniqueidentifier,
	@SiteGuid uniqueidentifier,
	@AuditedDateTimeStart DateTimeOffset,
	@AuditedDateTimeEnd DateTimeOffset,
	@ActionId nvarchar(20),
	@TypeId nvarchar(50),
	@Id nvarchar(256),
	@SourceNode nvarchar(256),
	@IncludeMemberSites bit,
	@BatchSize int,
	@BatchNumber int,	
	@FullRecordCount int output
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_GetAuditLogRecordsByBatch] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the Audit Log records for a given set of parameters by batch
	-- Notes:
	-- 1. @UserGuid: UserGuid to be used to set the AuditLog.CreatedBy filter
	-- 2. @SiteGuid : SiteGuid to be used to set the AuditLog.SiteGuid filter.
	-- 3. @AuditdDateTimeStart: Start filtering date on the AuditLog.AuditedDate field
	-- 4. @AuditdDateTimeEnd: End filtering date on the AuditLog.AuditedDate field
	-- 5. @ActionId: Filter Value on the AuditLog.ActionId field
	-- 6. @TypeId: Filter on the AuditLog.TypeId and AuditLog.ParentTypeId fields
	-- 7. @Id: Filter on the AuditLog.Id field. Note: The @TypeId must also be provided when filtering on the AuditLog.Id field.
	-- 8. @SourceNode: Filter on the AuditLog.SourceNode field
	-- 9. @IncludeMemberSites: Include the site hierarchy below the given site
	-- 9. @batchSize: Maximum number of records to be returned. This value cannot be null.
	-- 10. @batchNumber: Batch segment to be returned. This value cannot be null.
	-- 11. @FullRecordCount: Full number of records for the whole query (not just for one batch). This number is only returned on the first batch.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @tblSiteHierarchy table
		(
			SiteGuid uniqueidentifier,
			SiteId nvarchar(30),
			HierarchyLevel int
		)

		DECLARE @userId nvarchar(100)
		SELECT @userId = UserId FROM tblUsers WHERE UserGuid = @UserGuid

		IF (@IncludeMemberSites = 1)
		BEGIN
			INSERT INTO @tblSiteHierarchy
			EXEC [erv].[usp_GetFLCSiteHierarchy] @SiteGuid, 0
		END
		ELSE
		BEGIN
			INSERT INTO @tblSiteHierarchy
			(SiteGuid, SiteId, HierarchyLevel)
			SELECT SiteGuid, Id, 0 FROM tblSites
			WHERE SiteGuid = @SiteGuid
		END

		DECLARE @rowsToSkip int
		SET @rowsToSkip = 0
		IF (@BatchNumber = 1) 
		BEGIN
			SELECT @FullRecordCount = COUNT(*)  
			FROM tblAuditLog a
			INNER JOIN @tblSiteHierarchy b
			ON b.SiteGuid = a.SiteGuid
			WHERE ((a.CreatedBy = @userId) OR (@UserGuid IS NULL))
			AND ((a.AuditedDate >= @AuditedDateTimeStart) OR (@AuditedDateTimeStart IS NULL))
			AND ((a.AuditedDate <= @AuditedDateTimeEnd) OR (@AuditedDateTimeEnd IS NULL))
			AND ((a.TypeID = @TypeId) OR (a.ParentTypeID = @TypeId) OR (@TypeId IS NULL))
			AND ((a.ID = @Id AND a.TypeID = @TypeId) OR (a.ID LIKE('%' + @Id + '%') AND a.ParentTypeID = @TypeId) OR (@Id IS NULL))
			AND ((a.ActionID = @ActionId) OR (@ActionId IS NULL))
			AND ((a.SourceNode LIKE('%' + @SourceNode + '%')) OR (@SourceNode IS NULL)) 
		END
		ELSE IF (@BatchNumber > 1)
		BEGIN
			SET @rowsToSkip = (@BatchNumber - 1) * @BatchSize
		END

		
		SELECT a.AuditLogGuid, a.SiteGuid, b.SiteId, a.SourceNode, a.AuditContext, a.SessionID, 
		a.ActionID, a.TypeID, a.ParentTypeID, a.ID, a.PropertyID, a.NewValue, a.OldValue,
		a.CreatedDate, a.CreatedBy, a.AuditedDate  
		FROM tblAuditLog a
		INNER JOIN @tblSiteHierarchy b
		ON b.SiteGuid = a.SiteGuid
		WHERE ((a.CreatedBy = @userId) OR (@UserGuid IS NULL))
		AND ((a.AuditedDate >= @AuditedDateTimeStart) OR (@AuditedDateTimeStart IS NULL))
		AND ((a.AuditedDate <= @AuditedDateTimeEnd) OR (@AuditedDateTimeEnd IS NULL))
		AND ((a.TypeID = @TypeId) OR (a.ParentTypeID = @TypeId) OR (@TypeId IS NULL))
		AND ((a.ID = @Id AND a.TypeID = @TypeId) OR (a.ID LIKE('%' + @Id + '%') AND a.ParentTypeID = @TypeId) OR (@Id IS NULL))
		AND ((a.ActionID = @ActionId) OR (@ActionId IS NULL))
		AND ((a.SourceNode LIKE('%' + @SourceNode + '%')) OR (@SourceNode IS NULL))
		ORDER BY a.AuditedDate DESC, a._RowVersion DESC
		OFFSET @rowsToSkip ROWS
		FETCH NEXT @BatchSize ROWS ONLY

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
						+ 'Procedure Name: dbo.usp_GetAuditLogRecordsByBatch' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END