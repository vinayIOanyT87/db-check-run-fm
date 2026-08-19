CREATE PROCEDURE [dbo].[gsp_TransactionUserDataInsertByPK]
(
		@TransactionUserDataGuid uniqueidentifier=NULL OUTPUT
	,	@UserData1 nvarchar(max)=NULL
	,	@UserData2 nvarchar(max)=NULL
	,	@UserData3 nvarchar(max)=NULL
	,	@UserData4 nvarchar(max)=NULL
	,	@UserData5 nvarchar(max)=NULL
	,	@UserData6 nvarchar(max)=NULL
	,	@UserData7 nvarchar(max)=NULL
	,	@UserData8 nvarchar(max)=NULL
	,	@UserData9 nvarchar(max)=NULL
	,	@UserData10 nvarchar(max)=NULL
	,	@UserData11 nvarchar(max)=NULL
	,	@UserData12 nvarchar(max)=NULL
	,	@UserData13 nvarchar(max)=NULL
	,	@UserData14 nvarchar(max)=NULL
	,	@UserData15 nvarchar(max)=NULL
	,	@UserData16 nvarchar(max)=NULL
	,	@UserData17 nvarchar(max)=NULL
	,	@UserData18 nvarchar(max)=NULL
	,	@UserData19 nvarchar(max)=NULL
	,	@UserData20 nvarchar(max)=NULL
	,	@UserData21 nvarchar(max)=NULL
	,	@UserData22 nvarchar(max)=NULL
	,	@UserData23 nvarchar(max)=NULL
	,	@UserData24 nvarchar(max)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@TransactionGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionUserDataInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5842767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionUserData]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionUserDataGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionUserData] 
		(
			[TransactionUserDataGuid]
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
		,	[TransactionGuid]
		)
		VALUES
		(
			@TransactionUserDataGuid
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
		,	@TransactionGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionUserData]           
		WHERE TransactionUserDataGuid=@TransactionUserDataGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionUserDataInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
