using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionFields
{
    internal class InventoryDateTimeFG : InventoryDateFG
    {
        #region Constructor
        public InventoryDateTimeFG() : base() { }
        #endregion // Constructor

        #region Overrides
        public override string FieldID
        {
            get
            {
                return base.FieldID + "Time";
            }
        }
        #endregion // Overrides
    }
}
