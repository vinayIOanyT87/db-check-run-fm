CREATE PROCEDURE [dbo].[gsp_FuelCardsInsertByPK]
(
		@FuelCardGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(50)=NULL
	,	@Provider nvarchar(50)=NULL
	,	@ActivationStatus int=NULL
	,	@InactivityPeriod int=NULL
	,	@Notes nvarchar(max)=NULL
	,	@StatusModifiedDate datetimeoffset(7)=NULL
	,	@StatusModifiedBy nvarchar(50)=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@BillToCompanyGuid uniqueidentifier=NULL
	,	@ManagerCompanyGuid uniqueidentifier=NULL
	,	@OwnerCompanyGuid uniqueidentifier=NULL
	,	@ShipperCompanyGuid uniqueidentifier=NULL
	,	@ShipToCompanyGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_FuelCardsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2282767 -05:00
	-- Purpose: Insert into table [dbo].[tblFuelCards]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @FuelCardGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblFuelCards] 
		(
			[FuelCardGuid]
		,	[ID]
		,	[Provider]
		,	[ActivationStatus]
		,	[InactivityPeriod]
		,	[Notes]
		,	[StatusModifiedDate]
		,	[StatusModifiedBy]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[BillToCompanyGuid]
		,	[ManagerCompanyGuid]
		,	[OwnerCompanyGuid]
		,	[ShipperCompanyGuid]
		,	[ShipToCompanyGuid]
		)
		VALUES
		(
			@FuelCardGuid
		,	@ID
		,	@Provider
		,	@ActivationStatus
		,	@InactivityPeriod
		,	@Notes
		,	@StatusModifiedDate
		,	@StatusModifiedBy
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@BillToCompanyGuid
		,	@ManagerCompanyGuid
		,	@OwnerCompanyGuid
		,	@ShipperCompanyGuid
		,	@ShipToCompanyGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblFuelCards]           
		WHERE FuelCardGuid=@FuelCardGuid;
	
 
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: gsp_FuelCardsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
