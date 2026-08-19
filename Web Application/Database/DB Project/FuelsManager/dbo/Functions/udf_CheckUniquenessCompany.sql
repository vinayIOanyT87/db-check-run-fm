

CREATE FUNCTION [dbo].[udf_CheckUniquenessCompany]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(100))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblCompany
	IF 0 < (SELECT COUNT(*) FROM tblCompanies e
	LEFT JOIN map.tblEntityCompanyToSite em1 ON em1.CompanyGuid = e._MasterRecordGuid
	RIGHT JOIN map.tblEntityCompanyToSite em2 ON em2.CompanyGuid = @_MasterRecordGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
