///<Summary>
/// File Name:	InterfaceData04FG.cs
/// Purpose:	The purpose of this class is to generate a general purpose field on the transaction detail
///				for interface data that is returned from interfaces like SAP.
/// Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec.
/// Date: January 30, 2012
///</Summary>

using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	public class InterfaceData04FG : TextFieldGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Interface Data 01 Field Control.
		/// </summary>
		public InterfaceData04FG ( )
		{
		}
		#endregion

		#region Properties
		public override string FieldID
		{
			get { return "InterfaceData04"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 1000.
		/// </summary>
		protected override short MaxColumns
		{
			get { return ( short ) base.GetFieldLength ( FieldID, 100 ); }
		}

		/// <summary>
		/// This field is always non-editable.
		/// </summary>
		public override bool Editable
		{
			get { return false; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method returns the error text as an object type.
		/// </summary>
		/// <param name="trans"></param>
		/// <returns></returns>
		public object GetDataValue ( TransactionDO trans )
		{
			return trans.InterfaceData04;
		}

		/// <summary>
		/// This method returns the error text as a string type. It will return
		/// null if the error flag is not set.
		/// </summary>
		/// <param name="trans"></param>
		/// <returns></returns>
		public string GetDataText ( TransactionDO trans )
		{
			if ( trans != null )
			{
				string InterfaceData04 = GetDataValue ( trans ) as string;
				return InterfaceData04;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// This method sets the value of the error text.
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="newValue"></param>
		public void SetDataValue ( TransactionDO trans, object newValue )
		{
			string stringTemp = newValue as string;

			if ( stringTemp != null )
			{
				stringTemp = stringTemp.Trim ( );
			}

			trans.InterfaceData04 = stringTemp.Trim ( );
			OnFieldChanged ( );
		}
		#endregion
	}
}
