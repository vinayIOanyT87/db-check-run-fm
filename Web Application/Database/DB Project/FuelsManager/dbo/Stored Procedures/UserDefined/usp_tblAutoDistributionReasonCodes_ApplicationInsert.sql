
/*-----------------------------  Stored Procedures for tblAutoDistributionReasonCodes -----------------------------*/

/****************************** usp_tblAutoDistributionReasonCodes_ApplicationInsert ******************************/
CREATE PROCEDURE dbo.usp_tblAutoDistributionReasonCodes_ApplicationInsert
	@SiteGuid UNIQUEIDENTIFIER,
	@ReasonCode NVARCHAR(50),
	@Description NVARCHAR(255),
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy NVARCHAR(50),
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy NVARCHAR(50),
	@ReasonCodeGuid UNIQUEIDENTIFIER OUTPUT
AS
BEGIN

	DECLARE @PrimaryKeyGuid UNIQUEIDENTIFIER
	SET @PrimaryKeyGuid = NEWID()
	INSERT INTO dbo.tblAutoDistributionReasonCodes 
	( 
		AutoDistributionReasonCodeGuid, SiteGuid, ReasonCode, [Description],
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@PrimaryKeyGuid, @SiteGuid, @ReasonCode, @Description,
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ReasonCodeGuid = @PrimaryKeyGuid	
END