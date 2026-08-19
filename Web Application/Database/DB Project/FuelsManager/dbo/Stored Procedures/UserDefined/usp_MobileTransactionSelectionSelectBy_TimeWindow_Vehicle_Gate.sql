CREATE PROCEDURE [dbo].[usp_MobileTransactionSelectionSelectBy_TimeWindow_Vehicle_Gate] (
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
	-- Stored procedure: [dbo].[usp_MobileTransactionSelectionSelectBy_TimeWindow_Vehicle_Gate]
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.001 / 2012-10-30 
	-- Purpose: Select transactions based on some filter criteria
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	SELECT tblTransactions.TransVersion, 
            tblTransactions.InternationalRouteIndicator, 
            tblTransactions.DeleteFlag, 
            tblTransactions.AutoComplete, 
            tblTransactions.Flag01, 
            tblTransactions.Flag02, 
            tblTransactions.Flag03, 
            tblTransactions.Flag04, 
            tblTransactions.Flag05, 
            tblTransactions.Flag06, 
            tblTransactions.SubmittedToAccounting, 
            tblTransactions.ErrorFlag, 
            tblTransactions.InventoryDate, 
            tblTransactions.CreatedDate, 
            tblTransactions.RequestedDeliveryDate, 
            tblTransactions.UpdatedDate, 
            tblTransactions.TransDateTime, 
            tblTransactions.TimeIn, 
            tblTransactions.TimeOut, 
            tblTransactions.TimeEnd, 
            tblTransactions.CardExpiration, 
            tblTransactions.RouteOriginationDate, 
            tblTransactions.STD, 
            tblTransactions.ETD, 
            tblTransactions.STA, 
            tblTransactions.ETA, 
            tblTransactions.SFT, 
            tblTransactions.FST, 
            tblTransactions.EffectiveDate, 
            tblTransactions.ExpirationDate, 
            tblTransactions.ScheduledDate, 
            tblTransactions.Date01, 
            tblTransactions.Date02, 
            tblTransactions.Date03, 
            tblTransactions.Date04, 
            tblTransactions.RequestedDateTime, 
            tblTransactions.DispatchedDateTime, 
            tblTransactions.CreditAmount, 
            tblTransactions.CashAmount, 
            tblTransactions.Number01, 
            tblTransactions.Number02, 
            tblTransactions.Number03, 
            tblTransactions.Number04, 
            tblTransactions.Number05, 
            tblTransactions.Number06, 
            tblTransactions.EstimatedFuelingDuration, 
            tblTransactions.LookupTransactionStatusIndex, 
            tblTransactions.LookupOriginApplicationIndex, 
            tblTransactions.TransID, 
            tblTransactions.AliasName, 
            tblTransactions.SubType, 
            tblTransactions.Site, 
            tblTransactions.TransReferenceID, 
            tblTransactions.ShipToID, 
            tblTransactions.ShipToCode, 
            tblTransactions.SupplierID, 
            tblTransactions.SupplierCode, 
            tblTransactions.CreatedBy, 
            tblTransactions.UpdatedBy, 
            tblTransactions.SCACCode, 
            tblTransactions.CardNumber, 
            tblTransactions.ShipmentNumber, 
            tblTransactions.ShipperID, 
            tblTransactions.ShipperCode, 
            tblTransactions.OwnerID, 
            tblTransactions.OwnerCode, 
            tblTransactions.ManagerID, 
            tblTransactions.ManagerCode, 
            tblTransactions.CarrierID, 
            tblTransactions.CarrierCode, 
            tblTransactions.ConjoinTransID, 
            tblTransactions.ReversedTransID, 
            tblTransactions.LinkedDocumentNumber, 
            tblTransactions.ReversalType, 
            tblTransactions.PONumber, 
            tblTransactions.RoutingID, 
            tblTransactions.TicketSource, 
            tblTransactions.LoadID, 
            tblTransactions.BillToID, 
            tblTransactions.BillToCode, 
            tblTransactions.DriverIdentificationNumber, 
            tblTransactions.CardName, 
            tblTransactions.CardType, 
            tblTransactions.PreviousRoutingID, 
            tblTransactions.ShippingDocumentNumber, 
            tblTransactions.DocumentNumber, 
            tblTransactions.TicketMode, 
            tblTransactions.DestinationRegistrationID1, 
            tblTransactions.DestinationSerialNumber1, 
            tblTransactions.DestinationEquipmentType1, 
            tblTransactions.DestinationEquipmentModel1, 
            tblTransactions.DestinationCompanyEquipmentID1, 
            tblTransactions.DestinationRegistrationID2, 
            tblTransactions.DestinationSerialNumber2, 
            tblTransactions.DestinationEquipmentType2, 
            tblTransactions.DestinationEquipmentModel2, 
            tblTransactions.DestinationCompanyEquipmentID2, 
            tblTransactions.DestinationRegistrationID3, 
            tblTransactions.DestinationSerialNumber3, 
            tblTransactions.DestinationEquipmentType3, 
            tblTransactions.DestinationEquipmentModel3, 
            tblTransactions.DestinationCompanyEquipmentID3, 
            tblTransactions.SourceRegistrationID1, 
            tblTransactions.SourceSerialNumber1, 
            tblTransactions.SourceEquipmentType1, 
            tblTransactions.SourceEquipmentModel1, 
            tblTransactions.SourceCompanyEquipmentID1, 
            tblTransactions.SourceRegistrationID2, 
            tblTransactions.SourceSerialNumber2, 
            tblTransactions.SourceEquipmentType2, 
            tblTransactions.SourceEquipmentModel2, 
            tblTransactions.SourceCompanyEquipmentID2, 
            tblTransactions.SourceRegistrationID3, 
            tblTransactions.SourceSerialNumber3, 
            tblTransactions.SourceEquipmentType3, 
            tblTransactions.SourceEquipmentModel3, 
            tblTransactions.SourceCompanyEquipmentID3, 
            tblTransactions.OperatorID, 
            tblTransactions.ContactFirstName, 
            tblTransactions.ContactSurname, 
            tblTransactions.LegacyNumber, 
            tblTransactions.Country, 
            tblTransactions.ContactInfo, 
            tblTransactions.AssociatedDocNumber, 
            tblTransactions.AssociatedCLIN, 
            tblTransactions.FuelCardID, 
            tblTransactions.AssociatedTransportOrderNumber, 
            tblTransactions.LookupTransTypeIndex, 
            tblTransactions.TransactionGuid, 
            tblTransactions.SiteGuid, 
            tblTransactions.TransactionAliasGuid, 
            tblTransactions.BillToCompanyGuid, 
            tblTransactions.Destination1EquipmentGuid, 
            tblTransactions.Destination2EquipmentGuid, 
            tblTransactions.Destination3EquipmentGuid, 
            tblTransactions.FinalStationIATAGuid, 
            tblTransactions.FuelCardGuid, 
            tblTransactions.ManagerCompanyGuid, 
            tblTransactions.NextStationIATAGuid, 
            tblTransactions.OperatorPersonnelGuid, 
            tblTransactions.OriginStationIATAGuid, 
            tblTransactions.OwnerCompanyGuid, 
            tblTransactions.PreviousStationIATAGuid, 
            tblTransactions.ShipperCompanyGuid, 
            tblTransactions.ShipToCompanyGuid, 
            tblTransactions.Source1EquipmentGuid, 
            tblTransactions.Source2EquipmentGuid, 
            tblTransactions.Source3EquipmentGuid, 
            tblTransactions.SupplierCompanyGuid, 
            CONVERT(bigint,tblTransactions._RowVersion) AS _RowVersion, 
            tblTransactions.OriginStationIATAID, 
            tblTransactions.PreviousStationIATAID, 
            tblTransactions.NextStationIATAID, 
            tblTransactions.FinalStationIATAID, 
            tblTransactions.CarrierCompanyGuid, 
            tblTransactions.ReasonCodeGuid 		
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
						+ 'Procedure Name: usp_MobileTransactionSelectionSelectBy_TimeWindow_Vehicle_Gate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END