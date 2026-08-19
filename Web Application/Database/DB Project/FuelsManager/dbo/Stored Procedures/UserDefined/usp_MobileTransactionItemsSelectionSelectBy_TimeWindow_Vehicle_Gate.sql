
CREATE PROCEDURE [dbo].[usp_MobileTransactionItemsSelectionSelectBy_TimeWindow_Vehicle_Gate] (
	@OperatorID 		NVARCHAR(100),
	@filterOperatorID 	BIT,
	@VehicleID 		NVARCHAR(100),
	@filterVehicleID 	BIT,
	@GateID 		NVARCHAR(100),
	@filterGateID 		BIT,
	@HoursInPast 		INT,
	@HoursInFuture 		INT,
	@SiteGuid 		UNIQUEIDENTIFIER
) AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_MobileTransactionItemsSelectionSelectBy_TimeWindow_Vehicle_Gate]
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.001 / 2012-10-30 
	-- Purpose: Select transactions based on some filter criteria
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

	SELECT tblTransactionLineItems.TransVersion, 
             tblTransactionLineItems.DeleteFlag, 
             tblTransactionLineItems.COAWaiver, 
             tblTransactionLineItems.ImproperAdditization, 
             tblTransactionLineItems.BrokenBlend, 
             tblTransactionLineItems.ContaminatePrompt, 
             tblTransactionLineItems.CompartmentsPreviouslyLoaded, 
             tblTransactionLineItems.CompartmentsEmpty, 
             tblTransactionLineItems.Flag01, 
             tblTransactionLineItems.Flag02, 
             tblTransactionLineItems.Flag03, 
             tblTransactionLineItems.Flag04, 
             tblTransactionLineItems.Flag05, 
             tblTransactionLineItems.Flag06, 
             tblTransactionLineItems.PartialFill, 
             tblTransactionLineItems.NetManualValueFlag, 
             tblTransactionLineItems.MassManualValueFlag, 
             tblTransactionLineItems.GrossManualValueFlag, 
             tblTransactionLineItems.VcfManualValueFlag, 
             tblTransactionLineItems.MeterStartDateTime, 
             tblTransactionLineItems.MeterStopDateTime, 
             tblTransactionLineItems.RequestedDateTime, 
             tblTransactionLineItems.DispatchedDateTime, 
             tblTransactionLineItems.AcknowledgedDateTime, 
             tblTransactionLineItems.OnLocationTime, 
             tblTransactionLineItems.ValidationDateTime, 
             tblTransactionLineItems.CompletionDateTime, 
             tblTransactionLineItems.CreatedDate, 
             tblTransactionLineItems.UpdatedDate, 
             tblTransactionLineItems.TransactionInventoryDate, 
             tblTransactionLineItems.EndDeliveryDate, 
             tblTransactionLineItems.RequestedDeliveryDate, 
             tblTransactionLineItems.Date01, 
             tblTransactionLineItems.Date02, 
             tblTransactionLineItems.Date03, 
             tblTransactionLineItems.Date04, 
             tblTransactionLineItems.MeterStart, 
             tblTransactionLineItems.MeterStop, 
             tblTransactionLineItems.GrossQuantity, 
             tblTransactionLineItems.Temperature, 
             tblTransactionLineItems.Vcf, 
             tblTransactionLineItems.Density, 
             tblTransactionLineItems.ProductPrice, 
             tblTransactionLineItems.NetQuantity, 
             tblTransactionLineItems.MeterFactor, 
             tblTransactionLineItems.LineFill, 
             tblTransactionLineItems.BottomVolume, 
             tblTransactionLineItems.NetCapacity, 
             tblTransactionLineItems.ReceiptVariance, 
             tblTransactionLineItems.DifferentialPressure, 
             tblTransactionLineItems.LoadRackVariance, 
             tblTransactionLineItems.FreezePoint, 
             tblTransactionLineItems.PresetAmount, 
             tblTransactionLineItems.Tax1, 
             tblTransactionLineItems.Tax2, 
             tblTransactionLineItems.Tax3, 
             tblTransactionLineItems.Tax4, 
             tblTransactionLineItems.Tax5, 
             tblTransactionLineItems.Number01, 
             tblTransactionLineItems.Number02, 
             tblTransactionLineItems.Number03, 
             tblTransactionLineItems.Number04, 
             tblTransactionLineItems.Number05, 
             tblTransactionLineItems.Number06, 
             tblTransactionLineItems.OdometerHours, 
             tblTransactionLineItems.AlternativeGrossVolume, 
             tblTransactionLineItems.AlternativeNetVolume, 
             tblTransactionLineItems.TankLevel, 
             tblTransactionLineItems.NonDomesticPrice, 
             tblTransactionLineItems.ExchangeRate, 
             tblTransactionLineItems.Odometer, 
             tblTransactionLineItems.Variance, 
             tblTransactionLineItems.MassQuantity, 
             tblTransactionLineItems.ArmNumber, 
             tblTransactionLineItems.LineNumber, 
             tblTransactionLineItems.EngineeringUnitsIndex, 
             tblTransactionLineItems.AlternativeUnits, 
             tblTransactionLineItems.TankLevelUnits, 
             tblTransactionLineItems.CurrencyUnit, 
             tblTransactionLineItems.LookupTransactionStatusIndex, 
             tblTransactionLineItems.LookupQualityIndex, 
             tblTransactionLineItems.Product, 
             tblTransactionLineItems.ProductCode, 
             tblTransactionLineItems.ProductType, 
             tblTransactionLineItems.CLIN, 
             tblTransactionLineItems.ContractNumber, 
             tblTransactionLineItems.DestinationRegistrationID, 
             tblTransactionLineItems.DestinationSerialNumber, 
             tblTransactionLineItems.DestinationEquipmentType, 
             tblTransactionLineItems.DestinationEquipmentModel, 
             tblTransactionLineItems.DestinationCompanyEquipmentID, 
             tblTransactionLineItems.DestinationCompartmentID, 
             tblTransactionLineItems.SourceRegistrationID, 
             tblTransactionLineItems.SourceSerialNumber, 
             tblTransactionLineItems.SourceEquipmentType, 
             tblTransactionLineItems.SourceEquipmentModel, 
             tblTransactionLineItems.SourceCompanyEquipmentID, 
             tblTransactionLineItems.SourceCompartmentID, 
             tblTransactionLineItems.LineItemSequenceNumber, 
             tblTransactionLineItems.BatchNumber, 
             tblTransactionLineItems.DocumentNumber, 
             tblTransactionLineItems.Customs, 
             tblTransactionLineItems.OperatorID, 
             tblTransactionLineItems.TankStatus, 
             tblTransactionLineItems.Pit, 
             tblTransactionLineItems.RequestedBy, 
             tblTransactionLineItems.StorageLocationID, 
             tblTransactionLineItems.MeterID, 
             tblTransactionLineItems.AdditiveProfileID, 
             tblTransactionLineItems.CreatedBy, 
             tblTransactionLineItems.UpdatedBy, 
             tblTransactionLineItems.CustomerProductName, 
             tblTransactionLineItems.CustomerProductCode, 
             tblTransactionLineItems.COANote, 
             tblTransactionLineItems.COAID, 
             tblTransactionLineItems.LoadingLocationID, 
             tblTransactionLineItems.InvoiceNumber, 
             tblTransactionLineItems.InvoiceLineNumber, 
             tblTransactionLineItems.QualityTestNumber, 
             tblTransactionLineItems.DeliveryLocation, 
             tblTransactionLineItems.SequenceID, 
             tblTransactionLineItems.TransactionLineItemGuid, 
             tblTransactionLineItems.StorageLocationTankGuid, 
             tblTransactionLineItems.AdditiveProfileGuid, 
             tblTransactionLineItems.DestinationCompartmentEquipmentGuid, 
             tblTransactionLineItems.DestinationEquipmentGuid, 
             tblTransactionLineItems.OperatorPersonnelGuid, 
             tblTransactionLineItems.ProductGuid, 
             tblTransactionLineItems.SourceCompartmentEquipmentGuid, 
             tblTransactionLineItems.SourceEquipmentGuid, 
             tblTransactionLineItems.TransactionGuid, 
             tblTransactionLineItems.CurrencyGuid, 
             tblTransactionLineItems.OrderReferenceTransactionLineItemGuid, 
             tblTransactionLineItems.LoadingLocationStationGuid, 
             CONVERT(bigint, tblTransactionLineItems._RowVersion) AS _RowVersion, 
             tblTransactionLineItems.MeterGuid			 
	FROM tblTransactions INNER JOIN tblTransactionLineItems 
	ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid 
	WHERE (@filterVehicleID = 0 OR tblTransactions.SourceRegistrationID1 = @VehicleID) 
		AND (@filterGateID = 0 OR tblTransactionLineItems.LoadingLocationID = @GateID) 
		AND (@filterOperatorID = 0 OR tblTransactions.OperatorID = @OperatorID)
		AND (DATEADD(hour,@HoursInFuture,SYSDATETIMEOFFSET()) > tblTransactions.ETD)
		AND (SYSDATETIMEOFFSET() < DATEADD(hour,@HoursInPast,tblTransactions.ETD))
		AND (tblTransactions.SiteGuid = @SiteGuid)	
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
						+ 'Procedure Name: usp_MobileTransactionItemsSelectionSelectBy_TimeWindow_Vehicle_Gate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END