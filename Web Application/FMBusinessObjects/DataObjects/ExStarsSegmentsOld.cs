using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	public class ExStarsSegmentsOld
	{
		public ExStarsSegmentsOld()
		{
		}

		public string GetTFS(ref StringBuilder error, string TFS01, string TFS02)
		{
			return GetTFS(ref error, TFS01, TFS02, "", "", "", "");
		}

		public string GetTFS(ref StringBuilder error, string TFS01, string TFS02, string TFS03, string TFS04)
		{
			return GetTFS(ref error, TFS01, TFS02, TFS03, TFS04, "", "");
		}

		public string GetTFS(ref StringBuilder error, string TFS01, string TFS02, string TFS03, string TFS04, string TFS05, string TFS06)
		{
			string TFS = "TFS";

			if (TFS01 != "" && TFS02 != "")  // program must supply TFS02
				TFS += "~" + TFS01 + "~" + TFS02;

			if (TFS03 != "" && TFS04 != "")
				TFS += "~" + TFS03 + "~" + TFS04;

			if (TFS05 != "" && TFS06 != "")
			{
				if (TFS04 != "")
					TFS += "~" + TFS05 + "~" + TFS06;
				else
					TFS += "~~~" + TFS05 + TFS06;
			}
			TFS += "\\";
			return TFS;
		}

		public string GetDTM(ref StringBuilder error, string DTM01, string DTM02)
		{
			string DTM;
			DTM = "DTM~" + DTM01 + "~" + DTM02 + "\\";
			return DTM;
		}

		public string GetTIA(ref StringBuilder error, string TIA01, string TIA04)
		{
			string TIA;
			TIA = "TIA~" + TIA01 + "~~~" + TIA04 + "~" + "GA" + "\\";//Net Physical Inventory. TIA02 and TIA03 are not used.			
			return TIA;
		}
		public string GetFGS(ref StringBuilder error, string FGS01, string FGS02, string FGS03)
		{
			string FGS;

			FGS = "FGS~" + FGS01 + "~" + FGS02 + "~" + FGS03 + "\\";
			return FGS;
		}

		public string GetN1(ref StringBuilder error, string N101, string N102)
		{
			return GetN1(ref error, N101, N102, "", "");
		}

		public string GetN1(ref StringBuilder error, string N101, string N102, string N103, string N104)
		{
			string N1;
			N1 = "N1~" + N101;// N101 must be entered

			if (N102 != "")
				N1 += "~" + N102;

			if (N103 != "" && N104 != "")
				if (N102 != "")
					N1 += "~" + N103 + "~" + N104;
				else
					N1 += "~~" + N103 + "~" + N104;

			N1 += "\\";

			return N1;
		}

		public string GetN2(ref StringBuilder error, string N201)
		{
			return GetN2(ref error, N201, "");
		}

		public string GetN2(ref StringBuilder error, string N201, string N202)
		{
			string N2 = "";
			if (N201 != "")
				N2 = "N2~" + N201;

			if (N202 != "")
			{
				if (N201 != "")
					N2 += "~" + N202;
				else
					N2 = "N2~" + N202;
			}
			if (N2 != "")
				N2 += "\\";

			return N2;
		}

		public string GetN3(ref StringBuilder error, string Address1)
		{
			return GetN3(ref error, Address1, "");
		}

		public string GetN3(ref StringBuilder error, string Address1, string Address2)
		{
			if (Address1 == "")
			{
				error.Append("Manager address should be provided.\n");
			}

			string N3 = "N3~" + Address1;			 // N301 is required

			if (Address2 != "")
				N3 += "~" + Address2;

			N3 += "\\";
			return N3;
		}

		public string GetN4(ref StringBuilder error, string City, string State, string ZipCode, string Country)
		{
			string N4;
			if (City == "")
				error.Append("Manager City should be provided.\n");
			if (State == "")
				error.Append("Manager State should be provided.\n");
			if (ZipCode == "")
				error.Append("Manager ZipCode should be provided.\n");
			if (Country == "")
				error.Append("Manager Country should be provided.\n");
			N4 = "N4~" + City + "~" + State + "~" + Country + "\\";
			return N4;
		}
		public string GetPER(ref StringBuilder error, string PER01, string PER02, string Telephone, string Fax, string Email)
		{
			string PER;

			if (Telephone == "" && Email == "")
				error.Append("At least Manager Telephone or Email must be provided.\n");

			PER = "PER~" + PER01 + "~" + PER02; // must use per01 and per02
			if (Telephone != null || Telephone != "")
				PER += "~TE~" + Telephone;
			if (Fax != null || Fax != "")
				if (Telephone != "")
					PER += "~FX~" + Fax;
				else
					PER += "~~~FX~" + Fax;
			if (Email != null || Email != "")
				if (Fax != "")
					PER += "~EM~" + Email;
				else
					if (Telephone != "")
						PER += "~~~EM~" + Email;
					else
						PER += "~~~~~EM~" + Email;
			PER += "\\";
			return PER;
		}

		public string GetRelationshipREF(ref StringBuilder error, string State1)
		{
			return GetRelationshipREF(ref error, State1, "");
		}
		public string GetRelationshipREF(ref StringBuilder error, string State1, string State2)
		{
			string REF;
			REF = "REF~SU~IRS~~";
			if (State1 != null && State1 != "")
				REF += "S0^" + State1;
			if (State2 != null && State2 != "")
			{
				if (State1 != "")
					REF += "^S0^" + State2;
				else
					REF += "S0^^S0^" + State2;
			}
			REF += "\\";
			return REF;
			/*
			if (REF01 == "BE")
				REF += "~1";
			else
			{
				REF += "~" + REF02 + "~~";
				if (State1 != null && State1 != "")
					REF += "S0^" + State1;
				else
					REF += "S0^^";
				if (State2 != null && State2 != "")
				{
					REF += "^S0^" + State2;
				}
			}*/
		}
		public string GetREF(ref StringBuilder error, string SequenceNumber)
		{
			string REF;
			REF = "REF~55~" + SequenceNumber + "\\";
			return REF;
		}
	}
}
