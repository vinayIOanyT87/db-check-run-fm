namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [DataContract]
   [Serializable]
	public class LedgerLineItemDO : InventoryLineItemDO
	{
		#region Attributes
		[DataMember] private string owner;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Ledger Line Item DO.
		/// </summary>
		public LedgerLineItemDO ( )
		{
			base.Initialize ( null );
		}

		/// <summary>
		/// This constructor sets the convert engineering units object.
		/// </summary>
		/// <param name="convEngUnits"></param>
		public LedgerLineItemDO ( EngineeringUnit? convEngUnits )
		{
			base.Initialize ( convEngUnits );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the owner attribute.
		/// </summary>
		public string Owner
		{
			get { return this.owner; }
			set { this.owner = value; }
		}
		#endregion
	}
}
