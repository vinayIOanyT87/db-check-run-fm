CREATE PROCEDURE [dbo].[usp_GetFuelCardsByISAFID](
	@ISAFID nvarchar(60)
)
AS
BEGIN

	DECLARE @fuelCardGuids TABLE
	(
		FuelCardGuid uniqueidentifier
	)

	INSERT INTO @fuelCardGuids 
		SELECT DISTINCT([dbo].[tblFuelCards].[FuelCardGuid])
			FROM [dbo].[tblFuelCards]
				INNER JOIN [map].[tblEntityFuelCardToSite]
					ON [dbo].[tblFuelCards].[FuelCardGuid] = [map].[tblEntityFuelCardToSite].[FuelCardGuid]
			WHERE [dbo].[tblFuelCards].[ProviderID] = @ISAFID
				OR [dbo].[tblFuelCards].[UserData2] = @ISAFID

	SELECT [dbo].[tblFuelCards].*
			,[lookup].[tblActivationStatus].[ActivationStatusName] AS 'StatusID'
			,shipto.ID AS 'ShipToID' 
			,shipto.Code AS 'ShipToCode'
			,shipto.Name AS 'ShipToName' 
			,shipto.Address1 AS 'ShipToAddress' 
			,shipto.City AS 'ShipToCity' 
			,shipto.State AS 'ShipToState' 
			,billto.ID AS 'BillToID' 
			,billto.Code AS 'BillToCode' 
			,billto.Name AS 'BillToName' 
			,billto.Address1 AS 'BillToAddress' 
			,billto.City AS 'BillToCity' 
			,billto.State AS 'BillToState' 
			,shipper.ID AS 'ShipperID' 
			,shipper.Code AS 'ShipperCode' 
			,shipper.Name AS 'ShipperName' 
			,shipper.Address1 AS 'ShipperAddress' 
			,shipper.City AS 'ShipperCity' 
			,shipper.State AS 'ShipperState' 
			,owner.ID AS 'OwnerID' 
			,owner.Code AS 'OwnerCode' 
			,owner.Name AS 'OwnerName' 
			,owner.Address1 AS 'OwnerAddress' 
			,owner.City AS 'OwnerCity' 
			,owner.State AS 'OwnerState' 
			,manager.ID AS 'ManagerID' 
			,manager.Code AS 'ManagerCode' 
			,manager.Name AS 'ManagerName' 
			,manager.Address1 AS 'ManagerAddress' 
			,manager.City AS 'ManagerCity' 
			,manager.State AS 'ManagerState'
			,appstr.ID AS 'FuelCardTypeApplicationStringID'
		FROM [dbo].[tblFuelCards]
			INNER JOIN @fuelCardGuids a
				ON [dbo].[tblFuelCards].[FuelCardGuid] = a.[FuelCardGuid]
			INNER JOIN [lookup].[tblActivationStatus]
				ON [lookup].[tblActivationStatus].[ActivationStatusIndex] = [dbo].[tblFuelCards].[ActivationStatus]
			LEFT JOIN [dbo].[tblApplicationString] appstr
				ON [dbo].[tblFuelCards].[FuelCardTypeApplicationStringGuid] = appstr.[ApplicationStringGuid]
			LEFT JOIN [dbo].[tblCompanies] shipto
				ON [dbo].[tblFuelCards].[ShipToCompanyGuid] = shipto._MasterRecordGuid
			LEFT JOIN [dbo].[tblCompanies] billto 
				ON [dbo].[tblFuelCards].[BillToCompanyGuid] = billto._MasterRecordGuid
			LEFT JOIN [dbo].[tblCompanies] shipper
				ON [dbo].[tblFuelCards].[ShipperCompanyGuid] = shipper._MasterRecordGuid
			LEFT JOIN [dbo].[tblCompanies] owner
				ON [dbo].[tblFuelCards].[OwnerCompanyGuid] = owner._MasterRecordGuid
			LEFT JOIN [dbo].[tblCompanies] manager
				ON [dbo].[tblFuelCards].[ManagerCompanyGuid] = manager._MasterRecordGuid

/*
			LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) shipto ON tblFuelCards.ShipToCompanyGuid = shipto._MasterRecordGuid
			LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) billto ON tblFuelCards.BillToCompanyGuid = billto._MasterRecordGuid
			LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) shipper ON tblFuelCards.ShipperCompanyGuid = shipper._MasterRecordGuid
			LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) owner ON tblFuelCards.OwnerCompanyGuid = owner._MasterRecordGuid
			LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid))) manager ON tblFuelCards.ManagerCompanyGuid = manager._MasterRecordGuid
*/
		ORDER BY [dbo].[tblFuelCards].[ID]

END
