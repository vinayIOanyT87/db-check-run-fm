

/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Select company ID translations defined for records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAECompanyIDSelect]
(
	@FMAECompanyID NVARCHAR(100) = NULL,
	@FMAECompanyIDMapGuid UNIQUEIDENTIFIER = NULL,
	@FMAECompanyIDSearchFilter NVARCHAR(25) = NULL
)
AS
BEGIN
	IF (@FMAECompanyID IS NOT NULL)
	BEGIN
		SELECT
			map.tblFMAECompanyID.FMAECompanyIDMapGuid,
			map.tblFMAECompanyID.FMAECompanyID,
			map.tblFMAECompanyID.CompanyGuid,
			CompanyID = tblCompanies.ID,
			map.tblFMAECompanyID.CreatedDate,
			map.tblFMAECompanyID.CreatedBy,
			map.tblFMAECompanyID.UpdatedDate,
			map.tblFMAECompanyID.UpdatedBy
		FROM map.tblFMAECompanyID
		INNER JOIN tblCompanies ON map.tblFMAECompanyID.CompanyGuid = tblCompanies.CompanyGuid
		WHERE map.tblFMAECompanyID.FMAECompanyID = @FMAECompanyID
	END
	ELSE IF (@FMAECompanyIDMapGuid IS NOT NULL)
	BEGIN
		SELECT
			map.tblFMAECompanyID.FMAECompanyIDMapGuid,
			map.tblFMAECompanyID.FMAECompanyID,
			map.tblFMAECompanyID.CompanyGuid,
			CompanyID = tblCompanies.ID,
			map.tblFMAECompanyID.CreatedDate,
			map.tblFMAECompanyID.CreatedBy,
			map.tblFMAECompanyID.UpdatedDate,
			map.tblFMAECompanyID.UpdatedBy
		FROM map.tblFMAECompanyID
		INNER JOIN tblCompanies ON map.tblFMAECompanyID.CompanyGuid = tblCompanies.CompanyGuid
		WHERE map.tblFMAECompanyID.FMAECompanyIDMapGuid = @FMAECompanyIDMapGuid
	END
	ELSE
	BEGIN
		IF (@FMAECompanyIDSearchFilter IS NOT NULL AND @FMAECompanyIDSearchFilter != '')
		BEGIN
			SELECT
				map.tblFMAECompanyID.FMAECompanyIDMapGuid,
				map.tblFMAECompanyID.FMAECompanyID,
				map.tblFMAECompanyID.CompanyGuid,
				CompanyID = tblCompanies.ID,
				map.tblFMAECompanyID.CreatedDate,
				map.tblFMAECompanyID.CreatedBy,
				map.tblFMAECompanyID.UpdatedDate,
				map.tblFMAECompanyID.UpdatedBy
			FROM map.tblFMAECompanyID
			INNER JOIN tblCompanies ON map.tblFMAECompanyID.CompanyGuid = tblCompanies.CompanyGuid
			WHERE map.tblFMAECompanyID.FMAECompanyID LIKE ('%' + @FMAECompanyIDSearchFilter + '%')
		END
		ELSE 
		BEGIN
			SELECT
				map.tblFMAECompanyID.FMAECompanyIDMapGuid,
				map.tblFMAECompanyID.FMAECompanyID,
				map.tblFMAECompanyID.CompanyGuid,
				CompanyID = tblCompanies.ID,
				map.tblFMAECompanyID.CreatedDate,
				map.tblFMAECompanyID.CreatedBy,
				map.tblFMAECompanyID.UpdatedDate,
				map.tblFMAECompanyID.UpdatedBy
			FROM map.tblFMAECompanyID
			INNER JOIN tblCompanies ON map.tblFMAECompanyID.CompanyGuid = tblCompanies.CompanyGuid
		END
		
	END
	
END


