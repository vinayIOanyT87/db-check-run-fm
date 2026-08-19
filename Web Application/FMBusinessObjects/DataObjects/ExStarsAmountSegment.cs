using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class ExStarsAmountSegment: ExStarsSegment
	{
		/// <summary>
		/// Motor Fuel Excise Tax EDI Guide Pg 43, 94
		/// </summary>
		/// <param name="taxInfoCode"></param>
		/// <param name="gallonsFuel"></param>
		public ExStarsAmountSegment(MeasurementBeingTaxed taxInfoCode, double gallonsFuel)
			: base("TIA", "Tax Information Amount")
		{
			this.AddElement(1, "Tax Information Code", "", EnumExStarsElementTypes.AN, 4, 4, ((int)taxInfoCode).ToString());
			// skip 2,3
			this.AddElement(4, "Quantity", "In gallons", EnumExStarsRequired.X, EnumExStarsElementTypes.R, 1, 15, ExStarsConstants.RoundGallons(gallonsFuel));
			this.AddElement(5, "Unit of Measurement Code", "GA = Gallons", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.TIA01_Gallons);
		}
	}
}
