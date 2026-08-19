

CREATE TRIGGER [dbo].[trg_ins_tblExportResultDetails_ForSAP] 
   ON [dbo].[tblExportResultDetails]
   AFTER INSERT
AS 
BEGIN 

	SET NOCOUNT ON
	-- We need to determine the origin of the transaction since the trigger
	-- should only update transaction created where is is being fired
	DECLARE @LookupOrigin TABLE ( LookupOriginApplicationIndex int)
	
	IF (SELECT dbo.udf_IsEnterprise()) = 1
	BEGIN 
		INSERT @LookupOrigin( LookupOriginApplicationIndex )
		SELECT 13 -- EnterpriseLevelTransaction ( from lookup.tblTransactionOrigin )
		UNION
		SELECT 15 -- AdcUploadedAtEnterpriseLevel
	END
	ELSE
	BEGIN 
		INSERT @LookupOrigin( LookupOriginApplicationIndex )
		SELECT 12 -- BaseLevelTransaction
		UNION
		SELECT 14 -- AdcUploadedAtBaseLevel
	END

	-- This should fire only at enterprise level
	IF (SELECT dbo.udf_IsEnterprise()) = 1
	BEGIN 
		-- if there are previous versions of the transaction results change the interface name so they are not displayed
		UPDATE er2
		SET InterfaceName = 'SAPTransactionResultHistory'
		FROM inserted i 
		JOIN tblExportResults er
		ON i.ExportResultGuid = er.ExportResultGuid
		AND er.InterfaceName = 'SAPTransactionResult'
		JOIN tblExportResultDetails erd
		ON erd.RecordID = i.RecordID
		JOIN tblExportResults er2
		ON erd.ExportResultGuid = er2.ExportResultGuid
		AND er2.ExportResultGuid <> er.ExportResultGuid
		AND er2.InterfaceName = er.InterfaceName
	END

	--------------------- UPDATE EXISTING TRANSACTIONS ------------------	
	-- update the transaction status to pending if the results is because we successfully sent transactions
	UPDATE t
	SET  Flag05 = 1, -- Sent flag
	Flag06 = 0, -- SAP complete flag
	LookupTransactionStatusIndex = 16,  -- status to pending
	UpdatedBy = 'SAPInterface',
	UpdatedDate = SYSDATETIMEOFFSET()
	FROM tblTransactions t
	JOIN inserted i 
	ON i.RecordID = t.TransID
	JOIN tblExportResults er
	ON i.ExportResultGuid = er.ExportResultGuid
	AND er.BatchID = t.TransID
	JOIN @LookupOrigin lo
	ON lo.LookupOriginApplicationIndex  = t.LookupOriginApplicationIndex 
	WHERE er.InterfaceName = 'NSPASAPInterface'
	AND er.SuccessCount = 1

	-- now update the transaction line items to pending
	UPDATE tli
	SET LookupTransactionStatusIndex = 11,  -- status to posted
	UpdatedBy = 'SAPInterface',
	UpdatedDate = SYSDATETIMEOFFSET()
	FROM tblTransactions t
	JOIN tblTransactionLineItems tli
	ON tli.TransactionGuid = t.TransactionGuid
	JOIN inserted i 
	ON i.RecordID = t.TransID
	JOIN tblExportResults er
	ON i.ExportResultGuid = er.ExportResultGuid
	AND er.BatchID = t.TransID
	JOIN @LookupOrigin lo
	ON lo.LookupOriginApplicationIndex  = t.LookupOriginApplicationIndex 
	WHERE er.InterfaceName = 'NSPASAPInterface'
	AND er.SuccessCount = 1


	-- we need to update the transaction status to posted if we got results back
	UPDATE t
	SET Flag06 = 1, -- SAP complete flag
	LookupTransactionStatusIndex = 11,  -- status to posted
	ErrorFlag = (CASE WHEN COALESCE( i.Error, '' ) = '' THEN 0 ELSE 1 END ),
	UpdatedBy = 'SAPInterface',
	UpdatedDate = SYSDATETIMEOFFSET()
	FROM tblTransactions t
	JOIN tblTransactionUserData tu
	ON tu.TransactionGuid = t.TransactionGuid
	JOIN inserted i 
	ON i.RecordID = t.TransID
	JOIN tblExportResults er
	ON i.ExportResultGuid = er.ExportResultGuid
	JOIN @LookupOrigin lo
	ON lo.LookupOriginApplicationIndex  = t.LookupOriginApplicationIndex 
	WHERE er.InterfaceName = 'SAPTransactionResult'

	-- now update the transaction line items to posted
	UPDATE tli
	SET LookupTransactionStatusIndex = 11,  -- status to posted
	UpdatedBy = 'SAPInterface',
	UpdatedDate = SYSDATETIMEOFFSET()
	FROM tblTransactions t
	JOIN tblTransactionLineItems tli
	ON tli.TransactionGuid = t.TransactionGuid
	JOIN inserted i 
	ON i.RecordID = t.TransID
	JOIN tblExportResults er
	ON i.ExportResultGuid = er.ExportResultGuid
	JOIN @LookupOrigin lo
	ON lo.LookupOriginApplicationIndex  = t.LookupOriginApplicationIndex 
	WHERE er.InterfaceName = 'SAPTransactionResult'


END
