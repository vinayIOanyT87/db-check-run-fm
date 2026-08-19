
CREATE PROCEDURE [dbo].[usp_StandardReportsConfig]
@siteGuid UNIQUEIDENTIFIER, @createdDate DATETIMEOFFSET(7)
AS
SET NOCOUNT ON

	INSERT INTO dbo.tblReportGroups
				  (GroupName,SiteGuid,CreatedBy,CreatedDate,OrderNumber)
		  VALUES('IntoPlane Reports',
					@siteGuid,
					'Administrator',
					@createdDate,
					1)
	
	INSERT INTO dbo.tblReportGroups
				  (GroupName,SiteGuid,CreatedBy,CreatedDate,OrderNumber)
		  VALUES('Fuel Farm Reports',
					@siteGuid,
					'Administrator',
					@createdDate,
					2)
	
	DECLARE @grpIndx UNIQUEIDENTIFIER
	
	SELECT @grpIndx = ReportGroupGuid
	  FROM dbo.tblReportGroups
	 WHERE GroupName = 'IntoPlane Reports'
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Into-Plane Daily Summary',
					'Creates an Into-Plane daily report for a day range.',
					'IntoPlane Daily Summary Report',
					'Administrator',
					@createdDate,
					1)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Into-Plane Monthly Summary',
					'Creates an Into-Plane monthly report for a month range.',
					'IntoPlane Monthly Summary Report',
					'Administrator',
					@createdDate,
					2)
	
	SELECT @grpIndx = ReportGroupGuid
	  FROM dbo.tblReportGroups
	 WHERE GroupName = 'Fuel Farm Reports'
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Journal Gross',
					'Creates a Journal Gross report for a month range.',
					'Journal Gross',
					'Administrator',
					@createdDate,
					3)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Journal Net',
					'Creates a Journal Net report for a month range.',
					'Journal Net',
					'Administrator',
					@createdDate,
					4)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Summary Journal Gross',
					'Creates a Journal Summary (Gross) report for a month range.',
					'SUMMARY_GROSS_JOURNAL',
					'Administrator',
					@createdDate,
					5)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Receipt',
					'Creates a Receipt report for a month range.',
					'Receipt',
					'Administrator',
					@createdDate,
					6)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'Stock Transfer',
					'Creates a Stock Transfer report for a month range.',
					'Stock Transfer',
					'Administrator',
					@createdDate,
					7)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'24 Hour',
					'Creates a 24 Hour Report by Site.',
					'24hr',
					'Administrator',
					@createdDate,
					8)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'DOM Fuel by Flight',
					'Creates a Domestic fuel by flight report for a site.',
					'DOM Fuel By Flight',
					'Administrator',
					@createdDate,
					9)
	
	INSERT INTO dbo.tblReportDetails
				  (ReportGroupGuid,SiteGuid,ReportName,ReportDescription,ReportPath,CreatedBy,CreatedDate,OrderNumber)
		  VALUES(@grpIndx,
					@SiteGuid,
					'FTZ Fuel by Flight',
					'Creates a FTZ fuel by flight report for a site.',
					'FTZ Fuel By Flight',
					'Administrator',
					@createdDate,
					10)