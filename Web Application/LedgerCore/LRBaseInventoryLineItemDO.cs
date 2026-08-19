/// <summary>
///	File name:	BaseLineItemDO.cs
///	Purpose:	
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:       By:						Reason:
///   ----------	--------------------	----------------------------------
///   yyyy-mm-dd	Coder's name			Reason for change
///   
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace LedgerCore
{
	public abstract class LRBaseInventoryLineItemDO
	{
		#region Status flag settings
		[Flags]
		public enum Status
		{
			Default					= 0x0000,
			ClosedOut				= 0x0001,
			PhysInvExists			= 0x0002,
			OutOfToleranceGross		= 0x0004,
			OutOfToleranceNet		= 0x0008,
			InvError				= 0x0010,
			Suppress				= 0x0020, // Display empty string
			Na						= 0x0040, // Display "N/A"
			SuppressLink			= 0x0080, // Do not create hyperlink
			BrokenBlends			= 0x0100,
			TransWithZeroQuantity	= 0x0200, // This is used to add an asterisk to the cell when the transaction quantity is zero.
			TransErrorFlag			= 0x0400,  // Used to display error colors when the tblTransaction.ErrorFlag is true
			TransWithReversals		= 0x0800   // Display a "*" when transactions have a reversal that day.
		}
		#endregion

		#region Status Flag Class
		[System.Serializable]
		public class StatusFlags
		{
			protected Status flags;

			public StatusFlags()
			{
				this.flags = Status.Default;
			}

			public StatusFlags(Status flag)
			{
				this.flags = flag;
			}

			public bool CheckFlag(Status testFlag)
			{
				return ((this.flags & testFlag) == testFlag);
			}

			public Status Flags { get { return this.flags; } }

			public static StatusFlags operator +(StatusFlags a, StatusFlags b)
			{
				return a.flags | b.flags;
			}

			public static StatusFlags operator -(StatusFlags a, StatusFlags b)
			{
				return a.flags & ~b.flags;
			}

			public static StatusFlags operator |(StatusFlags a, StatusFlags b)
			{
				return a.flags | b.flags;
			}

			public static StatusFlags operator &(StatusFlags a, StatusFlags b)
			{
				return a.flags | b.flags;
			}

			public static implicit operator StatusFlags(Status flag)
			{
				return new StatusFlags(flag);
			}
		}
		#endregion

		#region Base Inventory Line Item DO class
		private StatusFlags flags;
		private Hashtable cellFlags;

		/// <summary>
		/// This is the default constructor for the base inventory line item data object.
		/// </summary>
		public LRBaseInventoryLineItemDO()
		{
			this.flags = new StatusFlags();
			this.cellFlags = new Hashtable();
		}

		/// <summary>
		/// This property will get or set the status flag list.
		/// </summary>
		public StatusFlags Flags
		{
			get { return this.flags; }
			set { this.flags = value; }
		}

		/// <summary>
		/// This method will return true if a given flag is set. Otherwise, it returns
		/// false.
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool CheckFlag(Status flag)
		{
			return this.flags.CheckFlag(flag);
		}

		/// <summary>
		/// This method will return true if a given flag and column name combination
		/// is set.  Otherwise, it will return false.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool CheckFlag(string columnName, Status flag)
		{
			if (this.cellFlags.Contains(columnName))
			{
				StatusFlags currentCellFlags = (StatusFlags)this.cellFlags[columnName];
				return currentCellFlags.CheckFlag(flag);
			}

			return this.flags.CheckFlag(flag);
		}

		/// <summary>
		/// This method will return the cell flags for a given column name.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public StatusFlags GetCellFlags(string columnName)
		{
			if (this.cellFlags.Contains(columnName))
			{
				return (StatusFlags)this.cellFlags[columnName];
			}

			return Status.Default;
		}

		/// <summary>
		/// This method will return the list of cell flags.
		/// </summary>
		/// <returns></returns>
		public Hashtable GetCellFlags()
		{
			return this.cellFlags;
		}

		/// <summary>
		/// This method will set a cell flag based on the column name is flag setting.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="flag"></param>
		public void SetCellFlag(string columnName, Status flag)
		{
			var statusFlags = new StatusFlags();

			if (this.cellFlags.Contains(columnName))
			{
				statusFlags = (StatusFlags)this.cellFlags[columnName];
				this.cellFlags.Remove(columnName);
			}

			statusFlags += flag;
			this.cellFlags.Add(columnName, statusFlags);
		}

		/// <summary>
		/// This method will clear all a cell flag for a given column name.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="flag"></param>
		public void ClearCellFlag(string columnName, Status flag)
		{
			if (this.cellFlags.Contains(columnName))
			{
				var statusFlags = (StatusFlags)this.cellFlags[columnName];
				statusFlags -= flag;
				this.cellFlags.Remove(columnName);

				if (statusFlags.Flags != Status.Default)
				{
					this.cellFlags.Add(columnName, statusFlags);
				}
			}
		}

		/// <summary>
		/// This method will clear the cell flags for a column name.
		/// </summary>
		/// <param name="columnName"></param>
		public void ClearCellFlags(string columnName)
		{
			this.cellFlags.Remove(columnName);
		}

		/// <summary>
		/// This method will clear all the cell flags.
		/// </summary>
		public void ClearCellFlags()
		{
			this.cellFlags.Clear();
		}
		#endregion
	}
}