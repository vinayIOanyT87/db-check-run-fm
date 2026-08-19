/// <summary>
/// File name:	OperatorID_FG.cs
/// Purpose:	The purpose of this class is to define the Operator field.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	--------------------------------------------
///		10/16/2008		W.Gray					7.4.6.0 - Changed to update Signature (CSI 6231)
///		
/// </summary>
using System;
using System.Web.UI.WebControls;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	public class OperatorID_FG : OperatorTextButtonGenerator, IHeaderField
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor for the OperatorID_FG class.
		/// </summary>
		public OperatorID_FG()
		{	
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID 
		{ 
			get { return "OperatorID"; } 
		}

		/// <summary>
		/// This method will operator ID in the transaction object.
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="newID"></param>
		protected override void SetOperatorID(string newID)
		{
			trans.OperatorID = newID;
		}

		/// <summary>
		/// This method will set the operator index value in the transaction object.
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="newGuid"></param>
		protected override void SetOperatorGuid(Guid newGuid)
		{
			trans.OperatorPersonnelGuid = newGuid;
		}

		protected override bool AutoPostBack
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// This method will set the operator signature value in the transaction object.
		/// </summary>
		/// <param name="trans"></param>
		/// <param name="newIndex"></param>
		protected override void SetSignature(byte[] Signature)
		{
			trans.Signature = Signature;
		}

		protected override void SetOperatorName(string operatorName)
		{
			trans.OperatorName = operatorName;

			if (transContext.aliasClass.TransactionFieldCollection.Find("OperatorName") != null)
			{
				var operatorNameFG = fieldGenerator.GetFieldGenerator("OperatorName") as OperatorNameFG;
				if (operatorNameFG != null)
				{
					operatorNameFG.SetDataValue(this.trans, operatorName);
				}
			}
		}


		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return base.GetFieldLength(FieldID, OperatorTextButtonGenerator.FIELD_LENGTH); } 
		}
		#endregion

		#region IHeaderField Members
		/// <summary>
		/// This method will return the operator ID as a string.
		/// </summary>
		/// <param name="trans"></param>
		/// <returns></returns>
		protected string GetOperatorID(TransactionDO trans)
		{
			return trans.OperatorID;
		}


		/// <summary>
		/// This method returns the operator ID as an object.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.OperatorID;
		}

		/// <summary>
		/// This method returns the transaction string value.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// This method will set the new value of the control into the transaction
		/// data object.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			SetValue(newValue);

			OnFieldChanged();
		}
		#endregion
	}

}
