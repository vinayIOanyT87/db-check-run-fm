// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliasNameHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// TODO: Break these helpers into separate files.

namespace FuelsManager.Areas.FieldHelpers
{
	using System;

	using FMBusinessObjects.DataObjects;

	public class AliasNameHelper : FMFieldHelper<string>
	{
		public override bool Editable { get { return false; } }

		public override string FieldId { get { return "Alias"; } }
	}

	public class TransIDHelper : FMFieldHelper<string>
	{
		public override bool Editable { get { return false; } }

		public override string FieldId { get { return "TransID"; } }

		protected override void SpecializeControl(TransactionAliasFieldClass fieldInfo, TransactionDO transaction)
		{
			base.SpecializeControl(fieldInfo, transaction);

			this.Style["min-width"] = "225px";
		}
	}

	public class OwnerIDHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "OwnerID"; } }
	}

	public class ShipperIDHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "ShipperID"; } }
	}

	public class BillToIDHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "BillToID"; } }
	}

	public class ShipToIDHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "ShipToID"; } }
	}

	public class CarrierIDHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "CarrierID"; } }
	}

	public class ManagerIDHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "ManagerID"; } }
	}

	public class ReversalTypeHelper : FMFieldHelper<string>
	{
		public override bool Editable { get { return false; } }

		public override string FieldId { get { return "ReversalType"; } }

		protected override void SpecializeControl( TransactionAliasFieldClass fieldInfo, TransactionDO transaction )
		{
			base.SpecializeControl( fieldInfo, transaction );

			this.Style["min-width"] = "50px";
			this.Style["width"] = "50px";
		}
	}

	public class LineItemProductHelper : FMFieldHelper<string>
	{
		public override string FieldId { get { return "Product"; } }
	}

	public class InventoryDateHelper : FMFieldHelper<DateTime>
	{
		public override string FieldId { get { return "InventoryDate"; } }

		protected override void SpecializeControl(TransactionAliasFieldClass fieldInfo, TransactionDO transaction)
		{
			base.SpecializeControl(fieldInfo, transaction);

			this.Attributes["class"] += " minWidth100 datepicker";
		}
	}

	public class TransDateTimeHelper : FMFieldHelper<DateTimeOffset?>
	{
		public override string FieldId { get { return "TransactionDateTime"; } }

		protected override void SpecializeControl( TransactionAliasFieldClass fieldInfo, TransactionDO transaction )
		{
			base.SpecializeControl( fieldInfo, transaction );

			this.Attributes["class"] += " minWidth100 datepicker";
			this.Style.Remove("min-width");
		}
	}

	public class LineItemLineNumberHelper : FMFieldHelper<int?>
	{
		public override string FieldId { get { return "LineNumber"; } }

		protected override void SpecializeControl( TransactionAliasFieldClass fieldInfo, TransactionDO transaction )
		{
			base.SpecializeControl( fieldInfo, transaction );

			this.Style["min-width"] = "100px";
			this.Style["width"] = "100px";
		}
	}

	public abstract class QuantityHelper : FMFieldHelper<double>
	{
		protected override void SpecializeControl( TransactionAliasFieldClass fieldInfo, TransactionDO transaction )
		{
			base.SpecializeControl( fieldInfo, transaction );

			this.Style["min-width"] = "100px";
			this.Style["width"] = "100px";
		}
	}

	public class LineItemGrossQuantityHelper : QuantityHelper
	{
		public override string FieldId { get { return "GrossInventoryChange"; } }
	}

	public class LineItemNetQuantityHelper : QuantityHelper
	{
		public override string FieldId { get { return "NetInventoryChange"; } }
	}

	public class LineItemMassQuantityHelper : QuantityHelper
	{
		public override string FieldId { get { return "MassQuantityChange"; } }
	}

	public abstract class TransactionStatusHelper : FMFieldHelper<TransactionStatus>
	{
		protected override void SpecializeControl( TransactionAliasFieldClass fieldInfo, TransactionDO transaction )
		{
			base.SpecializeControl( fieldInfo, transaction );

			this.Style["min-width"] = "50px";
		}
	}

	public class LookupTransactionStatusIndexHelper : TransactionStatusHelper
	{
		public override string FieldId { get { return "Status"; } }
	}

	public class LineItemLookupTransactionStatusIndexHelper : TransactionStatusHelper
	{
		public override string FieldId { get { return "Status"; } }
	}
}
