using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
    /// <summary>
    /// Summary description for OrderQtyListDO.
    /// </summary>
   [Serializable]
   [DataContract]
    public class OrderQtyListDO : DataObject
    {
        //*************************************************************************
        // Member variables
        //*************************************************************************    

        public ArrayList Values;

        //*************************************************************************
        // CTOR
        //*************************************************************************    

        public OrderQtyListDO()
        {
            this.Values = new ArrayList();
        }

        //*************************************************************************
        // Member functions
        //*************************************************************************    

        public void Add( Guid transactionLineItemGuid, double Gross, double Net, double Mass )
        {
            OrderQuantities Quantities = new OrderQuantities();

            Quantities.TransactionLineItemGuid          = transactionLineItemGuid;
            Quantities.AggregateGrossQuantity   = Gross;
            Quantities.AggregateNetQuantity     = Net;
            Quantities.AggregateMassQuantity    = Mass;

            this.Values.Add( Quantities );
        }


        public override string getSelectCommand()
        {
            return null;
        }


        public override string getUpdateCommand()
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

    }


    [System.Serializable]
    public class OrderQuantities
    {
        public Guid TransactionLineItemGuid = Guid.Empty;
        public double AggregateGrossQuantity = 0.0;
        public double AggregateNetQuantity = 0.0;
        public double AggregateMassQuantity = 0.0;
    }

}
