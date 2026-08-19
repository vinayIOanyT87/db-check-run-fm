CREATE PROCEDURE [dbo].[gsp_ScheduleTerminalOperationInsertByPK]
(
		@ScheduleTerminalOperationGuid uniqueidentifier=NULL OUTPUT
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupDayOfWeekIndex int=NULL
	,	@Enabled bit=NULL
	,	@OpeningTime datetimeoffset(7)=NULL
	,	@ClosingTime datetimeoffset(7)=NULL
	,	@EndOfDayEnabled bit=NULL
	,	@EndOfDayTime datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ScheduleTerminalOperationInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4292767 -05:00
	-- Purpose: Insert into table [dbo].[tblScheduleTerminalOperation]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ScheduleTerminalOperationGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblScheduleTerminalOperation] 
		(
			[ScheduleTerminalOperationGuid]
		,	[SiteGuid]
		,	[LookupDayOfWeekIndex]
		,	[Enabled]
		,	[OpeningTime]
		,	[ClosingTime]
		,	[EndOfDayEnabled]
		,	[EndOfDayTime]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@ScheduleTerminalOperationGuid
		,	@SiteGuid
		,	@LookupDayOfWeekIndex
		,	@Enabled
		,	@OpeningTime
		,	@ClosingTime
		,	@EndOfDayEnabled
		,	@EndOfDayTime
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblScheduleTerminalOperation]           
		WHERE ScheduleTerminalOperationGuid=@ScheduleTerminalOperationGuid;
	
 
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
						+ 'Procedure Name: gsp_ScheduleTerminalOperationInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
