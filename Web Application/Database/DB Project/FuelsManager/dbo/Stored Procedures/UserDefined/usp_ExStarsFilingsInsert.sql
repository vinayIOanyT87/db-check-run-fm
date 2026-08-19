CREATE PROCEDURE [dbo].[usp_ExStarsFilingsInsert]
  @FilingStartDate				DATE
, @FilingEndDate				DATE
, @ManagerCompanyGuid			UNIQUEIDENTIFIER
, @SiteGuid						UNIQUEIDENTIFIER
, @ControlNumber				NVARCHAR(9)
, @OriginalControlNumber		NVARCHAR(9)
, @TransSetControlNumber		NVARCHAR(9)
, @ReportType					NVARCHAR(30)
, @Modifier						NVARCHAR(30)
, @FilingStatus					NVARCHAR(30)
, @RawDataFileName				NVARCHAR(max)
, @EasyReadFileName				NVARCHAR(max)
, @EdiReport					NVARCHAR(max)
, @EasyReadReport				NVARCHAR(max)
, @SerializedData				NVARCHAR(max)
, @FilingCreated                DATETIMEOFFSET(7)
, @FilingSent					DATETIMEOFFSET(7)
, @UpdatedBy					[dbo].[udtUserID]
AS
BEGIN
	SET NOCOUNT ON;

	IF( @FilingCreated < {ts '1980-01-01 00:00:00.000'}) 
		SET @FilingCreated = NULL

	IF( @FilingSent < {ts '1980-01-01 00:00:00.000'}) 
		SET @FilingSent = NULL

	INSERT INTO [dbo].[tblExStarsFilings] (
		 [FilingStartDate]
		,[FilingEndDate]
		,[ManagerCompanyGuid]
		,[SiteGuid]
		,[ControlNumber]
		,[OriginalControlNumber]
		,[TransSetControlNumber]
		,[ReportType]
		,[Modifier]
		,[FilingStatus]
		,[RawDataFileName]
		,[EasyReadFileName]
		,[EdiReport]
		,[EasyReadReport]
		,[SerializedData]
		,[FilingCreated]
		,[FilingSent]
		,[CreatedBy]
		,[UpdatedBy])
	 VALUES(
		 @FilingStartDate			
		,@FilingEndDate				
		,@ManagerCompanyGuid		
		,@SiteGuid						
		,@ControlNumber				
		,@OriginalControlNumber	
		,@TransSetControlNumber
		,@ReportType					
		,@Modifier						
		,@FilingStatus				
		,@RawDataFileName			
		,@EasyReadFileName			
		,@EdiReport					
		,@EasyReadReport	
		,@SerializedData			
		,@FilingCreated
		,@FilingSent
		,@UpdatedBy		
		,@UpdatedBy		
	 )
END

GO