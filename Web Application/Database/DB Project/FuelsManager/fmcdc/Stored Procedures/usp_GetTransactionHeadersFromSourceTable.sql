/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionHeadersFromSourceTable]
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionHeadersFromSourceTable]
(
	@startDate datetime,
	@beginIndex  int,
	@endIndex int,
	@extractByInventoryDate bit
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [fmcdc].[usp_GetTransactionHeadersFromSourceTable]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Retrieves all the records from tblTransactions for which the UpdatedDate is on or greater a given date, and for 
--          the _ClusterIdx falls within a given range.
-- Notes:
-- 1. @startDate: UpdateDate from which to filter the records.
-- 2. @beginIndex: _ClusterIdx from which to filter the records. Leave as 0 to ignore this filter.
-- 3. @endIndex: _ClusterIdx up to which to filter the records. Leave as 0 to ignore this filter.
-- 4. @extractByInventoryDate: 0: Filter By Update Date; 1: Filter by Inventory Date
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		SELECT                   
		[AliasName],                       
		[AssociatedCLIN],                  
		[AssociatedDocNumber],             
		[AssociatedTransportOrderNumber],  
		[AutoComplete],                    
		[BillToCode],                      
		[BillToCompanyGuid],               
		[BillToID],                        
		[CardExpiration],                  
		[CardName],                        
		[CardNumber],                      
		[CardType],                        
		[CarrierCode],                     
		[CarrierCompanyGuid],              
		[CarrierID],                       
		[CashAmount],                      
		[ConjoinTransID],                  
		[ContactFirstName],                
		[ContactInfo],                     
		[ContactSurname],                  
		[Country],                         
		[CreatedBy],                       
		[CreatedDate],                     
		[CreditAmount],                    
		[Date01],                          
		[Date02],                          
		[Date03],                          
		[Date04],                          
		[DeleteFlag],                      
		[Destination1EquipmentGuid],       
		[Destination2EquipmentGuid],       
		[Destination3EquipmentGuid],       
		[DestinationCompanyEquipmentID1],  
		[DestinationCompanyEquipmentID2],  
		[DestinationCompanyEquipmentID3],  
		[DestinationEquipmentModel1],      
		[DestinationEquipmentModel2],      
		[DestinationEquipmentModel3],      
		[DestinationEquipmentType1],       
		[DestinationEquipmentType2],       
		[DestinationEquipmentType3],       
		[DestinationRegistrationID1],      
		[DestinationRegistrationID2],      
		[DestinationRegistrationID3],      
		[DestinationSerialNumber1],        
		[DestinationSerialNumber2],        
		[DestinationSerialNumber3],        
		[DispatchedDateTime],              
		[DocumentNumber],                  
		[DriverIdentificationNumber],      
		[EffectiveDate],                   
		[ErrorFlag],                       
		[EstimatedFuelingDuration],        
		[ETA],                             
		[ETD],                             
		[ExpirationDate],                  
		[FinalStationIATAGuid],            
		[FinalStationIATAID],              
		[Flag01],                          
		[Flag02],                          
		[Flag03],                          
		[Flag04],                          
		[Flag05],                          
		[Flag06],                          
		[FST],                             
		[FuelAdditiveFlag],                
		[FuelCardGuid],                    
		[FuelCardID],                      
		[GateGuid],                        
		[GateID],                          
		[InternationalRouteIndicator],     
		[InventoryDate],                   
		[IssuePoint],                      
		[IssuePointNumber],                
		[LegacyNumber],                    
		[LinkedDocumentNumber],            
		[LoadID],                          
		[LookupOriginApplicationIndex],    
		[LookupTransactionStatusIndex],    
		[LookupTransTypeIndex],            
		[ManagerCode],                     
		[ManagerCompanyGuid],              
		[ManagerID],                       
		[NextStationIATAGuid],             
		[NextStationIATAID],               
		[Number01],                        
		[Number02],                        
		[Number03],                        
		[Number04],                        
		[Number05],                        
		[Number06],                        
		[OperatorID],                      
		[OperatorName],                    
		[OperatorPersonnelGuid],           
		[OriginStationIATAGuid],           
		[OriginStationIATAID],             
		[OwnerCode],                       
		[OwnerCompanyGuid],                
		[OwnerID],                         
		[PONumber],                        
		[PreviousRoutingID],               
		[PreviousStationIATAGuid],         
		[PreviousStationIATAID],           
		[RadioNumber],                     
		[ReasonCodeGuid],                  
		[RequestedDateTime],               
		[RequestedDeliveryDate],           
		[ReversalType],                    
		[ReversedTransID],                 
		[RouteOriginationDate],            
		[RoutingID],                       
		[SCACCode],                        
		[ScheduledDate],                   
		[SFT],                             
		[ShipmentNumber],                  
		[ShipperCode],                     
		[ShipperCompanyGuid],              
		[ShipperID],                       
		[ShippingDocumentNumber],          
		[ShippingMethod],                  
		[ShipToCode],                      
		[ShipToCompanyGuid],               
		[ShipToID],                        
		[Site],                            
		[SiteGuid],                        
		[Source1EquipmentGuid],            
		[Source2EquipmentGuid],            
		[Source3EquipmentGuid],            
		[SourceCompanyEquipmentID1],       
		[SourceCompanyEquipmentID2],       
		[SourceCompanyEquipmentID3],       
		[SourceEquipmentModel1],           
		[SourceEquipmentModel2],           
		[SourceEquipmentModel3],           
		[SourceEquipmentType1],            
		[SourceEquipmentType2],            
		[SourceEquipmentType3],            
		[SourceRegistrationID1],           
		[SourceRegistrationID2],           
		[SourceRegistrationID3],           
		[SourceSerialNumber1],             
		[SourceSerialNumber2],             
		[SourceSerialNumber3],             
		[STA],                             
		[STD],                             
		[SubmittedToAccounting],           
		[SubType],                         
		[SupplierCode],                    
		[SupplierCompanyGuid],             
		[SupplierID],                      
		[TicketMode],                      
		[TicketSource],                    
		[TimeEnd],                         
		[TimeIn],                          
		[TimeOut],                         
		[TransactionAliasGuid],            
		[TransactionGuid],                 
		[TransDateTime],                   
		[TransID], 
		[TransReferenceID],                
		[TransVersion],                    
		[UpdatedBy],                       
		[UpdatedDate], 
		[_ClusterIdx],                     
		[_RowVersion],
		CONVERT(BigInt, _RowVersion) RowVersionInt
		FROM dbo.tblTransactions 
		WHERE 
		(
			((@extractByInventoryDate = 0) AND (UpdatedDate >= @startdate))
			OR ((@extractByInventoryDate = 1) AND (cast(InventoryDate as datetime) >= @startdate))
		)
		AND ((_ClusterIdx >= @beginIndex) OR (ISNULL(@beginIndex, 0) = 0)) 
		AND ((_ClusterIdx <= @endIndex) OR (ISNULL(@endIndex, 0) = 0))					
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionHeadersFromSourceTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END