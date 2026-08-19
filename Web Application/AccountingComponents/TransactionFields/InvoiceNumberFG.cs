namespace TransactionFields
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class InvoiceNumberFG : DocumentNumberFG
    {
        public InvoiceNumberFG() : base() { }

        public override string FieldID
        {
            get
            {
                return "InvoiceNumberFG";
            }
        }

        public override bool Editable
        {
            get
            {
                return false; // CCP-042 invoice number field is never editable
            }
        }
    }
}
