using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace FMBusinessServices.ServiceClasses
{
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class TransactionAliasFieldPlacementInformation : ITransactionAliasFieldPlacementInformation//, IDependency
    {
        private ConsolidatedDAClass consolidatedDA;

        public TransactionAliasFieldPlacementInformation()
        {
            this.consolidatedDA = new ConsolidatedDAClass();
        }

        private void Validate(TransactionAliasFieldPlacementInformationClass fieldPlacement)
        {
            if (fieldPlacement.TransactionAliasGuid == null)
            {
                throw new InvalidOperationException("Must have transaction alias guid");
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid AddOrUpdate(SecurityClass security, TransactionAliasFieldPlacementInformationClass fieldPlacement)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (fieldPlacement == null) throw new ArgumentNullException(nameof(fieldPlacement));
            Validate(fieldPlacement);

            using (SqlCommand cmd = new SqlCommand())
            {
                fieldPlacement.UpsertSQLByTransactionAliasGuid(cmd);
                consolidatedDA.ExecuteQuery(security, cmd);
                using (SqlCommand queryCmd = new SqlCommand())
                {
                    //lets update the passed in field placement
                    fieldPlacement.SelectSQLByTransactionAliasGuid(queryCmd);
                    var dataSet = consolidatedDA.GetDataSet(queryCmd, security);
                    fieldPlacement.Load(dataSet);
                }
            }
            return fieldPlacement.IdentityGuid;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public TransactionAliasFieldPlacementInformationClass GetByTransactionAlias(SecurityClass security, Guid transactionAliasGuid)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (transactionAliasGuid == null) throw new ArgumentNullException(nameof(transactionAliasGuid));
            var fieldPlacement = new TransactionAliasFieldPlacementInformationClass()
            {
                TransactionAliasGuid = transactionAliasGuid
            };
            using (SqlCommand cmd = new SqlCommand())
            {
                fieldPlacement.SelectSQLByTransactionAliasGuid(cmd);
                var dataSet = consolidatedDA.GetDataSet(cmd, security);
                fieldPlacement.Load(dataSet);
            }
            if (fieldPlacement.IdentityGuid == Guid.Empty)
            {
                return null;
            }
            return fieldPlacement;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Insert(SecurityClass Security, BaseDataObject Object, bool preOperation)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass Security, BaseDataObject Object)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Update(SecurityClass Security, BaseDataObject Object)
        {
            throw new NotImplementedException();
        }
    }
}