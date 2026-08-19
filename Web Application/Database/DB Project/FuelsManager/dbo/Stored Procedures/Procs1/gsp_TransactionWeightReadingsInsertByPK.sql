CREATE PROCEDURE [dbo].[gsp_TransactionWeightReadingsInsertByPK]
(
		@TransactionWeightReadingGuid uniqueidentifier=NULL OUTPUT
	,	@CompartmentID nvarchar(30)=NULL
	,	@BeginQuantityValue float=NULL
	,	@RequestedQuantityValue float=NULL
	,	@FinalQuantityValue float=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@TransVersion bigint=NULL
	,	@TransactionGuid uniqueidentifier=NULL
	,	@FuelsManagerVersionNumber int=NULL
	,	@SourceVersionNumber int=NULL
	,	@HistoricalFlag bit=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionWeightReadingsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5882767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionWeightReadings]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionWeightReadingGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionWeightReadings] 
		(
			[TransactionWeightReadingGuid]
		,	[CompartmentID]
		,	[BeginQuantityValue]
		,	[RequestedQuantityValue]
		,	[FinalQuantityValue]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[TransVersion]
		,	[TransactionGuid]
		,	[FuelsManagerVersionNumber]
		,	[SourceVersionNumber]
		,	[HistoricalFlag]
		)
		VALUES
		(
			@TransactionWeightReadingGuid
		,	@CompartmentID
		,	@BeginQuantityValue
		,	@RequestedQuantityValue
		,	@FinalQuantityValue
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@TransVersion
		,	@TransactionGuid
		,	@FuelsManagerVersionNumber
		,	@SourceVersionNumber
		,	@HistoricalFlag
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionWeightReadings]           
		WHERE TransactionWeightReadingGuid=@TransactionWeightReadingGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionWeightReadingsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
