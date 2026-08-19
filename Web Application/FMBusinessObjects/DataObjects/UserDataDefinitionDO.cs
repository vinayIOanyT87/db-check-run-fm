using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace FMBusinessObjects.DataObjects
{
	public enum UserDataType { TextBox, DropDown, Combo }

   [Serializable]
   [DataContract]
	public class UserDataDefinitionDO : DataObject
	{
		#region Attributes
		[DataMember]
		protected string fieldName;
		[DataMember]
		protected string databaseColumn;
		[DataMember]
		protected UserDataType fieldType;
		[DataMember]
		protected StringCollection dropDownList;
		#endregion Attributes

		#region Properties

		public string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
		}

		public string DatabaseColumn
		{
			get { return databaseColumn; }
			set { databaseColumn = value; }
		}

		public UserDataType FieldType
		{
			get { return fieldType; }
			set { fieldType = value; }
		}

		public StringCollection DropDownList
		{
			get { return dropDownList; }
			set { dropDownList = value; }
		}
		#endregion Properties

		#region Constructor
		/// <summary>
		/// This is the default constructor for the user data difenition 
		/// data object class.
		/// </summary>
		public UserDataDefinitionDO()
		{
		}
		#endregion

		#region Override methods
		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			//string sql = "";
			return null;
		}

		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}
		#endregion
	}
}
