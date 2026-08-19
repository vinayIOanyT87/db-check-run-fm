--============================================================================================================
-- The purpose of this SP is to convert an enterprise database to a base level database
-- November 29, 2010
-- Richard R. Panachida
--============================================================================================================
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fm_ConvertEnterpriseToBaselevel]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[fm_ConvertEnterpriseToBaselevel]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[fm_ConvertEnterpriseToBaselevel]
AS
	SET NOCOUNT ON

	DECLARE @BaseLevelSiteIndex INT
	DECLARE @JSCSiteIndex INT
	DECLARE @SiteAdminIndex INT

	SELECT @BaseLevelSiteIndex = SiteIndex FROM tblSites WHERE ID = 'BASE LEVEL'
	SELECT @JSCSiteIndex = SiteIndex FROM tblSites WHERE ID = 'JSC'
	SET @SiteAdminIndex = -1

	-- Delete all records
	PRINT ''
	PRINT 'Deleting all records from tables: tblAlarmAndEventLog, tblAuditLog, tblChangesQueue, tblSessions'
	DELETE FROM tblAlarmAndEventLog 
	DELETE FROM tblAuditLog
	DELETE FROM tblChangesQueue
	DELETE FROM tblSessions

	-- Delete all items in the Transaction tables
	PRINT ''
	PRINT 'Deleting all transactions...'
	SELECT	TransIndex INTO	#DeleteTransIndexList FROM	tblTransactions

	DELETE FROM tblTransactionSubLineItems
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);
	DELETE FROM tblTransactionUserData
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);
	DELETE FROM dbo.tblTransactionLineItemUserData
		   WHERE TransLineItemID in (SELECT l.TransLineItemID 
									 FROM tblTransactionLineItems l left outer join tblTransactions t ON l.TransIndex = t.TransIndex
									 WHERE t.TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList));
	DELETE FROM tblTransactionLineItems
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);
	DELETE FROM tblTransactionNotes
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);
	DELETE FROM tblTransactionWeightReadings
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);
	DELETE FROM dbo.tblTransactionSignature
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);
	DELETE FROM tblTransactions
		   WHERE TransIndex in (SELECT TransIndex FROM #DeleteTransIndexList);

	DROP TABLE #DeleteTransIndexList

	-- Remove all entity assignments that are not assigned to JSC or SiteAdmin or Base Level
	PRINT ''
	PRINT 'Deleting all records from the Entity To Site map that do not equal siteadmin, JSC, and Base Level'
	DELETE FROM tblEntityToSiteMap WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex

	-- Delete all records that do not belong to site admin or JSC or Base Level
	PRINT ''
	PRINT 'Deleting all records from the Entity tables that do not equal siteadmin, JSC, and Base Level'
	DELETE FROM tblListViewFields     WHERE ListViewIndex IN (SELECT [Index] FROM tblListViews WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex) 
	DELETE FROM tblListViews          WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblCompanyRoleMap     WHERE CompanyIndex IN (SELECT CompanyIndex FROM tblCompanies WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex)
	DELETE FROM tblCompanyMap         WHERE AssignedIndex IN (SELECT CompanyIndex FROM tblCompanies WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex)
	DELETE FROM tblUserGroupMap       WHERE UserIndex IN (SELECT UserIndex FROM tblUsers WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex)
	DELETE FROM tblUsers              WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblGroups             WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblProductMap         WHERE AssignedIndex IN (SELECT ProductIndex FROM tblProducts WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex)
	DELETE FROM tblProducts           WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblCompanies          WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblEquipment          WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblDataDictionaries   WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblEquipmentTypes     WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblFuelCards          WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblIATA               WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblTransactionAliases WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblUserDataFields     WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblQualificationsMap  WHERE AssignedIndex IN (SELECT [Index] FROM tblQualifications WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex)
	DELETE FROM tblQualifications     WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblPersonnel          WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblPersonRoleMap      WHERE PersonIndex IN (SELECT PersonIndex FROM tblPersonnel WHERE SiteIndex <> @JSCSiteIndex AND SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex)

	-- Change entity ownership from JSC to Base Level site.
	PRINT ''
	PRINT 'Updating ownership from JSC to Base Level'
	UPDATE tblProducts SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblCompanies SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblEquipment SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblUsers SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblGroups SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblDataDictionaries SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblEquipmentTypes SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblFuelCards SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblIATA SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblListViews SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblTransactionAliases SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblUserDataFields SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblQualifications SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblCompanyRoleMap SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblCompanyMap SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex
	UPDATE tblPersonnel SET SiteIndex = @BaseLevelSiteIndex WHERE SiteIndex = @JSCSiteIndex

	DELETE FROM tblSiteToSiteMap      WHERE ParentSiteIndex <> @SiteAdminIndex AND ParentSiteIndex <> @BaseLevelSiteIndex
	DELETE FROM tblSites              WHERE SiteIndex <> @SiteAdminIndex AND SiteIndex <> @BaseLevelSiteIndex

	PRINT ''
	PRINT 'Completed'
GO
