CREATE PROCEDURE [dbo].[gsp_DispatchConfigurationInsertByPK]
(
		@DispatchConfigurationGuid uniqueidentifier=NULL OUTPUT
	,	@SiteGuid uniqueidentifier=NULL
	,	@ID nvarchar(50)=NULL
	,	@DisplayCurrentTime bit=NULL
	,	@DispatchDataRefreshPeriod int=NULL
	,	@TabularViewDisplayMilitaryDate bit=NULL
	,	@QuantityNotZeroCheck bit=NULL
	,	@ExactlyOneManagerCheck bit=NULL
	,	@ExactlyOneOwnerCheck bit=NULL
	,	@DispatchFuelAdditiveFlagCheck bit=NULL
	,	@FastLogFuelAdditiveFlagCheck bit=NULL
	,	@FillstandVolumeWithinToleranceCheck bit=NULL
	,	@ReturnToBulkVolumeWithinToleranceCheck bit=NULL
	,	@RecirculationVolumesGreaterThanZeroCheck bit=NULL
	,	@OperatorIsInCheck bit=NULL
	,	@OperatorNotAssignedCheck bit=NULL
	,	@OperatorHasRequiredTrainingCheck bit=NULL
	,	@OperatorTrainingNotExpiredCheck bit=NULL
	,	@OperatorNotLockedOutCheck bit=NULL
	,	@OperatorHasRequiredQualificationsCheck bit=NULL
	,	@OperatorQualificationsNotExpiredCheck bit=NULL
	,	@DefuelStatusCheck bit=NULL
	,	@RefuelStatusCheck bit=NULL
	,	@EquipmentFuelGradeCheck bit=NULL
	,	@EquipmentNotLockedOutCheck bit=NULL
	,	@EquipmentNotAssignedCheck bit=NULL
	,	@EquipmentInServiceCheck bit=NULL
	,	@TagLicenseNotExpiredCheck bit=NULL
	,	@TestInspectionNotExpiredCheck bit=NULL
	,	@QualityControlCheckupDateCheck bit=NULL
	,	@CautionQualityTagCheck bit=NULL
	,	@WarningQualityTagCheck bit=NULL
	,	@DangerQualityTagCheck bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@EnableServiceRequests bit=NULL
	,	@AutomaticRestartDelay int=NULL
	,	@EquipmentRequired bit=NULL
	,	@PersonnelRequired bit=NULL
	,	@FillToActualOrStandard int=NULL
	,	@OperationalWindowPastHours int=NULL
	,	@OperationalWindowFutureHours int=NULL
	,	@ShowGridLines bit=NULL
	,	@StaticTimeDisplay bit=NULL
	,	@UseArrivalTime bit=NULL
	,	@UseStartTime bit=NULL
	,	@UseStopTime bit=NULL
	,	@FuelsManagerReportURL nvarchar(max)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_DispatchConfigurationInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1262767 -05:00
	-- Purpose: Insert into table [dbo].[tblDispatchConfiguration]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @DispatchConfigurationGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblDispatchConfiguration] 
		(
			[DispatchConfigurationGuid]
		,	[SiteGuid]
		,	[ID]
		,	[DisplayCurrentTime]
		,	[DispatchDataRefreshPeriod]
		,	[TabularViewDisplayMilitaryDate]
		,	[QuantityNotZeroCheck]
		,	[ExactlyOneManagerCheck]
		,	[ExactlyOneOwnerCheck]
		,	[DispatchFuelAdditiveFlagCheck]
		,	[FastLogFuelAdditiveFlagCheck]
		,	[FillstandVolumeWithinToleranceCheck]
		,	[ReturnToBulkVolumeWithinToleranceCheck]
		,	[RecirculationVolumesGreaterThanZeroCheck]
		,	[OperatorIsInCheck]
		,	[OperatorNotAssignedCheck]
		,	[OperatorHasRequiredTrainingCheck]
		,	[OperatorTrainingNotExpiredCheck]
		,	[OperatorNotLockedOutCheck]
		,	[OperatorHasRequiredQualificationsCheck]
		,	[OperatorQualificationsNotExpiredCheck]
		,	[DefuelStatusCheck]
		,	[RefuelStatusCheck]
		,	[EquipmentFuelGradeCheck]
		,	[EquipmentNotLockedOutCheck]
		,	[EquipmentNotAssignedCheck]
		,	[EquipmentInServiceCheck]
		,	[TagLicenseNotExpiredCheck]
		,	[TestInspectionNotExpiredCheck]
		,	[QualityControlCheckupDateCheck]
		,	[CautionQualityTagCheck]
		,	[WarningQualityTagCheck]
		,	[DangerQualityTagCheck]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[EnableServiceRequests]
		,	[AutomaticRestartDelay]
		,	[EquipmentRequired]
		,	[PersonnelRequired]
		,	[FillToActualOrStandard]
		,	[OperationalWindowPastHours]
		,	[OperationalWindowFutureHours]
		,	[ShowGridLines]
		,	[StaticTimeDisplay]
		,	[UseArrivalTime]
		,	[UseStartTime]
		,	[UseStopTime]
		,	[FuelsManagerReportURL]
		)
		VALUES
		(
			@DispatchConfigurationGuid
		,	@SiteGuid
		,	@ID
		,	@DisplayCurrentTime
		,	@DispatchDataRefreshPeriod
		,	@TabularViewDisplayMilitaryDate
		,	@QuantityNotZeroCheck
		,	@ExactlyOneManagerCheck
		,	@ExactlyOneOwnerCheck
		,	@DispatchFuelAdditiveFlagCheck
		,	@FastLogFuelAdditiveFlagCheck
		,	@FillstandVolumeWithinToleranceCheck
		,	@ReturnToBulkVolumeWithinToleranceCheck
		,	@RecirculationVolumesGreaterThanZeroCheck
		,	@OperatorIsInCheck
		,	@OperatorNotAssignedCheck
		,	@OperatorHasRequiredTrainingCheck
		,	@OperatorTrainingNotExpiredCheck
		,	@OperatorNotLockedOutCheck
		,	@OperatorHasRequiredQualificationsCheck
		,	@OperatorQualificationsNotExpiredCheck
		,	@DefuelStatusCheck
		,	@RefuelStatusCheck
		,	@EquipmentFuelGradeCheck
		,	@EquipmentNotLockedOutCheck
		,	@EquipmentNotAssignedCheck
		,	@EquipmentInServiceCheck
		,	@TagLicenseNotExpiredCheck
		,	@TestInspectionNotExpiredCheck
		,	@QualityControlCheckupDateCheck
		,	@CautionQualityTagCheck
		,	@WarningQualityTagCheck
		,	@DangerQualityTagCheck
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@EnableServiceRequests
		,	@AutomaticRestartDelay
		,	@EquipmentRequired
		,	@PersonnelRequired
		,	@FillToActualOrStandard
		,	@OperationalWindowPastHours
		,	@OperationalWindowFutureHours
		,	@ShowGridLines
		,	@StaticTimeDisplay
		,	@UseArrivalTime
		,	@UseStartTime
		,	@UseStopTime
		,	@FuelsManagerReportURL
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblDispatchConfiguration]           
		WHERE DispatchConfigurationGuid=@DispatchConfigurationGuid;
	
 
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
						+ 'Procedure Name: gsp_DispatchConfigurationInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
