CREATE TABLE [dbo].[tblWeightScaleTypeLookup] 
(
    [TypeIndex] INT NOT NULL,
    [TypeDescription] VARCHAR (200) NOT NULL,
    CONSTRAINT [PK_tblWeightScaleTypeLookup] PRIMARY KEY CLUSTERED ([TypeIndex] ASC)
);
GO


--  TRIGGER [dbo].[trg_Audit_del_tblWeightScaleTypeLookup]
CREATE TRIGGER [dbo].[trg_Audit_del_tblWeightScaleTypeLookup] ON [dbo].[tblWeightScaleTypeLookup] 
AFTER 
  DELETE AS BEGIN 
SET 
  NOCOUNT ON;
-- Verifies whether the trigger is active based on configuration and Audit
-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
IF [Fuelsmanagerdb].[fmaudit].[udf_DisableTriggerByAuditRule]('dbo', 'tblWeightScaleTypeLookup', 'D')= 1 -- Notes: Verify this setting
RETURN 
DECLARE @_AuditEventType CHAR(1), 
@_AuditEventSequence TINYINT, 
@_AuditSessionGUID UNIQUEIDENTIFIER, 
@_AuditSessionTokenID UNIQUEIDENTIFIER, 
@_AuditSiteGUID UNIQUEIDENTIFIER, 
@_AuditGUID UNIQUEIDENTIFIER, 
@_AuditDateTime DATETIMEOFFSET, 
@_UserId NVARCHAR(100), 
@_AuditContext varbinary(128);
SET   @_AuditDateTime = SYSDATETIMEOFFSET();
SET   @_AuditEventType = 'D'; -- For Deletes 
SET   @_AuditEventSequence = 1;
SELECT 
  @_AuditSessionGUID = s.SessionGuid, 
  @_AuditSessionTokenID = s.SessionTokenID, 
  @_AuditSiteGUID = s.SiteGuid, 
  @_UserId = u.UserId, 
  @_AuditContext = s.SynchronizationNodeGuid 
FROM 
  Fuelsmanagerdb.map.tblSessionToSQLProcess m 
  INNER JOIN Fuelsmanagerdb.dbo.tblSessions s ON m.SessionGuid = s.SessionGuid 
  LEFT JOIN Fuelsmanagerdb.dbo.tblUsers u ON u.UserGuid = s.UserGuid 
WHERE 
  m.SqlServerSessionID = @@SPID;
-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
-- Treat the change as a local change so it can be synchronized back to the remote system. 
IF (
  (
    SELECT  trigger_nestlevel()  ) > 1
) BEGIN 
SET 
  @_AuditContext = NULL END -- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
  -- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
  IF (@_AuditContext IS NOT NULL) BEGIN RETURN END IF @_UserId IS NULL 
SET   @_UserId = SUSER_NAME() 
  
  INSERT INTO [Fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleTypeLookup]
  (
[TypeIndex],
[TypeDescription],
[_AuditEventType], 
[_AuditEventSequence], 
[_AuditSiteGuid], 
[_AuditSessionGuid], 
[_AuditUserID], 
[_AuditSessionTokenID], 
[_AuditCreatedDate], 
[_AuditGUID], 
[_AuditContext]
  ) 
SELECT 
d.[TypeIndex],
d.[TypeDescription],
@_AuditEventType, 
@_AuditEventSequence, 
@_AuditSiteGUID, 
@_AuditSessionGUID, 
@_UserId, 
@_AuditSessionTokenID, 
@_AuditDateTime, 
NEWID(), 
@_AuditContext 
FROM 
deleted d 
END
GO


 --  TRIGGER [dbo].[trg_Audit_ins_tblWeightScaleTypeLookup]
CREATE TRIGGER [dbo].[trg_Audit_ins_tblWeightScaleTypeLookup] ON [dbo].[tblWeightScaleTypeLookup] 
AFTER INSERT AS 
BEGIN 
SET 
  NOCOUNT ON;
-- Verifies whether the trigger is active based on configuration and Audit
-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
IF [Fuelsmanagerdb].[fmaudit].[udf_DisableTriggerByAuditRule]('dbo', 'tblWeightScaleTypeLookup', 'D')= 1 -- Notes: Verify this setting
RETURN DECLARE @_AuditEventType CHAR(1), 
@_AuditEventSequence TINYINT, 
@_AuditSessionGUID UNIQUEIDENTIFIER, 
@_AuditSessionTokenID UNIQUEIDENTIFIER, 
@_AuditSiteGUID UNIQUEIDENTIFIER, 
@_AuditGUID UNIQUEIDENTIFIER, 
@_AuditDateTime DATETIMEOFFSET, 
@_UserId NVARCHAR(100), 
@_AuditContext varbinary(128);
SET   @_AuditDateTime = SYSDATETIMEOFFSET();
SET   @_AuditEventType = 'I'; -- For Inserts
SET   @_AuditEventSequence = 1;
SELECT 
  @_AuditSessionGUID = s.SessionGuid, 
  @_AuditSessionTokenID = s.SessionTokenID, 
  @_AuditSiteGUID = s.SiteGuid, 
  @_UserId = u.UserId, 
  @_AuditContext = s.SynchronizationNodeGuid 
FROM 
  Fuelsmanagerdb.map.tblSessionToSQLProcess m 
  INNER JOIN Fuelsmanagerdb.dbo.tblSessions s ON m.SessionGuid = s.SessionGuid 
  LEFT JOIN Fuelsmanagerdb.dbo.tblUsers u ON u.UserGuid = s.UserGuid 
WHERE 
  m.SqlServerSessionID = @@SPID;
-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
-- Treat the change as a local change so it can be synchronized back to the remote system. 
IF (
  (
    SELECT  trigger_nestlevel()  ) > 1
) BEGIN 
SET 
  @_AuditContext = NULL END -- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
  -- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this recori. 
  IF (@_AuditContext IS NOT NULL) BEGIN RETURN END IF @_UserId IS NULL 
SET   @_UserId = SUSER_NAME() 
  
INSERT INTO [Fuelsmanagerdb].[fmaudit].[WeightScaleOPC_tblWeightScaleTypeLookup]
  (
[TypeIndex],
[TypeDescription],
[_AuditEventType]
,[_AuditEventSequence]
,[_AuditSessionGUID]
,[_AuditSessionTokenID]
,[_AuditCreatedDate]
,[_AuditSiteGUID]
,[_AuditGUID]
,[_AuditUserId]
,[_AuditContext]
  ) 
SELECT 
i.[TypeIndex],
i.[TypeDescription],
@_AuditEventType, 
@_AuditEventSequence, 
@_AuditSessionGUID, 
@_AuditSessionTokenID, 
@_AuditDateTime, 
@_AuditSiteGUID, 
NEWID(), 
@_UserId, 
@_AuditContext 
FROM 
inserted i END
GO



-- TRIGGER [dbo].[trg_Audit_upd_tblWeightScaleTypeLookup] 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblWeightScaleTypeLookup] ON [dbo].[tblWeightScaleTypeLookup] 
AFTER 
UPDATE 
AS BEGIN 
SET 
  NOCOUNT ON;
-- Verifies whether the trigger is active based on configuration and Audit
-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
IF [FuelsManagerDB].[fmaudit].[udf_DisableTriggerByAuditRule]('dbo', 'tblWeightScaleTypeLookup', 'D')= 1 RETURN DECLARE @_AuditEventType CHAR(1), 
@_AuditEventSequence TINYINT, 
@_AuditSessionGUID UNIQUEIDENTIFIER, 
@_AuditSessionTokenID UNIQUEIDENTIFIER, 
@_AuditSiteGUID UNIQUEIDENTIFIER, 
@_AuditGUID UNIQUEIDENTIFIER, 
@_AuditDateTime DATETIMEOFFSET, 
@_UserId NVARCHAR(100), 
@_AuditContext varbinary(128);

SET @_AuditDateTime = SYSDATETIMEOFFSET();
SET @_AuditEventType = 'U' -- For Updates 
SET @_AuditEventSequence = 1 

SELECT 
@_AuditSessionGUID = s.SessionGuid, 
@_AuditSessionTokenID = s.SessionTokenID, 
@_AuditSiteGUID = s.SiteGuid, 
@_UserId = u.UserId, 
@_AuditContext = s.SynchronizationNodeGuid 
FROM 
[FuelsManagerDB].map.tblSessionToSQLProcess m 
  INNER JOIN [FuelsManagerDB].dbo.tblSessions s ON m.SessionGuid = s.SessionGuid 
  LEFT JOIN [FuelsManagerDB].dbo.tblUsers u ON u.UserGuid = s.UserGuid 
WHERE 
  m.SqlServerSessionID = @@SPID -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
  -- Treat the change as a local change so it can be synchronized back to the remote system. 
  IF (
    (
      SELECT 
        trigger_nestlevel()
    ) > 1
  ) BEGIN 
SET 
@_AuditContext = NULL END -- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
  -- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
  IF (@_AuditContext IS NOT NULL) BEGIN RETURN END IF @_UserId IS NULL 
SET 
@_UserId = SUSER_NAME() 

DECLARE @AuditGuidList TABLE (
    [TypeIndex] int NULL, 
    _AuditEventType CHAR(1), 
    _AuditEventSequence TINYINT, 
    _AuditCreatedDate DATETIMEOFFSET, 
    _AuditGUID UNIQUEIDENTIFIER) 

INSERT INTO [FuelsManagerDB].[fmaudit].[WeightScaleOPC_tblWeightScaleTypeLookup]
  (
[TypeIndex],
[TypeDescription],
[_AuditEventType], 
[_AuditEventSequence], 
[_AuditSessionGUID], 
[_AuditSessionTokenID], 
[_AuditCreatedDate], 
[_AuditSiteGUID], 
[_AuditGUID], 
[_AuditUserId], 
[_AuditContext]
  ) 
  OUTPUT inserted.[TypeIndex] AS 'TypeIndex', 
  inserted._AuditEventType AS '_AuditEventType', 
  inserted._AuditEventSequence AS '_AuditEventSequence', 
  inserted._AuditCreatedDate AS '_AuditCreatedDate', 
  inserted._AuditGUID AS '_AuditGUID' INTO @AuditGuidList 
SELECT 
d.[TypeIndex],
d.[TypeDescription],
@_AuditEventType, 
@_AuditEventSequence, 
@_AuditSessionGUID, 
@_AuditSessionTokenID, 
@_AuditDateTime, 
@_AuditSiteGUID, 
NEWID(), 
@_UserId, 
@_AuditContext 
FROM 
deleted d 
  
INSERT INTO [FuelsManagerDB].[fmaudit].[WeightScaleOPC_tblWeightScaleTypeLookup]
(
[TypeIndex],
[TypeDescription],
[_AuditEventType], 
[_AuditEventSequence], 
[_AuditSessionGUID], 
[_AuditSessionTokenID], 
[_AuditCreatedDate], 
[_AuditSiteGUID], 
[_AuditGUID], 
[_AuditUserId], 
[_AuditContext]
  ) 
SELECT 
i.[TypeIndex],
i.[TypeDescription],
@_AuditEventType, 
2, 
@_AuditSessionGUID, 
@_AuditSessionTokenID, 
@_AuditDateTime, 
@_AuditSiteGUID, 
agl._AuditGUID, 
@_UserId, 
@_AuditContext 
FROM 
inserted i 
INNER JOIN @AuditGuidList agl 
ON (
agl.[TypeIndex] = i.[TypeIndex]
  ) 
WHERE 
  agl._AuditEventType = 'U' 
  AND agl._AuditEventSequence = 1 
  AND agl._AuditCreatedDate = @_AuditDatetime END
GO


