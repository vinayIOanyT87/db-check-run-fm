// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveTransmitTranListResultDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Result data object from saving transmitted transactions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Result data object from saving transmitted transactions.
	/// </summary>
	[Serializable]
	[DataContract]
	public class SaveTransmitTranListResultDO : DataObject
	{
		#region Fields

		private string errormsg;

		private StatusEnum status;

		#endregion

		#region Constructors and Destructors

		public SaveTransmitTranListResultDO()
		{
			this.status = StatusEnum.FAIL;
			this.errormsg = string.Empty;
		}

		#endregion

		#region Enums

		public enum StatusEnum
		{
			SUCCESS, 

			FAIL
		}

		#endregion

		#region Public Properties

		[DataMember]
		public string ErrorMessage
		{
			get
			{
				return this.errormsg;
			}

			set
			{
				this.errormsg = value;
			}
		}

		[DataMember]
		public StatusEnum Status
		{
			get
			{
				return this.status;
			}

			set
			{
				this.status = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}

		#endregion
	}
}