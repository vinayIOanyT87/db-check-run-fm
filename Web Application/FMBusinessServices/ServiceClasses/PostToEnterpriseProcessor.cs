using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FMBusinessServices.ServiceClasses
{
    public class PostToEnterpriseProcessor : FMServiceBase, IPostToEnterpriseProcessor
    {

        public ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="security"></param>
        /// <param name="productGuid">Pass in an empty guid to get all products</param>
        /// <param name="managerGuid"></param>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int TransactionCountToUpdate(SecurityClass security, Guid productGuid, 
            Guid managerGuid, DateTime end, DateTime? start = null)
        {
            var postToEnterpriseDO = new PostToEnterpriseDO();
            using (var cmd = new SqlCommand())
            {
                postToEnterpriseDO.GetTransactionCountToUpdateToEnterpriseFromCompleteSQL(cmd, productGuid, managerGuid, end, start);
                return (int)this.ConsolidatedDA.ExecuteScalar(cmd, security);
            }

        }

        public int PostTransactionsToEnterprise(SecurityClass security, Guid productGuid, 
            Guid managerGuid, DateTime end, DateTime? start = null, bool doneInPeaceMeal = false)
        {
            var postToEnterpriseDO = new PostToEnterpriseDO();
            using (var cmd = new SqlCommand())
            {
                postToEnterpriseDO.PostTransactionToEnterpriseFromCompleteSQL(cmd, productGuid, managerGuid, end, start, doneInPeaceMeal);
                return (int)this.ConsolidatedDA.ExecuteScalar(cmd, security);
            }

        }
    }
}