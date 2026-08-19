
CREATE FUNCTION [dbo].[udf_VersionDetails]
( )
RETURNS 
	@tblVersionDetails TABLE (
		[ProductVersion]     NVARCHAR (128)  NOT NULL,
		[FullName]           NVARCHAR (128)  NOT NULL,
		[LongName]           NVARCHAR (256)  NOT NULL,
		[Edition]            NVARCHAR (128)  NOT NULL,
		[EditionID]          INT             NOT NULL,
		[EditionIDDesc]      NVARCHAR (128)  NOT NULL,
		[EngineEdition]      NVARCHAR (128)  NOT NULL,
		[ProductLevel]       NVARCHAR (128)  NOT NULL,
		[VersionNumber]      NVARCHAR (16)   NOT NULL,
		[MajorVersionNumber] TINYINT         NOT NULL,
		[YearNumber]         SMALLINT        NOT NULL,
		[CodeName]           NVARCHAR (32)   NOT NULL,
		[DateReleased]       DATETIMEOFFSET(7)   NOT NULL,
		[Comments]           NVARCHAR (4000) NOT NULL)
AS
BEGIN

	-- Get the items from SERVERPROPERTY.
	DECLARE @sProductVersion NVARCHAR(128)
	SELECT  @sProductVersion = CAST(SERVERPROPERTY('ProductVersion') AS NVARCHAR(128))	-- 9.00.4035.00

	DECLARE @sEdition NVARCHAR(128)
	SELECT  @sEdition = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128))

	-- 'EditionID' returns an *unsigned* 32-bit integer, so I corrected their table.
	DECLARE @nEditionID INT

	SELECT  @nEditionID =													-- Work around bug in the MSSQLDriver DetectRdbmsVersion() method.
					CAST(COALESCE(SERVERPROPERTY('EditionID'),
						CASE SERVERPROPERTY('Edition')
							WHEN 'Desktop Edition'									THEN -1253826760	-- 0x B544 1F38
							WHEN 'Express Edition'									THEN -1592396055	-- 0x A115 F6E9
							WHEN 'Standard Edition'									THEN -1534726760	-- 0x F807 B7DF
							WHEN 'Workgroup Edition'								THEN  1333529388	-- 0x 4F7C 0B2C
							WHEN 'Enterprise Edition'								THEN  1804890536	-- 0x 6B94 71A8
							WHEN 'Personal Edition'									THEN  -323382091	-- 0x ECB9 94B5
							WHEN 'Developer Edition'								THEN -2117995310	-- 0x 81C1 F4D2
							WHEN 'Enterprise Evaluation Edition'				THEN   610778273	-- 0x 2467 BCA1
							WHEN 'Windows Embedded SQL'							THEN  1044790755	-- 0x 3E46 3DE3
							WHEN 'Express Edition with Advanced Services'	THEN  -133711905	-- 0x F807 B7DF
							ELSE																  -1				-- Unknown.
						END) AS NVARCHAR(128))

	DECLARE @sEditionIDDesc NVARCHAR(128)
	SELECT @sEditionIDDesc =
						CASE @nEditionID
							WHEN -1253826760 THEN 'Desktop Edition'								-- 0x B544 1F38
							WHEN -1592396055 THEN 'Express Edition'								-- 0x A115 F6E9
							WHEN -1534726760 THEN 'Standard Edition'								-- 0x F807 B7DF
							WHEN  1333529388 THEN 'Workgroup Edition'								-- 0x 4F7C 0B2C
							WHEN  1804890536 THEN 'Enterprise Edition'							-- 0x 6B94 71A8
							WHEN  -323382091 THEN 'Personal Edition'								-- 0x ECB9 94B5
							WHEN -2117995310 THEN 'Developer Edition'								-- 0x 81C1 F4D2
							WHEN   610778273 THEN 'Enterprise Evaluation Edition'				-- 0x 2467 BCA1
							WHEN  1044790755 THEN 'Windows Embedded SQL'							-- 0x 3E46 3DE3
							WHEN  -133711905 THEN 'Express Edition with Advanced Services'	-- 0x F807 B7DF
							ELSE						 'Unknown'
						END

	-- Done with EditionIDDesc.
	DECLARE @sEngineEdition NVARCHAR(128)
	
	SELECT  @sEngineEdition =
					CASE SERVERPROPERTY('EngineEdition')
						WHEN 1 THEN 'Personal or Desktop Engine'		-- SQL Server 2000 only.
						WHEN 2 THEN 'Standard'								-- Returned for Standard and Workgroup.
						WHEN 3 THEN 'Enterprise'							-- Returned for Enterprise, Enterprise Evaluation, and Developer.
						WHEN 4 THEN 'Express'								-- Returned for Express, Express with Advanced Services, and Windows Embedded SQL.
						ELSE			'Unknown'
					END

	DECLARE @sProductLevel NVARCHAR(128)
	SELECT  @sProductLevel = CAST(SERVERPROPERTY('ProductLevel') AS NVARCHAR(128))

	-- Initialize version variables here in case this particular version is not in the list.
	DECLARE @sFullName				NVARCHAR(128)		
	DECLARE @sLongName				NVARCHAR(256)		
	DECLARE @sVersionNumber			NVARCHAR(16)		
	DECLARE @nMajorVersionNumber	TINYINT				
	DECLARE @nYearNumber				SMALLINT				
	DECLARE @sCodeName				NVARCHAR(32)		
	DECLARE @dtDateReleased			DATETIMEOFFSET(7)		
	DECLARE @sComments				NVARCHAR(MAX)		

	-- Be sure we have a default version.
	DECLARE @nDot TINYINT
	SET @nDot = CHARINDEX('.', @sProductVersion)
	
	SET @sLongName					= @@VERSION														-- 'Microsoft SQL Server 2005 - 9.00.4035.00 (Intel X86)   Nov 24 2008 13:01:59   Copyright (c) 1988-2005 Microsoft Corporation  Developer Edition on Windows NT 5.1 (Build 2600: Service Pack 2) '
	SET @sVersionNumber			= SUBSTRING(@sProductVersion, 1, @nDot - 1)
	SET @sFullName					= 'SQL Server ' + @sVersionNumber
	SET @nMajorVersionNumber	= CAST(ISNULL(@sVersionNumber, 0) AS TINYINT)
	SET @nYearNumber				= -1	
	SET @sCodeName					= 'Unknown'
	SET @dtDateReleased			= '1970-01-01'
	SET @sComments					= 'No entry found in VersionDetails function'

	-- Check each @sProductVersion for info from our table.
	IF @sProductVersion = ''
	BEGIN
		SET @sFullName			= ''
		SET @sVersionNumber	= ''
		SET @nYearNumber		= 0
		SET @sCodeName			= ''
		SET @dtDateReleased	= ''
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '1.0'
	BEGIN
		SET @sFullName			= 'SQL Server 1.0'
		SET @sVersionNumber	= '1.0'
		SET @nYearNumber		= 0
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1989-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '4.21'
	BEGIN
		SET @sFullName			= 'SQL Server 4'
		SET @sVersionNumber	= '4'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1993-01-01'
		SET @sComments			= ' Windows NT'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.00.121'
	BEGIN
		SET @sFullName			= 'SQL Server 6 Gold Release'
		SET @sVersionNumber	= '6'
		SET @nYearNumber		= '0'
		SET @sCodeName			= 'SQL95'
		SET @dtDateReleased	= '1995-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.00.124'
	BEGIN
		SET @sFullName			= 'SQL Server 6 SP1'
		SET @sVersionNumber	= '6'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.00.139'
	BEGIN
		SET @sFullName			= 'SQL Server 6 SP2'
		SET @sVersionNumber	= '6'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.00.151'
	BEGIN
		SET @sFullName			= 'SQL Server 6 SP3'
		SET @sVersionNumber	= '6'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.201'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 RTM'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= 'Hydra'
		SET @dtDateReleased	= '1996-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.213'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP1'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.240'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP2'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.252'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP3 - Known bad version'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Known bad version - use 6.50.258'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.258'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP3a'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.259'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP3a'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'For Small Business Server only'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.281'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP4'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Cannot be installed on SBS or Enterprise Edition'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.297'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP4 Site Server 3.0'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Site Server 3.0 Commerce Edition hotfix'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.339'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP5'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Y2K hotfix'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.415'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP5'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Bad release - use SP5a - SQL 7.0 compatibility'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.416'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP5a'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1998-12-24'
		SET @sComments			= 'Readme, fixlist, download'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.464'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP5a'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Hotfix for error 213 on INSERT EXEC'
		GOTO DoInsert
	END
	IF @sProductVersion = '6.50.479'
	BEGIN
		SET @sFullName			= 'SQL Server 6.5 SP5a + update'
		SET @sVersionNumber	= '6.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP5a with post SP5a update'
		GOTO DoInsert
	END
	IF @sProductVersion = '7'
	BEGIN
		SET @sFullName			= 'SQL Server 7 Beta 1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= 'Sphinx'
		SET @dtDateReleased	= '1999-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7'
	BEGIN
		SET @sFullName			= 'SQL Server 7 Beta 2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= 'Plato'
		SET @dtDateReleased	= '1999-01-01'
		SET @sComments			= 'OLAP'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.517'
	BEGIN
		SET @sFullName			= 'SQL Server 7 Beta 3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.583'
	BEGIN
		SET @sFullName			= 'SQL Server 7 RC1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.623'
	BEGIN
		SET @sFullName			= 'SQL Server 7 RTM'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1998-01-01'
		SET @sComments			= 'Gold, no SP'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.643'
	BEGIN
		SET @sFullName			= 'SQL Server 7'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB220156 FIX: SQL Cluster Install Fails When SVS Name Contains Special Characters'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.657'
	BEGIN
		SET @sFullName			= 'SQL Server 7'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB229875 FIX: Unable to Perform Automated Installation of SQL 7.0 Using File Images '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.658'
	BEGIN
		SET @sFullName			= 'SQL Server 7'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB244763 FIX: Access Violation Under High Cursor Stress "Slow Complex View" hotfix.'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.662'
	BEGIN
		SET @sFullName			= 'SQL Server 7'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB232707 FIX: Query with Complex View Hierarchy May Be Slow to Compile '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.677'
	BEGIN
		SET @sFullName			= 'SQL Server 7'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'MSDE from Office 2000 Developer, incorrect registry value, should be 0.623.'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.689'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP1 Beta'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.699 '
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1999-07-31'
		SET @sComments			= '(readme, fixlist, download)'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.722'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= '7.0 SP1 with "DB2 OLEDB" hotfix. FIX: Replication: Problems Mapping Characters to DB2 OLEDB Subscribers '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.745'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB253738 FIX: SQL Server Components that Access the Registry in a Cluster Environment May Cause a Memory Leak '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.770'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB252905 FIX: Slow Compile Time on Complex Joins with Unfiltered Table '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.776'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB258087 FIX: Non-Admin User That Executes Batch While Server Shuts Down May Encounter Retail Assertion '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.835'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2 Beta '
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.839'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2 Unidentified'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.842'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2000-03-20'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.843'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB266766 FIX: Temporary Stored Procedures in SA Owned Databases May Bypass Permission Checks When You Run Stored Procedures '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.857'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB260346 FIX: Transactional Publications with a Filter on Numeric Columns Fail to Replicate Data '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.879'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB281185 FIX: Linked Index Server Query Through OLE DB Provider with OR Clause Reports Error 7349 '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.889'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB243741 FIX: Replication Initialize Method Causes Handle Leak on Failure '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.905'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB274266 FIX: Data Modification Query with a Distinct Subquery on a View May Cause Error 3624 '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.910'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB275901 FIX: SQL RPC That Raises Error Will Mask @@ERROR with Msg 7221 '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.917'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB279180 FIX: Bcp.exe with Long Query String Can Result in Assertion Failure '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.918'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB280380 FIX: Buffer Overflow Exploit Possible with Extended Stored Procedures '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.919'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB282243 FIX: Incorrect Results with Join of Column Converted to Binary '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.921'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB283837 FIX: SQL Server May Generate Nested Query For Linked Server When Option Is Disabled '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.961'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2000-12-15'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.970'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB282243 FIX: Incorrect Results with Join of Column Converted to Binary '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.977'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB284351 FIX: SQL Server Profiler and SQL Server Agent Alerts May Fail to Work After Installing SQL Server 7.0 SP3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.978'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB285870 FIX: Update With Self Join May Update Incorrect Number Of Rows '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.996'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB299717 FIX: Query Method Used to Access Data May Allow Rights that the Login Might Not Normally Have '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1004'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB304851 FIX: SQL Server Text Formatting Functions Contain Unchecked Buffers '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1026'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB319851 FIX: Assertion and Error Message 3314 Occurs If You Try to Roll Back a Text Operation with READ UNCOMMITTED '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1033'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB324469 FIX: Error message 9004 may occur when you restore a log that does not contain any transactions '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1063'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1077'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB316333 SQL Server 2000 Security Update for Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1078'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB327068 INF: SQL Server 7.0 Security Update for Service Pack 4'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1079'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB329499 FIX: Replication Removed from Database After Restore WITH RECOVERY '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1087'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814693 FIX: SQL Server 7.0 Scheduler May Periodically Stop Responding During Large Sort Operation '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1092'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB820788 FIX: Delayed domain authentication may cause SQL Server to stop responding '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1094'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB821279 MS03-031: Security patch for SQL Server 7.0 Service Pack 4'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1097'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB822756 A Complex UPDATE Statement That Uses an Index Spool Operation May Cause an Assertion '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1143'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB829015 FIX: An attention signal that is sent from a SQL Server client application because of a query time-out may cause the SQL Server service to quit unexpectedly '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1149'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP4'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB867763 FIX: An access violation exception may occur when you run a SELECT statement that contains complex JOIN operations in SQL Server 7.0 '
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1295'
	BEGIN
		SET @sFullName			= 'SQL Server 7 OLAP SP1'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1325'
	BEGIN
		SET @sFullName			= 'SQL Server 7 OLAP SP2 Beta'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1458'
	BEGIN
		SET @sFullName			= 'SQL Server 7 OLAP SP2'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.1508'
	BEGIN
		SET @sFullName			= 'SQL Server 7 OLAP SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.10040'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP3 with "Text Formatting Unchecked Buffer" hotfix. (KB now includes 1020 instead)'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.10200'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP3 with "Text Formatting Unchecked Buffer" hotfix.'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.00.10210'
	BEGIN
		SET @sFullName			= 'SQL Server 7 SP3'
		SET @sVersionNumber	= '7'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP3 with "Security Update (RDS + Text)" hotfix.'
		GOTO DoInsert
	END
	IF @sProductVersion = '7.50.198'
	BEGIN
		SET @sFullName			= 'SQL Server 7.5 / 2000 Beta 1'
		SET @sVersionNumber	= '7.5'
		SET @nYearNumber		= '0'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.047'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 EAP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.078'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 EAP5'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.100'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 Beta 2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.190'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 Gold'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.194'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 64 bit RTM'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= 'Liberty'
		SET @dtDateReleased	= '2003-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.194'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 32 bit RTM'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= 'Shilol'
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.204'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 '
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB274329 FIX: Optimizer Slow to Generate Query Plan for Complex Queries that have Many Joins and Semi-Joins '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.205'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB274330 FIX: Sending Open Files as Attachment in SQL Mail Fails with Error 18025 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.210'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB275900 FIX: Linked Server Query with Hyphen in LIKE Clause May Run Slowly '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.211'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB276329 FIX: Complex Distinct or Group By Query Can Return Unexpected Results with Parallel Execution Plan '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.217'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB279293 FIX: CASE Using LIKE with Empty String Can Result in Access Violation or Abnormal Server Shutdown '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.218'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB279183 FIX: Scripting Object with Several Extended Properties May Cause Exception '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.222'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB281769 FIX: Exception Access Violation Encountered During Query Normalization '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.223'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB280380 FIX: Buffer Overflow Exploit Possible with Extended Stored Procedures '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.225'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB281663 "Access Denied" Error Message When You Try to Use a Network Drive to Modify Windows 2000 Permissions '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.226'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB278239 FIX: Extreme Memory Usage When Adding Many Security Roles '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.231'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB282279 FIX: Execution of sp_OACreate on COM Object Without Type Information Causes Server Shut Down '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.233'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB282416 FIX: Opening the Database Folder in SQL Server Enterprise Manager 2000 Takes a Long Time '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.239'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB285290 FIX: Complex ANSI Join Query with Distributed Queries May Cause Handled Access Violation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.249'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB288122 FIX: Lock Monitor Uses Excessive CPU '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.251'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB300194 FIX: Error 644 Using Two Indexes on a Column with Uppercase Preference Sort Order '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.287'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB297209 FIX: Deletes, Updates and Rank Based Selects May Cause Deadlock of MSSEARCH '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.296'
	BEGIN
		SET @sFullName			= 'SQL Server 2000'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB299717 FIX: Query Method Used to Access Data May Allow Rights that the Login Might Not Normally Have '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.382'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 OLAP SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.384'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2001-07-12'
		SET @sComments			= 'Service Pack 1, (readme, fixlist, download)'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.428'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB304850 FIX: SQL Server Text Formatting Functions Contain Unchecked Buffers '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.443'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB307538 FIX: SQLTrace Start and Stop is Now Reported in Windows NT Event Log for SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.444'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB307655 FIX: Querying Syslockinfo with Large Numbers of Locks May Cause Server to Stop Responding '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.452'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB308547 FIX: SELECT DISTINCT from Table with LEFT JOIN of View Causes Error Messages or Client Application May Stop Responding '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.469'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB313005 FIX: SELECT from Computed Column That References UDF Causes SQL Server to Terminate '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.471'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB313302 FIX: Shared Table Lock Is Not Released After Lock Escalation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.473'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB314003 FIX: Query That Uses DESC Index May Result in Access Violation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.474'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB315395 FIX: COM May Not Be Uninitialized for Worker Thread When You Use sp_OA '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.475'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP1+1/29 fix'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.532'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP1'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'OLAP SP2'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.534'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2001-11-30'
		SET @sComments			= 'Service Pack 2, (readme, fixlist, download) '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.552'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB313005 FIX: SELECT from Computed Column That References UDF Causes SQL Server to Terminate '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.558'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB314003 FIX: Query That Uses DESC Index May Result in Access Violation ; KB315395 FIX: COM May Not Be Uninitialized for Worker Thread When You Use sp_OA '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.561'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP2+1/29 fix'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.578'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB317979 FIX: Unchecked Buffer May Occur When You Connect to Remote Data Source ; KB318045 FIX: SELECT with Timestamp Column That Uses FOR XML AUTO May Fail with Stack Overflow or AV '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.584'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB318530 FIX: Reorder outer joins with filter criteria before non-selective joins and outer joins '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.594'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB319477 FIX: Extremely Large Number of User Tables on AWE System May Cause BPool::Map Errors '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.599'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB319869 FIX: Improved SQL Manager Robustness for Odd Length Buffer '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.604'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP2+3/29 fix'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.608'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB319507 FIX: SQL Extended Procedure Functions Contain Unchecked Buffers '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.644'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB324186 FIX: Slow Compile Time and Execution Time with Query That Contains Aggregates and Subqueries '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.650'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB322853 FIX: SQL Server Grants Unnecessary Permissions or an Encryption Function Contains Unchecked Buffers '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.652'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810010 FIX: The fn_get_sql System Table Function May Cause Various Handled Access Violations '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.655'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP2+7/24 fix'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.661'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB326999 FIX: Lock escalation on a scan while an update query is running causes a 1203 error message to occur '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.665'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP2+8/8 fix'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.667'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SP2+8/14 fix'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.678'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB328354 FIX: A RESTORE DATABASE WITH RECOVERY Statement Can Fail with Error 9003 or Error 9004 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.679'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB316333 SQL Server 2000 Security Update for Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.682'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB319851 FIX: Assertion and Error Message 3314 Occurs If You Try to Roll Back a Text Operation with READ UNCOMMITTED '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.686'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB316333 SQL Server 2000 Security Update for Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.688'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB329487 FIX: Transaction Log Restore Fails with Message 3456 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.689'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB329499 FIX: Replication Removed from Database After Restore WITH RECOVERY '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.690'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB311104 FIX: The SELECT Statement with Parallelism Enabled May Cause an Assertion '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.693'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB330212 FIX: Parallel logical operation returns results that are not consistent '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.695'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB331965 FIX: The xp_readmail Extended Stored Procedure Overwrites Attachment That Already Exists '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.695'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB331968 FIX: The xp_readmail and xp_findnextmsg Extended Stored Procedures Do Not Read Mail in Time Received Order ; KB331885 FIX: Update/Delete Statement Fails with Error 1203 During Page Lock Escalation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.696'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810010 FIX: The fn_get_sql System Table Function May Cause Various Handled Access Violations ; KB810052 FIX: A Memory Leak Occurs When Cursors Are Opened During a Connection '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.700'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810072 FIX: Merge Replication Reconciler Stack Overflow '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.701'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810026 FIX: A DELETE Statement with a Self-Join May Fail and You Receive a 625 Error ; KB810163 FIX: An Access Violation Occurs if an sp_cursoropen Call References a Parameter That Is Not Defined '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.702'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB328551 FIX: Concurrency enhancements for the tempdb database '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.703'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810526 FIX: Cursors That Have a Long Lifetime May Cause Memory Fragmentation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.705'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810920 FIX: The JOIN queries in the triggers that involve the inserted table or the deleted table may return results that are not consistent '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.710'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB811052 FIX: Latch Time-Out Message 845 Occurs When You Perform a Database or File SHRINK Operation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.713'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB811205 FIX: An error message occurs when you perform a database or a file SHRINK operation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.714'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB811478 FIX: Restoring a SQL Server 7.0 database backup in SQL Server 2000 Service Pack 2 (SP2) may cause an assertion error in the Xdes.cpp file '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.715'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB810688 FIX: Merge Agent Can Resend Changes for Filtered Publications ; KB811611 FIX: Reinitialized SQL Server CE 2.0 subscribers may experience data loss and non-convergence '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.718'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB811703 FIX: Unexpected results from partial aggregations based on conversions ; KB812250 FIX: Indexed View May Cause a Handled Access Violation in CIndex::SetLevel1Names '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.721'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB812393 FIX: Update or Delete Statement Fails with Error 1203 During Row Lock Escalation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.723'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB812798 FIX: A UNION ALL View May Not Use Index If Partitions Are Removed at Compile Time '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.725'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB812995 FIX: A Query with an Aggregate Function May Fail with a 3628 Error ; KB813494 FIX: Distribution Agent Fails with "Violation of Primary Key Constraint" Error Message '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.728'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814460 FIX: Merge Replication with Alternate Synchronization Partners May Not Succeed After You Change the Retention Period '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.730'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB813769 FIX: You May Experience Slow Performance When You Debug a SQL Server Service '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.733'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB813759 FIX: A Large Number of NULL Values in Join Columns Result in Slow Query Performance '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.735'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814889 FIX: A DELETE statement with a JOIN might fail and you receive a 625 error '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.736'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB816937 FIX: A memory leak may occur when you use the sp_OAMethod stored procedure to call a method of a COM object '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.741'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB818096 FIX: Many Extent Lock Time-outs May Occur During Extent Allocation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.743'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP2'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB818406 FIX: A Transact-SQL query that uses views may fail unexpectedly in SQL Server 2000 SP2 ; KB818763 FIX: Intense SQL Server Activity Results in Spinloop Wait in SQL Server 2000 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.760'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.762'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814032 FIX: Merge publications cannot synchronize on SQL Server 2000 Service Pack 3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.763'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814113 FIX: DTS Designer may generate an access violation after you install SQL Server 2000 Service Pack 3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.765'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.769'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814889 FIX: A DELETE statement with a JOIN might fail and you receive a 625 error ; KB814893 FIX: Error Message: "Insufficient key column information for updating" Occurs in SQL Server 2000 SP3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.775'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB815115 FIX: A DTS package that uses global variables ignores an error message raised by RAISERROR '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.776'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Unidentified'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.779'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB814035 FIX: A Full-Text Population Fails After You Apply SQL Server 2000 Service Pack 3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.780'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.781'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB815057 FIX: SQL Server 2000 Uninstall Option Does Not Remove All Files '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.788'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB816985 FIX: You cannot install SQL Server 2000 SP3 on the Korean version of SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.789'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB816840 FIX: Error 17883 May Display Message Text That Is Not Correct '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.790'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB817081 FIX: You receive an error message when you use the SQL-DMO BulkCopy object to import data into a SQL Server table '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.791'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB815249 FIX: Performance of a query that is run from a client program on a SQL Server SP3 database is slow after you restart the instance of SQL Server '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.794'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.798'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB817464 FIX: Using Sp_executesql in Merge Agent Operations '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.800'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.801'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB818540 FIX: SQL Server Enterprise Manager unexpectedly quits when you modify a DTS package '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.804'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB818729 FIX: Internal Query Processor Error 8623 When Microsoft SQL Server Tries to Compile a Plan for a Complex Query '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.807'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB818899 FIX: Error Message 3628 May Occur When You Run a Complex Query '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.811'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.814'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB819662 FIX: Distribution Cleanup Agent Incorrectly Cleans Up Entries for Anonymous Subscribers '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.816'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB818766 FIX: Intense SQL Server activity results in spinloop wait '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.818'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.819'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826161 FIX: You are prompted for password confirmation after you change a standard SQL Server login '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.837'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.839'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB823877 FIX: An Access Violation May Occur When You Run a Query That Contains 32,000 or More OR Clauses ; KB824027 FIX: A Cursor with a Large Object Parameter May Cause an Access Violation on CStmtCond::XretExecute '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.840'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB319477 FIX: Extremely Large Number of User Tables on AWE System May Cause BPool::Map Errors '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.841'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB825225 FIX: You receive an error message when you run a parallel query that uses an aggregation function or the GROUP BY clause '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.842'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB825043 FIX: Rows are unexpectedly deleted when you run a distributed query to delete or to update a linked server table '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.844'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826080 FIX: SQL Server 2000 protocol encryption applies to JDBC clients '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.845'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826364 FIX: A Query with a LIKE Comparison Results in a Non-Optimal Query Plan When You Use a Hungarian SQL Server Collation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.845'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB825854 FIX: No Exclusive Locks May Be Taken If the DisAllowsPageLocks Value Is SET to True '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.847'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826433 PRB: Additional SQL Server Diagnostics Added to Detect Unreported I/O Problems '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.848'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826822 FIX: A Member of the db_accessadmin Fixed Database Role Can Create an Alias for the dbo Special User '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.850'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826815 FIX: You receive an 8623 error message in SQL Server when you try to run a query that has multiple correlated subqueries '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.850'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826860 FIX: Linked Server Query May Return NULL If It Is Performed Through a Keyset Cursor ; KB826906 FIX: A query that uses a view that contains a correlated subquery and an aggregate runs slowly '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.851'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB826754 FIX: A Deadlock Occurs If You Run an Explicit UPDATE STATISTICS Command '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.852'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.854'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB828699 FIX: An Access Violation Occurs When You Run DBCC UPDATEUSAGE on a Database That Has Many Objects '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.856'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB828096 FIX: Key Locks Are Held Until the End of the Statement for Rows That Do Not Pass Filter Criteria '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.857'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.858'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB828637 FIX: Users Can Control the Compensating Change Process in Merge Replication '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.859'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB821334 FIX: Issues that are resolved in SQL Server 2000 build 8.00.0859 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.863'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB829205 FIX: Query performance may be slow and may be inconsistent when you run a query while another query that contains an IN operator with many values is compiled ; KB829444 FIX: A floating point exception occurs during the optimization of a query '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.865'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.866'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB830366 FIX: An access violation occurs in SQL Server 2000 when a high volume of local shared memory connections occur after you install security update MS03-031 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.869'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB830588 FIX: Access violation when you trace keyset-driven cursors by using SQL Profiler '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.870'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB830262 FIX: Unconditional Update May Not Hold Key Locks on New Key Values '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.871'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.873'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB830887 FIX: Some queries that have a left outer join and an IS NULL filter run slower after you install SQL Server 2000 post-SP3 hotfix '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.876'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.878'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB831950 FIX: You receive error message 3456 when you try to apply a transaction log to a server '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.879'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB832977 FIX: The DBCC PSS Command may cause access violations and 17805 errors in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.891'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB836141 FIX: An access violation exception may occur when SQL Server runs many parallel query processing operations on a multiprocessor computer '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.892'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB833710 FIX: You receive an error message when you try to restore a database backup that spans multiple devices '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.904'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB834453 FIX: The Snapshot Agent may fail after you make schema changes to the underlying tables of a publication '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.908'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB834290 FIX: You receive a 644 error message when you run an UPDATE statement and the isolation level is set to READ UNCOMMITTED '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.910'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB834798 FIX: SQL Server 2000 may not start if many users try to log in to SQL Server when SQL Server is trying to start '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.911'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB837957 FIX: When you use Transact-SQL cursor variables to perform operations that have large iterations, memory leaks may occur in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.913'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB836651 FIX: You receive query results that were not expected when you use both ANSI joins and non-ANSI joins '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.915'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB837401 FIX: Rows are not successfully inserted into a table when you use the BULK INSERT command to insert rows '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.916'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB317989 FIX: Sqlakw32.dll May Corrupt SQL Statements '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.919'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB837957 FIX: When you use Transact-SQL cursor variables to perform operations that have large iterations, memory leaks may occur in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.922'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB837970 FIX: You may receive an "Invalid object name..." error message when you run the DBCC CHECKCONSTRAINTS Transact-SQL statement on a table in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.923'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB838460 FIX: The xp_logininfo procedure may fail with error 8198 after you install Q825042 or any hotfix with SQL Server 8.00.0840 or later '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.926'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB839523 FIX: An access violation exception may occur when you update a text column by using a stored procedure in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.927'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB839688 FIX: Profiler RPC events truncate parameters that have a text data type to 16 characters '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.928'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB839589 FIX: The thread priority is raised for some threads in a parallel query '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.929'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB839529 FIX: 8621 error conditions may cause SQL Server 2000 64-bit to close unexpectedly '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.933'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB840856 FIX: The MSSQLServer service exits unexpectedly in SQL Server 2000 Service Pack 3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.934'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB841404 FIX: You may receive a "The query processor could not produce a query plan" error message in SQL Server when you run a query that includes multiple subqueries that use self-joins '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.935'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB841401 FIX: You may notice incorrect values for the "Active Transactions" counter when you perform multiple transactions on an instance of SQL Server 2000 that is running on an SMP computer '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.936'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB841627 FIX: SQL Server 2000 may underestimate the cardinality of a query expression under certain circumstances '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.937'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB841776 FIX: Additional diagnostics have been added to SQL Server 2000 to detect unreported read operation failures '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.944'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB839280 FIX: SQL debugging does not work in Visual Studio .NET after you install Windows XP Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.948'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB843263 FIX: You may receive an 8623 error message when you try to run a complex query on an instance of SQL Server '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.949'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB843266 FIX: Shared page locks can be held until end of the transaction and can cause blocking or performance problems in SQL Server 2000 Service Pack 3 (SP3) '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.952'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB867878 FIX: The Log Reader Agent may cause 17883 error messages ; KB867879 FIX: Merge replication non-convergence occurs with SQL Server CE subscribers '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.952'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB867880 FIX: Merge Agent may fail with an "Invalid character value for cast specification" error message '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.954'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB843282 FIX: The Osql.exe utility does not run a Transact-SQL script completely if you start the program from a remote session by using a background service and then log off the console session '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.955'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB867798 FIX: The @date_received parameter of the xp_readmail extended stored procedure incorrectly returns the date and the time that an e-mail message is submitted by the sender in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.957'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB870994 FIX: An access violation exception may occur when you run a query that uses index names in the WITH INDEX option to specify an index hint '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.959'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB878500 FIX: An Audit Object Permission event is not produced when you run a TRUNCATE TABLE statement '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.961'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB873446 FIX: An access violation exception may occur when multiple users try to perform data modification operations at the same time that fire triggers that reference a deleted or an inserted table in SQL Server 2000 on a computer that is running SMP '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.962'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB883415 FIX: A user-defined function returns results that are not correct for a query '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.967'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB878501 FIX: You may receive an error message when you run a SET IDENTITY_INSERT ON statement on a table and then try to insert a row into the table in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.970'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB872842 FIX: A CHECKDB statement reports a 2537 corruption error after SQL Server transfers data to a sql_variant column in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.972'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB885290 FIX: An assertion error occurs when you insert data in the same row in a table by using multiple connections to an instance of SQL Server '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.973'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB884554 FIX: A SPID stops responding with a NETWORKIO (0x800) waittype in SQL Server Enterprise Manager when SQL Server tries to process a fragmented TDS network packet '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.977'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB888007 You receive a "The product does not have a prerequisite update installed" error message when you try to install a SQL Server 2000 post-Service Pack 3 hotfix '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.980'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB887974 FIX: A fetch on a dynamic cursor can cause unexpected results in SQL Server 2000 Service Pack 3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.985'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB889239 FIX: Start times in the SQL Profiler are different for the Audit:Login and Audit:Logout Events in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.988'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB889166 FIX: You receive a "Msg 3628" error message when you run an inner join query in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.990'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB890200 FIX: SQL Server 2000 stops listening for new TCP/IP Socket connections unexpectedly after error message 17882 is written to the SQL Server 2000 error log '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.991'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB889314 FIX: Non-convergence may occur in a merge replication topology if the primary connection to the publisher is disconnected '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.993'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.994'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.996'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.997'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB891311 FIX: You cannot create new TCP/IP socket based connections after error messages 17882 and 10055 are written to the Microsoft SQL Server 2000 error log '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1000'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB891585 FIX: Database recovery does not occur, or a user database is marked as suspect in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1001'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB892205 FIX: You may receive a 17883 error message when SQL Server 2000 performs a very large hash operation '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1003'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB892923 FIX: Differential database backups may not contain database changes in the Page Free Space (PFS) pages in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1007'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB893312 FIX: You may receive a "SQL Server could not spawn process_loginread thread" error message, and a memory leak may occur when you cancel a remote query in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1009'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB894257 FIX: You receive an "Incorrect syntax near '')''" error message when you run a script that was generated by SQL-DMO for an Operator object in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1013'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB891866 FIX: The query runs slower than you expected when you try to parse a query in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1014'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1017'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB896425 FIX: The BULK INSERT statement silently skips insert attempts when the data value is NULL and the column is defined as NOT NULL for INT, SMALLINT, and BIGINT data types in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1019'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB897572 FIX: You may receive a memory-related error message when you repeatedly create and destroy an out-of-process COM object within the same batch or stored procedure in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1020'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB896985 FIX: The Subscriber may not be able to upload changes to the Publisher when you incrementally add an article to a publication in SQL Server 2000 SP3 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1021'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB887700 FIX: Server Network Utility may display incorrect protocol properties in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1024'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB898709 FIX: Error message when you use SQL Server 2000: "Time out occurred while waiting for buffer latch type 3" '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1025'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1027'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB900416 FIX: A 17883 error may occur you run a query that uses a hash join in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1029'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB902852 FIX: Error message when you run an UPDATE statement that uses two JOIN hints to update a table in SQL Server 2000: "Internal SQL Server error" '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1034'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB915328 FIX: You may intermittently experience an access violation error when a query is executed in a parallel plan and the execution plan contains either a HASH JOIN operation or a Sort operation in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1035'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917593 FIX: The "Audit Logout" event does not appear in the trace results file when you run a profiler trace against a linked server instance in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1036'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929410 FIX: Error message when you run a full-text query in SQL Server 2000: "Error: 17883, Severity: 1, State: 0" '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1037'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB930484 FIX: CPU utilization may approach 100 percent on a computer that is running SQL Server 2000 after you run the BACKUP DATABASE statement or the BACKUP LOG statement '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.1547'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB899410 FIX: You may experience slow server performance when you start a trace in an instance of SQL Server 2000 that runs on a computer that has more than four processors '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2026'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP3'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SQL Server 2000 ce Pack 4 (SP4) Beta'
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2039'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2040'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB899761 FIX: Not all memory is available when AWE is enabled on a computer that is running a 32-bit version of SQL Server 2000 SP4 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2145'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB836651 FIX: You receive query results that were not expected when you use both ANSI joins and non-ANSI joins ; KB826906 FIX: A query that uses a view that contains a correlated subquery and an aggregate runs slowly '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2147'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB899410 FIX: You may experience slow server performance when you start a trace in an instance of SQL Server 2000 that runs on a computer that has more than four processors '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2148'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2151'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2156'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB906790 FIX: You receive an error message when you try to rebuild the master database after you have installed hotfix builds in SQL Server 2000 SP4 64-bit '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2159'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB907250 FIX: You may experience concurrency issues when you run the DBCC INDEXDEFRAG statement in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2162'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB904660 A cumulative hotfix package is available for SQL Server 2000 Service Pack 4 build 2162 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2166'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB909734 FIX: An error message is logged, and new diagnostics do not capture the thread stack when the SQL Server User Mode Scheduler (UMS) experiences a nonyielding thread in SQL Server 2000 Service Pack 4 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2168'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB907813 FIX: An error occurs when you try to access the Analysis Services performance monitor counter object after you apply Windows Server 2003 SP1 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2171'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB909369 FIX: Automatic checkpoints on some SQL Server 2000 databases do not run as expected '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2172'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB910707 FIX: When you query a view that was created by using the VIEW_METADATA option, an access violation may occur in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2175'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB911678 FIX: No rows may be returned, and you may receive an error message when you try to import SQL Profiler trace files into tables by using the fn_trace_gettable function in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2180'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB913789 FIX: The password that you specify in a BACKUP statement appears in the SQL Server Errorlog file or in the Application event log if the BACKUP statement does not run in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2180'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB913684 FIX: You may receive error messages when you use linked servers in SQL Server 2000 on a 64-bit Itanium processor '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2187'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2189'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2191'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2192'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917606 FIX: You may notice a decrease in performance when you run a query that uses the UNION ALL operator in SQL Server 2000 Service Pack 4 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2194'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2196'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB919165 FIX: A memory leak occurs when you run a remote query by using a linked server in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2197'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2199'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB919221 FIX: SQL Server 2000 may take a long time to complete the synchronization phase when you create a merge publication '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2201'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB920930 FIX: Error message when you try to run a query on a linked server in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2207'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB923344 FIX: A SQL Server 2000 session may be blocked for the whole time that a Snapshot Agent job runs '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2215'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2217'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB924664 FIX: You cannot stop the SQL Server service, or many minidump files and many log files are generated in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2218'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB925297 FIX: The result may be sorted in the wrong order when you run a query that uses the ORDER BY clause to sort a column in a table in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2223'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2226'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2229'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB927186 FIX: Error message when you create a merge replication for tables that have computed columns in SQL Server 2000 Service Pack 4: "The process could not log conflict information" '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2231'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB928079 FIX: The Sqldumper.exe utility cannot generate a filtered SQL Server dump file when you use the Remote Desktop Connection service or Terminal Services to connect to a Windows 2000 Server-based computer in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2232'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB928568 FIX: SQL Server 2000 stops responding when you cancel a query or when a query time-out occurs, and error messages are logged in the SQL Server error log file '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2234'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929440 FIX: Error messages when you try to update table rows or insert table rows into a table in SQL Server 2000: "644" or "2511" '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2236'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB930484 FIX: CPU utilization may approach 100 percent on a computer that is running SQL Server 2000 after you run the BACKUP DATABASE statement or the BACKUP LOG statement '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2238'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB931932 FIX: The merge agent fails intermittently when you use merge replication that uses a custom resolver after you install SQL Server 2000 Service Pack 4 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2242'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929131 FIX: In SQL Server 2000, the synchronization process is slow, and the CPU usage is high on the computer that is configured as the Distributor '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2244'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB934203 FIX: A hotfix for Microsoft SQL Server 2000 Service Pack 4 may not update all the necessary files on an x64-based computer '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2245'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB933573 FIX: You may receive an assertion or database corruption may occur when you use the bcp utility or the "Bulk Insert" Transact-SQL command to import data in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2246'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB935465 An updated version of Sqlvdi.dll is now available for SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2248'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB935950 FIX: The foreign key that you created between two tables does not work after you run the CREATE INDEX statement in SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2249'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB936232 FIX: An access violation may occur when you try to log in to an instance of SQL Server 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2253'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB939317 FIX: The CPU utilization may suddenly increase to 100 percent when there are many connections to an instance of SQL Server 2000 on a computer that has multiple processors '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2265'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB944985 FIX: The data on the publisher does not match the data on the subscriber when you synchronize a SQL Server 2005 Mobile Edition subscriber with a SQL Server 2000 "merge replication" publisher '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2271'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB946584 FIX: The SPACE function always returns one space in SQL Server 2000 if the SPACE function uses a collation that differs from the collation of the current database '
		GOTO DoInsert
	END
	IF @sProductVersion = '8.00.2273'
	BEGIN
		SET @sFullName			= 'SQL Server 2000 SP4'
		SET @sVersionNumber	= '8'
		SET @nYearNumber		= '2000'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB948111 MS08-040: Description of the security update for SQL Server 2000 QFE and MSDE 2000 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1399'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 RTM'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= 'Yukon'
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'RTM'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1406'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB932557 FIX: A script task or a script component may not run correctly when you run an SSIS package in SQL Server 2005 build 1399 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1500'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB910416 FIX: Error message when you run certain queries or certain stored procedures in SQL Server 2005: "A severe error occurred on the current command" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1502'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB915793 FIX: You cannot restore the log backups on the mirror server after you remove database mirroring for the mirror database in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1503'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB911662 FIX: You may receive an access violation error message when you run a SELECT query in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1514'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB912471 FIX: The replication on the server does not work any longer when you manually fail over databases in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1518'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1519'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB913494 FIX: The merge agent does not use a specified custom user update to handle conflicting UPDATE statements in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1528'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1531'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB915918 FIX: The internal deadlock monitor may not detect a deadlock between two or more sessions in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1532'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB916046 FIX: Indexes may grow very large when you insert a row into a table and then update the same row in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1533'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB916086 FIX: Errors may be generated in the tempdb database when you create and then drop many temporary tables in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1534'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB916706 FIX: When you run the "dbcc dbreindex" command or the "alter index" command, some transactions are not replicated to the subscribers in a transactional replication in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1536'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917016 FIX: The monitor server does not monitor all primary servers and secondary servers when you configure log shipping in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1538'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917824 FIX: The SQL Server 2005 SqlCommandBuilder.DeriveParameters method returns an exception when the input parameter is a XML parameter that has an associated XSD from an SQL schema '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1539'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917738 FIX: SQL Server 2005 system performance may be slow when you use a keyset-driven cursor to execute a FETCH statement '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1541'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1545'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917905 FIX: SQL Server 2005 performance may be slower than SQL Server 2000 performance when you use an API server cursor '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1547'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB918276 FIX: You notice additional random trailing character in values when you retrieve the values from a fixed-size character column or a fixed-size binary column of a table in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1550'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB917887 FIX: The value of the automatic growth increment of a database file may be very large in SQL Server 2005 ; KB921106 FIX: You receive an error message when you try to create a differential database backup in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1551'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1554'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB926292 FIX: When you query through a view that uses the ORDER BY clause in SQL Server 2005, the result is still returned in random order '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1558'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB926493 FIX: Error message when you restore a transaction-log backup that is generated in SQL Server 2000 SP4 to an instance of SQL Server 2005: "Msg 3456, Level 16, State 1, Line 1. Could not redo log record" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.1561'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB932556 FIX: A script task or a script component may not run correctly when you run an SSIS package in SQL Server 2005 build 1500 and later builds '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2029'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= '(SP1) Beta '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2040'
	BEGIN
		SET @sFullName			= 'SQL Server 2005'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= '(SP1) CTP 03 2006 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2047'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'Service Pack 1'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2050'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB932555 FIX: A script task or a script component may not run correctly when you run an SSIS package in SQL Server 2005 build 2047 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2153'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB919224 FIX: You may receive an error message when you install the cumulative hotfix package (build 2153) for SQL Server 2005 ; KB918222 Cumulative hotfix package (build 2153) for SQL Server 2005 is available '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2156'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB919611 FIX: The value of the automatic growth increment of a database file may be very large in SQL Server 2005 with Service Pack 1 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2164'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2167'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB920974 FIX: SQL Server 2005 treats an identity column in a view as an ordinary int column when the compatibility level of the database is set to 80 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2174'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB922063 FIX: You may notice a large increase in compile time when you enable trace flags 2389 and 2390 in SQL Server 2005 Service Pack 1 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2175'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2176'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2181'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB923605 FIX: A deadlock occurs and a query never finishes when you run the query on a computer that is running SQL Server 2005 and has multiple processors '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2181'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB923624 FIX: Error message when you run an application against SQL Server 2005 that uses many unique user logins or performs many user login impersonations: "insufficient system memory to run this query" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2187'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB923849 FIX: When you run a query that references a partitioned table in SQL Server 2005, query performance may decrease '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2189'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB925153 FIX: You may receive different date values for each row when you use the getdate function within a case statement in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2190'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB925227 FIX: Error message when you call the SQLTables function against an instance of SQL Server 2005: "Invalid cursor state (0)" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2191'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB925135 FIX: An empty string is replicated as a NULL value when you synchronize a table to a SQL Server 2005 Compact Edition subscriber '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2192'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2194'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB925744 FIX: Error message when you try to use a SQL Server authenticated login to log on to an instance of SQL Server 2005: "Logon error: 18456" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2195'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB926240 FIX: SQL Server 2005 may stop responding when you use the SqlBulkCopy class to import data from another data source '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2196'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2198'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2201'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB927289 FIX: Updates to the SQL Server Mobile subscriber may not be reflected in the SQL Server 2005 merge publication '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2202'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB927643 FIX: Some search results are missing when you perform a full-text search operation on a Windows SharePoint Services 2.0 site after you upgrade to SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2206'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB928537 FIX: The full-text index population for the indexed view is very slow in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2206'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB926493 FIX: Error message when you restore a transaction-log backup that is generated in SQL Server 2000 SP4 to an instance of SQL Server 2005: Msg 3456, Level 16, State 1, Line 1. Could not redo log record" ; '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2206'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB928539 FIX: An access violation is logged in the SQL Server Errorlog file when you run a query that uses a plan guide in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2207'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2208'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929179 FIX: A memory leak may occur every time that you synchronize a SQL Server Mobile subscriber in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2209'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929278 FIX: SQL Server 2005 may not perform histogram amendments when you use trace flags 2389 and 2390 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2211'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB930283 FIX: You receive error 1456 when you add a witness to a database mirroring session and the database name is the same as an existing database mirroring session in SQL Server 2005 ; '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2214'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929240 FIX: I/O requests that are generated by the checkpoint process may cause I/O bottlenecks if the I/O subsystem is not fast enough to sustain the IO requests in SQL Server 2005 ; Could not find stored procedure" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2216'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB931821 FIX: High CPU utilization by SQL Server 2005 may occur when you use NUMA architecture on a computer that has an x64-based version of SQL Server 2005 installed '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2218'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB931843 FIX: SQL Server 2005 does not reclaim the disk space that is allocated to the temporary table if the stored procedure is stopped '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2219'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB932115 FIX: The ghost row clean-up thread does not remove ghost rows on some data files of a database in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2221'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB931593 FIX: A script task or a script component may not run correctly when you run an SSIS package in SQL Server 2005 build 2153 and later builds '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2223'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB932393 FIX: You may experience poor performance after you install SQL Server 2005 Service Pack 1 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2226'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2227'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB933265 FIX: You may receive error 1203 when you run an INSERT statement against a table that has an identity column in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2229'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB935446 FIX: You receive error messages when you use the BULK INSERT statement in SQL Server 2005 to import data in bulk '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2230'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB936179 FIX: Error message when you use SQL Native Client to connect to an instance of a principal server in a database mirroring session: "The connection attempted to fail over to a server that does not have a failover partner" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2231'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB934812 FIX: You cannot bring the SQL Server group online in a cluster environment after you rename the virtual server name of the default instance of SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2232'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB937277 FIX: A memory leak occurs when you use the sp_OAMethod stored procedure to call a method of a COM object in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2233'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2234'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB937343 FIX: SQL Server 2005 stops and then restarts unexpectedly and errors occur in the tempdb database '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2236'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB940286 FIX: A Service Broker endpoint stops passing messages in a database mirroring session of SQL Server 2005 ; '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2237'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB940719 FIX: A memory leak occurs when you call the Initialize method and the Terminate method of the SQLDistribution object in a loop in an application that you develop by using Microsoft ActiveX replication controls in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.2239'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3026'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB929376 FIX: A "17187" error message may be logged in the Errorlog file when an instance of SQL Server 2005 is under a heavy load '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3027'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CTP'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2006-11-01'
		SET @sComments			= 'SQL Server 2005 Service Pack 2 (SP2) - CTP'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3033'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3042.00'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CTP'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2006-12-01'
		SET @sComments			= 'SQL Server 2005 Service Pack 2 (SP2) - CTP'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3042.01'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2a'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-03-05'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3050'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB933508 SQL Server 2005 Service Pack 2 issue: Cleanup tasks run at different intervals than intended '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3054'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB934458 FIX: The Check Database Integrity task and the Execute T-SQL Statement task in a maintenance plan may lose database context in certain circumstances in SQL Server 2005 builds 3042 through 3053 ; Fix to check database in maintenance plans.'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3068.00'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB941203 MS08-040: Vulnerabilities in Microsoft SQL Server could allow elevation of privilege '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3073'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB954606 MS08-052: Description of the security update for GDI+ for SQL Server 2005 Service Pack 2 GDR '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3152'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-03-17'
		SET @sComments			= 'KB933097 Cumulative hotfix package (build 3152) for SQL Server 2005 Service Pack 2 is available '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3153'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 Cumulative Update'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB933564 FIX: A gradual increase in memory consumption for the USERSTORE_TOKENPERM cache store occurs in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3154'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3155'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3156'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB934226 FIX: Error message when you try to use Database Mail to send an e-mail message in SQL Server 2005: "profile name is not valid (Microsoft SQL Server, Error 14607)" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3159'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB934459 FIX: The Check Database Integrity task and the Execute T-SQL Statement task in a maintenance plan may lose database context in certain circumstances in SQL Server 2005 builds 3150 through 3158 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3161'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3166'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3169'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3171'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB937745 FIX: You may receive error messages when you try to log in to an instance of SQL Server 2005 and SQL Server handles many concurrent connections '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3175'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-06-28'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3177'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3178'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB938086 FIX: A SQL Server Agent job fails when you run the SQL Server Agent job in the context of a proxy account in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3179'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB938243 FIX: Error message when you run a full-text query against a catalog in SQL Server 2005: "The execution of a full-text query failed. The content index is corrupt." '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3182'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB940128 FIX: You receive error 8623 when you run a complex query in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3186'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU3'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-08-20'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3194'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB940933 FIX: Some changes from subscribers who use SQL Server 2005 Compact Edition or Web synchronization are not uploaded to the publisher when you use the republishing model in a merge publication in Microsoft SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3200'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU4'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-10-15'
		SET @sComments			= 'KB941450 Cumulative update package 4 (CU4) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3206'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB944677 FIX: Conflicts are not logged when you use the Microsoft SQL Server Subscriber Always Wins Conflict Resolver for an article in a merge replication in Microsoft SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3208'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB944902 FIX: A federated database server stops responding when you run parallel queries on a multiprocessor computer that uses NUMA architecture in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3215'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU5'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-12-17'
		SET @sComments			= 'KB943656 Cumulative update package 5 (CU5) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3221'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3224'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB947463 FIX: A stored procedure cannot finish its execution in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3228'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU6'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-02-18'
		SET @sComments			= 'KB946608 Cumulative update package 6 (CU6) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3230'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB949199 FIX: Error message when you run queries on a database that has the SNAPSHOT isolation level enabled in SQL Server 2005: "Unable to deallocate a kept page" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3231'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3232'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3233'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 - QFE Security Update'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-07-08'
		SET @sComments			= ''
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3239'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU7'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-04-14'
		SET @sComments			= 'KB949095 Cumulative update package 7 (CU7) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3240'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB951204 FIX: An access violation occurs when you update a table through a view by using a cursor in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3244'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB952330 FIX: The Replication Log Reader Agent may fail intermittently when a transactional replication synchronizes data in SQL Server 2005 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3246'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB952233 FIX: All the MDX queries that are running on an instance of SQL Server 2005 Analysis Services are canceled when you start or stop a SQL Server Profiler trace for the instance '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3257'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU8'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-06-16'
		SET @sComments			= 'KB951217 Cumulative update package 8 (CU8) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3259'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB954669 FIX: An ongoing MS DTC transaction is orphaned in SQL Server 2005 ; KB954831 FIX: In SQL Server 2005, the session that runs the TRUNCATE TABLE statement may stop responding, and you cannot end the session '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3260'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB954950 FIX: Error message when you run a distributed query in SQL Server 2005: "OLE DB provider ''SQLNCLI'' for linked server ''<Linked Server>'' returned message ''No transaction is active''" '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3282'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU9'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-08-18'
		SET @sComments			= 'KB953752 Cumulative update package 9 (CU9) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3294'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU10'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-10-20'
		SET @sComments			= 'KB956854 Cumulative update package 10 (CU10) for SQL Server 2005 Service Pack 2 '
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.3301'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP2 CU11'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-12-15'
		SET @sComments			= 'KB958735 Cumulative update package 11 (CU11) for SQL Server 2005 Service Pack 2'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.4028'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP3'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SQL Server 2005 Service Pack 3 (SP3)'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.4035.00'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP3'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-12-16'
		SET @sComments			= 'SQL Server 2005 Service Pack 3 (SP3)'
		GOTO DoInsert
	END
	IF @sProductVersion = '9.00.4207'
	BEGIN
		SET @sFullName			= 'SQL Server 2005 SP3 CU1'
		SET @sVersionNumber	= '9'
		SET @nYearNumber		= '2005'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2008-12-20'
		SET @sComments			= 'KB959195 Cumulative update package 1 (CU1) for SQL Server 2005 Service Pack 3'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1019.17'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 SP3 CTP'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-06-01'
		SET @sComments			= 'SQL Server 2008 CTP'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1049.14'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 SP3 CTP'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '2007-07-01'
		SET @sComments			= 'SQL Server 2008 CTP'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1075.23'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 SP3 CTP'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SQL Server 2008 CTP'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1300.13'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 SP3 CTP'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SQL Server 2008 CTP'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1442.32'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 SP3 RC0'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'SQL Server 2008 RC0'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1600.22'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 RTM CU2'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= 'Katmai'
		SET @dtDateReleased	= '2008-08-06'
		SET @sComments			= 'SQL Server 2008 RTM (MSDN/TechNet subscribers only) '
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1750.0'
	BEGIN
		SET @sFullName			= 'SQL Server 2008'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB956718 FIX: A MERGE statement may not enforce a foreign key constraint when the statement updates a unique key column that is not part of a clustering key that has a single row as the update source in SQL Server 2008 '
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1763.0'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 RTM CU1'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= 'Katmai'
		SET @dtDateReleased	= '2008-09-22'
		SET @sComments			= 'KB956717 Cumulative update package 1 for SQL Server 2008'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1771.0'
	BEGIN
		SET @sFullName			= 'SQL Server 2008'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= ''
		SET @dtDateReleased	= '1970-01-01'
		SET @sComments			= 'KB958611 FIX: You may receive incorrect results when you run a query that references three or more tables in the FROM clause in SQL Server 2008'
		GOTO DoInsert
	END
	IF @sProductVersion = '10.0.1779.0'
	BEGIN
		SET @sFullName			= 'SQL Server 2008 RTM CU2'
		SET @sVersionNumber	= '10'
		SET @nYearNumber		= '2008'
		SET @sCodeName			= 'Katmai'
		SET @dtDateReleased	= '2008-11-17'
		SET @sComments			= 'KB958186 Cumulative update package 2 for SQL Server 2008'
		GOTO DoInsert
	END
	IF @sProductVersion = '11'
	BEGIN
		SET @sFullName			= 'SQL Server 2010'
		SET @sVersionNumber	= '11'
		SET @nYearNumber		= '2010'
		SET @sCodeName			= 'Kilimanjaro'
		SET @dtDateReleased	= '2010-04-01'
		SET @sComments			= 'Names and dates are unknown.'
		GOTO DoInsert
	END

	-- Insert the values into the table.
	DoInsert:

	SET @nMajorVersionNumber = CAST(ISNULL(@sVersionNumber, 0) AS TINYINT)

	INSERT INTO @tblVersionDetails
	(
		ProductVersion,
		FullName,
		LongName,
		Edition,
		EditionID,
		EditionIDDesc,
		EngineEdition,
		ProductLevel,
		VersionNumber,
		MajorVersionNumber,
		YearNumber,
		CodeName,
		DateReleased,
		Comments
	)
	VALUES
	(
		@sProductVersion,
		@sFullName,
		@sLongName,
		@sEdition,
		@nEditionID,
		@sEditionIDDesc,
		@sEngineEdition,
		@sProductLevel,
		@sVersionNumber,
		@nMajorVersionNumber,
		@nYearNumber,
		@sCodeName,
		@dtDateReleased,
		@sComments
	)

	-- Done.  Return the table.
	RETURN
 END