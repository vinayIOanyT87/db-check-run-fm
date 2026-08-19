CREATE PROCEDURE [dbo].[gsp_ExStarsFilingStatusSelect]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
, @FilingStartDate			DATE
, @FilingEndDate			DATE
, @Modifier					NVARCHAR(30)
AS
BEGIN
	-- If there are replacements, corrections, incoming/outgoing manager reports this may return multiple rows
	-- return only the bare essentials

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

	WHERE  (1=1) -- the site is important, the manager is not [ManagerCompanyGuid]    =  @ManagerCompanyGuid	
		AND [SiteGuid]				=  @SiteGuid
		AND [FilingStartDate]       =  @FilingStartDate	
		AND [FilingEndDate]			=  @FilingEndDate	
		AND ISNULL([ReportType], '') <> 'Replaced' 
		AND ( ISNULL(@Modifier, '')  in ( '', [Modifier]))
END

GO

CREATE PROCEDURE [dbo].[gsp_ExStarsFilingHistory]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
, @FilingStartDate			DATE
, @FilingEndDate			DATE
AS
BEGIN
	-- If there are replacements, corrections, incoming/outgoing manager reports this may return multiple rows
	-- return only the bare essentials

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


CREATE PROCEDURE [dbo].[gsp_ExStarsFilingStatusSelectLast]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
AS
BEGIN
	-- Get the most recet update of the last transactions for this site/manager

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

CREATE PROCEDURE [dbo].[gsp_ExStarsFilingStatusByControlNumberSelect]
 @TransSetControlNumber		NVARCHAR(9)
, @Modifier					NVARCHAR(30)
AS
BEGIN
	-- If there are replacements, corrections, incoming/outgoing manager reports this may return multiple rows
	-- return only the bare essentials

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

