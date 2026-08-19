

CREATE PROCEDURE [dbo].[usp_AggregateAssociatedTxValues]
@SiteGuid UNIQUEIDENTIFIER, @parentLookupTransTypeIndex SMALLINT, @TransactionLineItemGuid UNIQUEIDENTIFIER, @Quantity FLOAT OUTPUT, @Excise FLOAT OUTPUT, @gst FLOAT OUTPUT, @markup FLOAT OUTPUT, @TotalValue FLOAT OUTPUT, @TotalPriceWithTax FLOAT OUTPUT
AS
SET NOCOUNT ON

	  DECLARE LineItemIDs_cursor CURSOR FOR
	  SELECT TransactionLineItemGuid
		 FROM dbo.tblTransactionLinks
		WHERE TransactionLineItemGuid = @TransactionLineItemGuid
	  OPEN LineItemIDs_cursor;
	  FETCH NEXT
		FROM LineItemIDs_cursor
		INTO @TransactionLineItemGuid;

	  IF @@FETCH_STATUS <> 0
	  BEGIN
				CLOSE LineItemIDs_cursor;
				
				DECLARE @VolumeUnits int
				SET		@VolumeUnits = (SELECT dbo.tblSites.VolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

				DECLARE @VolumeDecimalPlaces int
				SET		@VolumeDecimalPlaces = (SELECT dbo.tblSites.VolumeDecimalPlaces FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

				SELECT @Quantity          = @Quantity          + GrossQuantity,
						  @Excise            = @Excise            + ISNULL(Tax1, 0),
						  @gst               = @gst               + ISNULL(Tax2, 0),
						  @markup            = @markup            + ISNULL(Tax3, 0),
						  @TotalPriceWithTax = @TotalPriceWithTax + 
								dbo.udf_ConvertFromSIUnits(abs(GrossQuantity),@VolumeUnits,@VolumeDecimalPlaces) * ProductPrice
								+ CASE WHEN t.AliasName LIKE 'Sale%' THEN ISNULL(Tax3, 0) - (ISNULL(Tax1, 0) + ISNULL(Tax2, 0)) * l.flag04
										ELSE 0 END
					FROM dbo.tblTransactionLineItems l
							 JOIN dbo.tblTransactions t
								ON l.TransactionGuid = t.TransactionGuid
				WHERE TransactionLineItemGuid  = @TransactionLineItemGuid
					 AND l.DeleteFlag     = 0
					 AND t.DeleteFlag     = 0
					 AND l.LookupQualityIndex        = 1
					 AND
						  (
									@parentLookupTransTypeIndex = 21
								OR @parentLookupTransTypeIndex = 22
								OR
									(
											 LookupTransTypeIndex         = 8
										AND l.LookupTransactionStatusIndex = 0
									)
								OR t.LookupTransTypeIndex <> 8
						  )
				 
				 RETURN
	  END
	  WHILE @@FETCH_STATUS = 0
	  BEGIN
				 EXEC [usp_AggregateAssociatedTxValues] @SiteGuid, -1,
							@TransactionLineItemGuid,
							@Quantity OUT,
							@Excise OUT,
							@gst OUT,
							@markup OUT,
							@TotalValue OUT,
							@TotalPriceWithTax OUT
				 FETCH NEXT
				  FROM LineItemIDs_cursor
				  INTO @TransactionLineItemGuid;
     
	  END
	  CLOSE LineItemIDs_cursor;