CREATE FUNCTION [dbo].[udf_StatusValue]
(@TransactionStatus INT, @StatusType INT)
RETURNS NVARCHAR (30)
AS
BEGIN
	DECLARE @stringValue nvarchar (30)
	IF @StatusType = 0 
	BEGIN
		SELECT @stringValue=
			CASE
				WHEN @TransactionStatus=0 THEN 'Completed'
				WHEN @TransactionStatus=1 THEN 'InProgress'
				WHEN @TransactionStatus=2 THEN 'Dispatched'
				WHEN @TransactionStatus=3 THEN 'Requested'
				WHEN @TransactionStatus=4 THEN 'Closed'
				WHEN @TransactionStatus=5 THEN 'OnHold'
				WHEN @TransactionStatus=6 THEN 'Scheduled'
				WHEN @TransactionStatus=7 THEN 'Cancelled'
				WHEN @TransactionStatus=8 THEN 'Acknowledged'
				WHEN @TransactionStatus=9 THEN 'LoadPending'
				WHEN @TransactionStatus=10 THEN 'WeightOutPending'
			ELSE 'Unknown'
			END
	END
	ELSE IF @StatusType = 1
	BEGIN
		SELECT @stringValue =
		CASE
			WHEN @TransactionStatus=0 THEN 'Quarantined'
			WHEN @TransactionStatus=1 THEN 'Usable'
			WHEN @TransactionStatus=2 THEN 'Unusable'
		Else 'Unknown'		
		END
	END
	Else
		SELECT @stringValue ='Unknown'

	-- Return the result of the function
	RETURN @stringValue

END