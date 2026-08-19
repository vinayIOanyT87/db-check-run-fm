using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using System.Text.RegularExpressions;

	using FMBusinessObjects.Exceptions;

	public class ExStarsTransportMode
	{
		public enum EnumStorage{unknown, NA,Primary,Secondary}

		public string IrsModeCode { get; protected set; }

		public string Name { get; protected set; }

		public EnumStorage StorageType { get; protected set; }

		public override string ToString()
		{
			return string.Format("{0}:{1}", Name, StorageType.ToString());
		}

		public ExStarsTransportMode()
		{
			this.Name = "unknown";
			IrsModeCode = "J ";
			StorageType = EnumStorage.unknown;
		}

		protected const string Barge = "BARGE";
		protected const string Ship = "SHIP";


		/// <summary>
		/// Parse and populate class from dbo.tblConfigurationSetting.IrsExStarsIrsTransportModes 
		/// Expects StorageName=IRSmodeName{,PRIMARY}
		/// or
		/// Expects StorageName=IRSmodeName{,SECONDARY}
		/// </summary>
		/// <param name="settingFromDatabase"></param>
		public ExStarsTransportMode(string settingFromDatabase)
		{
			string setting = settingFromDatabase.Trim().ToUpper();
			string[] leftAndRightValues = setting.Split('=');
			if (leftAndRightValues.Count() != 2)
			{
				ThrowError(settingFromDatabase, " not formatted as lvalue=rvalue ");
			}
			Name = leftAndRightValues[0].Trim();
			Regex lvalueFormat = new Regex(@"^[A-Z0-9 \-]*[A-Z0-9]+$");
			if (!lvalueFormat.IsMatch(Name))
			{
				ThrowError(settingFromDatabase, " badly formed lvalue ");
			}
			Regex IRSmodeFormat = new Regex(@"^([A-Z]{2}|[A-Z] )$");

			// ExSTARS IRS Transportation Modes FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09	Rev 11-2005, page 14
			// TFS06 is 2 characters in length. Use trailing space following codes J, B, R and S. 
			string[] options = leftAndRightValues[1].Split(',');
			this.IrsModeCode = options[0].Trim().PadRight(2);
			if (!IRSmodeFormat.IsMatch(this.IrsModeCode))
			{
				ThrowError(settingFromDatabase, " badly formed IRS Mode  value ");
			}

			EnumStorage storageValue = EnumStorage.NA;

			if (options.Count() > 1 && ! Enum.TryParse(options[1].Trim(), true, out storageValue))
			{
				ThrowError(settingFromDatabase, " invalid storage type=" + options[1]);
			}
			this.StorageType = storageValue;
		}


		protected void ThrowError(string value, string msg = "")
		{
			throw new ExStarsSiteConfigException(string.Format(
					"Invalid value for dbo.tblConfigurationSetting, row IrsExStarsIrsTransportModes {0}:'{1}'",msg, value));			
		}

		public static bool SummarizedTransport(string irsTransportMode)
		{
			return irsTransportMode.Equals(ExStarsConstants.TFS06_SummaryReporting, StringComparison.OrdinalIgnoreCase) 
				|| irsTransportMode.Equals(ExStarsConstants.TFS06_DeliveryVehicle_GSE, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsBargeOrShip
		{
			get
			{
				return Name.ToUpper().StartsWith(Ship) || Name.ToUpper().StartsWith(Barge);
			}
		}

		public bool IsIrsRail
		{
			get
			{
				return IrsModeCode.Equals("R", StringComparison.OrdinalIgnoreCase) 
					|| IrsModeCode.Equals("ER", StringComparison.OrdinalIgnoreCase) 
					|| IrsModeCode.Equals("IR", StringComparison.OrdinalIgnoreCase) 
					|| IrsModeCode.Equals("RR", StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsIrsTruck
		{
			get
			{
				return IrsModeCode.Equals("J", StringComparison.OrdinalIgnoreCase) 
					|| IrsModeCode.Equals("AJ", StringComparison.OrdinalIgnoreCase) 
					|| IrsModeCode.Equals("EJ", StringComparison.OrdinalIgnoreCase) 
					|| IrsModeCode.Equals("IJ", StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsIrsHydrant
		{
			get
			{
				return IrsModeCode.Equals("AH", StringComparison.OrdinalIgnoreCase);
			}
		}


		public static void Test()
		{
			string[] tests =
				{
					"A=B"
					,"AA=BB"
					,"A A=B"
					,"A-A=B"
					,"C-C = D"
					,"E - E = F"
					,"G = H," // bad
					,"I = J,bad"
					,"HYDRANT TRUCK=J ,SECONDARY"
					, "HYDRANT CART=AH,SECONDARY"
					, "STATIONARY CART=AH"
					, "FILL STAND=AH"
					, "TANK=RT"
					, "FILTER=RT,NA"
					, "TANKER=J "
					, "PIPELINE-I=IP"
					, "TRUCK-I=IJ"
					, "RAIL-I=IR"
					, "SHIP-I=IS"
					, "BARGE-I=IB"
					, "PIPELINE-E=EP,PRIMARY"
					, "TRUCK-E=EJ"
					, "RAIL-E=ER"
					, "SHIP-E=ES"
					, "BARGE-E=EB"
					, "PIPELINE=PL"
					, "TRUCK=J "
					, "RAIL=R "
					, "SHIP=S "
					, "BARGE=B "
					, "BOOK ADJUSTMENT=BA"
					, "SUMMARY=CE"
					, "REMOVE FROM TERMINAL=RT"
				};
			foreach (var test in tests)
			{
				try
				{
					ExStarsTransportMode newMode = new ExStarsTransportMode(test);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("Nope " + e.Message + "\n");
				}
			}




























		}
	
}
}
