

/*
	SELECT [erv].[udf_GetEntityAssignedFromSite] ('Equipment', 'A6EAB4B5-3130-452C-8A23-9290C47E70E7', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421')
	SELECT [erv].[udf_GetEntityAssignedFromSite] ('Equipment', '40C7568E-21BE-4FA8-AC51-E01803FDE333', NULL)
	SELECT [erv].[udf_GetEntityAssignedFromSite] ('Equipment', '0FECBAA5-93B5-4BDC-8098-6DA3DE892D4A', NULL)
	SELECT [erv].[udf_GetEntityAssignedFromSite] ('Product', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '46426312-E408-4AF8-85FD-338B622B32BF')
	SELECT [erv].[udf_GetEntityAssignedFromSite] ('Company', '3D93EEE7-AD63-49FE-BE9A-8D50329BFB07', '46426312-E408-4AF8-85FD-338B622B32BF')

*/


	CREATE FUNCTION [erv].[udf_GetEntityAssignedFromSite]
	(
		@EntityTypeId nvarchar(100), @EntityGuid uniqueidentifier, @AssignedToSiteGuid uniqueidentifier
	)
	RETURNS uniqueidentifier
	AS
	BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetEntityAssignedFromSite] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the sitegroup from which a given entity record has been assigned from
	-- Notes:
	-- 1. The @EntityGuid can be either the index of the specific child record version being examined, or the index of the master record version.
	   2. @AssignedToSiteGuid is the site/sitegroup to which the entity record is know to be assigned, and from which the AssignedFromSiteGroup is to be found.
	      -- If not provided, then the AssignedToSiteGuid is simply retrieved from the Owner Site/Site group of the entity record, i.e. the @EntityGuid is then treated as the Guid of a child record version.
	------------------------------------------------------------------------------------------------------
	*/
		DECLARE @result uniqueidentifier

		IF (@EntityTypeId = 'Equipment')
		BEGIN
			SELECT @result = b.AssignedFromSiteGuid 
			FROM [dbo].[tblEquipment] a
			INNER JOIN map.tblEntityEquipmentToSite b
			ON b.EquipmentGuid = a._MasterRecordGuid
			WHERE a.EquipmentGuid = @EntityGuid
			AND 
			(
				((@AssignedToSiteGuid IS NOT NULL) AND (b.SiteGuid = @AssignedToSiteGuid))
				OR
				((@AssignedToSiteGuid IS NULL) AND (b.SiteGuid = a.SiteGuid))
			)
		END
		ELSE IF (@EntityTypeId = 'Product')
		BEGIN			
			SELECT @result = b.AssignedFromSiteGuid 
			FROM [dbo].[tblProducts] a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a._MasterRecordGuid
			WHERE a.ProductGuid = @EntityGuid
			AND 
			(
				((@AssignedToSiteGuid IS NOT NULL) AND (b.SiteGuid = @AssignedToSiteGuid))
				OR
				((@AssignedToSiteGuid IS NULL) AND (b.SiteGuid = a.SiteGuid))
			)
		END
		ELSE IF (@EntityTypeId = 'Company')
		BEGIN			
			SELECT @result = b.AssignedFromSiteGuid 
			FROM [dbo].[tblCompanies] a
			INNER JOIN map.tblEntityCompanyToSite b
			ON b.CompanyGuid = a._MasterRecordGuid
			WHERE a.CompanyGuid = @EntityGuid
			AND 
			(
				((@AssignedToSiteGuid IS NOT NULL) AND (b.SiteGuid = @AssignedToSiteGuid))
				OR
				((@AssignedToSiteGuid IS NULL) AND (b.SiteGuid = a.SiteGuid))
			)
		END
		ELSE IF (@EntityTypeId = 'Transaction_Alias')
		BEGIN			
			SELECT @result = b.AssignedFromSiteGuid 
			FROM [dbo].[tblTransactionAliases] a
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a._MasterRecordGuid
			WHERE a.TransactionAliasGuid = @EntityGuid
			AND 
			(
				((@AssignedToSiteGuid IS NOT NULL) AND (b.SiteGuid = @AssignedToSiteGuid))
				OR
				((@AssignedToSiteGuid IS NULL) AND (b.SiteGuid = a.SiteGuid))
			)
		END
		ELSE IF (@EntityTypeId = 'Personnel')
		BEGIN			
			SELECT @result = b.AssignedFromSiteGuid 
			FROM [dbo].[tblPersonnel] a
			INNER JOIN map.tblEntityPersonnelToSite b
			ON b.PersonnelGuid = a._MasterRecordGuid
			WHERE a.PersonnelGuid = @EntityGuid
			AND 
			(
				((@AssignedToSiteGuid IS NOT NULL) AND (b.SiteGuid = @AssignedToSiteGuid))
				OR
				((@AssignedToSiteGuid IS NULL) AND (b.SiteGuid = a.SiteGuid))
			)
		END
		RETURN @result;
	END