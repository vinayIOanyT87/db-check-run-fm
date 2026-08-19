using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
    public class SaveTransactionsResultDO : DataObject
    {
        [DataMember]
        public List<TransactionValidationResult> Results
        { get; private set; }

        public SaveTransactionsResultDO()
        {
			  Results = new List<TransactionValidationResult>();
        }

        public override string getSelectCommand()
        {
            return null;
        }
        public override string getDeleteCommand()
        {
            return null;
        }
        public override string getInsertCommand()
        {
            return null;
        }
        public override string getUpdateCommand()
        {
            return null;
        }
    }
}
