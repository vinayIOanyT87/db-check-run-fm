USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migration_LinkReversalTransactions]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migration_LinkReversalTransactions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migration_LinkReversalTransactions]
GO

CREATE PROCEDURE [dbo].[Migration_LinkReversalTransactions]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	4-11-2010	C. Knight	Exclude Contract and Transfer Destination transactions from wholesale completion.  Bug 13333
 =============================================*/
/*

EXEC Migration_LinkReversalTransactions 2,null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS
Create Table #tempTable
(
	OriginalReverseTransID nvarchar(64) NOT NULL,
	NewReverseTransID nvarchar(64) NOT NULL,
	OriginalTransID nvarchar(64) NOT NULL,
	ConjoinedReverseTransID nvarchar(64) NULL,
	ConjoinedOriginalTransID nvarchar(64) NULL
)

Insert Into #tempTable
Select
tt1.TransID,
NEWID(),
SUBSTRING(tt1.TransID,1,len(tt1.TransID)-1),
tt1.ConjoinTransID,
(Select ConjoinTransID from tblTransactions tt2 where tt2.TransID = SUBSTRING(tt1.TransID,1,len(tt1.TransID)-1))
from ConsolidatedDB.dbo.tblTransactions tt1 where TransID like '%R'

--Set Reversal Conjoined Tranactions
Update ConsolidatedDB.dbo.tblTransactions Set
ConsolidatedDB.dbo.tblTransactions.ReversedTransID = tt.ConjoinedOriginalTransID,
ConsolidatedDB.dbo.tblTransactions.ConjoinTransID = tt.NewReverseTransID,
ConsolidatedDB.dbo.tblTransactions.ReversalType = 'R'
from
#tempTable tt
where tt.ConjoinedReverseTransID = ConsolidatedDB.dbo.tblTransactions.TransID

--Set Reversal Tranactions
Update ConsolidatedDB.dbo.tblTransactions Set 
ConsolidatedDB.dbo.tblTransactions.TransID = tt.NewReverseTransID,
ConsolidatedDB.dbo.tblTransactions.ReversedTransID = tt.OriginalTransID,
ConsolidatedDB.dbo.tblTransactions.ReversalType = 'R'
from
#tempTable tt
where tt.OriginalReverseTransID = ConsolidatedDB.dbo.tblTransactions.TransID

--Set Original Conjoined Tranactions
Update ConsolidatedDB.dbo.tblTransactions Set
ConsolidatedDB.dbo.tblTransactions.ReversedTransID = '',--tt.ConjoinedReverseTransID, (Eric Simmons 4/15/2010 @ 5:17 AM.  Updated to reflect logic of system.  Originals do not have this field set.)
ConsolidatedDB.dbo.tblTransactions.ReversalType = 'O'
from
#tempTable tt
where tt.ConjoinedOriginalTransID = ConsolidatedDB.dbo.tblTransactions.TransID

--Set Original Tranactions
Update ConsolidatedDB.dbo.tblTransactions Set 
ConsolidatedDB.dbo.tblTransactions.ReversedTransID = '',--tt.NewReverseTransID, (Eric Simmons 4/15/2010 @ 5:17 AM.  Updated to reflect logic of system.  Originals do not have this field set.)
ConsolidatedDB.dbo.tblTransactions.ReversalType = 'O'
from
#tempTable tt
where tt.OriginalTransID = ConsolidatedDB.dbo.tblTransactions.TransID

Update ConsolidatedDB.dbo.tblTransactionLineItems Set
GrossQuantity = -1 * GrossQuantity,
NetQuantity = -1 * NetQuantity
from ConsolidatedDB.dbo.tblTransactions tt, #tempTable tt2 where
tt.TransIndex = ConsolidatedDB.dbo.tblTransactionLineItems.TransIndex AND
(tt.TransID = tt2.NewReverseTransID or tt.TransID = tt2.ConjoinedReverseTransID)



--Mark unsent transaction as completed.
Update ConsolidatedDB.dbo.tblTransactionLineItems 
Set TransactionStatus = 0 
from ConsolidatedDB.dbo.tblTransactions tt
where tt.TransIndex = ConsolidatedDB.dbo.tblTransactionLineItems.TransIndex and
tt.Flag05 = 0 and tt.TransactionStatus <> 7 and tt.AliasName not in ('Transfer Destination','Contract')

Update ConsolidatedDB.dbo.tblTransactions
Set TransactionStatus = 0 
where Flag05 = 0 and TransactionStatus <> 7 and AliasName not in ('Transfer Destination','Contract')

--Verify that all Gross and Net Quantity Values are non-null as this causes 
--an invalid cast exception
Update ConsolidatedDB.dbo.tblTransactionLineItems 
Set GrossQuantity = 0 
where GrossQuantity is null

Update ConsolidatedDB.dbo.tblTransactionLineItems 
Set NetQuantity = 0 
where NetQuantity is null

--Eric Simmons (4-13-2010)
--Add linkage between EOM Physicals and EOM Determines to resolve Bug 13298
Create Table #tempLinkDetermineWithPhysicalTable
(
	DetermineTransID nvarchar(64) NOT NULL,
	InventoryDate datetime NOT NULL,
	Product nvarchar(30) NOT NULL,
	PhysicalTransID nvarchar(64) NULL
)

--Scan Transaction Table for all non-deleted EOM Determines
Insert Into #tempLinkDetermineWithPhysicalTable
Select TransID,InventoryDate,Product,NULL from tblTransactions tt,tblTransactionUserData ttud, tblTransactionLineItems ttl where 
tt.TransIndex = ttud.TransIndex and tt.TransIndex = ttl.TransIndex and
AliasName = 'Determine' and
UserData4 = 'OP G/L, In Tolerance (E)' and
isnull(ReversalType,'') = '' and
tt.DeleteFlag = 0

--Update temporary table with linked TransID of matching physicals based on product and inventory date
Update #tempLinkDetermineWithPhysicalTable Set PhysicalTransID = tt.TransID 
from tblTransactions tt, tblTransactionLineItems ttl
where tt.TransIndex = ttl.TransIndex and 
ttl.Product = #tempLinkDetermineWithPhysicalTable.Product and
tt.InventoryDate = #tempLinkDetermineWithPhysicalTable.InventoryDate
and tt.AliasName = 'Physical Inventory'

--Update tblTransaction table to set the LinkedDocumentNumber field of the matching physicals
--with the matching determine TransID's.
Update tblTransactions Set LinkedDocumentNumber = tt.DetermineTransID
from #tempLinkDetermineWithPhysicalTable tt
where PhysicalTransID = TransID
