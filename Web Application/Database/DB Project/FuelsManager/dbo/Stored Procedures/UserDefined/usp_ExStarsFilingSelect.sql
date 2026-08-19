CREATE PROCEDURE [dbo].[usp_ExStarsFilingSelect]
  @ManagerCompanyGuid		UNIQUEIDENTIFIER
, @SiteGuid					UNIQUEIDENTIFIER
, @FilingStartDate			DATE -- presumes 1st of month
, @FilingEndDate			DATE -- presumes last of month
, @ReportType				NVARCHAR(30)
, @UnacknowledgedOnly		BIT
AS
BEGIN
	-- If there are replacements, corrections, incoming/outgoing manager reports this may return multiple rows
	SET NOCOUNT ON;

	SELECT 
		 [FilingStartDate]
		,[FilingEndDate]
		,[ManagerCompanyGuid]
		,[SiteGuid]

		,[ControlNumber]
		,[OriginalControlNumber]
		,[TransSetControlNumber]
		,[ReportType]
		,[Modifier]
		,[FilingStatus]
		,[RawDataFileName]
		,[EasyReadFileName]
		,[EdiReport]
		,[EasyReadReport]
		,[SerializedData]
		,[Acknowledgement]
		,[AckEasyRead]
		,[FilingCreated]
		,[FilingSent]
		,[ResponseLoaded]
		,[CreatedBy]
		,[UpdatedBy]
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

	FROM [dbo].[tblExStarsFilings] f

	WHERE (@UnacknowledgedOnly = 0 OR ResponseLoaded IS NULL)
		AND [ManagerCompanyGuid]    =  @ManagerCompanyGuid	
		AND [SiteGuid]				=  @SiteGuid
		AND [FilingStartDate]       >=  @FilingStartDate	
		AND [FilingEndDate]			<=  @FilingEndDate	
		AND ( ISNULL(@ReportType, '')  in ( '', [ReportType]))

	ORDER BY [UpdatedDate] DESC
END

GO
