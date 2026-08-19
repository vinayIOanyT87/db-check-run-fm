namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public class BOLBLRecord : BOLBase
	{
		#region Private attributes
		private int authorizationNumber;
		private int authorizedLoad;
		private int finalShipperTransactionSequence;
		private int bolVersion;
		private DateTimeOffset startLoadDateTime;
		private DateTimeOffset endLoadDateTime;
		private string destinationCounty;
		private string destinationCity;
		private string destinationZipCode;
		private string carrierFein;
		private string vehicleNumber;
		private string containerNumber1;
		private string containerNumber2;
      private string containerNumber3;
		private string purchaseOrderNumber;
		private string orderOrReleaseNumber;
		private string supplierContractNumber;
		private string splitLoadFlag;
		private string shipperInfo;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="BOLBLRecord"/> class. 
		/// This is the default constructor for the BOL build record class.
		/// </summary>
		public BOLBLRecord()
		{
			this.Initialize();
			this.TransactionType = PIDXConstants.BOL_BL;
		}
		#endregion

		#region Properties
		public string TotalPIDXProductsTransmitted => this.ProductArrayList.Count.ToString("D2");

        public string ShipperInfo
		{
			get { return this.shipperInfo.PadRight(10, ' '); }
			set { this.shipperInfo = value.Substring(0, (value.Length < 10) ? value.Length : 10); }
		}

		public string SplitLoadFlag
		{
			get { return this.splitLoadFlag.PadRight(1, ' '); }
			set { this.splitLoadFlag = value.Substring(0, (value.Length < 1) ? value.Length : 1); }
		}

		public string SupplierContractNumber
		{
			get { return this.supplierContractNumber.PadRight(32, ' '); }
			set { this.supplierContractNumber = value.Substring(0, (value.Length < 32) ? value.Length : 32); }
		}

		public string OrderOrReleaseNumber
		{
			get { return this.orderOrReleaseNumber.PadRight(16, ' '); }
			set { this.orderOrReleaseNumber = value.Substring(0, (value.Length < 16) ? value.Length : 16); }
		}

		public string PurchaseOrderNumber
		{
			get { return this.purchaseOrderNumber.PadRight(30, ' '); }
			set { this.purchaseOrderNumber = value.Substring(0, (value.Length < 30) ? value.Length : 30); }
		}

		public string VehicleNumber
		{
			get { return this.vehicleNumber.PadRight(20, ' '); }
			set { this.vehicleNumber = value.Substring(0, (value.Length < 20) ? value.Length : 20); }
		}

		public string ContainerNumber1
		{
			get { return this.containerNumber1.PadRight(20, ' '); }
			set { this.containerNumber1 = value.Substring(0, (value.Length < 20) ? value.Length : 20); }
		}

		public string ContainerNumber2
		{
			get { return this.containerNumber2.PadRight(20, ' '); }
			set { this.containerNumber2 = value.Substring(0, (value.Length < 20) ? value.Length : 20); }
		}

        public string ContainerNumber3
        {
            get { return this.containerNumber3.PadRight(20, ' '); }
            set { this.containerNumber3 = value.Substring(0, (value.Length < 20) ? value.Length : 20); }
        }

		public EQUIPMENT_TYPE VehicleType { get; set; }

		public string VehicleTypeID
		{
			get
			{
				switch (this.VehicleType)
				{
					case EQUIPMENT_TYPE.AIRCRAFT_TYPE:
						return "S";
					case EQUIPMENT_TYPE.BARGE_TYPE:
						return "B";
					case EQUIPMENT_TYPE.FILLSTAND_TYPE:
						return "S";
					case EQUIPMENT_TYPE.HYDRANT_CART_TYPE:
						return "S";
					case EQUIPMENT_TYPE.OTHER_TYPE:
						return "S";
					case EQUIPMENT_TYPE.PIPELINE_TYPE:
						return "S";
					case EQUIPMENT_TYPE.RAILCAR_TYPE:
						return "R";
					case EQUIPMENT_TYPE.SHIP_TYPE:
						return "S";
					case EQUIPMENT_TYPE.STATIONARY_CART_TYPE:
						return "S";
					case EQUIPMENT_TYPE.SYSTEM_TYPE:
						return "S";
					case EQUIPMENT_TYPE.TANK_TYPE:
						return "C";
					case EQUIPMENT_TYPE.TANKER_TYPE:
						return "T";
					case EQUIPMENT_TYPE.TRACTOR_TYPE:
						return "T";
					case EQUIPMENT_TYPE.TRAILER_TYPE:
						return "C";
					default:
						return "S";
				}					
			}
		}
		
        // ReSharper disable once InconsistentNaming
		public string CarrierFEIN
		{
			get { return this.carrierFein.PadRight(10, ' '); }
			set { this.carrierFein = value.Substring(0, (value.Length < 10) ? value.Length : 10); }
		}

		public string DestinationState { get; set; }

		public string DestinationStateAbrev
		{
			get
			{
				switch (this.DestinationState.ToUpper().Trim().TrimEnd('.'))
				{
					case "ALABAMA":
                    case "AL":
						return "AL";

					case "ALASKA":
                    case "AK":
						return "AK";
						
					case "ARIZONA":
                    case "AZ":
						return "AZ";
						
					case "ARKANSAS":
                    case "AR":
						return "AR";
						
					case "CALIFORNIA":
                    case "CA":
						return "CA";
						
					case "COLORADO":
                    case "CO":
						return "CO";
						
					case "CONNECTICUT":
                    case "CT":
						return "CT";
						
					case "DELAWARE":
                    case "DE":
						return "DE";
						
					case "FLORIDA":
                    case "FL":
						return "FL";
						
					case "GEORGIA":
                    case "GA":
						return "GA";
						
					case "HAWAII":
                    case "HI":
						return "HI";
						
					case "IDAHO":
                    case "ID":
						return "ID";
						
					case "ILLINOIS":
                    case "IL":
						return "IL";
						
					case "INDIANA":
                    case "IN":
						return "IN";
						
					case "IOWA":
                    case "IA":
						return "IA";
						
					case "KANSAS":
                    case "KA":
						return "KA";
						
					case "KENTUCKY":
                    case "KY":
						return "KY";
						
					case "LOUISIANA":
                    case "LA":
						return "LA";
						
					case "MAINE":
                    case "ME":
						return "ME";
						
					case "MARYLAND":
                    case "MD":
						return "MD";
						
					case "MASSACHUSETTS":
                    case "MA":
						return "MA";
						
					case "MICHIGAN":
                    case "MI":
						return "MI";
						
					case "MINNESOTA":
                    case "MN":
						return "MN";
						
					case "MISSISSIPPI":
                    case "MS":
						return "MS";
						
					case "MISSOURI":
                    case "MO":
						return "MO";
						
					case "MONTANA":
                    case "MT":
						return "MT";
						
					case "NEBRASKA":
                    case "NE":
						return "NE";
						
					case "NEVADA":
                    case "NV":
						return "NV";
						
					case "NEW HAMPSHIRE":
                    case "NH":
						return "NH";
						
					case "NEW JERSEY":
                    case "NJ":
						return "NJ";
						
					case "NEW MEXICO":
                    case "NM":
						return "NM";
						
					case "NEW YORK":
                    case "NY":
						return "NY";
					
					case "NORTH CAROLINA":
                    case "NC":
						return "NC";
						
					case "NORTH DAKOTA":
                    case "ND":
						return "ND";
					
					case "OHIO":
                    case "OH":
						return "OH";
						
					case "OKLAHOMA":
                    case "OK":
						return "OK";
						
					case "OREGON":
                    case "OR":
						return "OR";
						
					case "PENNSYLVANIA":
                    case "PA":
						return "PA";
						
					case "RHODE ISLAND":
                    case "RI":
						return "RI";
						
					case "SOUTH CAROLINA":
                    case "SC":
						return "SC";
						
					case "SOUTH DAKOTA":
                    case "SD":
						return "SD";
						
					case "TENNESSEE":
                    case "TN":
						return "TN";
						
					case "TEXAS":
                    case "TX":
						return "TX";
						
					case "UTAH":
                    case "UT":
						return "UT";
						
					case "VERMONT":
                    case "VT":
						return "VT";
						
					case "VIRGINIA":
                    case "VA":
						return "VA";
						
					case "WASHINGTON":
                    case "WA":
						return "WA";
					
					case "WEST VIRGINIA":
                    case "WV":
						return "WV";
					
					case "WISCONSIN":
                    case "WI":
						return "WI";
						
					case "WYOMING":
                    case "WY":
						return "WY";
										
					default:
						return "  ";
				}
			}
		}

		public string DestinationCounty
		{
			get { return this.destinationCounty.ToUpper().PadRight(30, ' '); }
			set { this.destinationCounty = value.Substring(0, (value.Length < 30) ? value.Length : 30); }
		}

		public string DestinationCity
		{
			get { return this.destinationCity.ToUpper().PadRight(30, ' '); }
			set { this.destinationCity = value.Substring(0, (value.Length < 30) ? value.Length : 30); }
		}

		public string DestinationZipCode
		{
			get { return this.destinationZipCode.PadRight(9, ' '); }
			
			// The dash in an extended zip code does not get sent to PIDX.  Remove it now, before trimming the length to nine digits.
			set { this.destinationZipCode = value.Replace("-", string.Empty).Substring(0, (value.Length < 9) ? value.Length : 9); }
		}
	
		public DateTimeOffset StartLoadDateTime
		{
			get { return this.startLoadDateTime; }
			set { this.startLoadDateTime = value; }
		}

		public string StartLoadDate => this.startLoadDateTime.ToString("MMddyyyy");

        public string StartLoadTime => this.startLoadDateTime.ToString("HHmm");

        public DateTimeOffset EndLoadDateTime
		{
			get { return this.endLoadDateTime; }
			set { this.endLoadDateTime = value; }
		}

		public string EndLoadDate => this.endLoadDateTime.ToString("MMddyyyy");

        public string EndLoadTime => this.endLoadDateTime.ToString("HHmm");

        // ReSharper disable once InconsistentNaming
		public int BOLVersionDigit
		{
			get { return this.bolVersion; }
			set { this.bolVersion = Math.Abs(value); }
		}

        // ReSharper disable once InconsistentNaming
		public string BOLVersion => this.bolVersion.ToString("D2");

        public int FinalShipperTransactionSequenceDigit
		{
			get { return this.finalShipperTransactionSequence; }
			set { this.finalShipperTransactionSequence = Math.Abs(value); }
		}

		public string FinalShipperTransactionSequence => this.finalShipperTransactionSequence.ToString("D9");

        public int AuthorizationNumberDigit
		{
			get { return this.authorizationNumber; }
			set { this.authorizationNumber = Math.Abs(value); }
		}

		public string AuthorizationNumber
		{
			get
			{
				return this.authorizedLoad == 0 ? this.authorizationNumber.ToString("D8") : "        ";
			}
		}

        // ReSharper disable once InconsistentNaming
		public override string BOLNumber => this.bolNumber.ToString("D16");

        public int AuthorizedLoadDigit
		{
		    get
		    {
		        return this.authorizedLoad;
		    }

		    set
		    {
		        if (value != 0 && value != 1)
		        {
		            throw new Exception("Invalid Authorized Load");
		        }
				
				this.authorizedLoad = value;
			}
		}

		public string AuthorizedLoad => this.authorizedLoad.ToString(CultureInfo.InvariantCulture);

        public override string CarrierID
        {
            get { return this.carrierID.PadLeft(4, '0'); }
            set { this.carrierID = value.Substring(0, (value.Length < 4) ? value.Length : 4); }
        }

        public bool AddBOLProduct(
            string productCode,
            string productCodeType,
            string additiveCode,
				int blendOrAlterationIndicatorDigit,
				double gross,
				string grossCreditSign,
				double net,
				string netCreditSign,
				double temperature,
				string temperatureMeasurementType,
				double gravity,
				string measurementType,
				int finishedProductBatchID,
				string componentContractNumber,
            string subCompanyID)
        {
            bool result = false;

			if (PIDXConstants.MAX_VERSION_4_BOL_PRODUCTS > this.ProductArrayList.Count
			&& !string.IsNullOrEmpty(productCode))
			{
				var blProduct = new BOLBLProduct
				                    {
				                        ProductCode = productCode,
				                        ProductCodeType = productCodeType,
				                        AdditiveCode = additiveCode,
				                        BlendOrAlterationIndicatorDigit = blendOrAlterationIndicatorDigit,
				                        GrossDigit = gross,
				                        GrossCreditSign = grossCreditSign,
				                        NetDigit = net,
				                        NetCreditSign = netCreditSign,
				                        TemperatureDigit = temperature,
				                        TemperatureMeasurementType = temperatureMeasurementType,
				                        GravityDigit = gravity,
				                        MeasurementType = measurementType,
				                        FinishedProductBatchIDDigit = finishedProductBatchID,
				                        ComponentContractNumber = componentContractNumber,
				                        SubCompanyID = subCompanyID
				                    };

			    this.ProductArrayList.Add(blProduct);
				result = true;
			}

			return result;
		}

		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the BOL build record.
		/// </summary>
		/// <param name="version">
		/// PIDX version to create the record for.
		/// </param>
		/// <returns>
		/// The properly formatted record
		/// </returns>
		public override string GetDataRecord(PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			this.ValidateRecord();

			// build header portion of BB
		    string bolRecord = this.TransactionType +
		                       PIDXProfileClass.VersionID(version) +
		                       this.SPLCCode +
		                       this.TerminalOperator +
		                       this.SellerID +
		                       this.FinalShipperID +
		                       this.TerminalControlNumber +
		                       this.BOLNumber +
		                       this.AuthorizationNumber +
		                       this.FinalShipperTransactionSequence +
		                       this.AuthorizedLoad +
		                       this.BOLVersion +
		                       this.StartLoadDate +
		                       this.StartLoadTime +
		                       this.EndLoadDate +
		                       this.EndLoadTime +
		                       this.ConsigneeNumber +
		                       this.DestinationStateAbrev +
		                       this.DestinationCounty +
		                       this.DestinationCity +
		                       this.DestinationZipCode +
		                       this.CarrierID +
		                       this.CarrierFEIN +
		                       this.RackDriverID +
		                       this.VehicleNumber +
		                       this.ContainerNumber1 +
		                       this.ContainerNumber2 +
		                       this.VehicleTypeID +
		                       this.PurchaseOrderNumber +
		                       this.OrderOrReleaseNumber +
		                       this.SupplierContractNumber +
		                       this.SplitLoadFlag +
		                       this.ShipperInfo +
		                       this.TotalPIDXProductsTransmitted;

			// Add each defined bolbbproduct to the BB
		    // ReSharper disable once ForCanBeConvertedToForeach
			for (int count = 0; count < this.ProductArrayList.Count; count++)
			{
				var blproduct = (BOLBLProduct)this.ProductArrayList[count];

				bolRecord +=
				blproduct.ProductCodeType +
				blproduct.ProductCode +
				blproduct.AdditiveCode +
				blproduct.Gross +
				blproduct.GrossCreditSign +
				blproduct.Net +
				blproduct.NetCreditSign +
				blproduct.Temperature +
				blproduct.TemperatureMeasurementType +
				blproduct.Gravity +
				blproduct.BlendOrAlterationIndicator +
				blproduct.MeasurementType +
				blproduct.FinishedProductBatchID +
				blproduct.ComponentContractNumber +
				blproduct.SubCompanyID;
			}

			this.GenerateCheckBit(bolRecord);
			bolRecord += this.CheckDigit;

            // We are only supposed to send either CheckDigit37 or CheckDigit16,
            // not both. We'll send only CheckDigit37
            // Per TDS 4/27/2016, the CheckDigit16, if not used, should be space-filled
			//// bolRecord += CRC16(bolRecord,bolRecord.Length-1);
            bolRecord += "    ";

			return bolRecord;
		}

		/// <summary>
		/// This method implement that validation for the BOL Build record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord()
		{
			this.ValidateSpecific();

			if (this.authorizationNumber == -99)
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_017);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
		    this.authorizationNumber = -99;
		    this.authorizedLoad = 0;
		    this.bolVersion = 0;
		    this.finalShipperTransactionSequence = 1;
		    this.startLoadDateTime = DateTime.Now;
		    this.endLoadDateTime = DateTime.Now;
		    this.DestinationState = string.Empty;
		    this.destinationCounty = string.Empty;
		    this.destinationCity = string.Empty;
		    this.destinationZipCode = string.Empty;
		    this.carrierFein = "000000000U";
		    this.vehicleNumber = string.Empty;
		    this.containerNumber1 = string.Empty;
		    this.containerNumber2 = string.Empty;
		    this.VehicleType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
		    this.purchaseOrderNumber = string.Empty;
		    this.orderOrReleaseNumber = string.Empty;
		    this.supplierContractNumber = string.Empty;
		    this.splitLoadFlag = string.Empty;
		    this.shipperInfo = string.Empty;
		}

        #endregion
	}
}
