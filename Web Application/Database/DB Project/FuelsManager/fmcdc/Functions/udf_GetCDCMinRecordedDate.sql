/*
	DROP FUNCTION [fmcdc].[udf_GetCDCMinRecordedDate]

	SELECT [fmcdc].[udf_GetCDCMinRecordedDate] ()

*/
CREATE FUNCTION [fmcdc].[udf_GetCDCMinRecordedDate] ()
RETURNS VARCHAR(50)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [fmcdc].[udf_GetCDCMinRecordedDate]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Get the CDC timestamp of the ealiest data currently recorded in the CDC tables.
	
	------------------------------------------------------------------------------------------------------
	DECLARE @result datetimeoffset
	SET @result = NULL
	
	SELECT @result = MIN(x.RecordUpdatedDate)
	FROM
	(
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblApplicationString]	
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblAutoDistributionReasonCodes]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblCompanyCompanyToUserGroup]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEntityCompanyToSite]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEntityEquipmentToSite]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEntityProductToSite]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEntityTransactionAliasToSite]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEntityUserToSite]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEquipmentTypes]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblProducts]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblCompanies]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEquipment]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblEquipmentTypes]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblPersonnel]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTransactionAliases]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblLoadArms]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblSites]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblSiteToSite]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblStations]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTanks]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTransactions]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTransactionLineItems]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTransactionUserData]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTransactionLineItemUserData]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblTransactionSubLineItems]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblUserToGroup]
		UNION
		SELECT MIN(RecordUpdatedDate) RecordUpdatedDate FROM [fmcdc].[tblUsers]		

	) x

	
	RETURN CONVERT(VARCHAR(50), @result);
END     

GO