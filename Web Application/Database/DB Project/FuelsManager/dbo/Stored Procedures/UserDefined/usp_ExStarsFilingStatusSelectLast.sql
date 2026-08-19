CREATE PROCEDURE [dbo].[usp_ExStarsFilingStatusSelectLast]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
AS
BEGIN
	-- Get the most recet update of the last transactions for this site/manager
	SET NOCOUNT ON;

	SELECT TOP 1
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

	WHERE  (1=1) -- the site is important, the manager is not[ManagerCompanyGuid]    =  @ManagerCompanyGuid	
		AND [SiteGuid]				=  @SiteGuid
		AND ISNULL([ReportType], '') <> 'Replaced' 

	ORDER BY [FilingEndDate] DESC, [UpdatedDate] DESC

END

GO