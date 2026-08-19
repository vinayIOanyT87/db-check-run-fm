namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Collections;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public abstract class BOLBase : PIDXRecordBase
	{
		#region Private attributes
		private int shipDay;
		private string shippedDate;
		#endregion

		#region Protected attributes
		protected int bolNumber;
		protected ArrayList productArrayList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL base class.
		/// </summary>
		protected BOLBase ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties
		public string ShippedDate
		{
			get { return this.shippedDate; }
			set
			{
				this.shippedDate = value;
				this.shipDay = 0;
			}
		}

		public int BOLNumberDigit
		{
			get { return this.bolNumber; }
			set { this.bolNumber = Math.Abs ( value ); }
		}

		public virtual string BOLNumber
		{
			get
			{
				string outStr = this.bolNumber.ToString ( );
				int length = 9 - outStr.Length;

				for (int count = length; count > 0; count--)
				{
					outStr = "0" + outStr;
				}

				return outStr;
			}
		}

		public int ShipDayDigit
		{
			get { return this.shipDay; }
			set { this.shipDay = Math.Abs ( value ); }
		}

		public string ShipDay
		{
			get
			{
				string outStr = this.shipDay.ToString ( );
				int length = 2 - outStr.Length;

				for (int count = length; count > 0; count--)
				{
					outStr = "0" + outStr;
				}

				return outStr;
			}
		}

		public ArrayList ProductArrayList
		{
			get { return this.productArrayList; }
			set { this.productArrayList = value; }
		}

		#endregion

		#region Protected methods
		/// <summary>
		/// This method validates for the BOL common fields.  It throws
		/// an exception if the validation fails.
		/// </summary>
		protected void ValidateSpecific ( )
		{
			if (this.bolNumber == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_010 );
			}

			if (this.shipDay == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_011 );
			}

			if (( this.shipDay == 0 ) && ( string.IsNullOrEmpty(this.shippedDate) ))
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_031 );
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.bolNumber = -99;
			this.shipDay = -99;
		    this.productArrayList = new ArrayList { Capacity = PIDXConstants.MAX_VERSION_1_BOL_PRODUCTS };
		    this.shippedDate = null;
		}
		#endregion
	}
}