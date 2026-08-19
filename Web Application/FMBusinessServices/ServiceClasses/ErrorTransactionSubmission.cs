using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace FMBusinessServices.ServiceClasses
{
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class ErrorTransactionSubmission : IErrorTransactionSubmission
    {
        private ConsolidatedDAClass consolidatedDA;

        public ErrorTransactionSubmission()
        {
            this.consolidatedDA = new ConsolidatedDAClass();
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, ErrorTransactionSubmissionClass errorTransactionSubmissionClass)
        {
            if (security == null) throw new ArgumentNullException(nameof(security));
            if (errorTransactionSubmissionClass == null) throw new ArgumentNullException(nameof(errorTransactionSubmissionClass));
            using (SqlCommand cmd = new SqlCommand())
            {

                errorTransactionSubmissionClass.AddSQL(cmd);
                var initialResults = consolidatedDA.ExecuteScalar(cmd, security);
                var insertedGuid = initialResults as Guid?;
                errorTransactionSubmissionClass.IdentityGuid = insertedGuid.HasValue ? insertedGuid.Value : Guid.Empty;
                return errorTransactionSubmissionClass.IdentityGuid;
            }
        }

        public IEnumerable<ErrorTransactionSubmissionClass> GetByCustomer(SecurityClass security, Guid customerGuid)
        {
            throw new NotImplementedException();
        }
    }
}