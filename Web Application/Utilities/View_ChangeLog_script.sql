USE [ConsolidatedDB]

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

IF EXISTS (SELECT * FROM sys.views WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[vwChangeLog]'))
	DROP VIEW [dbo].[vwChangeLog]
GO

CREATE VIEW vwChangeLog
AS
	SELECT  cl.ChangeLogID      AS 'Event Counter'
			, cl.DateEvent      AS 'Event Date'
			, cl.USerID         AS 'Login ID'
			, cl.WorkStation    AS 'IP'
			, cl.TableName      AS 'Table Name'
            , cl.RowID          AS 'Row ID'
            , cl.DmlType        AS 'Action Type'
            , cl.ASPSessionID   AS 'ASP Session ID'
            , cl.SPID           AS 'SPID'
            , cl.ClientDomain   AS 'Client Domain'
            , cl.ClientUserName AS 'Client User Name'
            , cl.AppName        AS 'Application Name'
	FROM dbo.tblChangeLog cl LEFT OUTER JOIN sys.dm_exec_sessions es ON cl.SPID = es.session_id 
	WHERE AppName NOT LIKE 'Microsoft SQL Server Management Studio%'		-- Developers running queries. 
	  AND Token <> 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC'					-- Logging in, writing to tblUsers, no token yet.
	  AND ClientIPAddr not in (-1, 42)										-- No ASP.NET SessionID.
	  AND (DateEvent >= es.login_time	OR  es.login_time IS NULL)			-- SPIDs get reused - make sure this is the current SPID. 
	  AND 2 = (SELECT COUNT(*)												-- Must have both columns. 
			   FROM sys.columns
			   WHERE object_id = OBJECT_ID(TableName) 
					AND name IN ('CreatedBy', 'UpdatedBy')) 
GO

SELECT * FROM vwChangeLog
ORDER BY 'Event Date' DESC, IP
