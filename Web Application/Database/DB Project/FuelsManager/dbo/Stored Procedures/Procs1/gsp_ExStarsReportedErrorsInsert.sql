CREATE PROCEDURE [dbo].[gsp_ExStarsReportedErrorsInsert]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
, @ExStarsFilingsGuid		UNIQUEIDENTIFIER
, @SequenceNumber			NVARCHAR(20)
, @MustCorrect				BIT
, @PBI01_Primary			NVARCHAR(10)
, @PBI01_Secondary			NVARCHAR(10)
, @PBI03_Primary			NVARCHAR(10)
, @PBI03_Secondary			NVARCHAR(10)
, @PBI04					NVARCHAR(10)
, @OriginalValue			NVARCHAR(MAX)
, @IrsErrorText				NVARCHAR(MAX)
, @ErrorCorrected			BIT
, @UpdatedBy				[dbo].[udtUserID]

AS
BEGIN

	INSERT INTO [dbo].[tblExStarsReportedErrors] (
		  ManagerCompanyGuid
		, SiteGuid
		, ExStarsFilingsGuid
		, SequenceNumber
		, MustCorrect
		, PBI01_Primary
		, PBI01_Secondary
		, PBI03_Primary
		, PBI03_Secondary
		, PBI04
		, OriginalValue
		, IrsErrorText
		, ErrorCorrected
		, CreatedBy
		, UpdatedBy
		, ExStarsReportedErrorsGuid
	) VALUES( 
		  @ManagerCompanyGuid		
		, @SiteGuid	
		, @ExStarsFilingsGuid				
		, @SequenceNumber			
		, @MustCorrect
		, @PBI01_Primary			
		, @PBI01_Secondary			
		, @PBI03_Primary			
		, @PBI03_Secondary			
		, @PBI04					
		, @OriginalValue			
		, @IrsErrorText				
		, @ErrorCorrected			
		, @UpdatedBy				
		, @UpdatedBy				
		, NEWID()
	)
END


RETURN 0
