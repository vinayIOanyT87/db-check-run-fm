
/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Select the custom configuration data for the Delta FPES adaptor
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFPESSelect]
AS
BEGIN
	SET NOCOUNT ON

	--There should only be one configuration setting, but we select the most recent one
	--just in case
	SELECT TOP(1) 
	    SRMAdaptorFPESGuid,
		TransactionAliasGuid,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	FROM tblSRMAdaptorFPES	
	ORDER BY CreatedDate DESC

END