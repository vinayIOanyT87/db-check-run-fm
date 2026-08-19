CREATE PROCEDURE [dbo].[usp_ExternalStationTransactionEnumerate]
	@SiteGuid UNIQUEIDENTIFIER,
	@ExternalStationGuid UNIQUEIDENTIFIER = NULL,
	@BeginDate DATETIMEOFFSET, 
	@EndDate DATETIMEOFFSET, 
	@TransactionID NVARCHAR(20) = NULL
AS
BEGIN
	SET NOCOUNT ON

	IF @BeginDate IS NULL
	BEGIN
		-- Let's drop off the time part by using a DATE before we assign it to @BeginDate
		DECLARE @EarliestBeginDate DATE;
		SET @EarliestBeginDate = DATEADD(month, -6, SYSDATETIMEOFFSET());
		SET @BeginDate = @EarliestBeginDate
	END

	IF @EndDate IS NULL
	BEGIN
		-- We typically set the time on the search end date to 23:59:59 since we perform a <= comparison.
		-- To keep consistent with this behavior, our default value will use the same pattern.
		DECLARE @FirstOfNextMonth DATETIMEOFFSET(7);
		SET @FirstOfNextMonth = DATEADD(day, 1, EOMONTH(sysdatetimeoffset()))
		SET @EndDate = DATEADD(millisecond, -1, @FirstOfNextMonth);
	END

	BEGIN TRY

		IF (@ExternalStationGuid IS NOT NULL AND @TransactionID IS NOT NULL)
		BEGIN
			SELECT 
				[dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid],
				[dbo].[tblExternalStationTransaction].[ExternalStationGuid],
				[dbo].[tblExternalStationTransaction].[SiteGuid],
				[dbo].[tblExternalStation].[ID] AS ExternalStationID,
				[dbo].[tblExternalStationTransaction].[StationTransactionID],
				[dbo].[tblExternalStationTransaction].[RawTransactionData],
				[dbo].[tblExternalStationTransaction].[CreatedBy],
				[dbo].[tblExternalStationTransaction].[CreatedDate],
				[dbo].[tblExternalStationTransaction].[UpdatedBy],
				[dbo].[tblExternalStationTransaction].[UpdatedDate],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionStatusIndex],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex]
			FROM [dbo].[tblExternalStationTransaction]
				INNER JOIN [dbo].[tblExternalStation] ON [dbo].[tblExternalStation].[ExternalStationGuid] = [dbo].[tblExternalStationTransaction].[ExternalStationGuid]
			WHERE [dbo].[tblExternalStationTransaction].[ExternalStationGuid] = @ExternalStationGuid 
					AND [dbo].[tblExternalStationTransaction].[StationTransactionID] LIKE (@TransactionID + '%')
					AND [dbo].[tblExternalStationTransaction].[SiteGuid] = @SiteGuid 
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] >= @BeginDate 
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] <= @EndDate
			ORDER BY [dbo].[tblExternalStationTransaction].[CreatedDate] DESC
		END
		ELSE IF (@ExternalStationGuid IS NOT NULL)
		BEGIN
			SELECT 
				[dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid],
				[dbo].[tblExternalStationTransaction].[ExternalStationGuid],
				[dbo].[tblExternalStationTransaction].[SiteGuid],
				[dbo].[tblExternalStation].[ID] AS ExternalStationID,
				[dbo].[tblExternalStationTransaction].[StationTransactionID],
				[dbo].[tblExternalStationTransaction].[RawTransactionData],
				[dbo].[tblExternalStationTransaction].[CreatedBy],
				[dbo].[tblExternalStationTransaction].[CreatedDate],
				[dbo].[tblExternalStationTransaction].[UpdatedBy],
				[dbo].[tblExternalStationTransaction].[UpdatedDate],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionStatusIndex],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex]
			FROM [dbo].[tblExternalStationTransaction]
				INNER JOIN [dbo].[tblExternalStation] ON [dbo].[tblExternalStation].[ExternalStationGuid] = [dbo].[tblExternalStationTransaction].[ExternalStationGuid]
			WHERE [dbo].[tblExternalStationTransaction].[ExternalStationGuid] = @ExternalStationGuid 
					AND [dbo].[tblExternalStationTransaction].[SiteGuid] = @SiteGuid
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] >= @BeginDate 
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] <= @EndDate
			ORDER BY [dbo].[tblExternalStationTransaction].[CreatedDate] DESC
		END
		ELSE IF (@TransactionID IS NOT NULL)
		BEGIN
			SELECT 
				[dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid],
				[dbo].[tblExternalStationTransaction].[ExternalStationGuid],
				[dbo].[tblExternalStationTransaction].[SiteGuid],
				[dbo].[tblExternalStation].[ID] AS ExternalStationID,
				[dbo].[tblExternalStationTransaction].[StationTransactionID],
				[dbo].[tblExternalStationTransaction].[RawTransactionData],
				[dbo].[tblExternalStationTransaction].[CreatedBy],
				[dbo].[tblExternalStationTransaction].[CreatedDate],
				[dbo].[tblExternalStationTransaction].[UpdatedBy],
				[dbo].[tblExternalStationTransaction].[UpdatedDate],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionStatusIndex],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex]
			FROM [dbo].[tblExternalStationTransaction]
				INNER JOIN [dbo].[tblExternalStation] ON [dbo].[tblExternalStation].[ExternalStationGuid] = [dbo].[tblExternalStationTransaction].[ExternalStationGuid]
			WHERE [dbo].[tblExternalStationTransaction].[StationTransactionID] LIKE (@TransactionID + '%')
					AND [dbo].[tblExternalStationTransaction].[SiteGuid] = @SiteGuid
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] >= @BeginDate 
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] <= @EndDate
			ORDER BY [dbo].[tblExternalStationTransaction].[CreatedDate] DESC
		END
		ELSE 
		BEGIN
			SELECT
				[dbo].[tblExternalStationTransaction].[ExternalStationTransactionGuid],
				[dbo].[tblExternalStationTransaction].[ExternalStationGuid],
				[dbo].[tblExternalStationTransaction].[SiteGuid],
				[dbo].[tblExternalStation].[ID] AS ExternalStationID,
				[dbo].[tblExternalStationTransaction].[StationTransactionID],
				[dbo].[tblExternalStationTransaction].[RawTransactionData],
				[dbo].[tblExternalStationTransaction].[CreatedBy],
				[dbo].[tblExternalStationTransaction].[CreatedDate],
				[dbo].[tblExternalStationTransaction].[UpdatedBy],
				[dbo].[tblExternalStationTransaction].[UpdatedDate],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionStatusIndex],
				[dbo].[tblExternalStationTransaction].[LookupExternalStationTransactionFailedStatusIndex]
			FROM [dbo].[tblExternalStationTransaction]
				INNER JOIN [dbo].[tblExternalStation] ON [dbo].[tblExternalStation].[ExternalStationGuid] = [dbo].[tblExternalStationTransaction].[ExternalStationGuid]
			WHERE [dbo].[tblExternalStationTransaction].[SiteGuid] = @SiteGuid
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] >= @BeginDate 
					AND [dbo].[tblExternalStationTransaction].[CreatedDate] <= @EndDate
			ORDER BY [dbo].[tblExternalStationTransaction].[CreatedDate] DESC
		END

	END TRY
	BEGIN CATCH
		DECLARE	@_ErrMessage NVARCHAR(2048)      
			, @_ErrNumber INT           
			, @_ErrProcName NVARCHAR(126)           
			, @_ErrLineNumber INT;      
				      
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)                 
						+ 'Procedure Name: usp_ExternalStationTransactionEnumerate' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
