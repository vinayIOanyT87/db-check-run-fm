/// <summary>
/// File name:	CurrencyUnitDO.cs
/// Purpose:	To contain and load currency unit data.
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
	#region Currency Unit DO Collection Class
   [Serializable]
   [CollectionDataContract]
	public class CurrencyUnitDOCollectionClass : List<CurrencyUnitDO> { }
	#endregion

	#region Currency Unit DO Class
	[DataContract]
   [Serializable]
	public class CurrencyUnitDO : BaseDataObject
	{
		#region Protected data members
		[DataMember] protected int currencyUnitIndex;
		[DataMember] protected string currencyUnitName;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Currency Unit Data Object class.
		/// </summary>
		public CurrencyUnitDO ( )
		{
		}

		/// <summary>
		/// This constructor initializes the Currency Unit Data Object class
		/// based on the index and currency unit name.
		/// </summary>
		/// <param name="currencyUnitIndex"></param>
		/// <param name="currencyUnitName"></param>
		public CurrencyUnitDO ( int currencyUnitIndex, string currencyUnitName )
		{
			this.currencyUnitIndex	= currencyUnitIndex;
			this.currencyUnitName	= currencyUnitName;
		}
		#endregion

		#region Properties
		public int CurrencyUnitIndex
		{
			get { return currencyUnitIndex; }
			set { currencyUnitIndex = value; }
		}

		public string CurrencyUnitName
		{
			get { return currencyUnitName; }
			set { currencyUnitName = value; }
		}
		#endregion

		#region Public methods
		public void Populate ( DataRow dr )
		{
			this.currencyUnitIndex = DataObject.getValue<int>(dr["CurrencyUnitIndex"], 0);
			this.currencyUnitName = DataObject.getValue<string>(dr["CurrencyUnitName"], "");
		}
		#endregion
	}
	#endregion
}
