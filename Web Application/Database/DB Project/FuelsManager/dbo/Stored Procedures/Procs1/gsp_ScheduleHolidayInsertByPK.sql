CREATE PROCEDURE [dbo].[gsp_ScheduleHolidayInsertByPK]
(
		@ScheduleHolidayGuid uniqueidentifier=NULL OUTPUT
	,	@SiteGuid uniqueidentifier=NULL
	,	@Enabled bit=NULL
	,	@OpeningTime datetimeoffset(7)=NULL
	,	@ClosingTime datetimeoffset(7)=NULL
	,	@EndOfDayEnabled bit=NULL
	,	@EndOfDayTime datetimeoffset(7)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@HolidayDate datetimeoffset(7)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ScheduleHolidayInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4252767 -05:00
	-- Purpose: Insert into table [dbo].[tblScheduleHoliday]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ScheduleHolidayGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblScheduleHoliday] 
		(
			[ScheduleHolidayGuid]
		,	[SiteGuid]
		,	[Enabled]
		,	[OpeningTime]
		,	[ClosingTime]
		,	[EndOfDayEnabled]
		,	[EndOfDayTime]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[HolidayDate]
		)
		VALUES
		(
			@ScheduleHolidayGuid
		,	@SiteGuid
		,	@Enabled
		,	@OpeningTime
		,	@ClosingTime
		,	@EndOfDayEnabled
		,	@EndOfDayTime
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@HolidayDate
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblScheduleHoliday]           
		WHERE ScheduleHolidayGuid=@ScheduleHolidayGuid;
	
 
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
						+ 'Procedure Name: gsp_ScheduleHolidayInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
