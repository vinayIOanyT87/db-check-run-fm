

CREATE FUNCTION [map].[udf_CheckUniquenessCompany]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(100)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblCompanies e WHERE e.CompanyGuid = @_MasterRecordGuid)
	IF 0 < (SELECT COUNT(*) FROM tblCompanies e 
	RIGHT JOIN map.tblEntityCompanyToSite em ON em.SiteGuid = @SiteGuid AND em.CompanyGuid = e._MasterRecordGuid 
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END