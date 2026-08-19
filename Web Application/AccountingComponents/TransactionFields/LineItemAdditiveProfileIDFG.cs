//*****************************************************************************************************************
//  FILE NAME:		LineItemAdditiveProfileIDFG.cs
//	PURPOSE:		This class inherits from the TextFieldGenerator and ILineItemField classes. 
//					It is used when the additive profile file is selected.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Richard Panachida
//	VERSION:	1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2006-11-06	Richard Panachida	Corrected the defect for the missing additive profile field (CSI 3325).
//*****************************************************************************************************************

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemAdditiveProfileIDFG.
	/// </summary>
	public class LineItemAdditiveProfileIDFG : TextFieldGenerator, ILineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the additive profile ID field class.
		/// </summary>
		public LineItemAdditiveProfileIDFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the field identification (name).
		/// </summary>
		public override string FieldID 
		{ 
			get { return "LineItem AdditiveProfileID"; } 
		}

		/// <summary>
		/// This property return true if the field is editable. Otherwise,
		/// it returns false.
		/// </summary>
		public override bool Editable 
		{ 
			get { return false; } 
		}

      /// <summary>
      /// This property will returned either a figured data length or the 
      /// default length of 30.
      /// </summary>
      protected override short MaxColumns 
		{ 
			get { return this.GetFieldLength(FieldID, 30); } 
		}
		#endregion

		#region ILineItemField Members
		/// <summary>
		/// This method will return the additive profile ID object.
		/// </summary>
	  /// <param name="inLineItem"></param>
		/// <returns></returns>
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.AdditiveProfileID;
		}

		/// <summary>
		/// This method will return the additive profile ID as a string.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns></returns>
		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		/// <summary>
		/// This method will set the additive profile ID value.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			OnFieldChanged();
		}
		#endregion
	}
}
