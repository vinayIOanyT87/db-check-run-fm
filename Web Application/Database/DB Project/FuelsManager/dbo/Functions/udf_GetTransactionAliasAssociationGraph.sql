
CREATE FUNCTION [dbo].[udf_GetTransactionAliasAssociationGraph](
	@TransactionAliasGuid uniqueidentifier
)
RETURNS @tblAssociationGraph TABLE
(
	TransactionAliasGuid uniqueidentifier
)
AS
BEGIN
	-- =================================================================================
	-- Author:		George C. Peters Iv
	-- Create Date:	1/30/2013
	-- Description:	Given a TransactionAliasGuid, all associated transaction alias
	--				references are iterated until all unique alias guids have been 
	--				identified.
	--
	--				This also includes any Associations stored in map.tblAssociatedTransactionAliases
	-- =================================================================================
	DECLARE @MatchFound int

	DECLARE @MasterAssociationGraph TABLE
	(
		TransactionAliasGuid uniqueidentifier
	)

	; WITH TransactionAliasMap AS (
		SELECT TransactionAliasGuid
				,AssociatedTransactionAliasGuid
		FROM dbo.tblTransactionAliases
		WHERE AssociatedTransactionAliasGuid IS NOT NULL 
			OR TransactionAliasGuid <> AssociatedTransactionAliasGuid
	),
	Recursive_CTE AS (
		SELECT
				TransactionAliasGuid
				,AssociatedTransactionAliasGuid
				,CONVERT(varchar(3000), TransactionAliasGuid) AS PreviousGuids
			FROM TransactionAliasMap
			WHERE TransactionAliasGuid = @TransactionAliasGuid
		UNION ALL
		SELECT child.TransactionAliasGuid
				,child.AssociatedTransactionAliasGuid
				,CONVERT(varchar(3000), (parent.PreviousGuids + ',' + CONVERT(varchar(128), child.TransactionAliasGuid))) AS PreviousGuids
--				,PATINDEX(CONVERT(varchar(128), child.TransactionAliasGuid), parent.PreviousGuids) 'Terminate'
		FROM Recursive_CTE parent 
			INNER JOIN dbo.tblTransactionAliases child 
				ON child.TransactionAliasGuid = parent.AssociatedTransactionAliasGuid
					AND child.TransactionAliasGuid NOT IN (SELECT * FROM dbo.udf_SplitString(parent.PreviousGuids, ',', 0))
	)
	INSERT INTO @MasterAssociationGraph
		SELECT TransactionAliasGuid
		FROM Recursive_CTE

	; WITH AssociatedTransactionAliasMap AS (
		SELECT ParentTransactionAliasGuid
				,ChildTransactionAliasGuid
		FROM map.tblAssociatedTransactionAliases map
			INNER JOIN dbo.tblTransactionAliases ta
				ON map.ChildTransactionAliasGuid = ta.TransactionAliasGuid
		WHERE map.ParentTransactionAliasGuid = @TransactionAliasGuid
				AND map.ParentTransactionAliasGuid <> map.ChildTransactionAliasGuid
	),Recursive_AssociatedCTE AS (
		SELECT
				ParentTransactionAliasGuid
				,ChildTransactionAliasGuid
				,CONVERT(varchar(3000), ParentTransactionAliasGuid) AS PreviousGuids
			FROM AssociatedTransactionAliasMap
		UNION ALL
		SELECT child.ParentTransactionAliasGuid
				,child.ChildTransactionAliasGuid
				,CONVERT(varchar(3000), (parent.PreviousGuids + ',' + CONVERT(varchar(128), child.ParentTransactionAliasGuid))) AS PreviousGuids
--				,PATINDEX(CONVERT(varchar(128), child.TransactionAliasGuid), parent.PreviousGuids) 'Terminate'
		FROM Recursive_AssociatedCTE parent 
			INNER JOIN map.tblAssociatedTransactionAliases child 
				ON child.ParentTransactionAliasGuid = parent.ChildTransactionAliasGuid
					AND child.ParentTransactionAliasGuid NOT IN (SELECT * FROM dbo.udf_SplitString(parent.PreviousGuids, ',', 0))
	)
	INSERT INTO @MasterAssociationGraph
		SELECT ChildTransactionAliasGuid FROM Recursive_AssociatedCTE

	INSERT INTO @tblAssociationGraph SELECT DISTINCT(TransactionAliasGuid) FROM @MasterAssociationGraph;

	RETURN;
END