using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class DocumentTop
	{
		#region Attributes
		private AccountingDataDictionary dataDict;
		private DataObject dataObj;
		private string siteId;
		private string aliasId;
		private const int EMPTY_STRING = 0;
		#endregion

		#region Contructor
		/// <summary>
		/// This is the default constructor for the DocumentTop object.
		/// </summary>
		public DocumentTop ( )
		{
			this.dataDict = null;
			this.dataObj = null;
		}

		/// <summary>
		/// This constructor will allow the user to set the data object.
		/// </summary>
		/// <param name="dataObj"></param>
		public DocumentTop ( DataObject dataObj )
		{
			this.AddDataObject ( dataObj );
			this.dataDict = null;
		}

		/// <summary>
		/// This constructor will allow the user to set the data dictionary object.
		/// </summary>
		/// <param name="dataDict"></param>
		public DocumentTop ( AccountingDataDictionary dataDict )
		{
			this.AddDataDictionary ( dataDict );
			this.dataObj = null;
		}

		/// <summary>
		/// This constructor will allow the user to set both the data object and
		/// data dictionary object.
		/// </summary>
		/// <param name="dataObj"></param>
		/// <param name="dataDict"></param>
		public DocumentTop ( DataObject dataObj, AccountingDataDictionary dataDict )
		{
			this.AddDataDictionary ( dataDict );
			this.AddDataObject ( dataObj );
		}
		#endregion

		#region Methods
		/// <summary>
		/// This method sets the data object to be used.
		/// </summary>
		/// <param name="dataObj"></param>
		public void AddDataObject ( DataObject dataObj )
		{
			this.dataObj = dataObj;
		}

		/// <summary>
		/// This method sets the data dictionary object to be used.
		/// </summary>
		/// <param name="dataDict"></param>
		public void AddDataDictionary ( AccountingDataDictionary dataDict )
		{
			this.dataDict = dataDict;
		}

		/// <summary>
		/// This method will return the data object to be used in the page
		/// population process.  It will return a null object if not set.
		/// </summary>
		/// <returns></returns>
		public DataObject getDataObject ( )
		{
			return this.dataObj;
		}

		/// <summary>
		/// This method will return the accounting data dictionary (contains the
		/// shared components data dictionary) to be used in the page population
		/// process.  It will return a null object if not set.
		/// </summary>
		/// <returns></returns>
		public AccountingDataDictionary getDataDictionary ( )
		{
			return this.dataDict;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the site ID.
		/// </summary>
		[DataMember] 
		public string SiteID
		{
			get
			{
				return this.siteId;
			}
			set
			{
				if (( value == null ) || ( value.Length == EMPTY_STRING ))
				{
					this.siteId = "";
				}
				else
				{
					this.siteId = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the alias ID.
		/// </summary>
		[DataMember]
		public string AliasID
		{
			get
			{
				return this.aliasId;
			}
			set
			{
				if (( value == null ) || ( value.Length == EMPTY_STRING ))
				{
					this.aliasId = "";
				}
				else
				{
					this.aliasId = value;
				}
			}
		}
		#endregion
	}
}
