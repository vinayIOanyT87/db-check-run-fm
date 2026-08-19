USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_DropStuff]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_DropStuff') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_DropStuff
GO

CREATE PROCEDURE [dbo].Migration_DropStuff
 /*=============================================
 Author:			Sijuan Jiang
 Create date:		3/5/2010
 Description:		Migrating Clear up
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_DropStuff

*/
@IsBaseDB smallint = 0  -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 

AS 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_SetBaseLevelSiteID') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migrate_SetBaseLevelSiteID

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Site_1') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8Site_1

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Users_2') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8Users_2

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8UserGroupMap_3') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8UserGroupMap_3

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDBtblEntityToSiteMap_4') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDBtblEntityToSiteMap_4

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_CreateLoginUserRole_5') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_CreateLoginUserRole_5

/*************************/
If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Products') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_ConsolidatedDB6To8Products 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_FMD6ConsumersToFMD8ShipTo') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_FMD6ConsumersToFMD8ShipTo 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_FMD6SuppliersToFMD8Suppliers') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_FMD6SuppliersToFMD8Suppliers 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_FMD6VendorsToFMD8Carriers') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_FMD6VendorsToFMD8Carriers 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_FMD6ShippersToFMD8Shippers') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_FMD6ShippersToFMD8Shippers 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8EquipmentTypes') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_ConsolidatedDB6To8EquipmentTypes 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Equipment') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_ConsolidatedDB6To8Equipment 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6EmployeesTo8Personnel_6') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_ConsolidatedDB6EmployeesTo8Personnel_6 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6OperatorsTo8Personnel') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_ConsolidatedDB6OperatorsTo8Personnel 

/***********************/
If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDBTrainingToConsolidatedDB8_3') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDBTrainingToConsolidatedDB8_3 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8QualityTag_1') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6To8QualityTag_1 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8TestResults') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6To8TestResults 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6QCAssignedTo8EqQualityTagLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6QCAssignedTo8EqQualityTagLog 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8MaintenanceLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6To8MaintenanceLog 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8Appointments') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6To8Appointments  

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6To8ControllersLog') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6To8ControllersLog

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6CustomerAccountsToFMD8FuelCards') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6CustomerAccountsToFMD8FuelCards

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6CommonRequestToFMD8FuelCards') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6CommonRequestToFMD8FuelCards

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AviationDB6ControlLogTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AviationDB6ControlLogTo8Transactions

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6RefContractTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6RefContractTo8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6RefTransferTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6RefTransferTo8Transactions

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6ReissueTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6ReissueTo8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6ReceiveTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6ReceiveTo8Transactions

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6InflightTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6InflightTo8Transactions

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6RegradeTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6RegradeTo8Transactions

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_AccountingDB6CommercialTo8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_AccountingDB6CommercialTo8Transactions

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SALETransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SALETransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SALEFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SALEFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SALEUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SALEUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6DEFUELTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6DEFUELTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6DEFUELFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6DEFUELFieldsToFMD8LineItems

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6DEFUELUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6DEFUELUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6DETERMINETransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6DETERMINETransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6DETERMINEFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6DETERMINEFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6DETERMINEUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6DETERMINEUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6ADJUSTTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6ADJUSTTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6ADJUSTFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6ADJUSTFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6ADJUSTUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6ADJUSTUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6REQUESTTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6REQUESTTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6REQUESTFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6REQUESTFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6REQUESTUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6REQUESTUserDataToFMD8UserData 

/*************/
If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTCONTRACTWithoutShippingDocumentUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTCONTRACTWithShippingDocumentUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTTRANSFERWithoutShippingDocumentUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentTransactionToFMD8Transactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentTransactionToFMD8Transactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentFieldsToFMD8LineItems') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentFieldsToFMD8LineItems 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentUserDataToFMD8UserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTTRANSFERWithShippingDocumentUserDataToFMD8UserData 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData 

/*************/

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_FMD6TransactionNotesToFMD8TransactionNotes') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_FMD6TransactionNotesToFMD8TransactionNotes

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_CreateEmptyTransactionWeightReadings') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_CreateEmptyTransactionWeightReadings 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_LinkReversalTransactions') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_LinkReversalTransactions 

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_SetInhibitAccountingFlagForProducts') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_SetInhibitAccountingFlagForProducts

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_SetTransactionDocumentNumbers') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_SetTransactionDocumentNumbers  

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_DisableTriggers') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_DisableTriggers

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_EnableTriggers') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_EnableTriggers

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migrate_EnableDisableFuelCards') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migrate_EnableDisableFuelCards

   

IF @IsBaseDB <> 2
BEGIN
	EXEC master.dbo.Migration_ClearDBUsers 'ConsolidatedDB6'
	if db_id('ConsolidatedDB6') is not null 
		DROP DATABASE ConsolidatedDB6 

	EXEC master.dbo.Migration_ClearDBUsers 'AccountingDB6'
	if db_id('AccountingDB6') is not null 
		DROP DATABASE AccountingDB6
		
	EXEC master.dbo.Migration_ClearDBUsers 'AviationDB6'
	if db_id('AviationDB6') is not null 
		DROP DATABASE AviationDB6 	
		
	EXEC master.dbo.Migration_ClearDBUsers 'FMArchive6'
	if db_id('FMArchive6') is not null 
		DROP DATABASE FMArchive6    

	EXEC master.dbo.Migration_ClearDBUsers 'Movement6'
	if db_id('Movement6') is not null 
		DROP DATABASE Movement6 	                 
END

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_DropStuff') and OBJECTPROPERTY(id, N'IsProcedure') = 1) 
Drop Procedure dbo.Migration_DropStuff 