

/*
	EXEC [map].[usp_TransactionAliasToSiteDeleteAll] 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'

*/
CREATE PROCEDURE [map].[usp_TransactionAliasToSiteDeleteAll]
(
	@AssignedFromSiteGroupGuid uniqueidentifier, @AssignedToSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_TransactionAliasToSiteDeleteAll]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Cascade deletes all the TransactionAlias-to-site mappings and the associated record versions for all the TransactionAlias-to-site mappings between two sites.
	-- Notes:
	-- 1. @AssignedFromSiteGroupGuid: Guid of the AssignedFrom sitegroup for which the TransactionAlias-to-site assignments are to be deleted.
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the TransactionAlias-to-site assignments are to be deleted.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @runningMappingLevel int

		IF (@AssignedFromSiteGroupGuid = @AssignedToSiteGuid)  -- this is not to be used to delete base mappings
		BEGIN
			RETURN
		END

		DECLARE @tblEntityToSiteMappings TABLE
		(
			MasterRecGuid uniqueidentifier
			, AssignedFromSiteGuid uniqueidentifier
			, AssignedToSiteGuid uniqueidentifier
			, MappingLevel int
			, Processed bit
		);		

		DECLARE @BeginTran BIT = 0 
		
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION
            SET @BeginTran = 1   
		END  		

		--Retrieve all the direct entity-to-site assignment mappings from the Target sitegroup to any of the site/groups to which the target sitegroup is no longer a parent
		INSERT INTO @tblEntityToSiteMappings
		(MasterRecGuid, AssignedFromSiteGuid, AssignedToSiteGuid, MappingLevel, Processed)
		SELECT a.TransactionAliasGuid, a.AssignedFromSiteGuid, a.SiteGuid, 0, 0 FROM map.tblEntityTransactionAliasToSite a
		WHERE a.AssignedFromSiteGuid = @AssignedFromSiteGroupGuid
		AND a.SiteGuid = @AssignedToSiteGuid

		--Also extract all the subsequent entity-to-site mappings that derive from the direct mappings above
		SET @runningMappingLevel = 0
		WHILE ((SELECT COUNT(*) FROM @tblEntityToSiteMappings WHERE MappingLevel = @runningMappingLevel) > 0)
		BEGIN
			SET @runningMappingLevel = @runningMappingLevel + 1
			INSERT INTO @tblEntityToSiteMappings
			(MasterRecGuid, AssignedFromSiteGuid, AssignedToSiteGuid, MappingLevel, Processed)
			SELECT a.TransactionAliasGuid, a.AssignedFromSiteGuid, a.SiteGuid, @runningMappingLevel, 0 FROM map.tblEntityTransactionAliasToSite a
			INNER JOIN @tblEntityToSiteMappings b
			ON b.MasterRecGuid = a.TransactionAliasGuid
			WHERE b.MappingLevel = 0
			AND a.AssignedFromSiteGuid IN 
			(
				SELECT AssignedToSiteGuid FROM @tblEntityToSiteMappings WHERE MappingLevel = @runningMappingLevel-1
			)									
		END


		--For each affected entity-to-site mapping, delete the corresponding child record version
		--Delete the external attributes of the parent record version

		--Associations
		DELETE a FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.ParentTransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.ParentTransactionAliasGuid <> b._MasterRecordGuid

		DELETE a FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.ChildTransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.ChildTransactionAliasGuid <> b._MasterRecordGuid
			
		--Fields and FieldOrder
		DELETE a FROM [dbo].[tblTransactionAliasFields] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.TransactionAliasGuid <> b._MasterRecordGuid

		--Products
		DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.AssignedToTransactionAliasGuid <> b._MasterRecordGuid

		--Statuses
		DELETE a FROM [map].[tblTransactionAliasToStatus] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.TransactionAliasGuid <> b._MasterRecordGuid						

		--UserData
		--[dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
		DELETE a FROM [dbo].[tblUserDataListValueTransactionAlias] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
		ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c.TransactionAliasGuid = b.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings d
		ON d.MasterRecGuid = c._MasterRecordGuid
		AND d.AssignedToSiteGuid = c.SiteGuid
		WHERE b.TransactionAliasGuid <> c._MasterRecordGuid
		
		DELETE a FROM [dbo].[tblUserDataFieldTransactionAlias] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.TransactionAliasGuid <> b._MasterRecordGuid

		--[dbo].[tblUserDataFieldTransactionAliasLineItem] and [dbo].[tblUserDataListValueTransactionAliasLineItem]
		DELETE a FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
		ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
		INNER JOIN dbo.tblTransactionAliases c
		ON c.TransactionAliasGuid = b.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings d
		ON d.MasterRecGuid = c._MasterRecordGuid
		AND d.AssignedToSiteGuid = c.SiteGuid
		WHERE b.TransactionAliasGuid <> c._MasterRecordGuid

		DELETE a FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.TransactionAliasGuid <> b._MasterRecordGuid


		--Delete the UserGroups mappings of the child record versions
		DELETE a FROM [map].[tblGroupToTransactionAlias] a
		INNER JOIN dbo.tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteMappings c
		ON c.MasterRecGuid = b._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		WHERE a.TransactionAliasGuid <> b._MasterRecordGuid



		--Delete the child record versions
		DELETE a FROM tblTransactionAliases a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.MasterRecGuid = a._MasterRecordGuid
		AND a.SiteGuid = b.AssignedToSiteGuid
		WHERE a.TransactionAliasGuid <> a._MasterRecordGuid

		--Delete the entity-to-site mappings affected by the site-to-site mapping deletion
		DELETE a 
		FROM map.tblEntityTransactionAliasToSite a
		INNER JOIN @tblEntityToSiteMappings b
		ON b.MasterRecGuid = a.TransactionAliasGuid
		AND a.SiteGuid = b.AssignedToSiteGuid
		WHERE a.AssignedFromSiteGuid <> a.SiteGuid
		
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
						+ 'Procedure Name: map.usp_TransactionAliasToSiteDeleteAll' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
