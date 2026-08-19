namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class GeneralConfigAlias : DataObject
	{
		#region private attributes
		[DataMember] private Guid _GeneralConfigurationAliasGuid;
		[DataMember] private Guid _GeneralConfigurationGuid;
		[DataMember] private Guid _TransactionAliasGuid;
		[DataMember] private string _AliasName;
		[DataMember] private string _CreatedBy;
		[DataMember] private string _UpdatedBy;
		[DataMember] private bool _DeleteFlag;
		[DataMember] private DateTimeOffset _CreatedDate;
		[DataMember] private DateTimeOffset _UpdatedDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the general configuration alias class.
		/// </summary>
		public GeneralConfigAlias()
		{
			this.Init();
		}
		#endregion

		#region Properties

		public DateTimeOffset CreatedDate
		{
			get { return this._CreatedDate; }
			set { this._CreatedDate = value; }
		}

		public DateTimeOffset UpdatedDate
		{
			get { return this._UpdatedDate; }
			set { this._UpdatedDate = value; }
		}

		/// <summary>
		/// This property will get and set the general config alias guid attribute.
		/// </summary>
		public Guid GeneralConfigurationAliasGuid
		{
			get { return this._GeneralConfigurationAliasGuid; }
			set { this._GeneralConfigurationAliasGuid = value; }
		}

		/// <summary>
		/// This property will get and set the general config guid attribute.
		/// </summary>
		public Guid GeneralConfigurationGuid
		{
			get { return this._GeneralConfigurationGuid; }
			set { this._GeneralConfigurationGuid = value; }
		}

		/// <summary>
		/// This property will get and set the transaction alias guid attribute.
		/// </summary>
		public Guid TransactionAliasGuid
		{
			get { return this._TransactionAliasGuid; }
			set { this._TransactionAliasGuid = value; }
		}

		/// <summary>
		/// This property will get and set the alias name attribute.
		/// </summary>
		public string AliasName
		{
			get { return this._AliasName; }
			set { this._AliasName = value; }
		}

		/// <summary>
		/// This property will get and set the created by attribute.
		/// </summary>
		public string CreatedBy
		{
			get { return this._CreatedBy; }
			set { this._CreatedBy = value; }
		}

		/// <summary>
		/// This property will get and set the updated by attribute.
		/// </summary>
		public string UpdatedBy
		{
			get { return this._UpdatedBy; }
			set { this._UpdatedBy = value; }
		}

		/// <summary>
		/// This property will get and set the delete flag attribute.
		/// </summary>
		public bool DeleteFlag
		{
			get { return this._DeleteFlag; }
			set { this._DeleteFlag = value; }
		}
		#endregion

		#region private methods
		/// <summary>
		/// This method initializes this object to its initial state.
		/// </summary>
		private void Init()
		{
			this._GeneralConfigurationAliasGuid = Guid.Empty;
			this._GeneralConfigurationGuid = Guid.Empty;
			this._TransactionAliasGuid = Guid.Empty;
			this._AliasName = "";
			this._CreatedBy = BaseDataObject.ADMIN;
			this._UpdatedBy = BaseDataObject.ADMIN;
			this._DeleteFlag = false;
			this._CreatedDate = System.DateTimeOffset.Now;
			this._UpdatedDate = _CreatedDate;
		}
		#endregion

		#region Override Methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}
		#endregion
	}
}
