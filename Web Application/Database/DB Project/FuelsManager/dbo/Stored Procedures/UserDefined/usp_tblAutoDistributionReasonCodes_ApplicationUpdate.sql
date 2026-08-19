

/****************************** usp_tblAutoDistributionReasonCodes_ApplicationUpdate ******************************/
CREATE PROCEDURE dbo.usp_tblAutoDistributionReasonCodes_ApplicationUpdate
	@ReasonCodeGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@ReasonCode NVARCHAR(50),
	@Description NVARCHAR(255),
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy NVARCHAR(50)
AS
BEGIN
	UPDATE 
		dbo.tblAutoDistributionReasonCodes  
	SET 
		SiteGuid = @SiteGuid,  
		ReasonCode = @ReasonCode,  
		Description = @Description,  
		UpdatedDate = @UpdatedDate,  
		UpdatedBy  = @UpdatedBy  
	WHERE 
		AutoDistributionReasonCodeGuid=@ReasonCodeGuid;			
END