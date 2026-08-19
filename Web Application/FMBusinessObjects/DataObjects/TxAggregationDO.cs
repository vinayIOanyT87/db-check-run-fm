using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TxAggregationDO : DataObject
	{
		#region Properties
		[DataMember] public double Quantity { get; set; }
		[DataMember] public double Excise { get; set; }
		[DataMember] public double Gst { get; set; }
		[DataMember] public double Margin { get; set; }
		[DataMember] public double OnCost { get; set; }
		[DataMember] public double TotalValue { get; set; }
		[DataMember] public double TotalPriceWithTax { get; set; }
		[DataMember] public double TotalForeignPrice { get; set; }
		[DataMember] public double Number01 { get; set; }
		[DataMember] public double Number02 { get; set; }
		[DataMember] public double Number03 { get; set; }
		[DataMember] public double Number04 { get; set; }
		[DataMember] public double Number05 { get; set; }
		[DataMember] public double Number06 { get; set; }
		#endregion // Properties

		#region Construction
		public TxAggregationDO ( )
		{
			Quantity = 0.0;
			Excise = 0.0;
			Gst = 0.0;
			Margin = 0.0;
			OnCost = 0.0;
			TotalValue = 0.0;
			TotalPriceWithTax = 0.0;
			TotalForeignPrice = 0.0;
			Number01 = 0.0;
			Number02 = 0.0;
			Number03 = 0.0;
			Number04 = 0.0;
			Number05 = 0.0;
			Number06 = 0.0;
		}

		public TxAggregationDO ( DataRow a_dr )
		{
			this.Load ( a_dr );
		}
		#endregion // Construction

		public void Load ( DataRow a_dr )
		{
			ArrayList fieldList = new ArrayList ( )
			{
				"Quantity", "Excise", "GST", "Margin", "OnCost", "TotalValue", "TotalPriceWithTax", "TotalForeignPrice",
				"Number01", "Number02", "Number03", "Number04", "Number05", "Number06"
			};

			ArrayList resultList = new ArrayList ( );

			foreach (string field in fieldList)
			{
				double value;
				try
				{
					value = a_dr.IsNull ( field ) ? 0.0 : double.Parse ( a_dr[field].ToString ( ) );
				}
				catch (Exception)
				{
					value = 0.0;
				}

				resultList.Add ( value );
			}

			// now set the properties accordingly
			int i = -1;
			Quantity = (double) fieldList[++i];
			Excise = (double) fieldList[++i];
			Gst = (double) fieldList[++i];
			Margin = (double) fieldList[++i];
			OnCost = (double) fieldList[++i];
			TotalValue = (double) fieldList[++i];
			TotalPriceWithTax = (double) fieldList[++i];
			TotalForeignPrice = (double) fieldList[++i];
			Number01 = (double) fieldList[++i];
			Number02 = (double) fieldList[++i];
			Number03 = (double) fieldList[++i];
			Number04 = (double) fieldList[++i];
			Number05 = (double) fieldList[++i];
			Number06 = (double) fieldList[++i];
		}

		#region Overrides
		public override string getDeleteCommand ( )
		{
			return null;
		}

		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}
		#endregion // Overrides
	}
}
