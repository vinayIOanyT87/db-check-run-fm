using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace FMBusinessObjects.DataObjects
{
	[XmlRoot("BaseLineItem")]
	[XmlType("BaseLineItem")]
	[DataContract]
   [Serializable]
	[KnownType ( typeof ( StatusFlags ) )]
	public abstract class BaseLineItemDO : DataObject
	{		
		#region Status Flags Class
		[DataContract]
		[Serializable]
		public class StatusFlags
		{
			public StatusFlags()
			{
				flags = Status.DEFAULT;
			}

			public StatusFlags(Status flag)
			{
				flags = flag;
			}

			public bool CheckFlag(Status testFlag)
			{
				return ((flags & testFlag) == testFlag);
			}

			public Status Flags { get { return flags; } }

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


			[DataMember]
			protected Status flags;
		}
		#endregion

		[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), System.Flags]
		public enum Status
		{
			DEFAULT = 0x0000,
			CLOSED_OUT = 0x0001,
			PHYS_INV_EXISTS = 0x0002,
			OUT_OF_TOLERANCE_GROSS = 0x0004,
			OUT_OF_TOLERANCE_NET = 0x0008,
			INV_ERROR = 0x0010,
			SUPPRESS = 0x0020, // Display empty string
			NA = 0x0040, // Display "N/A"
			SUPPRESS_LINK = 0x0080, // Do not create hyperlink
			BROKEN_BLENDS = 0x0100,
			TRANS_WITH_ZERO_VOLUME = 0x0200, // This is used to bold cell values when the cell has transactions.
			TRANS_ERROR_FLAG = 0x0400,  // Used to display error colors when the tblTransaction.ErrorFlag is true
            TRANS_WITH_REVERSALS = 0x0800  // Display a "*" when transactions have a reversal that day. 
		}

		#region Private data members
		[DataMember] private StatusFlags flags;
		[DataMember] private Dictionary<string, StatusFlags> cellFlags;
		#endregion

		#region Constructors
		public BaseLineItemDO()
		{
			flags = new StatusFlags();
			this.cellFlags = new Dictionary<string, StatusFlags>();
		}
		#endregion

		public StatusFlags Flags
		{
			get { return flags; }
			set { flags = value; }
		}

		public bool CheckFlag(Status flag)
		{
			return flags.CheckFlag(flag);
		}

		public bool CheckFlag(string columnName, Status flag)
		{
			if (cellFlags.ContainsKey(columnName) == true)
			{
				StatusFlags currentCellFlags = (StatusFlags)cellFlags[columnName];
				return currentCellFlags.CheckFlag(flag);
			}

			return flags.CheckFlag(flag);
		}

		public BaseLineItemDO.StatusFlags GetCellFlags(string columnName)
		{
			if (cellFlags.ContainsKey(columnName) == true)
			{
				return (BaseLineItemDO.StatusFlags)cellFlags[columnName];
			}

			return BaseLineItemDO.Status.DEFAULT;
		}

		public Dictionary<string, StatusFlags> GetCellFlags()
		{
			return cellFlags;
		}

		public void SetCellFlag(string columnName, Status flag)
		{
			BaseLineItemDO.StatusFlags statusFlags = new StatusFlags();

			if (cellFlags.ContainsKey(columnName) == true)
			{
				statusFlags = (BaseLineItemDO.StatusFlags)cellFlags[columnName];
				cellFlags.Remove(columnName);
			}

			statusFlags += flag;
			cellFlags.Add(columnName, statusFlags);
		}

		public void ClearCellFlag(string columnName, Status flag)
		{
			if (cellFlags.ContainsKey(columnName) == true)
			{
				BaseLineItemDO.StatusFlags statusFlags = (BaseLineItemDO.StatusFlags)cellFlags[columnName];
				statusFlags -= flag;
				cellFlags.Remove(columnName);

				if (statusFlags.Flags != Status.DEFAULT)
				{
					cellFlags.Add(columnName, statusFlags);
				}
			}
		}

		public void ClearCellFlags(string columnName)
		{
			cellFlags.Remove(columnName);
		}

		public void ClearCellFlags()
		{
			cellFlags.Clear();
		}
	}
}
