-- =============================================
-- Author:		Gregory Lybanon
-- Create date: 11/14/2018
-- Description:	Retuns list of Owners based on values selected in report parameters.  Site/role 
--				mappings have already been determined so do not need to be rechecked here. 
-- =============================================
CREATE PROCEDURE [rpt].[usp_OwnerActivityReportOwners]
	@OwnerCompanyGuid NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID], _MasterRecordGuid 
	FROM tblCompanies 
	WHERE _MasterRecordGuid IN (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@OwnerCompanyGuid) c) 
	ORDER BY [ID]
END