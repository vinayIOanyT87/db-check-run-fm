/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionLineItemsFromSourceTable]
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionLineItemsFromSourceTable]
(
	@startDate datetime,
	@beginIndex  int,
	@endIndex int,
	@extractByInventoryDate bit
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [fmcdc].[usp_GetTransactionLineItemsFromSourceTable]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Retrieves all the records from tblTransactionLineItems for which the UpdatedDate is on or greater a given date, and for 
--          which the _ClusterIdx falls within a given range.
-- Notes:
-- 1. @startDate: UpdateDate from which to filter the records.
-- 2. @beginIndex: _ClusterIdx from which to filter the records. Leave as 0 to ignore this filter.
-- 3. @endIndex: _ClusterIdx up to which to filter the records. Leave as 0 to ignore this filter.
-- 4. @extractByInventoryDate: 0: Filter By Update Date; 1: Filter by Inventory Date
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT  		                             
		a.[AcknowledgedDateTime],                    
		a.[AdditiveProfileGuid],                     
		a.[AdditiveProfileID],                       
		a.[AlternativeGrossVolume],                  
		a.[AlternativeNetVolume],                    
		a.[AlternativeUnits],                        
		a.[ArmNumber],                               
		a.[BatchNumber],                             
		a.[BottomVolume],                            
		a.[BrokenBlend],                             
		a.[CleanLineDeductItem],                     
		a.[CleanLineDeductQuantity],                 
		a.[CleanLineItem],                           
		a.[CleanLinePackQuantity],                   
		a.[CLIN],                                    
		a.[COAID],                                   
		a.[COANote],                                 
		a.[COAWaiver],                               
		a.[CompartmentsEmpty],                       
		a.[CompartmentsPreviouslyLoaded],            
		a.[CompletionDateTime],                      
		a.[ContaminatePrompt],                       
		a.[ContractNumber],                          
		a.[CreatedBy],                               
		a.[CreatedDate],                             
		a.[CurrencyGuid],                            
		a.[CurrencyUnit],                            
		a.[CustomerProductCode],                     
		a.[CustomerProductName],                     
		a.[Customs],                                 
		a.[Date01],                                  
		a.[Date02],                                  
		a.[Date03],                                  
		a.[Date04],                                  
		a.[DeleteFlag],                              
		a.[DeliveryLocation],                        
		a.[Density],                                 
		a.[DestinationCompanyEquipmentID],           
		a.[DestinationCompartmentEquipmentGuid],     
		a.[DestinationCompartmentID],                
		a.[DestinationEquipmentGuid],                
		a.[DestinationEquipmentModel],               
		a.[DestinationEquipmentType],                
		a.[DestinationRegistrationID],               
		a.[DestinationSerialNumber],                 
		a.[DifferentialPressure],                    
		a.[DispatchedDateTime],                      
		a.[DocumentNumber],                          
		a.[DualFuelingModeFlag],                     
		a.[DualFuelingPrimaryFlag],                  
		a.[EndDeliveryDate],                         
		a.[EngineeringUnitsIndex],                   
		a.[EngineRunTime],                           
		a.[ExchangeRate],                            
		a.[Flag01],                                  
		a.[Flag02],                                  
		a.[Flag03],                                  
		a.[Flag04],                                  
		a.[Flag05],                                  
		a.[Flag06],                                  
		a.[FlowRate],                                
		a.[FreezePoint],                             
		a.[FuelCompressionFactor],                   
		a.[GrossManualValueFlag],                    
		a.[GrossQuantity],                           
		a.[HydrantPressure],                         
		a.[ImproperAdditization],                    
		a.[InvoiceLineNumber],                       
		a.[InvoiceNumber],                           
		a.[LineFill],                                
		a.[LineItemSequenceNumber],                  
		a.[LineNumber],                              
		a.[LoadingLocationID],                       
		a.[LoadingLocationStationGuid],              
		a.[LoadRackVariance],                        
		a.[LookupQualityIndex],                      
		a.[LookupTransactionStatusIndex],            
		a.[MassManualValueFlag],                     
		a.[MassQuantity],                            
		a.[MeterFactor],                             
		a.[MeterGuid],                               
		a.[MeterID],                                 
		a.[MeterStart],                              
		a.[MeterStartDateTime],                      
		a.[MeterStartObtainedAutomaticallyFlag],     
		a.[MeterStop],                               
		a.[MeterStopDateTime],                       
		a.[MeterStopObtainedAutomaticallyFlag],      
		a.[MobileDeviceGuid],                        
		a.[MobileDeviceID],                          
		a.[NetCapacity],                             
		a.[NetManualValueFlag],                      
		a.[NetQuantity],                             
		a.[NonDomesticPrice],                        
		a.[Number01],                                
		a.[Number02],                                
		a.[Number03],                                
		a.[Number04],                                
		a.[Number05],                                
		a.[Number06],                                
		a.[Odometer],                                
		a.[OdometerHours],                           
		a.[OnLocationTime],                          
		a.[OperatorID],                              
		a.[OperatorPersonnelGuid],                   
		a.[OrderReferenceTransactionLineItemGuid],   
		a.[PackageManualValueFlag],                  
		a.[PartialFill],                             
		a.[Pit],                                     
		a.[PresetAmount],                            
		a.[Product],                                 
		a.[ProductCode],                             
		a.[ProductGuid],                             
		a.[ProductPrice],                            
		a.[ProductType],                             
		a.[QualityTestNumber],                       
		a.[ReceiptVariance],                         
		a.[RequestedBy],                             
		a.[RequestedDateTime],                       
		a.[RequestedDeliveryDate],                   
		a.[SequenceID], 
		a.[SourceCompanyEquipmentID],                
		a.[SourceCompartmentEquipmentGuid],          
		a.[SourceCompartmentID],                     
		a.[SourceEquipmentGuid],                     
		a.[SourceEquipmentModel],                    
		a.[SourceEquipmentType],                     
		a.[SourceRegistrationID],                    
		a.[SourceSerialNumber],                      
		a.[StorageLocationID],                       
		a.[StorageLocationTankGuid],                 
		a.[TankLevel],                               
		a.[TankLevelUnits],                          
		a.[TankStatus],                              
		a.[Tax1],                                    
		a.[Tax2],                                    
		a.[Tax3],                                    
		a.[Tax4],                                    
		a.[Tax5],                                    
		a.[Temperature],                             
		a.[TemperatureQualityStatus],                
		a.[TransactionGuid],                         
		a.[TransactionInventoryDate],                
		a.[TransactionLineItemGuid],                 
		a.[TransVersion],                            
		a.[UpdatedBy],                               
		a.[UpdatedDate],                             
		a.[ValidationDateTime],  
		a.[Variance],                                
		a.[Vcf],                                     
		a.[VcfManualValueFlag],                      
		a.[_ClusterIdx],                             
		a.[_RowVersion],
		CONVERT(BigInt, a.[_RowVersion]) RowVersionInt
		FROM dbo.tblTransactionLineItems a
		INNER JOIN dbo.tbltransactions b
		ON a.TransactionGuid = b.TransactionGuid
		WHERE 
		(
			((@extractByInventoryDate = 0) AND (a.UpdatedDate >= @startdate))
			OR ((@extractByInventoryDate = 1) AND (cast(b.InventoryDate as datetime) >= @startdate))
		)
		AND ((b._ClusterIdx >= @beginIndex) OR (ISNULL(@beginIndex, 0) = 0))
		AND ((b._ClusterIdx <= @endIndex) OR (ISNULL(@endIndex, 0) = 0))	
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionLineItemsFromSourceTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END