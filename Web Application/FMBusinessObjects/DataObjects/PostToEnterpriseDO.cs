using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class PostToEnterpriseDO
    {
        private static string selectorSQL { get; } =
            @"
DECLARE @CompleteStatusIndex int
DECLARE @EnterpriseStatusIndex int
SELECT @CompleteStatusIndex = ts.TransactionStatusIndex from lookup.tblTransactionStatus ts where ts.TransactionStatusCode = 'Completed'
SELECT @EnterpriseStatusIndex = ts.TransactionStatusIndex from lookup.tblTransactionStatus ts where ts.TransactionStatusCode = 'Enterprise'
DECLARE @ClosedOutProducts TABLE (ProductName NVARCHAR(300), ProductGuid UNIQUEIDENTIFIER, CloseoutDate Date)
INSERT INTO @ClosedOutProducts
SELECT cls.ProductName, cls.ProductGuid, cls.CloseoutDate FROM (
SELECT *, ROW_NUMBER() OVER (PARTITION BY productguid ORDER BY closeoutdate desc) AS DateID
FROM tblCloseoutInventory WITH (NOLOCK)
WHERE ManagerCompanyGuid = @ManagerGuid
) AS cls WHERE cls.DateID = 1
DECLARE @ToUpdate TABLE (TransactionGuid UNIQUEIDENTIFIER)
INSERT @ToUpdate
SELECT t.TransactionGuid
FROM tblTransactions t WITH (NOLOCK)
JOIN tblTransactionLineItems tli WITH (NOLOCK) ON t.TransactionGuid = tli.TransactionGuid
LEFT JOIN @ClosedOutProducts cp ON tli.ProductGuid = cp.ProductGuid
WHERE t.ManagerCompanyGuid = @ManagerGuid
AND t.InventoryDate <= @Stop
AND t.InventoryDate >= @Start
AND t.LookupTransactionStatusIndex = @CompleteStatusIndex
AND tli.ProductGuid = @ProductGuid
AND (t.InventoryDate > cp.CloseoutDate OR cp.CloseoutDate IS NULL)
";
        public void GetTransactionCountToUpdateToEnterpriseFromCompleteSQL(
            SqlCommand cmd,
            Guid productGuid,
            Guid managerGuid,
            DateTime end,
            DateTime? start = null)
        {
            cmd.CommandText = selectorSQL +
@"
SELECT COUNT(*) FROM @ToUpdate
";
            AddSQLParameters(cmd, productGuid, managerGuid, end, start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="productGuid"></param>
        /// <param name="managerGuid"></param>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="doneInPeaceMeal">Will only update 1000 entries of the list</param>
        public void PostTransactionToEnterpriseFromCompleteSQL(
            SqlCommand cmd,
            Guid productGuid,
            Guid managerGuid,
            DateTime end,
            DateTime? start = null,
            bool doneInPeaceMeal = false)
        {
            cmd.CommandText = selectorSQL +
                              @"
UPDATE tblTransactions 
	SET LookupTransactionStatusIndex = @EnterpriseStatusIndex
	WHERE TransactionGuid IN (SELECT TransactionGuid FROM @ToUpdate)
SELECT @@ROWCOUNT
UPDATE tblTransactionLineItems 
	SET LookupTransactionStatusIndex = @EnterpriseStatusIndex
	WHERE TransactionGuid IN (SELECT TransactionGuid FROM @ToUpdate)
";
            if (doneInPeaceMeal)
            {
                cmd.CommandText = cmd.CommandText.Replace(
                    "SELECT t.TransactionGuid",
                    "SELECT TOP 1000 t.TransactionGuid");
            }

            AddSQLParameters(cmd, productGuid, managerGuid, end, start);
        }

        private void AddSQLParameters(SqlCommand cmd, Guid productGuid, Guid managerGuid, DateTime end, DateTime? start)
        {
            if (productGuid == Guid.Empty)
            {
                cmd.CommandText = cmd.CommandText.Replace("AND tli.ProductGuid = @ProductGuid", "");
            }
            else
            {
                cmd.Parameters.Add(
                    new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier, 100) { Value = productGuid });
            }

            if (start == null)
            {
                cmd.CommandText = cmd.CommandText.Replace("AND t.InventoryDate >= @Start", "");
            }
            else
            {
                cmd.Parameters.Add(
                    new SqlParameter("@Start", SqlDbType.DateTime, 100) { Value = start });
            }

            cmd.Parameters.AddRange(new[] {
                  new SqlParameter("@ManagerGuid", SqlDbType.UniqueIdentifier, 100) { Value = managerGuid },
                  new SqlParameter("@Stop", SqlDbType.DateTime, 100) { Value = end }
              });
        }
    }
}
