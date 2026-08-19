/*
	 DROP PROCEDURE [map].[usp_CreateAlarmAndEventToSiteMapping]

	 EXEC [map].[usp_CreateAlarmAndEventToSiteMapping] '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', '05/22/2026', 'Administrator'

*/

CREATE PROCEDURE [map].[usp_CreateAlarmAndEventToSiteMapping]
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
	-- Stored procedure: [map].[usp_CreateAlarmAndEventToSiteMapping]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new AlarmAndEventToSite mapping record. 	
	-- Notes:
	-- 1. AlarmAndEventToSite mappings are not applied/managed on individual AlarmAndEvent records, but across all AlarmAndEvent records for a given sitegroup (@EntityRecordGuid).
	-- 2. @EntityRecordGuid: Guid of the sitegroup for which all the AlarmAndEvent records are to be mapped.
	-- 3. @AssignedFromSiteGuid: Guid of the AssignedFrom sitegroup from which the mapping is to be created.
	-- 4. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup to which the mapping is to  created.
	-- 5. If the AssignedToSite is an indirect child of the AssignedFromSite, the entity-to-site mapping request is cascaded as necessary.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @errMsg nvarchar(250)

		IF (@AssignedToSiteGuid = @AssignedFromSiteGuid)
		BEGIN
			 INSERT INTO [map].[tblEntityAlarmAndEventToSite]
			 (
				[OwnerSiteGuid]
				,	[MapToSiteGuid]
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
				INSERT INTO [map].[tblEntityAlarmAndEventToSite]
				(
					 [OwnerSiteGuid]
					 ,	[MapToSiteGuid]
					 ,	[CreatedDate]
					 ,	[CreatedBy]
					 ,	[UpdatedDate]
					 ,	[UpdatedBy]
					 ,	[AssignedFromSiteGuid]
				)
				SELECT @EntityRecordGuid, @childSiteGuid, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, @parentSiteGuid
				WHERE NOT EXISTS
				(
					SELECT * FROM [map].[tblEntityAlarmAndEventToSite]
					WHERE OwnerSiteGuid = @EntityRecordGuid
					AND MapToSiteGuid = @ChildSiteGuid   /* Entity Types that are mapped as a whole do not support multiple mappings to/from a site/sitegroup. If there has already been a mapping to a sitegroup, irrespective of its EntityRecordGuid (OwnersiteGuid), that mapping would have to be deleted before that sitegroup can be the recipient of a new mapping from another sitegroup */				
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
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: map.usp_CreateAlarmAndEventToSiteMapping' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
