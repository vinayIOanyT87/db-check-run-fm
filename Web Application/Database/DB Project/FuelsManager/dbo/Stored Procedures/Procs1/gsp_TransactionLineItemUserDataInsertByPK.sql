CREATE PROCEDURE [dbo].[gsp_TransactionLineItemUserDataInsertByPK]
(
		@TransactionLineItemUserDataGuid uniqueidentifier=NULL OUTPUT
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@UserData9 nvarchar(60)=NULL
	,	@UserData10 nvarchar(60)=NULL
	,	@UserData11 nvarchar(60)=NULL
	,	@UserData12 nvarchar(60)=NULL
	,	@UserData13 nvarchar(60)=NULL
	,	@UserData14 nvarchar(60)=NULL
	,	@UserData15 nvarchar(60)=NULL
	,	@UserData16 nvarchar(60)=NULL
	,	@UserData17 nvarchar(60)=NULL
	,	@UserData18 nvarchar(60)=NULL
	,	@UserData19 nvarchar(60)=NULL
	,	@UserData20 nvarchar(60)=NULL
	,	@UserData21 nvarchar(60)=NULL
	,	@UserData22 nvarchar(60)=NULL
	,	@UserData23 nvarchar(60)=NULL
	,	@UserData24 nvarchar(60)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@TransactionLineItemGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionLineItemUserDataInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5422767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionLineItemUserData]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionLineItemUserDataGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionLineItemUserData] 
		(
			[TransactionLineItemUserDataGuid]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[UserData9]
		,	[UserData10]
		,	[UserData11]
		,	[UserData12]
		,	[UserData13]
		,	[UserData14]
		,	[UserData15]
		,	[UserData16]
		,	[UserData17]
		,	[UserData18]
		,	[UserData19]
		,	[UserData20]
		,	[UserData21]
		,	[UserData22]
		,	[UserData23]
		,	[UserData24]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[TransactionLineItemGuid]
		)
		VALUES
		(
			@TransactionLineItemUserDataGuid
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@UserData9
		,	@UserData10
		,	@UserData11
		,	@UserData12
		,	@UserData13
		,	@UserData14
		,	@UserData15
		,	@UserData16
		,	@UserData17
		,	@UserData18
		,	@UserData19
		,	@UserData20
		,	@UserData21
		,	@UserData22
		,	@UserData23
		,	@UserData24
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@TransactionLineItemGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionLineItemUserData]           
		WHERE TransactionLineItemUserDataGuid=@TransactionLineItemUserDataGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionLineItemUserDataInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
