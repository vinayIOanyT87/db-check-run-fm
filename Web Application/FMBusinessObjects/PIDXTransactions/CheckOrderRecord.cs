using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FMBusinessObjects.Constants;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.PIDXTransactions;

namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.DataObjects;

    class CheckOrderRecord : PIDXRecordBase
	{

		#region Private attributes
		private int orderNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the check order record class.
		/// </summary>
		public CheckOrderRecord ( )
		{
			this.Initialize ( );
			base.TransactionType = PIDXConstants.CHECK_ORDER_AUTHORIZATION;
		}
		#endregion

		#region Properties
		public int OrderNumberDigit
		{
			get { return this.orderNumber; }
			set { this.orderNumber = Math.Abs ( value ); }
		}

		public string OrderNumber
		{
			get
			{
				string outStr = this.orderNumber.ToString ( );
				int length = 7 - outStr.Length;

				for (int count = length; count > 0; count--)
				{
					outStr = "0" + outStr;
				}

				return outStr;
			}
		}
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the check order record.
		/// </summary>
		/// <returns></returns>
		public override string GetDataRecord (PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			this.ValidateRecord ( );

			string coRecord = "";
			coRecord = coRecord +
					   base.TransactionType +
					   base.SPLCCode +
					   base.TerminalOperator +
					   base.SellerID +
					   base.ConsigneeNumber +
					   base.FinalShipperID +
					   base.CarrierID +
					   base.TruckNumber +
					   this.OrderNumber;
			GenerateCheckBit ( coRecord );
			coRecord += base.CheckDigit;

			return coRecord;
		}

		/// <summary>
		/// This method implement that validation for the check and order record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord ( )
		{
			base.Validate ( );

			if (this.orderNumber == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_009 );
			}

			if (( base.ConsigneeNumber == null ) ||
				( base.ConsigneeNumber.Length > PIDXConstants.CONSIGNEE_ID_LENGTH ) ||
				( base.ConsigneeNumber.Length < 1 ))
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_008 );
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.orderNumber = -99;
		}
		#endregion
	}
}