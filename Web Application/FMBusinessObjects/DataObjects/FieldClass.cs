using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using FMBusinessObjects.UtilityObjects;
using System.IO;

namespace FMBusinessObjects.DataObjects
{
	#region Field Collection Class
   [Serializable]
   [CollectionDataContract]
	public class FieldCollectionClass : FMCollectionBaseBindingList<FieldClass> 
	{
		public void Add( FieldClass Field )
		{
			List.Add( Field );
		}

		public void Remove( int index )
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception( "Invalid Index" ));
			}
			else
			{
				List.RemoveAt( index );
			}
		}

		public FieldClass Item( int Index )
		{
			return (FieldClass)List[Index];
		}
	}
	#endregion

	[DataContract]
	[Serializable]
	public abstract class FieldClass : BaseSerializedDataObject, IComparable
	{
		#region Protected data members
		[DataMember] protected int _DisplayOrder;
		#endregion

		#region Public data members
		[DataMember] protected string _DbName;
		[DataMember] protected string _DisplayName;
		[DataMember] protected string _AliasName;
		#endregion

		#region Properties

		[DataMember]
		public Guid TransactionAliasGuid
		{
			get;
			set;
		}

		public int DisplayOrder 
		{ 
			get { return this._DisplayOrder; } 
			set { this._DisplayOrder = value; } 
		}

		public string DisplayName 
		{ 
			get { return _DisplayName; } 
			set { SetString( "DisplayName", 50, value, ref _DisplayName ); } 
		}

		public string AliasName 
		{ 
			get { return _AliasName; } 
			set { SetString( "AliasName", 32, value, ref _AliasName ); } 
		}

		[DataMember]
		public bool FieldRequired 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public bool VirtualField 
		{ 
			get; 
			set; 
		}

		/// <summary>
		/// Gets or sets a value indicating whether field is a dispatch field.
		/// </summary>
		[DataMember]
		public bool DispatchField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether field should be cleared on new or copy
		/// </summary>
		[DataMember]
		public bool ClearOnNew
		{
			get;
			set;
		}

		[DataMember]
		public bool ReadOnly
		{
			get;
			set;
		}

		[DataMember]
		public Guid UserGroupGuid
		{
			get;
			set;
		}

		[DataMember]
		public string UserGroupID 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public TransactionFieldVisibility Visibility
		{
			get;
			set;
		}

		[DataMember]
		[FMPersistedField("DefaultValueType")]
		public string DefaultValueTypeString
		{
			get
			{
				return ValueTypeString;
			}
            set
            {
                this.ValueTypeString = value;
            }
        }

		[DataMember]
		public object DefaultValue
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		[FMPersistedField("DefaultValue")]
		public string DefaultValueXml
		{

			get
			{
				return this.ValueXml;
			}
			set
			{
				this.ValueXml = value;
			}
		}

		public string DbName
		{
			get { return _DbName; }
			set { SetString( "DbName", 50, value, ref _DbName ); }
		}
		#endregion

		int IComparable.CompareTo( Object Field )
		{
			return DisplayOrder - ((FieldClass)Field).DisplayOrder;
		}

		/// <summary>
		/// This method will reset the object to its initial state.
		/// </summary>
		public override void Reset ( )
		{
			base.Reset();

			this.TransactionAliasGuid = Guid.Empty;
			this.UserGroupGuid = Guid.Empty;
			this.UserGroupID = string.Empty;
			this.DisplayOrder = 0;
			this.DisplayName = string.Empty;
			this.AliasName = string.Empty;
			this.FieldRequired = false;
			this.VirtualField = false;
			this.DispatchField = false;
			this.ClearOnNew = false;
			this.DbName = string.Empty;
			this.ReadOnly = false;
			this.DefaultValueTypeString = "System.String";
			this.DefaultValueXml = string.Empty;
		}
	}
}