/// <summary>
/// File name:	CurrencyLineItemDO.cs
/// Purpose:	To contain and load currency line item data.
/// 
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000. This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Van Thompson
///	Version:	1.0.0 Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		yyyy-mm-dd		Developer's name		Reason for the changes
///		
///</summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;

namespace FMBusinessObjects.DataObjects
{
	#region Currency Line Item DO Collection Class
   [Serializable]
   [CollectionDataContract]
	public class CurrencyLineItemDOCollectionClass : List<CurrencyLineItemDO>
	{
		public void RemoveByIdentityGuid(CurrencyLineItemDO lineItem)
		{
			int idx = 0;
			foreach (CurrencyLineItemDO item in this)
			{
				if (item.IdentityGuid == lineItem.IdentityGuid)
				{
					this.RemoveAt(idx);
					return;
				}
				idx++;
			}
		}
	}
	#endregion

	#region Currency Line Item DO Class
	[DataContract]
   [Serializable]
	public class CurrencyLineItemDO : BaseDataObject
	{
		#region Protected data members
		[DataMember] protected Guid currencyGuid;
		[DataMember] protected DateTimeOffset effectiveDate;
		[DataMember] protected double rate;
		[DataMember] protected bool isDirty;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Currency Line Item data object class.
		/// </summary>
		public CurrencyLineItemDO()
		{
			this.isDirty = false;
		}
		#endregion

		#region Properties
		public bool IsDirty
		{
			get { return this.isDirty; }
			set { this.isDirty = value; }
		}

		public Guid CurrencyGuid
		{
			get { return currencyGuid; }
			set { currencyGuid = value; }
		}

		public DateTimeOffset EffectiveDate
		{
			get { return effectiveDate; }
			set { effectiveDate = value; }
		}

		public double Rate
		{
			get { return rate; }
			set { rate = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Populates the Currency Line Item with data from a DataRow
		/// </summary>
		/// <param name="dr">Contains the data used to populate the Currency Line Item</param>
		public void Populate(DataRow dr)
		{
			base._IdentityGuid = DataObject.getValue<Guid>(dr["CurrencyLineItemGuid"], Guid.Empty);
			this.currencyGuid = DataObject.getValue<Guid>(dr["CurrencyGuid"], Guid.Empty);
			this.effectiveDate = DataObject.getValue<DateTimeOffset>(dr["Date"], DateTimeOffset.Now);
			this.rate = DataObject.getValue<double>(dr["Rate"], 0.0);
			base._CreatedDate = DataObject.getValue<DateTimeOffset>(dr["CreatedDate"], DateTimeOffset.Now);
			base._CreatedBy = DataObject.getValue<string>(dr["CreatedBy"], ADMIN);
			base._UpdatedDate = DataObject.getValue<DateTimeOffset>(dr["UpdatedDate"], _CreatedDate);
			base._UpdatedBy = DataObject.getValue<string>(dr["UpdatedBy"], ADMIN);
		}
		#endregion
	}
	#endregion
}
