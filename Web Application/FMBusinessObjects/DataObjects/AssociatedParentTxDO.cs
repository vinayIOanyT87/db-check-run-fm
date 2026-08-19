/// <summary>
/// File name:	AssociatedParentTxDO.cs
/// Purpose:	The purpose of this class is to contain the data for the associated
///            parent transactions.
///            
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
/// Author(s):	Richard Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
/// Date:		   By:					   Reason:
/// ----------	   -----------------	   ---------------------------------------------------
/// yyyy-mm-dd	   Developer's name		Reason for the change
///
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace FMBusinessObjects.DataObjects
{
	#region AssociatedParentTxDO Class
	[DataContract]
   [Serializable]
   public class AssociatedParentTxDO : DataObject
	{
		#region Private data members
		[DataMember]
		private string documentNumber;
		[DataMember]
		private TransactionTypes transTypeID;
		[DataMember]
		private string transID;
		[DataMember]
		private string clin;
		[DataMember]
		private string transportOrderNumber;
		[DataMember]
		private bool flag01;
		[DataMember]
		private bool flag02;
		[DataMember]
		private bool flag03;
		[DataMember]
		private bool flag04;
		[DataMember]
		private bool flag05;
		[DataMember]
		private bool flag06;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor the the Associated Parent Tx data object class.
		/// </summary>
		public AssociatedParentTxDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets the document number data member.
		/// </summary>
		public string DocumentNumber
		{
			get { return this.documentNumber; }
			set { this.documentNumber = value; }
		}

		/// <summary>
		/// This property gets and sets the transaction type ID data member.
		/// </summary>
		public TransactionTypes TransTypeID
		{
			get { return this.transTypeID; }
			set { this.transTypeID = value; }
		}

		/// <summary>
		/// This property gets and sets the transaction ID data member.
		/// </summary>
		public string TransID
		{
			get { return this.transID; }
			set { this.transID = value; }
		}

		/// <summary>
		/// This property gets and sets the CLIN data member.
		/// </summary>
		public string CLIN
		{
			get { return this.clin; }
			set { this.clin = value; }
		}

		/// <summary>
		/// This property gets and sets the TransportOrderNumber data member.
		/// </summary>
		public string TransportOrderNumber
		{
			get { return this.transportOrderNumber; }
			set { this.transportOrderNumber = value; }
		}

		/// <summary>
		/// This property gets and sets the Flag01 data member.
		/// </summary>
		public bool Flag01
		{
			get { return this.flag01; }
			set { this.flag01 = value; }
		}

		/// <summary>
		/// This property gets and sets the Flag02 data member.
		/// </summary>
		public bool Flag02
		{
			get { return this.flag02; }
			set { this.flag02 = value; }
		}

		/// <summary>
		/// This property gets and sets the Flag03 data member.
		/// </summary>
		public bool Flag03
		{
			get { return this.flag03; }
			set { this.flag03 = value; }
		}

		/// <summary>
		/// This property gets and sets the Flag04 data member.
		/// </summary>
		public bool Flag04
		{
			get { return this.flag04; }
			set { this.flag04 = value; }
		}

		/// <summary>
		/// This property gets and sets the Flag05 data member.
		/// </summary>
		public bool Flag05
		{
			get { return this.flag05; }
			set { this.flag05 = value; }
		}

		/// <summary>
		/// This property gets and sets the Flag06 data member.
		/// </summary>
		public bool Flag06
		{
			get { return this.flag06; }
			set { this.flag06 = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method set the object to its default state.
		/// </summary>
		private void Init()
		{
			this.documentNumber = "";
			this.transTypeID = TransactionTypes.T_Maximum;
			this.transID = "";
			this.clin = "";
			this.transportOrderNumber = "";
			this.flag01 = false;
			this.flag02 = false;
			this.flag03 = false;
			this.flag04 = false;
			this.flag05 = false;
			this.flag06 = false;
		}
		#endregion

		#region Overrides
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Overrides
	}
	#endregion

	#region AssociatedParentTxListDO Class
	[DataContract]
	[Serializable]
	[KnownType(typeof(AssociatedParentTxDO))]
	public class AssociatedParentTxListDO : DataObject
	{
		#region Private data members
		private ArrayList list;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the associated parent transaction
		/// list data object class.
		/// </summary>
		public AssociatedParentTxListDO()
		{
			this.list = new ArrayList();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the list of Associated Parent Transaction data objects.
		/// </summary>
		[DataMember]
		public ArrayList List
		{
			get { return this.list; }
			private set { this.list = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add an object to the collection.
		/// </summary>
		/// <param name="associatedParentTxDO"></param>
		public void Add(AssociatedParentTxDO associatedParentTxDO)
		{
			this.list.Add(associatedParentTxDO);
		}
		#endregion

		#region Overrides
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Overrides
	}
	#endregion
}
