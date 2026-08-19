
/*
	DECLARE @MasterRecordGuid uniqueidentifier
	DECLARE @OwnerSiteGuid uniqueidentifier
	--EXEC [erv].[usp_GetEntityBasicInfo] 'Equipment', '40C7568E-21BE-4FA8-AC51-E01803FDE333', @MasterRecordGuid OUTPUT,  @OwnerSiteGuid OUTPUT
	--EXEC [erv].[usp_GetEntityBasicInfo] 'Product', '80B08634-D356-4569-B9A2-CD36DF955BD0', @MasterRecordGuid OUTPUT,  @OwnerSiteGuid OUTPUT
	EXEC [erv].[usp_GetEntityBasicInfo] 'Company', '012D8DD3-E6FA-4B78-A81A-C84F1C360558', @MasterRecordGuid OUTPUT,  @OwnerSiteGuid OUTPUT
	SELECT @MasterRecordGuid, @OwnerSiteGuid

*/

	CREATE PROCEDURE [erv].[usp_GetEntityBasicInfo]
	(
		@EntityTypeId nvarchar(100), @EntityGuid uniqueidentifier, @MasterRecordGuid uniqueidentifier OUTPUT, @OwnerSiteGuid uniqueidentifier OUTPUT
	)
	AS
	BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[usp_GetEntityBasicInfo] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Procedure to return the basic information for a given entity record
	-- Notes:
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityGuid: Guid of the record to the examined. This is the specific guid of the entity record and not the master record guid of the entity record.
	------------------------------------------------------------------------------------------------------
	*/

		IF (@EntityTypeId = 'Equipment')
		BEGIN
			SELECT @MasterRecordGuid = _MasterRecordGuid, @OwnerSiteGuid = SiteGuid
			FROM [dbo].[tblEquipment] a
			WHERE a.EquipmentGuid = @EntityGuid
		END
		ELSE IF (@EntityTypeId = 'Product')
		BEGIN			
			SELECT @MasterRecordGuid = _MasterRecordGuid, @OwnerSiteGuid = SiteGuid
			FROM [dbo].[tblProducts] a
			WHERE a.ProductGuid = @EntityGuid
		END
		ELSE IF (@EntityTypeId = 'Company')
		BEGIN			
			SELECT @MasterRecordGuid = _MasterRecordGuid, @OwnerSiteGuid = SiteGuid
			FROM [dbo].[tblCompanies] a
			WHERE a.CompanyGuid = @EntityGuid
		END
		ELSE IF (@EntityTypeId = 'Transaction_Alias')
		BEGIN			
			SELECT @MasterRecordGuid = _MasterRecordGuid, @OwnerSiteGuid = SiteGuid
			FROM [dbo].[tblTransactionAliases] a
			WHERE a.TransactionAliasGuid = @EntityGuid
		END
		ELSE IF (@EntityTypeId = 'Personnel')
		BEGIN			
			SELECT @MasterRecordGuid = _MasterRecordGuid, @OwnerSiteGuid = SiteGuid
			FROM [dbo].[tblPersonnel] a
			WHERE a.PersonnelGuid = @EntityGuid
		END
	END