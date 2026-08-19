
-- =============================================
-- Author:		Chris Knight
-- Create date: 8/3/2012
-- Description:	Script to get unsent PIDX BOLS.
--              Pulled out of code and into SP call
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetUnsentPidxRecordBols] 
	@SiteGuid uniqueidentifier,
	@SendTime datetimeoffset
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DensityUnitIndex int
	SELECT @DensityUnitIndex = DensityUnitIndex FROM tblSites where SiteGuid = @SiteGuid
	
    -- Note that transaction status 0 is completed, 7 is cancelled, and 11 is posted.
	SELECT p.TransactionPIDXGuid,
		p.TransactionGuid,
		p.AuthorizationNumber,
		p.PIDXProfileGuid,
		p.CompanyPersonnelToShipToBillToGuid,
		p.SentFlag,
		p.DateSent,
		p.CreatedBy,
		p.CreatedDate,
		p.UpdatedBy,
		p.UpdatedDate,
		p.BrokenBlend, 
		p.BOLVersion,
		t.SiteGuid 
	FROM tblTransactionPIDX p WITH(NOLOCK) INNER JOIN tblTransactions t WITH(NOLOCK) 
		ON p.TransactionGuid = t.TransactionGuid 
	WHERE p.SentFlag = 0 
		AND p.BrokenBlend = 0
		AND t.TimeEnd IS NOT NULL
		AND ((p.UpdatedDate < dateadd(n,-5,@SendTime) AND (t.LookupTransactionStatusIndex = 0 OR t.LookupTransactionStatusIndex = 11)) 
		OR (p.UpdatedDate < dateadd(n,-50,@SendTime) AND (t.LookupTransactionStatusIndex = 7))) 
		AND t.SiteGuid = @SiteGuid
		AND NOT EXISTS (SELECT i.TransactionLineItemGuid FROM tblTransactionLineItems i WHERE i.TransactionLineItemGuid = p.TransactionGuid AND
						(((i.Density IS NULL OR i.Density < 1.0 OR dbo.udf_ConvertFromSIUnits(i.Density,@DensityUnitIndex,0) = 0.0) AND
						(i.GrossQuantity > 0.0 OR i.GrossQuantity < 0.0 OR i.NetQuantity > 0.0 OR i.NetQuantity < 0.0)) OR SubString(i.LoadingLocationID,5,1)='' or i.LoadingLocationID is null or i.GrossQuantity is null or i.NetQuantity is null or i.ProductCode is null or i.Temperature is null))
		AND 8 >= (SELECT Count(TransactionLineItemGuid) FROM tblTransactionLineItems i WHERE i.TransactionLineItemGuid = p.TransactionGuid)
END


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_GetUnsentPidxRecordBols] TO [FMDUserRole]
    AS [dbo];

