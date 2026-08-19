CREATE PROCEDURE [dbo].[gsp_TestSetEquipmentResultsInsertByPK]
(
		@TestSetEquipmentResultGuid uniqueidentifier=NULL OUTPUT
	,	@ResultTimeStamp datetimeoffset(7)=NULL
	,	@TestSetName nvarchar(80)=NULL
	,	@Inspector nvarchar(100)=NULL
	,	@Supervisor nvarchar(100)=NULL
	,	@EquipmentID nvarchar(50)=NULL
	,	@SampleNumber int=NULL
	,	@SampleSize float=NULL
	,	@IsRetest bit=NULL
	,	@PreviousSampleNumber int=NULL
	,	@DocumentNumber nvarchar(50)=NULL
	,	@Memo nvarchar(1000)=NULL
	,	@GallonsRepresented float=NULL
	,	@Override bit=NULL
	,	@DeleteFlag bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupTestSetStatusIndex int=NULL
	,	@EquipmentGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TestSetEquipmentResultsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5092767 -05:00
	-- Purpose: Insert into table [dbo].[tblTestSetEquipmentResults]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TestSetEquipmentResultGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTestSetEquipmentResults] 
		(
			[TestSetEquipmentResultGuid]
		,	[ResultTimeStamp]
		,	[TestSetName]
		,	[Inspector]
		,	[Supervisor]
		,	[EquipmentID]
		,	[SampleNumber]
		,	[SampleSize]
		,	[IsRetest]
		,	[PreviousSampleNumber]
		,	[DocumentNumber]
		,	[Memo]
		,	[GallonsRepresented]
		,	[Override]
		,	[DeleteFlag]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[LookupTestSetStatusIndex]
		,	[EquipmentGuid]
		)
		VALUES
		(
			@TestSetEquipmentResultGuid
		,	@ResultTimeStamp
		,	@TestSetName
		,	@Inspector
		,	@Supervisor
		,	@EquipmentID
		,	@SampleNumber
		,	@SampleSize
		,	@IsRetest
		,	@PreviousSampleNumber
		,	@DocumentNumber
		,	@Memo
		,	@GallonsRepresented
		,	@Override
		,	@DeleteFlag
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@LookupTestSetStatusIndex
		,	@EquipmentGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTestSetEquipmentResults]           
		WHERE TestSetEquipmentResultGuid=@TestSetEquipmentResultGuid;
	
 
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
						+ 'Procedure Name: gsp_TestSetEquipmentResultsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
