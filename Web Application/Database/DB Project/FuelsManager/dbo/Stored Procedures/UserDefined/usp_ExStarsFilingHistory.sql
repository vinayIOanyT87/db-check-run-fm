CREATE PROCEDURE [dbo].[usp_ExStarsFilingHistory]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
, @FilingStartDate			DATE
, @FilingEndDate			DATE
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

	WHERE  (1=1) 
		AND [ManagerCompanyGuid]    =  @ManagerCompanyGuid	
		AND [SiteGuid]				=  @SiteGuid
		AND [FilingStartDate]       >=  @FilingStartDate	
		AND [FilingEndDate]			<=  @FilingEndDate	
		AND ISNULL([ReportType], '') <> 'Replaced' 

END

GO
