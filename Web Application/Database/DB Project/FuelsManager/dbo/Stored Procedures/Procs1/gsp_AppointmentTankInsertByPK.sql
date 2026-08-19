CREATE PROCEDURE [dbo].[gsp_AppointmentTankInsertByPK]
(
		@AppointmentTankGuid uniqueidentifier=NULL OUTPUT
	,	@TankGuid uniqueidentifier=NULL
	,	@TestSetDefinitionGuid uniqueidentifier=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@AssetText nvarchar(100)=NULL
	,	@AppointmentCategory nvarchar(50)=NULL
	,	@AppointmentIsSingle bit=NULL
	,	@ScheduleOnWeekends bit=NULL
	,	@ScheduleOnHolidays bit=NULL
	,	@StartDate datetimeoffset(7)=NULL
	,	@Duration int=NULL
	,	@AppointmentPeriod int=NULL
	,	@AppointmentPeriodText nvarchar(50)=NULL
	,	@Description nvarchar(50)=NULL
	,	@AppointmentTimeInterval int=NULL
	,	@AppointmentDayOfTheWeekText nvarchar(20)=NULL
	,	@AppointmentDayOfTheWeek int=NULL
	,	@AppointmentReoccuranceInterval int=NULL
	,	@AppointmentOption2Selected bit=NULL
	,	@AppointmentTimeOptionSelectionText nvarchar(20)=NULL
	,	@AppointmentTimeOptionSelection int=NULL
	,	@AppointmentMonthSelectionText nvarchar(20)=NULL
	,	@AppointmentMonthSelection int=NULL
	,	@AppointmentDayOfTheMonth int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AppointmentTankInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0732767 -05:00
	-- Purpose: Insert into table [dbo].[tblAppointmentTank]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AppointmentTankGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAppointmentTank] 
		(
			[AppointmentTankGuid]
		,	[TankGuid]
		,	[TestSetDefinitionGuid]
		,	[SiteGuid]
		,	[AssetText]
		,	[AppointmentCategory]
		,	[AppointmentIsSingle]
		,	[ScheduleOnWeekends]
		,	[ScheduleOnHolidays]
		,	[StartDate]
		,	[Duration]
		,	[AppointmentPeriod]
		,	[AppointmentPeriodText]
		,	[Description]
		,	[AppointmentTimeInterval]
		,	[AppointmentDayOfTheWeekText]
		,	[AppointmentDayOfTheWeek]
		,	[AppointmentReoccuranceInterval]
		,	[AppointmentOption2Selected]
		,	[AppointmentTimeOptionSelectionText]
		,	[AppointmentTimeOptionSelection]
		,	[AppointmentMonthSelectionText]
		,	[AppointmentMonthSelection]
		,	[AppointmentDayOfTheMonth]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@AppointmentTankGuid
		,	@TankGuid
		,	@TestSetDefinitionGuid
		,	@SiteGuid
		,	@AssetText
		,	@AppointmentCategory
		,	@AppointmentIsSingle
		,	@ScheduleOnWeekends
		,	@ScheduleOnHolidays
		,	@StartDate
		,	@Duration
		,	@AppointmentPeriod
		,	@AppointmentPeriodText
		,	@Description
		,	@AppointmentTimeInterval
		,	@AppointmentDayOfTheWeekText
		,	@AppointmentDayOfTheWeek
		,	@AppointmentReoccuranceInterval
		,	@AppointmentOption2Selected
		,	@AppointmentTimeOptionSelectionText
		,	@AppointmentTimeOptionSelection
		,	@AppointmentMonthSelectionText
		,	@AppointmentMonthSelection
		,	@AppointmentDayOfTheMonth
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAppointmentTank]           
		WHERE AppointmentTankGuid=@AppointmentTankGuid;
	
 
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
						+ 'Procedure Name: gsp_AppointmentTankInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
