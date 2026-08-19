CREATE PROCEDURE [dbo].[usp_ExStarsFilingSelectByTransSetCtrlNumber]
@TransSetControlNumber NVARCHAR(9)
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

	WHERE @TransSetControlNumber = TransSetControlNumber
END

GO
