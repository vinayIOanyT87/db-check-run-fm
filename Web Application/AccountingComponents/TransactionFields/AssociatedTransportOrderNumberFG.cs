/// <summary>
/// File name:	AssociatedTransportOrderNumber.cs
/// Purpose:	The purpose of this class is to create a field that
///            contains a list of Transport Order Numbers that are associated to a 
///            parent transaction.
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
using System.Collections;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

namespace TransactionFields
{
	class AssociatedTransportOrderNumberFG : DropDownGenerator, IHeaderField
	{
		#region Private data members
		private string transID;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Associated Transprot Order Number
		/// field generator.
		/// </summary>
		public AssociatedTransportOrderNumberFG ( )
		{
			this.transID = "";
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the field identify.
		/// </summary>
		public override string FieldID
		{
			get { return "AssociatedTransportOrderNumber"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return (short) base.GetFieldLength ( FieldID, 30 ); }
		}

		/// <summary>
		/// This property return true if the field is editable.
		/// </summary>
		public override bool Editable
		{
			get { return base.bFieldEditible; }
		}

		/// <summary>
		/// This property gets and sets the Transaction ID data member.
		/// </summary>
		public string TransID
		{
			get { return this.transID; }
			set
			{
				this.transID = value;

				if (this.transID == null)
				{
					this.transID = "";
				}
			}
		}

		/// <summary>
		/// This method will return the control.
		/// </summary>
		public System.Web.UI.WebControls.TableCell Cell
		{
			get { return base.cell; }
		}
		#endregion

		#region Override methods
		/// <summary>
		/// Returns the statuses configured for use with the transaction alias
		/// </summary>
		/// <returns>A HybridDictionary containing the configured statuses</returns>
		public override HybridDictionary GetEntries ( )
		{
			// Create a new dictionary
			HybridDictionary newDictionary = new HybridDictionary ( );

			if (( base.transContext.IntermediateTransID == null ) || ( base.transContext.IntermediateTransID.Length <= 0 ))
			{
				newDictionary = this.GetEntriesBasedOnDocumentNumber ( );
			}
			else
			{
				// Retrieve document number from all parent associated transactions.
				// Note: there should only be one parent for implementation.
				GetAssociatedParentTxSR associatedParentTxSR = new GetAssociatedParentTxSR ( );
				associatedParentTxSR.TransID = base.transContext.IntermediateTransID;
				associatedParentTxSR.SubTypeRequest = GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_TRANSPORT_LINE;
				associatedParentTxSR.Security = base.transContext.security;

				AssociatedParentTxListDO associatedParentTxListDO = FMChannelHelper.MakeCall<IGetAssociatedParentTxProcessor, AssociatedParentTxListDO>(
																	 x =>
																	 x.Process ( associatedParentTxSR )
																);
				if (associatedParentTxListDO != null)
				{
					foreach (AssociatedParentTxDO associatedParentTxDO in associatedParentTxListDO.List)
					{
						newDictionary.Add ( associatedParentTxDO.TransportOrderNumber, associatedParentTxDO.TransportOrderNumber );
					}
				}
			}

			return newDictionary;
		}

		/// <summary>
		/// This method will retrieve a list of Transport Order Numbers based on the associated document number.
		/// </summary>
		private HybridDictionary GetEntriesBasedOnDocumentNumber ( )
		{
			// Create a new dictionary
			HybridDictionary newDictionary = new HybridDictionary ( );

			if (( base.trans.AssociatedDocumentNumber != null ) && ( base.trans.AssociatedDocumentNumber.Length > 0 ))
			{
				string docNumber = base.trans.AssociatedDocumentNumber;
				GetAssociatedParentTxSR associatedParentTxSR = new GetAssociatedParentTxSR ( );
				associatedParentTxSR.AliasName = base.trans.Alias;
				associatedParentTxSR.CurrentSiteGuid =  base.trans.SiteGuid;
				associatedParentTxSR.TransTypeID = base.trans.TransTypeID;
				associatedParentTxSR.TransactionAliasGuid =  base.trans.TransactionAliasGuid;
				associatedParentTxSR.SubTypeRequest = GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_TRANSPORT_LINE_PER_DOC;
				associatedParentTxSR.AssociatedDocNumber = docNumber;
				associatedParentTxSR.Security = base.transContext.security;

				AssociatedParentTxListDO associatedParentTxListDO = FMChannelHelper.MakeCall<IGetAssociatedParentTxProcessor, AssociatedParentTxListDO>(
																	 x =>
																	 x.Process ( associatedParentTxSR )
																);


				if (associatedParentTxListDO != null)
				{
					foreach (AssociatedParentTxDO associatedParentTxDO in associatedParentTxListDO.List)
					{
						newDictionary.Add(associatedParentTxDO.TransportOrderNumber, associatedParentTxDO.TransportOrderNumber);
					}
				}

				if (base.transContext.AssociatedDocNumFlags != null)
				{
					ArrayList flags = base.transContext.AssociatedDocNumFlags[docNumber] as ArrayList;
					if (( flags != null ) && ( (bool) flags[3] == true ))
					{
						base.bFieldEditible = false;
					}
				}
			}

			return newDictionary;
		}
		#endregion

		#region IHeaderField Members
		public object GetDataValue ( TransactionDO transaction )
		{
			return transaction.AssociatedTransportOrderNumber;
		}

		public string GetDataText ( TransactionDO transaction )
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

		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			transaction.AssociatedTransportOrderNumber = (string) newValue;
		}
		#endregion
	}
}
