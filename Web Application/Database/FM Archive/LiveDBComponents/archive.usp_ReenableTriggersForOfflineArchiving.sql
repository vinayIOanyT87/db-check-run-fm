/*
	DROP PROCEDURE [archive].[usp_ReenableTriggersForOfflineArchiving]

	EXEC [archive].[usp_ReenableTriggersForOfflineArchiving]

*/
CREATE PROCEDURE [archive].[usp_ReenableTriggersForOfflineArchiving]
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_ReenableTriggersForOfflineArchiving]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Reenable the Auditing and FMSync OnDelete triggers on the tables covered by archiving.
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		--AlarmAndEventLog
		ENABLE TRIGGER [trg_del_tblAlarmAndEventLog_ForSync] ON [dbo].[tblAlarmAndEventLog];
	
		--AuditLog
		ENABLE TRIGGER [trg_del_tblAuditLog_ForSync] ON [dbo].[tblAuditLog];

		--Transaction
		ENABLE TRIGGER [trg_Audit_del_tblTransactionLineItems] ON [dbo].[tblTransactionLineItems];
		ENABLE TRIGGER [trg_del_tblTransactionLineItems_ForSync] ON [dbo].[tblTransactionLineItems];

		ENABLE TRIGGER [trg_Audit_del_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData];
		ENABLE TRIGGER [trg_del_tblTransactionLineItemUserData_ForSync] ON [dbo].[tblTransactionLineItemUserData];
	
		ENABLE TRIGGER [trg_del_tblTransactionLinks_ForSync] ON [dbo].[tblTransactionLinks];
	
		ENABLE TRIGGER [trg_Audit_del_tblTransactionNotes] ON [dbo].[tblTransactionNotes];
		ENABLE TRIGGER [trg_del_tblTransactionNotes_ForSync] ON [dbo].[tblTransactionNotes];

		ENABLE TRIGGER [trg_Audit_del_tblTransactionPIDX] ON [dbo].[tblTransactionPIDX];
		ENABLE TRIGGER [trg_del_tblTransactionPIDX_ForSync] ON [dbo].[tblTransactionPIDX];

		ENABLE TRIGGER [trg_Audit_del_tblTransactions] ON [dbo].[tblTransactions];
		ENABLE TRIGGER [trg_del_tblTransactions_ForSync] ON [dbo].[tblTransactions];

		ENABLE TRIGGER [trg_Audit_del_tblTransactionSignature] ON [dbo].[tblTransactionSignature];
		ENABLE TRIGGER [trg_del_tblTransactionSignature_ForSync] ON [dbo].[tblTransactionSignature];

		ENABLE TRIGGER [trg_Audit_del_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems];
		ENABLE TRIGGER [trg_del_tblTransactionSubLineItems_ForSync] ON [dbo].[tblTransactionSubLineItems];

		ENABLE TRIGGER [trg_Audit_del_tblTransactionTransportLineItems] ON [dbo].[tblTransactionTransportLineItems];
		ENABLE TRIGGER [trg_del_tblTransactionTransportLineItems_ForSync] ON [dbo].[tblTransactionTransportLineItems];

		ENABLE TRIGGER [trg_Audit_del_tblTransactionUserData] ON [dbo].[tblTransactionUserData];
		ENABLE TRIGGER [trg_del_tblTransactionUserData_ForSync] ON [dbo].[tblTransactionUserData];

 		ENABLE TRIGGER [trg_Audit_del_tblTransactionWeightReadings] ON [dbo].[tblTransactionWeightReadings];
		ENABLE TRIGGER [trg_del_tblTransactionWeightReadings_ForSync] ON [dbo].[tblTransactionWeightReadings];

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
						+ 'Procedure Name: [archive].[usp_ReenableTriggersForOfflineArchiving]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


