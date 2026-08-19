CREATE PROCEDURE [dbo].[usp_ExStarsFilingStatusByControlNumberSelect]
 @TransSetControlNumber		NVARCHAR(9)
, @Modifier					NVARCHAR(30)
AS
BEGIN
	-- If there are replacements, corrections, incoming/outgoing manager reports this may return multiple rows
	-- return only the bare essentials
	SET NOCOUNT ON;

	SELECT
		 [FilingStartDate]
		,[FilingEndDate]
		,[ManagerCompanyGuid]
		,[ControlNumber]
		,[OriginalControlNumber]
		,[TransSetControlNumber]
		,[ReportType]
		,[Modifier]
		,[FilingStatus]
		,[FilingCreated]
		,[FilingSent]
		,[ResponseLoaded]
		, ( 
			SELECT  count(*) as count 
			FROM  [dbo].[tblExStarsReportedErrors] re
			WHERE  re.[ExStarsFilingsGuid] = f.[ExStarsFilingsGuid] 
			AND [ErrorCorrected] = 0 AND re.[MustCorrect] = 1
			) as UnresolvedErrors
		, ( 
			SELECT  count(*) as count 
			FROM  [dbo].[tblExStarsReportedErrors] re
			WHERE  re.[ExStarsFilingsGuid] = f.[ExStarsFilingsGuid] 
			AND [ErrorCorrected] = 0 AND re.[MustCorrect] = 0
			) as UnresolvedWarnings
		, [ExStarsFilingsGuid]

	FROM [dbo].[tblExStarsFilings]  f

	WHERE [TransSetControlNumber] = @TransSetControlNumber
		AND ISNULL([ReportType], '') <> 'Replaced' 
		AND ( ISNULL(@Modifier, '')  in ( '', [Modifier]))
END

GO