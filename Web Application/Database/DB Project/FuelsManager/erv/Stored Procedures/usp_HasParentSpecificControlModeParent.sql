/*
	DROP PROCEDURE [erv].[usp_HasParentSpecificControlModeParent]

	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
	DECLARE @HasParentSpecificParent bit
	EXEC [erv].[usp_HasParentSpecificControlModeParent] 'Company', 'DF5060D4-25E4-4F56-AE46-50C25331863E', Null, Null, 'AllowDriverEntry',  @HasParentSpecificParent OUTPUT
	--EXEC [erv].[usp_HasParentSpecificControlModeParent] 'Company', '00000000-0000-0000-0000-000000000001' , Null, Null, 'AllowDriverEntry', @HasParentSpecificParent OUTPUT
	--EXEC [erv].[usp_HasParentSpecificControlModeParent] 'Equipment', '6F38FF9E-D815-4E5B-B6B6-E6EAC0B1B76B' , 'EquipmentTypeGuid', @emptyGuid, 'LockedOutReason', @HasParentSpecificParent OUTPUT
	--EXEC [erv].[usp_HasParentSpecificControlModeParent] 'Equipment', 'B7BD440B-674F-46F6-977A-CEFC540B1A90' , 'EquipmentTypeGuid', 'B233964F-3D4C-4500-B43F-E170BAE94F41', 'FixedVolume', @HasParentSpecificParent OUTPUT
	--EXEC [erv].[usp_HasParentSpecificControlModeParent] 'Product', 'B7BD440B-674F-46F6-977A-CEFC540B1A90' , NULL, NULL, 'StockTrack', @HasParentSpecificParent OUTPUT
	SELECT @HasParentSpecificParent
*/


CREATE PROCEDURE [erv].[usp_HasParentSpecificControlModeParent]
(
	@EntityTypeId nvarchar(100), @SiteGroupGuid uniqueidentifier, @FilterFieldName nvarchar(100), @FilterValueGuid uniqueidentifier, @TargetField nvarchar(100), @Result bit OUTPUT
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_HasParentSpecificControlModeParent] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose :Determines whether a Field Level Configuration item (actual or mocked-up) has a ParentSpecifc parent item or not. 
	-- Notes:
	-- 1. The determination is based on the ForwardControlMode values of the parent items of the target FLC item being tested (and not from the direct reading of the InheritedControlMode value of the target FLC item).
	--    This routine assumes that there are only two possible FLC mode priority/dominance hierarchies (from high to low): ParentSpecific -> VersionSpecific and ParentSpecific -> GlobalSpecific
	-- 2. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 3. @SiteGroupGuid: SiteGroup for which the FLC configuration is to be examined.
	-- 4. @FilterFieldName: Name of the filter for which the FLC configuration is to be retrieved. This is only applicable when the Entity Segment Template defined for the EntityTypeId has a filter specified.
	-- 5. @FilterValueGuid: Specific filter value of the entity segment to be examined. The @FilterValueGuid parameter is only pertinent to entity segment templates for which a FilterFieldName has been defined.
	-- 6. @TargetField: Specific Target Field for which to examine the FLC configuration.
	-- 7. @Result: 0 - No parent sitegroups has a more restrictive control mode; 1 - at least one parent sitegroup has a more restrictive control mode	
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @parentCount int
		DECLARE @versionSpecificParentCount int
		DECLARE @entitySegmentTemplateGuid uniqueidentifier

		SET @Result = 1

		DECLARE @tblParentSiteGroupFLC TABLE
		(
			SiteGroupGuid uniqueidentifier,
			ForwardControlMode nvarchar(20)
		)

		DECLARE @tblParentSiteGroup TABLE
		(
			ParentSiteGuid uniqueidentifier
		)

		INSERT INTO @tblParentSiteGroup
		SELECT ParentSiteGuid FROM map.tblSiteToSite 
		WHERE ChildSiteGuid = @SiteGroupGuid 
		AND ParentSiteGuid IS NOT NULL 
		AND ParentSiteGuid <> ChildSiteGuid		

		SELECT @parentCount = COUNT(*) FROM @tblParentSiteGroup

		IF (@parentCount > 0)
		BEGIN			
			SELECT @entitySegmentTemplateGuid = EntitySegmentTemplateGuid FROM erv.tblEntitySegmentTemplate
			WHERE EntityTypeId = @EntityTypeId
			AND ISNULL(FilterFieldName, '') = ISNULL(@FilterFieldName, '')			

			SELECT @versionSpecificParentCount = COUNT(*) FROM erv.tblEntityRecordVersioningFieldConfig a
			INNER JOIN @tblParentSiteGroup b
			on b.ParentSiteGuid = a.SiteGroupGuid
			WHERE a.EntitySegmentTemplateGuid = @entitySegmentTemplateGuid
			AND ISNULL(a.FilterValueGuid, @emptyGuid) = ISNULL(@FilterValueGuid, @emptyGuid)
			AND a.TargetField = @TargetField
			AND (a.ForwardControlMode IN ('VersionSpecific', 'GlobalSpecific'))
			GROUP BY a.TargetField

			IF (@versionSpecificParentCount = @parentCount)
			BEGIN
				SET @Result = 0  -- The Target Field has a VersionSpecific FCM on all the parent sitegroups 
			END
		END
		ELSE
		BEGIN
			SET @Result = 0  -- Target site/sitegroup is a root node (has no parent sitegroups)
		END

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
						+ 'Procedure Name: [erv].usp_HasParentSpecificControlModeParent' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END