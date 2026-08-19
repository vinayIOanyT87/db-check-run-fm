
CREATE PROCEDURE [map].[usp_CreateExternalStationDeviceToSiteMapping] 
(
	@MappingGuid uniqueidentifier=NULL OUTPUT
	,	@EntityRecordGuid uniqueidentifier=NULL
	,	@AssignedFromSiteGuid uniqueidentifier=NULL
	,	@AssignedToSiteGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_CreateExternalStationDeviceToSiteMapping]
	-- Author: Caleb Townsend
	-- created from:
	--
	-- Stored procedure: [map].[usp_CreateDataDictionaryToSiteMapping]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new DataDictionaryToSite mapping record. 	
	-- Notes:
	-- 1. DataDictionaryToSite mappings are not applied/managed on individual DataDictionary records, but across all DataDictionary records for a given sitegroup (@EntityRecordGuid).
	-- 2. @EntityRecordGuid: Guid of the sitegroup for which all the DataDictionary records are to be mapped.
	-- 3. @AssignedFromSiteGuid: Guid of the AssignedFrom sitegroup from which the mapping is to be created.
	-- 4. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup to which the mapping is to  created.
	-- 5. @CreatedDate
	-- 6. @CreatedBy
	-- 7. @_RowVersion (output): RowVersion of the record created by the Stored Procedure.
	-- 8. @MappingGuid (output): Guid of the mapping guid created by this Stored Procedure.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @errMsg nvarchar(250)
		DECLARE @assignedFromSiteId nvarchar(30)
		DECLARE @assignedToSiteId nvarchar(30)

		SELECT @assignedFromSiteId = ID FROM tblSites WHERE SiteGuid = @AssignedFromSiteGuid		
		SELECT @assignedToSiteId = ID FROM tblSites WHERE SiteGuid = @AssignedToSiteGuid
		
		/* Verify if the site hierarchy allows an entity-to-site mapping between the AssignedFrom sitegroup and the AssignedTo site/sitegroup */
		IF ((SELECT COUNT(*) FROM [map].[tblSiteToSite] WHERE ParentSiteGuid = @AssignedFromSiteGuid AND ChildSiteGuid = @AssignedToSiteGuid) = 0)
		BEGIN			
			SET @errMsg = 'Invalid Mapping. Site: ' + @assignedToSiteId + ' is not mapped to site: ' + @assignedFromSiteId
			RAISERROR(@errMsg,16,1); 
			RETURN;
		END

		/* Entity Types that are mapped as a whole do not support multiple mappings to/from a site/sitegroup. If there has already been a mapping from a sitegroup, irrespective of its EntityRecordGuid (OwnersiteGuid), that mapping would have to be deleted before that sitegroup can be the recipient of a new mapping from a higher level sitegroup */
		IF ((SELECT COUNT(*) FROM [map].[tblEntityGasboyDeviceToSite] WHERE AssignedFromSiteGuid = @AssignedToSiteGuid) > 0)
		BEGIN			
			SET @errMsg = 'Invalid Mapping. There are one or more mappings assigned from site: ' + @assignedToSiteId;
			RAISERROR(@errMsg,16,1); 
			RETURN;
		END

		/* Entity Types that are mapped as a whole do not support multiple mappings to/from a site/sitegroup. If there has already been a mapping to a sitegroup, irrespective of its EntityRecordGuid (OwnersiteGuid), that mapping would have to be deleted before that sitegroup can be the recipient of a new mapping from another sitegroup */
		IF ((SELECT COUNT(*) FROM [map].[tblEntityGasboyDeviceToSite] WHERE AssignedFromSiteGuid = @AssignedToSiteGuid) > 0)
		BEGIN			
			SET @errMsg = 'Invalid Mapping. There is already an Alarm and Event mapping assigned to site: ' + @assignedToSiteId;
			RAISERROR(@errMsg,16,1); 
			RETURN;
		END

		SET @MappingGuid=NEWID();
		INSERT INTO [map].[tblEntityGasboyDeviceToSite]
		(
			[GasboyDeviceToSiteGuid]
		,	[OwnerSiteGuid]
		,	[MapToSiteGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[AssignedFromSiteGuid]
		)
		VALUES
		(
			@MappingGuid
		,	@EntityRecordGuid
		,	@AssignedToSiteGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@CreatedDate
		,	@CreatedBy
		,	@AssignedFromSiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion
		FROM [map].[tblEntityGasboyDeviceToSite]
		WHERE [GasboyDeviceToSiteGuid] = @MappingGuid

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
						+ 'Procedure Name: map.usp_CreateExternalStationDeviceToSiteMapping' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END