
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12

Description:	
	This function retrieves meter closeout information for all meters in the system for a specific inventory date
	
	The data are returned as a table:
		The MeterGuid is the meter the information in the row corresponds to.
		The meter start value is based on the most recent closeout before the inventory date provided.
		The meter stop value is based on the closeout which occurred on the inventory date provided.
		The transaction ID of the closeout which occurred on the inventory date provided.
		If there was more than one closeout on the inventory date provided, the MoreThanOneCloseoutFlag will be set to 1
		If there was no closeout on the inventory date provided, the NoCurrentCloseoutFlag will be set to 1
		If there was no closeout before the inventory date provided, the NoPreviousCloseoutFlag will be set to 1

    Modification History:
    Date         Version     By          Description
    ----------   -------     ----        -------------
    04/24/2012   1.0.000     Ryan Hill   --
    07/08/2019   1.0.001     Jay R       Rewrote logic to use temp table to make function more efficient.
	08/02/2022	 12.0.0		 FJM		 Fixed the selection by only working with original transactions
=============================================
*/
CREATE FUNCTION [dbo].[udf_MeterReconciliationSelectCloseoutInformation]
(
	@InventoryDate DATE,
	@SiteGuid UNIQUEIDENTIFIER,
	@CloseoutTransactionAliasGuid UNIQUEIDENTIFIER
)
RETURNS @MeterCloseouts TABLE 
(
    MeterGuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
    MeterStart FLOAT NULL,
	MeterStop FLOAT NULL,
	CurrentCloseoutTransactionID NVARCHAR(64) NULL,
	MoreThanOneCloseoutFlag BIT NULL,
	NoCurrentCloseoutFlag BIT NULL,
	NoPreviousCloseoutFlag BIT NULL,
	CurrentCloseoutTransactionGuid UNIQUEIDENTIFIER NULL
)
AS
BEGIN
    DECLARE @TempTransactions TABLE (
	    MeterGuid UNIQUEIDENTIFIER
	    ,InventoryDate DATE
        ,TransID NVARCHAR(64)
        ,MeterStop FLOAT
		,TransGuid UNIQUEIDENTIFIER
	)

    INSERT INTO @TempTransactions (MeterGuid, InventoryDate, TransID, MeterStop, TransGuid)
    SELECT tli.MeterGuid, t.InventoryDate, t.TransID, tli.MeterStop, t.TransactionGuid
    FROM tblTransactions t WITH (NOLOCK)
    INNER JOIN tblTransactionLineItems tli WITH (NOLOCK) ON tli.TransactionGuid = t.TransactionGuid
    INNER JOIN tblMeter m WITH (NOLOCK) ON m.MeterGuid = tli.MeterGuid
    WHERE t.TransactionAliasGuid = @CloseoutTransactionAliasGuid
    AND t.SiteGuid = @SiteGuid
    AND t.InventoryDate <= @InventoryDate
	AND ( t.ReversalType IS NULL OR t.ReversalType = 'O' )
    AND tli.MeterStop IS NOT NULL --we have to have a meter stop for a meter closeout
    AND (tli.DeleteFlag = 0 OR tli.DeleteFlag IS NULL) --ignore deleted transactions


	--First, determine which meters contain errors that prevent us from determining the meter start and stop values
	INSERT INTO @MeterCloseouts(MeterGuid, MeterStart, MeterStop, MoreThanOneCloseoutFlag, NoCurrentCloseoutFlag, NoPreviousCloseoutFlag)
		SELECT MeterGuid, MeterStart, MeterStop, MoreThanOneCloseoutFlag, NoCurrentCloseoutFlag, NoPreviousCloseoutFlag 
		FROM(
			SELECT 
			MeterGuid,
			MeterStart = NULL, 
			MeterStop = NULL,
			--if the meter has more than one closeout on the inventory date specified, indicate an error with a 1 for the MoreThanOneCloseout field
            MoreThanOneCloseoutFlag = CASE 
		        WHEN EXISTS (
				        SELECT 1
				        FROM @TempTransactions
				        WHERE InventoryDate = @InventoryDate
                            AND MeterGuid = tblMeter.MeterGuid
				        GROUP BY MeterGuid
				        HAVING COUNT(*) > 1
				        )
			        THEN 1
		        ELSE 0
		        END,
			--if the meter has no closeout on the inventory date specified, indicate an error with a 1 for the NoCurrentCloseoutFlag field
			NoCurrentCloseoutFlag = CASE 
                WHEN NOT EXISTS (
                        SELECT 1 
                        FROM @TempTransactions 
						WHERE InventoryDate = @InventoryDate
                            AND MeterGuid = tblMeter.MeterGuid
						) 
                    THEN 1 
				ELSE 0 
				END,
			--if the meter has no closeout before the inventory date specified, indicate an error with a 1 for the NoPreviousCloseoutFlag field
			NoPreviousCloseoutFlag = CASE 
                WHEN NOT EXISTS (
                        SELECT 1 
                        FROM @TempTransactions
						WHERE InventoryDate < @InventoryDate
                            AND MeterGuid = tblMeter.MeterGuid
                        ) 
                    THEN 1 
				ELSE 0 
				END
		    FROM tblMeter WITH (NOLOCK)
            WHERE SiteGuid = @SiteGuid) Results 
		WHERE NoPreviousCloseoutFlag = 1 
            OR NoCurrentCloseoutFlag = 1 
            OR MoreThanOneCloseoutFlag = 1
	
	--Set the meter start values
	INSERT INTO @MeterCloseouts(MeterGuid, MeterStart, MeterStop, MoreThanOneCloseoutFlag, NoCurrentCloseoutFlag, NoPreviousCloseoutFlag)
		SELECT
			tblMeter.MeterGuid, 
			--the meter start is the meter stop from a closeout which occurred before the inventory date specified
			MeterStart = (
                SELECT TOP 1 MeterStop
                FROM @TempTransactions
                WHERE InventoryDate < @InventoryDate
                    AND MeterGuid = tblMeter.MeterGuid
                ORDER BY InventoryDate DESC
            ),
			NULL, --MeterStop
			0, --MoreThanOneCloseoutFlag
			0, --NoCurrentCloseoutFlag
			0 --NoPreviousCloseoutFlag
		FROM tblMeter WITH (NOLOCK)
		WHERE tblMeter.MeterGuid NOT IN (SELECT MeterGuid FROM @MeterCloseouts) --if the meter is in @MeterCloseouts already, it has an error
			AND SiteGuid = @SiteGuid

	--set the meter stop values 
	--the meter stop is the meter stop from a closeout which occurred on the inventory date specified
	UPDATE @MeterCloseouts
	SET MeterStop = tblTransactionLineItems.MeterStop,
		CurrentCloseoutTransactionID = tblTransactions.TransID,
		CurrentCloseoutTransactionGuid = tblTransactions.TransactionGuid
	FROM @MeterCloseouts AS PreviousCloseouts
		INNER JOIN tblTransactionLineItems WITH (NOLOCK) ON PreviousCloseouts.MeterGuid = tblTransactionLineItems.MeterGuid
		INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
	WHERE tblTransactions.TransactionAliasGuid = @CloseoutTransactionAliasGuid
		AND tblTransactions.SiteGuid = @SiteGuid
		AND tblTransactions.InventoryDate = @InventoryDate
		AND ( tblTransactions.ReversalType IS NULL OR tblTransactions.ReversalType = 'O' )
		AND tblTransactionLineItems.MeterStop IS NOT NULL --we have to have a meter stop for a meter closeout
		AND (tblTransactionLineItems.DeleteFlag = 0 OR tblTransactionLineItems.DeleteFlag IS NULL) --ignore deleted transactions
		AND PreviousCloseouts.MoreThanOneCloseoutFlag = 0 AND PreviousCloseouts.NoCurrentCloseoutFlag = 0 AND PreviousCloseouts.NoPreviousCloseoutFlag = 0
	
	--return the results
	RETURN 
END
GO
