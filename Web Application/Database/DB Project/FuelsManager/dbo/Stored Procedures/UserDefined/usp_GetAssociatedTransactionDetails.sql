
CREATE PROCEDURE [dbo].[usp_GetAssociatedTransactionDetails]
@TransactionLineItemGuidList VARCHAR(8000), @TransactionAliasGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER
AS
BEGIN 
	-- Parse the comma-delimited list of Guids into a temp table
	CREATE TABLE #TransactionLineItemGuids (TransactionLineItemGuid UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED)
	DECLARE @prevCommaPos INT
	DECLARE @commaPos INT
	SET @prevCommaPos = 0
	SET @commaPos = 0

	WHILE (@commaPos < LEN(@TransactionLineItemGuidList))
	BEGIN
		SET @commaPos = CHARINDEX(',', @TransactionLineItemGuidList, @prevCommaPos + 1)
		IF (@commaPos = 0)
			SET @commaPos = LEN(@TransactionLineItemGuidList) + 1

		print @prevCommaPos
		print @commaPos
		print SUBSTRING(@TransactionLineItemGuidList, @prevCommaPos + 1, @commaPos - @prevCommaPos - 1)
		INSERT #TransactionLineItemGuids VALUES (SUBSTRING(@TransactionLineItemGuidList, @prevCommaPos + 1, @commaPos - @prevCommaPos - 1))

		SET @prevCommaPos = @commaPos
	END

    DECLARE TransactionLineItemGuids_cursor CURSOR FOR 
        SELECT li.TransactionLineItemGuid, t.LookupTransTypeIndex, li.GrossQuantity
		  FROM tblTransactionLineItems li INNER JOIN #TransactionLineItemGuids
		    ON li.TransactionLineItemGuid = #TransactionLineItemGuids.TransactionLineItemGuid 
		  LEFT JOIN tblTransactions t ON
			   li.TransactionGuid = t.TransactionGuid

    CREATE TABLE #RESULTS (TransactionLineItemGuid UNIQUEIDENTIFIER, LookupTransTypeIndex SMALLINT, GrossQuantity float, 
                GrossQuantityReceived float, Excise float, GST float, Markup float, TotalValue float, TotalPriceWithTax float); 

    DECLARE @TransactionLineItemGuid UNIQUEIDENTIFIER; 
    DECLARE @LookupTransTypeIndex int; 
    DECLARE @Quantity float; 
    DECLARE @QuantityReceived float; 
    DECLARE @Excise float; 
    DECLARE @gst float; 
    DECLARE @markup float; 
    DECLARE @TotalValue float; 
    DECLARE @TotalPriceWithTax float;
    DECLARE @ParentLookupTransTypeIndex smallint;
    SELECT @ParentLookupTransTypeIndex =  LookupTransTypeIndex FROM tblTransactionAliases WHERE TransactionAliasGuid = @TransactionAliasGuid;
    IF @LookupTransTypeIndex IS NULL
       SET @LookupTransTypeIndex = 0; 
    OPEN TransactionLineItemGuids_cursor; 
    FETCH NEXT FROM TransactionLineItemGuids_cursor INTO @TransactionLineItemGuid, @LookupTransTypeIndex, @Quantity; 
    WHILE @@FETCH_STATUS = 0 
    BEGIN    
            SET @QuantityReceived = 0;    
            SET @Excise  = 0;    
            SET @gst  = 0;    
            SET @markup  = 0;   
            SET @TotalValue  = 0;    
            SET @TotalPriceWithTax  = 0;    
            EXEC [usp_AggregateAssociatedTxValues] @SiteGuid, @ParentLookupTransTypeIndex, 
                                         @TransactionLineItemGuid, @QuantityReceived out, @Excise out, 
							                        @gst out, @markup out, @TotalValue out, @TotalPriceWithTax out ;   
            INSERT INTO #RESULTS (TransactionLineItemGuid, LookupTransTypeIndex, GrossQuantity, GrossQuantityReceived, Excise, GST, Markup, TotalValue, TotalPriceWithTax) 
		                        VALUES  
		                        ( @TransactionLineItemGuid, @LookupTransTypeIndex, @Quantity, @QuantityReceived, @Excise, @gst, @markup, @TotalValue, @TotalPriceWithTax);    
            FETCH NEXT FROM TransactionLineItemGuids_cursor INTO @TransactionLineItemGuid, @LookupTransTypeIndex, @Quantity; 
    END 
    CLOSE TransactionLineItemGuids_cursor; 
    SELECT * FROM #RESULTS ; 
    DROP TABLE #RESULTS;  
	DROP TABLE #TransactionLineItemGuids
END