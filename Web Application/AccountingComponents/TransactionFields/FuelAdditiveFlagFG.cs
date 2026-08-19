// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelAdditiveFlagFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Field generator for FuelAdditiveFlag
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Field generator for FuelAdditiveFlagFG transaction flag.
	/// </summary>
	public class FuelAdditiveFlagFG : CheckBoxGenerator, IHeaderField
	{
		/// <summary>
		/// Gets FieldID.
		/// </summary>
		public override string FieldID
		{
			get { return "FuelAdditiveFlag"; }
		}

		#region IHeaderField Members

		/// <summary>
		/// Gets the data value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>A data object of the type represented by this field generator.</returns>
		public object GetDataValue( TransactionDO transaction )
		{
			return transaction.FuelAdditiveFlag;
		}

		/// <summary>
		/// Gets the data text.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The text representation of the data value represented by this field generator.</returns>
		public string GetDataText( TransactionDO transaction )
		{
			return transaction.FuelAdditiveFlag.ToString( CultureInfo.InvariantCulture );
		}

		/// <summary>
		/// Sets the data value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="newValue">The new value.</param>
		public void SetDataValue( TransactionDO transaction, object newValue )
		{
			transaction.FuelAdditiveFlag = (bool) newValue;
			if ( this.cell != null )
			{
				CheckBox checkBox = cell.Controls[0] as CheckBox;
				if ( checkBox != null )
				{
					checkBox.Checked = (bool) newValue;
				}
			}

			this.OnFieldChanged();
		}

		#endregion
	}
}
