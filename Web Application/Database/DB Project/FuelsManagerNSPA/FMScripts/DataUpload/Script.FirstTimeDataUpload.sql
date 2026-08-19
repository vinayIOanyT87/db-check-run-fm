/*************************************************'
* Script.FirstTimeDataUpload.sql file
* Use this file for add scripts which insert data into an empty table or brand new table (e.g. new lookup table). This file tests and whether the table is empty in order to insert the data
* For incremental insertions, e.g. adding new records to a lookup table, use the Script.IncrementalDataMaintenance.sql file instead.
**************************************************/

/*
IF (SELECT COUNT(*) FROM <TABLE_NAME_HERE>)=0
BEGIN
	
	<ADD INSERT SCRIPT HERE>

END
*/

IF (SELECT COUNT(*) FROM [lookup].[tblReportApprovalState])=0
BEGIN
	INSERT INTO [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex], [ReportApprovalStateCode], [ReportApprovalStateName], [ReportApprovalStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'Pending', N'Pending', N'28f21542-d9f2-4c77-a016-a4cdfb82313d', N'6/18/2012 1:03:02 PM +00:00', N'Administrator', N'6/18/2012 1:03:02 PM +00:00', N'Adminsitrator')
	INSERT INTO [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex], [ReportApprovalStateCode], [ReportApprovalStateName], [ReportApprovalStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'AccountableApproved', N'AccountableApproved', N'f5aae4aa-afce-4cbf-b62f-39d0a3f995f0', N'6/18/2012 1:03:02 PM +00:00', N'Administrator', N'6/18/2012 1:03:02 PM +00:00', N'Adminsitrator')
	INSERT INTO [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex], [ReportApprovalStateCode], [ReportApprovalStateName], [ReportApprovalStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'AccountableRevoked', N'AccountableRevoked', N'2eacfa14-5117-4899-944b-9db4729289d9', N'6/18/2012 1:03:02 PM +00:00', N'Administrator', N'6/18/2012 1:03:02 PM +00:00', N'Adminsitrator')
	INSERT INTO [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex], [ReportApprovalStateCode], [ReportApprovalStateName], [ReportApprovalStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'ApprovingApproved', N'ApprovingApproved', N'e5666bbf-c2b9-42ff-a835-bb5095d18c65', N'6/18/2012 1:03:02 PM +00:00', N'Administrator', N'6/18/2012 1:03:02 PM +00:00', N'Adminsitrator')
	INSERT INTO [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex], [ReportApprovalStateCode], [ReportApprovalStateName], [ReportApprovalStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'ApprovingDeclined', N'ApprovingDeclined', N'5faf5b00-e686-42a5-b300-d2d2687ce9fb', N'6/18/2012 1:03:02 PM +00:00', N'Administrator', N'6/18/2012 1:03:02 PM +00:00', N'Adminsitrator')
	INSERT INTO [lookup].[tblReportApprovalState] ([ReportApprovalStateIndex], [ReportApprovalStateCode], [ReportApprovalStateName], [ReportApprovalStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'DataChanged', N'DataChanged', N'30f4247a-a271-4ac5-a166-5f2d934bf253', N'6/18/2012 1:03:02 PM +00:00', N'Administrator', N'6/18/2012 1:03:02 PM +00:00', N'Adminsitrator')
END

