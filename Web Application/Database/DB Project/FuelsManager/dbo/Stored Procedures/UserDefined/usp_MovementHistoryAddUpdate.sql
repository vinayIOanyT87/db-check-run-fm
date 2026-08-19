CREATE PROCEDURE [dbo].[usp_MovementHistoryAddUpdate]
(
	@MovementHistoryParmTable dbo.MovementHistoryType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
		MERGE tblMovementHistory AS target
		USING (
			SELECT 
				MovementHistoryGuid
				, SiteGuid
				, [Name]
				, [Node]
				, [InitiationCount]
				, [RecordType]
				, [TimeStamp]
				, [ParentGuid]
				, AutoStart
				, AutoStartTime
				, AutoStop
				, AutoStopTime
				, CloseoutDataModifiedBy
				, CloseoutDensityProductInAir
				, CloseoutDensityProductObserved
				, CloseoutDensityProductObservedTime
				, CloseoutDensityProductStandard
				, CloseoutDensityProductStandardTime
				, CloseoutDensityProductStandardInAir
				, CloseoutLevelProduct
				, CloseoutLevelProductTime
				, CloseoutLevelWater
				, CloseoutMassLiquid
				, CloseoutPercentBsw
				, CloseoutRoofMass
				, CloseoutTankShellCorrection
				, CloseoutTemperatureAmbient
				, CloseoutTemperatureAmbientTime
				, CloseoutTemperatureDensity
				, CloseoutTemperatureProduct
				, CloseoutTime
				, CloseoutTransferGov
				, CloseoutTransferNsv
				, CloseoutTransferMassLiquid
				, CloseoutTransferVolumeWater
				, CloseoutVolumeBsw
				, CloseoutVolumeCorrectionFactor
				, CloseoutVolumeGrossObserved
				, CloseoutVolumeGrossStandard
				, CloseoutVolumeNetStandard
				, CloseoutVolumeRoofCorrection
				, CloseoutVolumeTotalObserved
				, CloseoutVolumeWater
				, Comment
				, [Type]
				, OrderNumber
				, PlannedStartTime
				, Product
				, ProductDescription
				, StartTime
				, StopTime
				, StartDensityProductObserved
				, StartDensityProductObservedTime
				, StartDensityProductObservedInAir
				, StartDensityProductStandard
				, StartDensityProductStandardTime
				, StartUserID
				, StartLevelProduct
				, StartLevelProductTime
				, StartLevelWater
				, StartLevelWaterTime
				, StartMassLiquid
				, StartPercentBsw
				, StartTankShellCorrection
				, StartTemperatureAmbient
				, StartTemperatureAmbientTime
				, StartTemperatureProduct
				, StartTemperatureProductTime
				, StartTemperatureDensity
				, StartTemperatureDensityTime
				, StartVolume
				, StartVolumeBsw
				, StartVolumeCorrectionFactor
				, StartVolumeGrossObserved
				, StartVolumeGrossStandard
				, StartVolumeNetStandard
				, StartVolumeRoofCorrection
				, StartVolumeTotalObserved
				, StartVolumeWater
				, UnitsLevelProductIndex
				, UnitsTemperatureAmbientIndex
				, UnitsTemperatureDensityIndex
				, UnitsTemperatureProductIndex
				, UnitsDensityProductObservedIndex
				, UnitsDensityProductStandardIndex
				, UnitsVolumeIndex
				, UnitsMassIndex
				, DecimalPlacesVolume
				, DecimalPlacesLevel
				, DecimalPlacesDensity
				, DecimalPlacesTemperature
				, UserData01
				, UserData02
				, UserData03
				, UserData04
				, UserData05
				, UserData06
				, UserData07
				, UserData08
				, UserData09
				, UserData10
				, TransferDeviation
				, TransferPercentDeviation
				, DecimalPlacesPercent
				, TransferMode
				, TransferStatus
				, TransferTarget
				, TransferTargetUnitsIndex
				, TransferLevelTarget
				, TransferVolumeTarget
				, TransferTimeRemaining
				, TransferDirection
				, CommentDateTime
				, CommentUserID
				, [Status]
				, VolumeWater
				, LevelProduct
				, StartDensityProductStandardInAir
				, TransferredVolumeWater
				, TransferredVolume
				, MidnightRecord
				, PointGuid
				, RootParentGuid
				, RecordSeq
				, CreatedBy
				, UpdatedBy
			FROM @MovementHistoryParmTable
			) AS source
		ON source.MovementHistoryGuid = target.MovementHistoryGuid
		   AND source.SiteGuid = target.SiteGuid
		WHEN MATCHED THEN UPDATE SET
			SiteGuid								= source.SiteGuid
			, [Name]								= source.[Name]
			, [Node]								= source.[Node]
			, [InitiationCount]						= source.[InitiationCount]
			, [RecordType]							= source.[RecordType]
			, [TimeStamp]							= source.[TimeStamp]
			, [ParentGuid]							= source.[ParentGuid]
			, AutoStart								= source.AutoStart
			, AutoStartTime							= source.AutoStartTime
			, AutoStop								= source.AutoStop
			, AutoStopTime							= source.AutoStopTime
			, CloseoutDataModifiedBy				= source.CloseoutDataModifiedBy
			, CloseoutDensityProductInAir			= source.CloseoutDensityProductInAir
			, CloseoutDensityProductObserved		= source.CloseoutDensityProductObserved
			, CloseoutDensityProductObservedTime	= source.CloseoutDensityProductObservedTime
			, CloseoutDensityProductStandard		= source.CloseoutDensityProductStandard
			, CloseoutDensityProductStandardTime	= source.CloseoutDensityProductStandardTime
			, CloseoutDensityProductStandardInAir	= source.CloseoutDensityProductStandardInAir
			, CloseoutLevelProduct					= source.CloseoutLevelProduct
			, CloseoutLevelProductTime				= source.CloseoutLevelProductTime
			, CloseoutLevelWater					= source.CloseoutLevelWater
			, CloseoutMassLiquid					= source.CloseoutMassLiquid
			, CloseoutPercentBsw					= source.CloseoutPercentBsw
			, CloseoutRoofMass						= source.CloseoutRoofMass
			, CloseoutTankShellCorrection			= source.CloseoutTankShellCorrection
			, CloseoutTemperatureAmbient			= source.CloseoutTemperatureAmbient
			, CloseoutTemperatureAmbientTime		= source.CloseoutTemperatureAmbientTime
			, CloseoutTemperatureDensity			= source.CloseoutTemperatureDensity
			, CloseoutTemperatureProduct			= source.CloseoutTemperatureProduct
			, CloseoutTime							= source.CloseoutTime
			, CloseoutTransferGov					= source.CloseoutTransferGov
			, CloseoutTransferNsv					= source.CloseoutTransferNsv
			, CloseoutTransferMassLiquid			= source.CloseoutTransferMassLiquid
			, CloseoutTransferVolumeWater			= source.CloseoutTransferVolumeWater
			, CloseoutVolumeBsw						= source.CloseoutVolumeBsw
			, CloseoutVolumeCorrectionFactor		= source.CloseoutVolumeCorrectionFactor
			, CloseoutVolumeGrossObserved			= source.CloseoutVolumeGrossObserved
			, CloseoutVolumeGrossStandard			= source.CloseoutVolumeGrossStandard
			, CloseoutVolumeNetStandard				= source.CloseoutVolumeNetStandard
			, CloseoutVolumeRoofCorrection			= source.CloseoutVolumeRoofCorrection
			, CloseoutVolumeTotalObserved			= source.CloseoutVolumeTotalObserved
			, CloseoutVolumeWater					= source.CloseoutVolumeWater
			, Comment									= source.Comment
			, [Type]										= source.[Type]
			, OrderNumber								= source.OrderNumber
			, PlannedStartTime						= source.PlannedStartTime
			, Product									= source.Product
			, ProductDescription					= source.ProductDescription
			, StartTime								= source.StartTime
			, StopTime								= source.StopTime
			, StartDensityProductObserved			= source.StartDensityProductObserved
			, StartDensityProductObservedTime		= source.StartDensityProductObservedTime
			, StartDensityProductObservedInAir		= source.StartDensityProductObservedInAir
			, StartDensityProductStandard			= source.StartDensityProductStandard
			, StartDensityProductStandardTime		= source.StartDensityProductStandardTime
			, StartUserID							= source.StartUserID
			, StartLevelProduct						= source.StartLevelProduct
			, StartLevelProductTime					= source.StartLevelProductTime
			, StartLevelWater						= source.StartLevelWater
			, StartLevelWaterTime					= source.StartLevelWaterTime
			, StartMassLiquid						= source.StartMassLiquid
			, StartPercentBsw						= source.StartPercentBsw
			, StartTankShellCorrection				= source.StartTankShellCorrection
			, StartTemperatureAmbient				= source.StartTemperatureAmbient
			, StartTemperatureAmbientTime			= source.StartTemperatureAmbientTime
			, StartTemperatureProduct				= source.StartTemperatureProduct
			, StartTemperatureProductTime			= source.StartTemperatureProductTime
			, StartTemperatureDensity				= source.StartTemperatureDensity
			, StartTemperatureDensityTime			= source.StartTemperatureDensityTime
			, StartVolume							= source.StartVolume
			, StartVolumeBsw					= source.StartVolumeBsw
			, StartVolumeCorrectionFactor			= source.StartVolumeCorrectionFactor
			, StartVolumeGrossObserved				= source.StartVolumeGrossObserved
			, StartVolumeGrossStandard				= source.StartVolumeGrossStandard
			, StartVolumeNetStandard				= source.StartVolumeNetStandard
			, StartVolumeRoofCorrection				= source.StartVolumeRoofCorrection
			, StartVolumeTotalObserved				= source.StartVolumeTotalObserved
			, StartVolumeWater						= source.StartVolumeWater
			, UnitsLevelProductIndex				= source.UnitsLevelProductIndex
			, UnitsTemperatureAmbientIndex			= source.UnitsTemperatureAmbientIndex
			, UnitsTemperatureDensityIndex			= source.UnitsTemperatureDensityIndex
			, UnitsTemperatureProductIndex			= source.UnitsTemperatureProductIndex
			, UnitsDensityProductObservedIndex		= source.UnitsDensityProductObservedIndex
			, UnitsDensityProductStandardIndex		= source.UnitsDensityProductStandardIndex
			, UnitsVolumeIndex						= source.UnitsVolumeIndex
			, UnitsMassIndex						= source.UnitsMassIndex
			, DecimalPlacesVolume					= source.DecimalPlacesVolume
			, DecimalPlacesLevel					= source.DecimalPlacesLevel
			, DecimalPlacesDensity					= source.DecimalPlacesDensity
			, DecimalPlacesTemperature				= source.DecimalPlacesTemperature
			, UserData01							= source.UserData01
			, UserData02							= source.UserData02
			, UserData03							= source.UserData03
			, UserData04							= source.UserData04
			, UserData05							= source.UserData05
			, UserData06							= source.UserData06
			, UserData07							= source.UserData07
			, UserData08							= source.UserData08
			, UserData09							= source.UserData09
			, UserData10							= source.UserData10
			, TransferDeviation					= source.TransferDeviation
			, TransferPercentDeviation			= source.TransferPercentDeviation
			, DecimalPlacesPercent				= source.[DecimalPlacesPercent]
			, TransferMode							= source.TransferMode
			, TransferStatus						= source.TransferStatus
			, TransferTarget						= source.TransferTarget
			, TransferTargetUnitsIndex		= source.TransferTargetUnitsIndex
			, TransferLevelTarget				= source.TransferLevelTarget
			, TransferVolumeTarget				= source.TransferVolumeTarget
			, TransferTimeRemaining				= source.TransferTimeRemaining
			, TransferDirection					= source.TransferDirection
			, CommentDateTime						= source.CommentDateTime
			, CommentUserID						= source.CommentUserID
			, [Status]								= source.[Status]
			, VolumeWater							= source.VolumeWater
			, LevelProduct							= source.LevelProduct
			, StartDensityProductStandardInAir		= source.StartDensityProductStandardInAir
			, TransferredVolumeWater			= source.TransferredVolumeWater
			, TransferredVolume					= source.TransferredVolume
			, MidnightRecord						= source.MidnightRecord
			, PointGuid								= source.PointGuid
			, RootParentGuid						= source.RootParentGuid
			, RecordSeq								= source.RecordSeq
			, UpdatedDate							= SYSDATETIMEOFFSET()
			, UpdatedBy								= source.UpdatedBy
		WHEN NOT MATCHED THEN INSERT 
		(
			MovementHistoryGuid
			, SiteGuid
			, [Name]
			, [Node]
			, [InitiationCount]
			, [RecordType]
			, [TimeStamp]
			, [ParentGuid]
			, AutoStart
			, AutoStartTime
			, AutoStop
			, AutoStopTime
			, CloseoutDataModifiedBy
			, CloseoutDensityProductInAir
			, CloseoutDensityProductObserved
			, CloseoutDensityProductObservedTime
			, CloseoutDensityProductStandard
			, CloseoutDensityProductStandardTime
			, CloseoutDensityProductStandardInAir
			, CloseoutLevelProduct
			, CloseoutLevelProductTime
			, CloseoutLevelWater
			, CloseoutMassLiquid
			, CloseoutPercentBsw
			, CloseoutRoofMass
			, CloseoutTankShellCorrection
			, CloseoutTemperatureAmbient
			, CloseoutTemperatureAmbientTime
			, CloseoutTemperatureDensity
			, CloseoutTemperatureProduct
			, CloseoutTime
			, CloseoutTransferGov
			, CloseoutTransferNsv
			, CloseoutTransferMassLiquid
			, CloseoutTransferVolumeWater
			, CloseoutVolumeBsw
			, CloseoutVolumeCorrectionFactor
			, CloseoutVolumeGrossObserved
			, CloseoutVolumeGrossStandard
			, CloseoutVolumeNetStandard
			, CloseoutVolumeRoofCorrection
			, CloseoutVolumeTotalObserved
			, CloseoutVolumeWater
			, Comment
			, [Type]
			, OrderNumber
			, PlannedStartTime
			, Product
			, ProductDescription
			, StartTime
			, StopTime
			, StartDensityProductObserved
			, StartDensityProductObservedTime
			, StartDensityProductObservedInAir
			, StartDensityProductStandard
			, StartDensityProductStandardTime
			, StartUserID
			, StartLevelProduct
			, StartLevelProductTime
			, StartLevelWater
			, StartLevelWaterTime
			, StartMassLiquid
			, StartPercentBsw
			, StartTankShellCorrection
			, StartTemperatureAmbient
			, StartTemperatureAmbientTime
			, StartTemperatureProduct
			, StartTemperatureProductTime
			, StartTemperatureDensity
			, StartTemperatureDensityTime
			, StartVolume
			, StartVolumeBsw
			, StartVolumeCorrectionFactor
			, StartVolumeGrossObserved
			, StartVolumeGrossStandard
			, StartVolumeNetStandard
			, StartVolumeRoofCorrection
			, StartVolumeTotalObserved
			, StartVolumeWater
			, UnitsLevelProductIndex
			, UnitsTemperatureAmbientIndex
			, UnitsTemperatureDensityIndex
			, UnitsTemperatureProductIndex
			, UnitsDensityProductObservedIndex
			, UnitsDensityProductStandardIndex
			, UnitsVolumeIndex
			, UnitsMassIndex
			, DecimalPlacesVolume
			, DecimalPlacesLevel
			, DecimalPlacesDensity
			, DecimalPlacesTemperature
			, UserData01
			, UserData02
			, UserData03
			, UserData04
			, UserData05
			, UserData06
			, UserData07
			, UserData08
			, UserData09
			, UserData10
			, TransferDeviation
			, TransferPercentDeviation
			, DecimalPlacesPercent
			, TransferMode
			, TransferStatus
			, TransferTarget
			, TransferTargetUnitsIndex
			, TransferLevelTarget
			, TransferVolumeTarget
			, TransferTimeRemaining
			, TransferDirection
			, CommentDateTime
			, CommentUserID
			, [Status]
			, VolumeWater
			, LevelProduct
			, StartDensityProductStandardInAir
			, TransferredVolumeWater
			, TransferredVolume
			, MidnightRecord
			, PointGuid
			, RootParentGuid
			, RecordSeq
			, CreatedDate
			, CreatedBy
			, UpdatedDate
			, UpdatedBy
		)
		VALUES
		(
			source.MovementHistoryGuid
			, source.SiteGuid
			, source.[Name]
			, source.[Node]
			, source.[InitiationCount]
			, source.[RecordType]
			, source.[TimeStamp]
			, source.[ParentGuid]
			, source.AutoStart
			, source.AutoStartTime
			, source.AutoStop
			, source.AutoStopTime
			, source.CloseoutDataModifiedBy
			, source.CloseoutDensityProductInAir
			, source.CloseoutDensityProductObserved
			, source.CloseoutDensityProductObservedTime
			, source.CloseoutDensityProductStandard
			, source.CloseoutDensityProductStandardTime
			, source.CloseoutDensityProductStandardInAir
			, source.CloseoutLevelProduct
			, source.CloseoutLevelProductTime
			, source.CloseoutLevelWater
			, source.CloseoutMassLiquid
			, source.CloseoutPercentBsw
			, source.CloseoutRoofMass
			, source.CloseoutTankShellCorrection
			, source.CloseoutTemperatureAmbient
			, source.CloseoutTemperatureAmbientTime
			, source.CloseoutTemperatureDensity
			, source.CloseoutTemperatureProduct
			, source.CloseoutTime
			, source.CloseoutTransferGov
			, source.CloseoutTransferNsv
			, source.CloseoutTransferMassLiquid
			, source.CloseoutTransferVolumeWater
			, source.CloseoutVolumeBsw
			, source.CloseoutVolumeCorrectionFactor
			, source.CloseoutVolumeGrossObserved
			, source.CloseoutVolumeGrossStandard
			, source.CloseoutVolumeNetStandard
			, source.CloseoutVolumeRoofCorrection
			, source.CloseoutVolumeTotalObserved
			, source.CloseoutVolumeWater
			, source.Comment
			, source.[Type]
			, source.OrderNumber
			, source.PlannedStartTime
			, source.Product
			, source.ProductDescription
			, source.StartTime
			, source.StopTime
			, source.StartDensityProductObserved
			, source.StartDensityProductObservedTime
			, source.StartDensityProductObservedInAir
			, source.StartDensityProductStandard
			, source.StartDensityProductStandardTime
			, source.StartUserID
			, source.StartLevelProduct
			, source.StartLevelProductTime
			, source.StartLevelWater
			, source.StartLevelWaterTime
			, source.StartMassLiquid
			, source.StartPercentBsw
			, source.StartTankShellCorrection
			, source.StartTemperatureAmbient
			, source.StartTemperatureAmbientTime
			, source.StartTemperatureProduct
			, source.StartTemperatureProductTime
			, source.StartTemperatureDensity
			, source.StartTemperatureDensityTime
			, source.StartVolume
			, source.StartVolumeBsw
			, source.StartVolumeCorrectionFactor
			, source.StartVolumeGrossObserved
			, source.StartVolumeGrossStandard
			, source.StartVolumeNetStandard
			, source.StartVolumeRoofCorrection
			, source.StartVolumeTotalObserved
			, source.StartVolumeWater
			, source.UnitsLevelProductIndex
			, source.UnitsTemperatureAmbientIndex
			, source.UnitsTemperatureDensityIndex
			, source.UnitsTemperatureProductIndex
			, source.UnitsDensityProductObservedIndex
			, source.UnitsDensityProductStandardIndex
			, source.UnitsVolumeIndex
			, source.UnitsMassIndex
			, source.DecimalPlacesVolume
			, source.DecimalPlacesLevel
			, source.DecimalPlacesDensity
			, source.DecimalPlacesTemperature
			, source.UserData01
			, source.UserData02
			, source.UserData03
			, source.UserData04
			, source.UserData05
			, source.UserData06
			, source.UserData07
			, source.UserData08
			, source.UserData09
			, source.UserData10
			, source.TransferDeviation
			, source.TransferPercentDeviation
			, source.DecimalPlacesPercent
			, source.TransferMode
			, source.TransferStatus
			, source.TransferTarget
			, source.TransferTargetUnitsIndex
			, source.TransferLevelTarget
			, source.TransferVolumeTarget
			, source.TransferTimeRemaining
			, source.TransferDirection
			, source.CommentDateTime
			, source.CommentUserID
			, source.[Status]
			, source.VolumeWater
			, source.LevelProduct
			, source.StartDensityProductStandardInAir
			, source.TransferredVolumeWater
			, source.TransferredVolume
			, source.MidnightRecord
			, source.PointGuid		
			, source.RootParentGuid
			, source.RecordSeq
			, SYSDATETIMEOFFSET() --CreatedDate
			, source.CreatedBy
			, SYSDATETIMEOFFSET() --UpdatedDate
			, source.UpdatedBy
		);

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
						+ 'Procedure Name: dbo.usp_MovementHistoryAddUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
