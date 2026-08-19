CREATE PROCEDURE [dbo].[gsp_ExStarsSiteConfigSelect]
(
	 @SiteGuid UNIQUEIDENTIFIER=NULL
	,@ManagerCompanyGuid UNIQUEIDENTIFIER=NULL
	)
AS
BEGIN
	SELECT [SiteGuid]
		  ,[ManagerCompanyGuid]
		  ,InterchangeSenderId
		  ,[ApplicationSendersCode]
		  ,[AuthorizationCode]
		  ,[FeinCode]
		  ,[SecurityCode]
		  ,[InfoProviderName]
		  ,[AbbreviatedProviderName]
		  ,[GroupControlNumber]
		  ,[IRS_637Registration]
		  ,[TerminalControlNumber]
		  ,[CreatedDate]
		  ,[CreatedBy]
		  ,[UpdatedDate]
		  ,[UpdatedBy]
	  FROM [dbo].[tblExStarsSiteConfig]
	  WHERE [SiteGuid]=@SiteGuid AND [ManagerCompanyGuid]=@ManagerCompanyGuid
END
GO


