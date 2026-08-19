using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
    [DataContract]
    [Serializable]
    public class GetTransactionDO : DataObject
    {
        [DataMember]
        public DataSet TransactionDataSet {get; set;}

        #region Overrides
        public override string getDeleteCommand()
        {
            return null;
        }
        public override string getInsertCommand()
        {
            return null;
        }
        public override string getSelectCommand()
        {
            return null;
        }
        public override string getUpdateCommand()
        {
            return null;
        }
        #endregion Overrides
    }
}
