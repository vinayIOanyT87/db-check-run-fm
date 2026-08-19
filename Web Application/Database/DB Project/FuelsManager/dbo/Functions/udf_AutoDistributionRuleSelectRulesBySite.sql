

-- ==================================================================================================================
-- Author:		Daniel Or
-- Updated date:	7/30/2013
-- Description:	Select all rules for the given site
-- ==================================================================================================================
CREATE FUNCTION [dbo].[udf_AutoDistributionRuleSelectRulesBySite] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER
)	
RETURNS @RetTable TABLE
(
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL,
	[RuleID] NVARCHAR(50) NOT NULL,
	[RuleDescription] NVARCHAR(255) NOT NULL,
	[RuleEnabled] BIT NOT NULL,
	[DefaultEOM] BIT NOT NULL,
	[TransactionAliasGuid] UNIQUEIDENTIFIER NOT NULL,
	[DefaultReasonCodeGuid] UNIQUEIDENTIFIER NOT NULL,
	[DefaultNotes] NVARCHAR(1000) NOT NULL,
	[CreatedDate] DATETIMEOFFSET NOT NULL,	
	[CreatedBy] NVARCHAR(50) NOT NULL,	
	[UpdatedDate] DATETIMEOFFSET NOT NULL,	
	[UpdatedBy] [NVARCHAR](50) NOT NULL,	
	[_RowVersion] VARBINARY(8) NOT NULL
)
AS
BEGIN
	INSERT INTO @RetTable
	SELECT 
		MAIN.AutoDistributionRuleGuid, MAIN.SiteGuid, MAIN.RuleID, 
		MAIN.RuleDescription, MAIN.RuleEnabled, MAIN.DefaultEOM, MAIN.TransactionAliasGuid, 
		MAIN.DefaultReasonCodeGuid, MAIN.DefaultNotes, MAIN.CreatedDate, MAIN.CreatedBy, 
		MAIN.UpdatedDate, MAIN.UpdatedBy, MAIN._RowVersion
	FROM 
		[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
		
		INNER JOIN [map].[tblEntityAutoDistributionRuleToSite] MAP WITH (NOLOCK)
		ON MAIN.[AutoDistributionRuleGuid] = MAP.[AutoDistributionRuleGuid]
		
	WHERE
		/* the rule is assigned to the current site */
		MAP.SiteGuid = @SelectedSiteGuid
	ORDER BY
		MAIN.RuleID
	RETURN
END
