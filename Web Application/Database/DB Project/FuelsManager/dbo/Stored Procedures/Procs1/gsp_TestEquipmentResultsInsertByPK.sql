CREATE PROCEDURE [dbo].[gsp_TestEquipmentResultsInsertByPK]
(
		@TestEquipmentResultGuid uniqueidentifier=NULL OUTPUT
	,	@TestName nvarchar(80)=NULL
	,	@Measurement nvarchar(50)=NULL
	,	@TestDate datetimeoffset(7)=NULL
	,	@DeleteFlag bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@PerformedBy nvarchar(100)=NULL
	,	@Supervisor nvarchar(100)=NULL
	,	@LookupTestSetStatusIndex int=NULL
	,	@TestSetEquipmentResultGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TestEquipmentResultsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5052767 -05:00
	-- Purpose: Insert into table [dbo].[tblTestEquipmentResults]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TestEquipmentResultGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTestEquipmentResults] 
		(
			[TestEquipmentResultGuid]
		,	[TestName]
		,	[Measurement]
		,	[TestDate]
		,	[DeleteFlag]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PerformedBy]
		,	[Supervisor]
		,	[LookupTestSetStatusIndex]
		,	[TestSetEquipmentResultGuid]
		)
		VALUES
		(
			@TestEquipmentResultGuid
		,	@TestName
		,	@Measurement
		,	@TestDate
		,	@DeleteFlag
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@PerformedBy
		,	@Supervisor
		,	@LookupTestSetStatusIndex
		,	@TestSetEquipmentResultGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTestEquipmentResults]           
		WHERE TestEquipmentResultGuid=@TestEquipmentResultGuid;
	
 
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
						+ 'Procedure Name: gsp_TestEquipmentResultsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
