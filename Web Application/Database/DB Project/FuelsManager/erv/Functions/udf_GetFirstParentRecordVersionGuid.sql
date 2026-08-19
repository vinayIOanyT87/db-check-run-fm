
/*
	SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', 'B7BD440B-674F-46F6-977A-CEFC540B1A90')
	SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', 'B44649AD-877A-4A41-93B1-9B0E048BE377', '23A3F8FC-0D49-43BC-B20B-04CEDA6A4346')
	SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'B7BD440B-674F-46F6-977A-CEFC540B1A90')
	SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', '012D8DD3-E6FA-4B78-A81A-C84F1C360558', 'B7BD440B-674F-46F6-977A-CEFC540B1A90')
	SELECT [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', '0DC68ACA-11AD-4F43-AD2B-87609738C453', 'B7BD440B-674F-46F6-977A-CEFC540B1A90')

*/



CREATE FUNCTION [erv].[udf_GetFirstParentRecordVersionGuid]
(
	@EntityTypeId nvarchar(100), @EntityRecGuid uniqueidentifier, @StartSiteIndex uniqueidentifier
)
RETURNS uniqueidentifier
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetFirstParentRecordVersionGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Traverses up the Entity-to-site mapping tree from a given site.sitegroup for the first occurrence of a record version of a given MasterRecordGuid. 
	--          Returns the Guid of that first record version if found.
	-- Notes: 
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityRecGuid: Record Guid of the record  (MasterRecordGuid or child record guid)
	-- 3. @StartSiteIndex: Site/SiteGroup from which to start looking for a record version for the entity record.
	------------------------------------------------------------------------------------------------------


	DECLARE @result uniqueidentifier
	SET @result = NULL
	DECLARE @targetSiteGuid uniqueidentifier
	SET @targetSiteGuid = @StartSiteIndex
	DECLARE @newTargetSiteGuid uniqueidentifier
	DECLARE @EntityMasterRecGuid uniqueidentifier

	IF (@EntityTypeId = 'Equipment')
	BEGIN
		SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM tblEquipment WHERE EquipmentGuid = @EntityRecGuid
		WHILE ((SELECT COUNT(*) FROM map.tblEntityEquipmentToSite WHERE EquipmentGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid) > 0)
		BEGIN
			SELECT @result = EquipmentGuid FROM tblEquipment WHERE _MasterRecordGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
			IF (@result IS NOT NULL)
			BEGIN
				RETURN @result;
			END
			ELSE
			BEGIN
				SELECT @newTargetSiteGuid = AssignedFromSiteGuid FROM map.tblEntityEquipmentToSite WHERE EquipmentGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
				IF (@newTargetSiteGuid = @targetSiteGuid)
				BEGIN
					RETURN NULL;   --This would indicate an invalid entity-to-site mapping. E.g. AssignedFromSiteGuid is at a level higher than the Owner Site Guid. If not trapped this condition would result in an infinite loop.
				END
				ELSE
				BEGIN
					SET @targetSiteGuid = @newTargetSiteGuid
				END
			END
		END
	END
	ELSE IF (@EntityTypeId = 'Product')
	BEGIN
		SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM tblProducts WHERE ProductGuid = @EntityRecGuid
		WHILE ((SELECT COUNT(*) FROM map.tblEntityProductToSite WHERE ProductGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid) > 0)
		BEGIN
			SELECT @result = ProductGuid FROM tblProducts WHERE _MasterRecordGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
			IF (@result IS NOT NULL)
			BEGIN
				RETURN @result;
			END
			ELSE
			BEGIN
				SELECT @newTargetSiteGuid = AssignedFromSiteGuid FROM map.tblEntityProductToSite WHERE ProductGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
				IF (@newTargetSiteGuid = @targetSiteGuid)
				BEGIN
					RETURN NULL;   --This would indicate an invalid entity-to-site mapping. E.g. AssignedFromSiteGuid is at a level higher than the Owner Site Guid. If not trapped this condition would result in an infinite loop.
				END
				ELSE
				BEGIN
					SET @targetSiteGuid = @newTargetSiteGuid
				END
			END
		END
	END
	ELSE IF (@EntityTypeId = 'Company')
	BEGIN
		SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM tblCompanies WHERE CompanyGuid = @EntityRecGuid
		WHILE ((SELECT COUNT(*) FROM map.tblEntityCompanyToSite WHERE CompanyGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid) > 0)
		BEGIN
			SELECT @result = CompanyGuid FROM tblCompanies WHERE _MasterRecordGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
			IF (@result IS NOT NULL)
			BEGIN
				RETURN @result;
			END
			ELSE
			BEGIN
				SELECT @newTargetSiteGuid = AssignedFromSiteGuid FROM map.tblEntityCompanyToSite WHERE CompanyGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
				IF (@newTargetSiteGuid = @targetSiteGuid)
				BEGIN
					RETURN NULL;   --This would indicate an invalid entity-to-site mapping. E.g. AssignedFromSiteGuid is at a level higher than the Owner Site Guid. If not trapped this condition would result in an infinite loop.
				END
				ELSE
				BEGIN
					SET @targetSiteGuid = @newTargetSiteGuid
				END
			END
		END
	END
	ELSE IF (@EntityTypeId = 'Transaction_Alias')
	BEGIN
		SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM tblTransactionAliases WHERE TransactionAliasGuid = @EntityRecGuid
		WHILE ((SELECT COUNT(*) FROM map.tblEntityTransactionAliasToSite WHERE TransactionAliasGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid) > 0)
		BEGIN
			SELECT @result = TransactionAliasGuid FROM tblTransactionAliases WHERE _MasterRecordGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
			IF (@result IS NOT NULL)
			BEGIN
				RETURN @result;
			END
			ELSE
			BEGIN
				SELECT @newTargetSiteGuid = AssignedFromSiteGuid FROM map.tblEntityTransactionAliasToSite WHERE TransactionAliasGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
				IF (@newTargetSiteGuid = @targetSiteGuid)
				BEGIN
					RETURN NULL;   --This would indicate an invalid entity-to-site mapping. E.g. AssignedFromSiteGuid is at a level higher than the Owner Site Guid. If not trapped this condition would result in an infinite loop.
				END
				ELSE
				BEGIN
					SET @targetSiteGuid = @newTargetSiteGuid
				END
			END
		END
	END
	ELSE IF (@EntityTypeId = 'Personnel')
	BEGIN
		SELECT @EntityMasterRecGuid = _MasterRecordGuid FROM tblPersonnel WHERE PersonnelGuid = @EntityRecGuid
		WHILE ((SELECT COUNT(*) FROM map.tblEntityPersonnelToSite WHERE PersonnelGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid) > 0)
		BEGIN
			SELECT @result = PersonnelGuid FROM tblPersonnel WHERE _MasterRecordGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
			IF (@result IS NOT NULL)
			BEGIN
				RETURN @result;
			END
			ELSE
			BEGIN
				SELECT @newTargetSiteGuid = AssignedFromSiteGuid FROM map.tblEntityPersonnelToSite WHERE PersonnelGuid = @EntityMasterRecGuid AND SiteGuid = @targetSiteGuid
				IF (@newTargetSiteGuid = @targetSiteGuid)
				BEGIN
					RETURN NULL;   --This would indicate an invalid entity-to-site mapping. E.g. AssignedFromSiteGuid is at a level higher than the Owner Site Guid. If not trapped this condition would result in an infinite loop.
				END
				ELSE
				BEGIN
					SET @targetSiteGuid = @newTargetSiteGuid
				END
			END
		END
	END
	RETURN @result;
END