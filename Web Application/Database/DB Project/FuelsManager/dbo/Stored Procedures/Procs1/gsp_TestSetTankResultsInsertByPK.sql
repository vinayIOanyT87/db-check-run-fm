CREATE PROCEDURE [dbo].[gsp_TestSetTankResultsInsertByPK]
(
		@TestSetTankResultGuid uniqueidentifier=NULL OUTPUT
	,	@ResultTimeStamp datetimeoffset(7)=NULL
	,	@TestSetName nvarchar(80)=NULL
	,	@Inspector nvarchar(100)=NULL
	,	@Supervisor nvarchar(100)=NULL
	,	@TankID nvarchar(50)=NULL
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
	,	@TankGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TestSetTankResultsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5122767 -05:00
	-- Purpose: Insert into table [dbo].[tblTestSetTankResults]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TestSetTankResultGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTestSetTankResults] 
		(
			[TestSetTankResultGuid]
		,	[ResultTimeStamp]
		,	[TestSetName]
		,	[Inspector]
		,	[Supervisor]
		,	[TankID]
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
		,	[TankGuid]
		)
		VALUES
		(
			@TestSetTankResultGuid
		,	@ResultTimeStamp
		,	@TestSetName
		,	@Inspector
		,	@Supervisor
		,	@TankID
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
		,	@TankGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTestSetTankResults]           
		WHERE TestSetTankResultGuid=@TestSetTankResultGuid;
	
 
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
						+ 'Procedure Name: gsp_TestSetTankResultsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
