/*
	DROP PROCEDURE [archive].[usp_DisableTriggersForOfflineArchiving]

	EXEC [archive].[usp_DisableTriggersForOfflineArchiving]

*/
CREATE PROCEDURE [archive].[usp_DisableTriggersForOfflineArchiving]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_DisableTriggersForOfflineArchiving]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Diasble the Auditing and FMSync OnDelete triggers on the tables covered by archiving.
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		--AlarmAndEventLog
		DISABLE TRIGGER [trg_del_tblAlarmAndEventLog_ForSync] ON [dbo].[tblAlarmAndEventLog];
	
		--AuditLog
		DISABLE TRIGGER [trg_del_tblAuditLog_ForSync] ON [dbo].[tblAuditLog];

		--Transaction
		DISABLE TRIGGER [trg_Audit_del_tblTransactionLineItems] ON [dbo].[tblTransactionLineItems];
		DISABLE TRIGGER [trg_del_tblTransactionLineItems_ForSync] ON [dbo].[tblTransactionLineItems];

		DISABLE TRIGGER [trg_Audit_del_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData];
		DISABLE TRIGGER [trg_del_tblTransactionLineItemUserData_ForSync] ON [dbo].[tblTransactionLineItemUserData];
	
		DISABLE TRIGGER [trg_del_tblTransactionLinks_ForSync] ON [dbo].[tblTransactionLinks];
	
		DISABLE TRIGGER [trg_Audit_del_tblTransactionNotes] ON [dbo].[tblTransactionNotes];
		DISABLE TRIGGER [trg_del_tblTransactionNotes_ForSync] ON [dbo].[tblTransactionNotes];

		DISABLE TRIGGER [trg_Audit_del_tblTransactionPIDX] ON [dbo].[tblTransactionPIDX];
		DISABLE TRIGGER [trg_del_tblTransactionPIDX_ForSync] ON [dbo].[tblTransactionPIDX];

		DISABLE TRIGGER [trg_Audit_del_tblTransactions] ON [dbo].[tblTransactions];
		DISABLE TRIGGER [trg_del_tblTransactions_ForSync] ON [dbo].[tblTransactions];

		DISABLE TRIGGER [trg_Audit_del_tblTransactionSignature] ON [dbo].[tblTransactionSignature];
		DISABLE TRIGGER [trg_del_tblTransactionSignature_ForSync] ON [dbo].[tblTransactionSignature];

		DISABLE TRIGGER [trg_Audit_del_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems];
		DISABLE TRIGGER [trg_del_tblTransactionSubLineItems_ForSync] ON [dbo].[tblTransactionSubLineItems];

		DISABLE TRIGGER [trg_Audit_del_tblTransactionTransportLineItems] ON [dbo].[tblTransactionTransportLineItems];
		DISABLE TRIGGER [trg_del_tblTransactionTransportLineItems_ForSync] ON [dbo].[tblTransactionTransportLineItems];

		DISABLE TRIGGER [trg_Audit_del_tblTransactionUserData] ON [dbo].[tblTransactionUserData];
		DISABLE TRIGGER [trg_del_tblTransactionUserData_ForSync] ON [dbo].[tblTransactionUserData];

 		DISABLE TRIGGER [trg_Audit_del_tblTransactionWeightReadings] ON [dbo].[tblTransactionWeightReadings];
		DISABLE TRIGGER [trg_del_tblTransactionWeightReadings_ForSync] ON [dbo].[tblTransactionWeightReadings];

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
						+ 'Procedure Name: [archive].[usp_DisableTriggersForOfflineArchiving]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


