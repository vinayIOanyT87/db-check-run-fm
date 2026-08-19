namespace FMBusinessObjects.PIDXTransactions
{
	using System;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	// ReSharper disable once InconsistentNaming
	class BOLBLProduct : BOLProductBase
	{
		#region Private attributes
		private string productCodeType;
		private string additiveCode;
		private double temperature;
		private string temperatureMeasurementType;
		private double gravity;
		private string grossCreditSign;
		private string netCreditSign;
		private double net;
		private string measurementType;
		private int finishedProductBatchID;
		private string componentContractNumber;
		private string subCompanyID;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL BL Product class.
		/// </summary>
		public BOLBLProduct()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>For some reason, the paradigm seems to process this as 6 characters, with leading zeroes
		/// Following the lead, although I don't know if that's the best plan</remarks>
		public string ProductCode
		{
			get { return this.productCode.Substring(this.productCode.Length - 3, 3); }
			set { this.productCode = value.ToUpper().Substring(0, (value.Length < 6) ? value.Length : 6); }
		}

		public string ProductCodeType
		{
			get { return this.productCodeType.PadRight(1, ' '); }
			set { this.productCodeType = value.Substring(0, (value.Length < 1) ? value.Length : 1); }
		}

		public string AdditiveCode
		{
			get { return this.additiveCode.PadRight(20, ' '); }
			set { this.additiveCode = value.Substring(0, (value.Length < 20) ? value.Length : 20); }
		}

		public string GrossCreditSign
		{
			get { return this.grossCreditSign.PadRight(1, ' '); }
			set
			{
				if (value != " "
				&& value != "-")
					throw new PIDXException(PIDXConstants.ERR_MSG_035);

				this.grossCreditSign = value;
			}
		}

		public string NetCreditSign
		{
			get { return this.netCreditSign.PadRight(1, ' '); }
			set
			{
				if (value != " "
				&& value != "-")
					throw new PIDXException(PIDXConstants.ERR_MSG_035);

				this.netCreditSign = value;
			}
		}

		public string Gross => Convert.ToInt32((int)(this.gross * 100)).ToString("D10");

		public double NetDigit
		{
			get { return this.net; }
			set { this.net = value; }
		}

		public string Net => Convert.ToInt32(this.net * 100).ToString("D10");

		public double TemperatureDigit
		{
			get { return this.temperature; }
			set { this.temperature = value; }
		}

		public string Temperature => Convert.ToInt32(this.temperature * 10).ToString("D4");

		public string TemperatureMeasurementType
		{
			get { return this.temperatureMeasurementType.PadRight(1, ' '); }
			set
			{
				if (string.IsNullOrEmpty(value)
				|| (value != "F"
				&& value != "C"))
					throw new PIDXException(PIDXConstants.ERR_MSG_036);

				this.temperatureMeasurementType = value;
			}
		}

		public double GravityDigit
		{
			get { return this.gravity; }
			set { this.gravity = value; }
		}

		// Per PIDX 4.01 specification, Gravity field is exactly four digits,including one implied decimal place only
		public string Gravity => Convert.ToInt32(this.gravity * 10) // adjust for the implied decimal place
													.ToString("D4") // convert to string, padding with leading zeros to ensure at least four places
													.Left(4); // ensure only four places

		public string MeasurementType
		{
			get { return this.measurementType.PadRight(3, ' '); }
			set
			{
				if (string.IsNullOrEmpty(value)
				|| (value != "BBL"
				&& value != "GAL"
				&& value != "LTR"
				&& value != "TON"
				&& value != "LBS"
				&& value != "MTN"
				&& value != "KGS"))
					throw new PIDXException(PIDXConstants.ERR_MSG_036);

				this.measurementType = value;
			}
		}

		public int FinishedProductBatchIDDigit
		{
			get { return this.finishedProductBatchID; }
			set { this.finishedProductBatchID = value; }
		}


		public string FinishedProductBatchID => this.finishedProductBatchID.ToString().PadRight(3, ' ');

		public string ComponentContractNumber
		{
			get { return this.componentContractNumber.PadRight(32, ' '); }
			set { this.componentContractNumber = value.Substring(0, (value.Length < 32) ? value.Length : 32); }
		}

		public string SubCompanyID
		{
			get { return this.subCompanyID.PadRight(9, ' '); }
			set { this.subCompanyID = value.Substring(0, (value.Length < 32) ? value.Length : 32); }
		}

		public string BlendOrAlterationIndicator => this.blendOrAlterationIndicator.ToString("D").PadRight(2, ' ');

		#endregion


		#region Abstract method implementations
		/// <summary>
		/// This method implement that validation for the BOL BB product. It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateProduct()
		{
			this.Validate();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.productCodeType = "F";
			this.additiveCode = "";
			this.grossCreditSign = " ";
			this.netCreditSign = " ";
			this.temperature = 0;
			this.temperatureMeasurementType = "F";
			this.gravity = 0;
			this.measurementType = "GAL";
			this.componentContractNumber = "";
			this.finishedProductBatchID = 1;
			this.subCompanyID = "";
		}
		#endregion

	}
}
