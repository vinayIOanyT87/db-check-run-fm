/*
	DROP PROCEDURE [map].[usp_CreateLedgerAggregateColumnToSiteMapping]

*/  
CREATE PROCEDURE [map].[usp_CreateLedgerAggregateColumnToSiteMapping]
(
		@EntityRecordGuid uniqueidentifier=NULL
	,	@AssignedFromSiteGuid uniqueidentifier=NULL
	,	@AssignedToSiteGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL

)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_CreateLedgerAggregateColumnToSiteMapping]
	-- Author: Vivian Kiarie /  Hansraj Bapoo
	-- Version/Date: 1.0.002 / 2013-12-30 14:44:52.5114383 -05:00
	-- Purpose: Insert into table [map].[tblEntityLedgerAggregateColumnToSite]
		-- Notes:
	-- 1. @EntityRecordGuid: Record Guid of the entity record to be mapped. 
	-- 2. @AssignedFromSiteGuid: SiteGroup from which the entity record should be mapped from.
	-- 3. @AssignedToSiteGuid: Site/SiteGroup to which the entity record should be mapped to.
	-- 4. If the AssignedToSite is an indirect child of the AssignedFromSite, the entity-to-site mapping request is cascaded as necessary.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@AssignedToSiteGuid = @AssignedFromSiteGuid)
		BEGIN
			-- Create the self-site EntityToSite assignment
		INSERT INTO [map].[tblEntityLedgerAggregateColumnToSite]
		(
		  [LedgerAggregateColumnGuid]
		  ,	[SiteGuid]
		  ,	[CreatedDate]
		  ,	[CreatedBy]
		  ,	[UpdatedDate]
		  ,	[UpdatedBy]
		  ,	[AssignedFromSiteGuid]
		)
		VALUES
		(
		  @EntityRecordGuid
		  ,	@AssignedToSiteGuid
		  ,	@CreatedDate
		  ,	@CreatedBy
		  ,	@CreatedDate
		  ,	@CreatedBy
		  ,	@AssignedFromSiteGuid
		)
 
	-- Cascading Assignments do not apply to the base entity assignment (assignment of the entity record with its owner site guid).
			RETURN;  
		END 

		DECLARE @tblSiteHierarchy TABLE
		(
			ParentSiteGuid uniqueidentifier
			, ChildSiteGuid uniqueidentifier
			, ParentSiteId nvarchar(30)
			, ChildSiteId nvarchar(30)
			, HierarchyLevel int
		)
		INSERT INTO @tblSiteHierarchy
		SELECT ParentSiteGuid, ChildSiteGuid, ParentSiteId, ChildSiteId, HierarchyLevel 
		FROM [erv].[udf_GetReverseSiteHierarchy] (@AssignedToSiteGuid, @AssignedFromSiteGuid) ORDER BY HierarchyLevel

		--Cascade the entity-to-site mappings from the original parent sitegroup down to the target site.
		DECLARE @parentSiteGuid uniqueidentifier
		DECLARE @childSiteGuid uniqueidentifier
		DECLARE @hierarchyLevel int		

		DECLARE TableCursor CURSOR FOR 
		  SELECT ParentSiteGuid, ChildSiteGuid, HierarchyLevel FROM @tblSiteHierarchy 
		  WHERE ParentSiteGuid <> ChildSiteGuid 
		  ORDER BY HierarchyLevel
		OPEN TableCursor 

			FETCH NEXT FROM TableCursor INTO @parentSiteGuid, @childSiteGuid, @hierarchyLevel 
 
			WHILE @@FETCH_STATUS = 0  
			BEGIN 			
				INSERT INTO [map].[tblEntityLedgerAggregateColumnToSite]
				(
					 [LedgerAggregateColumnGuid]
					 ,	[SiteGuid]
					 ,	[CreatedDate]
					 ,	[CreatedBy]
					 ,	[UpdatedDate]
					 ,	[UpdatedBy]
					 ,	[AssignedFromSiteGuid]
				)
				SELECT @EntityRecordGuid, @childSiteGuid, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, @parentSiteGuid
				WHERE NOT EXISTS
				(
					SELECT * FROM [map].[tblEntityLedgerAggregateColumnToSite]
					WHERE LedgerAggregateColumnGuid = @EntityRecordGuid
					AND SiteGuid = @ChildSiteGuid
					AND AssignedFromSiteGuid = @parentSiteGuid				
				)
						
				FETCH NEXT FROM TableCursor INTO @parentSiteGuid, @childSiteGuid, @hierarchyLevel 
			END 
		CLOSE TableCursor 
		DEALLOCATE TableCursor 

 
	END TRY
	BEGIN CATCH
		DECLARE	@_ErrMessage NVARCHAR(2048)
		, @_ErrNumber INT
		, @_ErrProcName NVARCHAR(126)
		, @_ErrLineNumber INT;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		IF(@_ErrNumber = 547 AND CHARINDEX('Uniqueness',@_ErrMessage,0) <> 0)
			RAISERROR('Operation would result in duplicate identifiers.',16,1);
		ELSE
		BEGIN
			SET @_ErrProcName= ERROR_PROCEDURE();
			SET @_ErrLineNumber = ERROR_LINE();
			SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)
			+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)
			+ 'Procedure Name: usp_CreateLedgerAggregateColumnToSiteMapping' + CHAR(13)+CHAR(10)
			+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
			RAISERROR(@_ErrMessage,18,1);
		END
	END CATCH
	
END
GO


 