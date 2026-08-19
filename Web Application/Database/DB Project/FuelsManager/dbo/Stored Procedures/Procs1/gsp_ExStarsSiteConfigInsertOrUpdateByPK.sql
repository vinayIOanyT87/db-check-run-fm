CREATE PROCEDURE [dbo].[gsp_ExStarsSiteConfigInsertOrUpdateByPK]
(
	 @SiteGuid UNIQUEIDENTIFIER=NULL
	,@ManagerCompanyGuid UNIQUEIDENTIFIER=NULL
	,@InterchangeSenderId NCHAR(15)=NULL
	,@ApplicationSendersCode NCHAR(15)=NULL
	,@AuthorizationCode NCHAR(10)=NULL
	,@FeinCode NCHAR(18)=NULL
	,@SecurityCode NCHAR(10)=NULL
	,@InfoProviderName NCHAR(18)=NULL
	,@AbbreviatedProviderName NCHAR(18)=NULL
	,@GroupControlNumber NCHAR(9)=NULL
	,@IRS_637Registration NCHAR(18)=NULL
	,@UpdatedBy udtUserID=NULL
)
AS
BEGIN
	IF( NOT EXISTS( SELECT 1 FROM  [dbo].[tblExStarsSiteConfig] 
		WHERE [SiteGuid]=@SiteGuid AND [ManagerCompanyGuid]=@ManagerCompanyGuid))
	BEGIN
		INSERT INTO [dbo].[tblExStarsSiteConfig]
				   ([SiteGuid]
				   ,[ManagerCompanyGuid]
				   ,[InterchangeSenderId]
				   ,[ApplicationSendersCode]
				   ,[AuthorizationCode]
				   ,[FeinCode]
				   ,[SecurityCode]
				   ,[InfoProviderName]
				   ,[AbbreviatedProviderName]
				   ,[GroupControlNumber]
				   ,[IRS_637Registration]
				   ,[CreatedDate]
				   ,[CreatedBy]
				   ,[UpdatedDate]
				   ,[UpdatedBy])
			 VALUES(
					 @SiteGuid
					,@ManagerCompanyGuid
					,@InterchangeSenderId
					,@ApplicationSendersCode
					,@AuthorizationCode
					,@FeinCode
					,@SecurityCode
					,@InfoProviderName
					,@AbbreviatedProviderName
					,@GroupControlNumber
					,@IRS_637Registration
					,GETDATE()
					,@UpdatedBy
					,GETDATE()
					,@UpdatedBy
					 )
	END
	ELSE
	BEGIN
		UPDATE [dbo].[tblExStarsSiteConfig]
		   SET 
				 [ApplicationSendersCode] = @ApplicationSendersCode     
				,[InterchangeSenderId] =	@InterchangeSenderId
				,[AuthorizationCode] =      @AuthorizationCode          
				,[FeinCode] =               @FeinCode                   
				,[SecurityCode] =           @SecurityCode               
				,[InfoProviderName] =       @InfoProviderName           
				,[AbbreviatedProviderName] =@AbbreviatedProviderName     
				,[GroupControlNumber] =     @GroupControlNumber       
				,[IRS_637Registration] =	@IRS_637Registration  
				,[UpdatedDate] =			GETDATE()
				,[UpdatedBy] =				 @UpdatedBy
		 WHERE [SiteGuid]=@SiteGuid AND [ManagerCompanyGuid]=@ManagerCompanyGuid
	END
END


